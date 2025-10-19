using CommunityToolkit.Mvvm.ComponentModel;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Net.Http.Json;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace trading_platform.Model.KoreaInvestment;

public partial class WebSocketModel {
  public class MessageReceivedEventArgs(string id, ImmutableArray<ImmutableArray<string>> tokens) : EventArgs {
    public string TransactionId { get; set; } = id;
    public ImmutableArray<ImmutableArray<string>> Tokens { get; set; } = tokens;
  }
  internal class SubscriptionKey(string id, string key) {
    public string TransactionId { get; set; } = id;
    public string TransactionKey { get; set; } = key;
    // override object.Equals
    public override bool Equals(object? obj) {
      if (obj == null || obj is not SubscriptionKey comp) {
        return false;
      }
      return TransactionId == comp.TransactionId && TransactionKey == comp.TransactionKey;
    }
    // override object.GetHashCode
    public override int GetHashCode() {
      return (TransactionId + TransactionKey).GetHashCode();
    }
  }
  internal class SubscriptionValue(Aes? aes) {
    public Aes? EncryptionAlgorithm { get; set; } = aes;
    public event EventHandler<MessageReceivedEventArgs> MessageReceived = default!;
    public void InvokeCallback(string id, ImmutableArray<ImmutableArray<string>> tokens) {
      MessageReceived?.Invoke(this, new(id, tokens));
    }
  }
}

public partial class WebSocketModel : ObservableObject{
  public bool IsPersonal { get; private set; }
  public string AccessToken { get; private set; } = "";
  public CancellationTokenSource Cancellation { get; } = new();
  public WebSocketState ClientState => Client.State;
  private ClientWebSocket Client { get; set; } = new() { };
  private Task? PollingTask { get; set; } = null;
  private ConcurrentDictionary<SubscriptionKey, SubscriptionValue> Subscriptions { get; set; } = [];
  private ConcurrentDictionary<SubscriptionKey, EventHandler<MessageReceivedEventArgs>> PendingCallbacks = [];
  private ApiModel? Api { get; set; }

  private byte[] RequestJsonString(bool subscribe, string transId, string transKey) => JsonSerializer.SerializeToUtf8Bytes(new {
    header = new {
      approval_key = AccessToken,
      custtype = Api == null ? "P" : (Api.IsPersonal ? "P" : "B"),
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
  private ImmutableArray<ImmutableArray<string>> ParseTransactionData(SubscriptionKey subscription, string input, bool encrypted, int rowCount) {
    if (encrypted) {
      byte[] bytes = Convert.FromBase64String(input);
      try {
        var encryption = Subscriptions[subscription]!.EncryptionAlgorithm;
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
    List<ImmutableArray<string>> result = [];
    try {
      for (int i = 0; i < rowCount; i++) {
        result.Add(dataTokens.AsSpan()[(itemCount * i)..(itemCount * (i + 1))].ToImmutableArray());
      }
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return [];
    }
    return [.. result];
  }
  private async ValueTask<bool> ParseResponseJson(string input) {
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
        SubscriptionKey dictKey = new(transId!, transKey!);
        SubscriptionValue dictValue = new(encryption);
        // search for the callback (there must be at least one)
        if (PendingCallbacks.TryRemove(dictKey, out var callback)) {
          dictValue.MessageReceived += callback;
        }
        if (!Subscriptions.TryAdd(dictKey, dictValue)) Subscriptions[dictKey] = dictValue;
      }
      else if (responseCode == "OPSP0001" || responseCode == "OPSP0003") { // UNSUBSCRIBE SUCCESS || UNSUBSCRIBE ERROR(not found!)
        Subscriptions.Remove(new(transId!, transKey!), out var _);
      }
      return responseCode == "OPSP0000" || responseCode == "OPSP0001" || responseCode == "OPSP0002" || responseCode == "OPSP0003";
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return false;
    }
  }

  public async ValueTask<bool> Connect(ApiModel api) {
    if (Client.State == WebSocketState.Open) return true;
    if (Client.State == WebSocketState.Aborted) {
      // 소켓이 죽었으므로 CancellationTokenSource와 소켓을 다시 생성
      Client.Dispose();
      Client = new();
      Cancellation.TryReset();
    }
    Client.Options.KeepAliveInterval = TimeSpan.FromSeconds(0);
    Client.Options.KeepAliveTimeout = TimeSpan.FromSeconds(0);
    var token = await api.IssueWebSocketToken();
    if (token == null) return false;
    AccessToken = token;
    Api = api;
    Uri uri = new($"ws://ops.koreainvestment.com:{(api.IsSimulation ? 31000 : 21000)}");
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

  public async Task Subscribe(string id, string key, EventHandler<MessageReceivedEventArgs> callback) {
    if (Subscriptions.Count >= 41) return;
    if (Subscriptions.ContainsKey(new(id, key))) return;
    try {
      await Client.SendAsync(RequestJsonString(true, id, key), WebSocketMessageType.Text, true, CancellationToken.None);
      PendingCallbacks.TryAdd(new(id, key), callback);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
    }
  }
  public async Task Unsubscribe(string id, string key) {
    if (Client.State != WebSocketState.Open) return;
    if (!Subscriptions.ContainsKey(new(id, key))) return;
    try {
      await Client.SendAsync(RequestJsonString(false, id, key), WebSocketMessageType.Text, true, CancellationToken.None);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
    }
  }
  public void Close() {
    Cancellation.Cancel();
  }
}