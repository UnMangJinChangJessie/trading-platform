using System.Buffers;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace trading_platform.Model.KoreaInvestment;

public class KisWebSocket : IDisposable {
  public string AccessToken { get; private set; } = "";
  public string TransactionId { get; private set; } = "";
  public string TransactionKey { get; private set; } = "";
  public delegate void MessageEventHandler(object? sender, string transId, List<string[]> messages);
  public event MessageEventHandler OnMessage;
  public CancellationTokenSource Cancellation { get; init; } = new();
  private ClientWebSocket Client { get; init; } = new() {};
  private Task? PollingTask { get; set; } = null;
  private readonly Lock WebSocketCountLock = new();
  private static int WebSocketCount { get; set; } = 0;
  private Aes Aes { get; set; } = Aes.Create();
  private byte[] RequestJsonString => JsonSerializer.SerializeToUtf8Bytes(new {
    header = new {
      approval_key = AccessToken,
      custtype = ApiClient.Personal ? "P" : "B",
      tr_type = "1",
      content_type = "utf-8"
    },
    body = new {
      input = new {
        tr_id = TransactionId,
        tr_key = TransactionKey,
      },
    },
  });
  public KisWebSocket() {
    OnMessage = default!;
    Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(10);
    Client.Options.KeepAliveTimeout = TimeSpan.FromSeconds(5);
  }
  public async ValueTask<bool> ConnectAsync(string relUri, string id, string key) {
    if (Client.State != WebSocketState.None && Client.State != WebSocketState.Closed) return false;
    // Issue a token
    var accessToken = await ApiClient.IssueWebSocketToken();
    if (accessToken == null) return false;
    AccessToken = accessToken;
    TransactionId = id;
    TransactionKey = key;
    // Connect WebSocket
    Uri baseUri = new($"ws://ops.koreainvestment.com:{(ApiClient.Simulation ? 31000 : 21000)}");
    try {
      await Client.ConnectAsync(uri: new(baseUri, relUri), Cancellation.Token);
    }
    catch (TaskCanceledException) {
      return false;
    }
    byte[] requestBytes = RequestJsonString;
    byte[] buffer = new byte[1024];
    ArrayBufferWriter<byte> writer = new();
    WebSocketReceiveResult requestReceiveResult;
    do {
      try {
        await Client.SendAsync(requestBytes, WebSocketMessageType.Text, true, Cancellation.Token);
        requestReceiveResult = await Client.ReceiveAsync(buffer, Cancellation.Token);
      }
      catch (TaskCanceledException) {
        return false;
      }
      if (requestReceiveResult.CloseStatus.HasValue) {
        Debug.WriteLine($"Failed to register a WebSocket to the remote server: {requestReceiveResult.CloseStatusDescription}");
        return false;
      }
      writer.Write(buffer.AsSpan()[..requestReceiveResult.Count]);
    }
    while (!requestReceiveResult.EndOfMessage);
    JsonNode? requestReceiveJson = JsonSerializer.Deserialize<JsonNode>(writer.WrittenSpan);
    if (requestReceiveJson == null) return false;
    string? message;
    string? messageCode;
    try {
      var responseBodyJson = requestReceiveJson["body"];
      message = responseBodyJson?["msg1"]?.GetValue<string>();
      messageCode = responseBodyJson?["msg_cd"]?.GetValue<string>();
      var encrypted = responseBodyJson?["output"];
      if (encrypted != null) {
        var ivBytes = Convert.FromBase64String(encrypted?["iv"]?.GetValue<string>() ?? "");
        var keyBytes = Convert.FromBase64String(encrypted?["key"]?.GetValue<string>() ?? "");
        if (ivBytes.Length == 16 && keyBytes.Length == 32) {
          Aes.KeySize = 256;
          Aes.Key = keyBytes;
          Aes.IV = ivBytes;
        }
      }
    }
    catch (JsonException ex) {
      Debug.WriteLine($"Failed to manipulating JSON object: {ex.Message}\n{ex.StackTrace}");
      return false;
    }
    if (message?.Trim() != "SUBSCRIBE SUCCESS") {
      Debug.WriteLine($"Failed to establish WebSocket Connection: [{messageCode}] {message}\n");
      return false;
    }
    return true;
  }
  public async ValueTask<bool> StartReceivingAsync(string relUri, string id, string key) {
    if (PollingTask != null) return false;
    lock (WebSocketCountLock) {
      if (WebSocketCount >= 41) return false;
    }
    if (Client.State != WebSocketState.Open && !await ConnectAsync(relUri, id, key)) return false;
    PollingTask = Task.Run(async () => {
      lock (WebSocketCountLock) {
        WebSocketCount++;
      }
      var buffer = new byte[4 * 1024]; // 4 KiB
      ArrayBufferWriter<byte> writer = new(4 * 1024);
      WebSocketReceiveResult receiveResult;
      while (Client.State == WebSocketState.Open && Cancellation.Token.IsCancellationRequested) {
        try {
          do {
            receiveResult = await Client.ReceiveAsync(buffer, Cancellation.Token);
            if (receiveResult.CloseStatus.HasValue) {
              Debug.WriteLine(receiveResult.CloseStatusDescription);
              return;
            }
            writer.Write(buffer.AsSpan()[..receiveResult.Count]);
          }
          while (!receiveResult.EndOfMessage);
          string rawMessage = Encoding.UTF8.GetString(writer.WrittenSpan);
          var (transId, messages) = SplitWebSocketMessage(rawMessage);
          OnMessage?.Invoke(this, transId, messages);
          writer.Clear();
        }
        catch (Exception ex) {
          ExceptionHandler.PrintExceptionMessage(ex);
        }
      }
      lock (WebSocketCountLock) {
          WebSocketCount--;
        }
    }, Cancellation.Token);
    return true;
  }
  private (string TransactionId, List<string[]> Messages) SplitWebSocketMessage(string content) {
    if (string.IsNullOrEmpty(content)) return ("", []);
    var tokens = content.Split('|');
    if (tokens.Length != 4) return ("", []);
    var encryption = tokens[0] != "0";
    var transId = tokens[1];
    var recordCount = int.Parse(tokens[2]);
    var contentTokens = (encryption ? Encoding.UTF8.GetString(Aes.DecryptCbc(Convert.FromBase64String(tokens[3]), Aes.IV)) : tokens[3]).Split('^');
    var propertyCount = contentTokens.Length / recordCount;
    var message = new List<string[]>();
    for (int i = 0; i < recordCount; i++) {
      var line = new string[propertyCount];
      for (int j = 0; j < propertyCount; j++) {
        line[j] = contentTokens[propertyCount * i + j];
      }
      message.Add(line);
    }
    return (transId, message);
  }
  public async Task StopReceivingAsync() {
    if (PollingTask == null) return;
    Cancellation.Cancel();
    try { await PollingTask; }
    catch (TaskCanceledException) { }
    PollingTask = null;
  }
  async void IDisposable.Dispose() {
    GC.SuppressFinalize(this);
    await StopReceivingAsync();
  }
}

public partial class ApiClient {
  public static async Task<string?> IssueWebSocketToken() {
    var body = new {
      grant_type = "client_credentials",
      appkey = AppPublicKey,
      secretkey = AppSecretKey,
    };
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
}