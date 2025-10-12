using System.Buffers;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace trading_platform.Model.KoreaInvestment;

using Subscription = (string TransactionId, string TransactionKey);

public partial class ApiClient {
  public static class KisWebSocket {
    public static string AccessToken { get; private set; } = "";
    public static CancellationTokenSource Cancellation { get; } = new();
    public static WebSocketState ClientState => Client.State;
    private static ClientWebSocket Client { get; set; } = new() { };
    private static Task? PollingTask { get; set; } = null;
    private static readonly Lock SubscriptionsLock = new();
    private static Dictionary<Subscription, Aes?> Subscriptions { get; set; } = [];
    private static byte[] RequestJsonString(bool subscribe, string transId, string transKey) => JsonSerializer.SerializeToUtf8Bytes(new {
      header = new {
        approval_key = AccessToken,
        custtype = Personal ? "P" : "B",
        tr_type = subscribe ? "1" : "2",
        content_type = "utf-8"
      },
      body = new {
        input = new {
          tr_id = transId,
          tr_key = transKey,
        },
      },
    });
    public static event EventHandler<(string TransactionId, List<string[]> Message)> MessageReceived;

    private static async Task PollReceivedMessage() {
      WebSocketReceiveResult receiveResult;
      byte[] buffer = new byte[1024 * 4];
      ArrayBufferWriter<byte> writer = new();
      try {
        while (Client.State == WebSocketState.Open) {
          writer.Clear();
          if (Cancellation.Token.IsCancellationRequested) break;
          do {
            receiveResult = await Client.ReceiveAsync(buffer, CancellationToken.None);
            writer.Write(buffer.AsSpan()[..receiveResult.Count]);
          }
          while (!receiveResult.EndOfMessage);
          // WebSocket 서버에서 보내는 메시지의 경우 subscribe가 성공했음을 알리는 JSON 혹은 실시간 데이터만이 주어진다.
          // 실시간 데이터의 가장 앞 글자는 암호화 여부를 나타내는 0/1이 있으므로,
          // '{'로 시작하면 JSON으로 간주하고 그렇지 않으면 데이터로 간주한다.
          string message = Encoding.UTF8.GetString(writer.WrittenSpan);
          if (receiveResult.MessageType == WebSocketMessageType.Close) {
            Debug.WriteLine($"[{nameof(KisWebSocket)}.{nameof(PollReceivedMessage)}] Connection closed: {receiveResult.CloseStatusDescription}");
            break;
          }
          if (message[0] == '{') {
            var subscriptionSuccess = ParseResponseJson(message);
          }
          else {
            var tokens = message.Split('|');
            bool encrypted = tokens[0] != "0";
            string transId = tokens[1];
            int rowCount = int.Parse(tokens[2]);
            string rawData = tokens[3];
            var subscriptions = Subscriptions.Keys.Where(x => x.TransactionId == transId);
            foreach (var subscription in subscriptions) {
              var result = ParseTransactionData(
                subscription,
                input: rawData,
                encrypted,
                rowCount
              );
              MessageReceived?.Invoke(null, (TransactionId: transId, Message: result));
            }
          }
        }
        await Client.CloseAsync(WebSocketCloseStatus.Empty, ":3", CancellationToken.None);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
    private static List<string[]> ParseTransactionData(Subscription subscription, string input, bool encrypted, int rowCount) {
      if (encrypted) {
        byte[] bytes = Convert.FromBase64String(input);
        try {
          var encryption = Subscriptions[subscription]!;
          byte[] decrypted = encryption!.DecryptCbc(bytes.AsSpan(), encryption.IV);
          input = Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception ex) {
          ExceptionHandler.PrintExceptionMessage(ex);
          return [];
        }
      }
      var dataTokens = input.Split('^');
      var itemCount = dataTokens.Length / rowCount;
      List<string[]> result = [];
      try {
        for (int i = 0; i < rowCount; i++) {
          result.Add(dataTokens[(itemCount * i)..(itemCount * (i + 1))]);
        }
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
        return [];
      }
      return result;
    }
    private static async ValueTask<bool> ParseResponseJson(string input) {
      try {
        var node = JsonSerializer.Deserialize<JsonNode>(input);
        var responseCode = node?["body"]?["msg_cd"]?.GetValue<string>();
        var responseMessage = node?["body"]?["msg1"]?.GetValue<string>();
        var iv = node?["body"]?["output"]?["iv"]?.GetValue<string>();
        var key = node?["body"]?["output"]?["key"]?.GetValue<string>();
        var transId = node?["header"]?["tr_id"]?.GetValue<string>();
        var transKey = node?["header"]?["tr_key"]?.GetValue<string>();
        // Ping-Pong request, sends pong.
        // According to the API refs, we just send the JSON back.
        if (transId == "PINGPONG") {
          await Client.SendAsync(Encoding.UTF8.GetBytes(input), WebSocketMessageType.Text, true, CancellationToken.None);
          return true;
        }
        Debug.WriteLine($"[{responseCode}] {responseMessage}");
        if (responseCode == "OPSP0000" || responseCode == "OPSP0002") { // SUBSCRIBE SUCCESS || ALREADY IN SUBSCRIBE
          Aes? encryption = null;
          if (!string.IsNullOrEmpty(iv) && !string.IsNullOrEmpty(key)) {
            encryption = Aes.Create();
            encryption.KeySize = 256;
            byte[] paddedIv = new byte[16];
            byte[] rawIv = Convert.FromBase64String(iv);
            rawIv.CopyTo(paddedIv, 0);
            encryption.IV = paddedIv;
            encryption.Key = Convert.FromBase64String(key);
          }
          lock (SubscriptionsLock) {
            if (!Subscriptions.TryAdd((transId!, transKey!), encryption)) Subscriptions[(transId!, transKey!)] = encryption;
          }
        }
        else if (responseCode == "OPSP0001" || responseCode == "OPSP0003") { // UNSUBSCRIBE SUCCESS || UNSUBSCRIBE ERROR(not found!)
          lock (SubscriptionsLock) {
            Subscriptions.Remove((transId!, transKey!));
          }
        }
        return responseCode == "OPSP0000" || responseCode == "OPSP0001" || responseCode == "OPSP0002" || responseCode == "OPSP0003";
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
        return false;
      }
    }

    public static async ValueTask<bool> Connect() {
      if (Client.State == WebSocketState.Open) return true;
      if (Client.State == WebSocketState.Aborted) {
        // 소켓이 죽었으므로 CancellationTokenSource와 소켓을 다시 생성
        Client.Dispose();
        Client = new();
        Cancellation.TryReset();
      }
      Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
      Client.Options.KeepAliveTimeout = TimeSpan.FromSeconds(0);
      var token = await IssueWebSocketToken();
      if (token == null) return false;
      AccessToken = token;
      Uri uri = new($"ws://ops.koreainvestment.com:{(Simulation ? 31000 : 21000)}");
      try {
        await Client.ConnectAsync(uri, CancellationToken.None);
        SpinWait.SpinUntil(() => Client.State != WebSocketState.Connecting, 10_000);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
        return false;
      }
      PollingTask = Task.Run(PollReceivedMessage);
      return true;
    }

    public static async Task Subscribe(string id, string key) {
      lock (SubscriptionsLock) {
        if (Subscriptions.Count >= 41) return;
      }
      try {
        await Client.SendAsync(RequestJsonString(true, id, key), WebSocketMessageType.Text, true, CancellationToken.None);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
    public static async Task Unsubscribe(string id, string key) {
      if (Client.State != WebSocketState.Open) return;
      try {
        await Client.SendAsync(RequestJsonString(false, id, key), WebSocketMessageType.Text, true, CancellationToken.None);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
    public static void Close() {
      Cancellation.Cancel();
    }
  }
  public static async Task<string?> IssueWebSocketToken() {
    var body = new {
      grant_type = "client_credentials",
      appkey = AppPublicKey,
      secretkey = AppSecretKey,
    };
    try {
      var result = await RequestClient.PostAsJsonAsync("/oauth2/Approval", body);
      var responseBody = await result.Content.ReadFromJsonAsync<JsonNode>();
      if (responseBody == null) {
        Debug.WriteLine("Failed to get response from the server: [{0}] {1}", args: [result.StatusCode, result.ReasonPhrase]);
        return null;
      }
      if (!result.IsSuccessStatusCode) {
        Debug.WriteLine(
          "Failed to issue a WebSocket token: [{0}] {1}",
          responseBody["error_code"]?.GetValue<string>(),
          responseBody["error_description"]?.GetValue<string>()
        );
        return null;
      }
      return responseBody["approval_key"]?.GetValue<string>();
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return null;
    }
  }
}