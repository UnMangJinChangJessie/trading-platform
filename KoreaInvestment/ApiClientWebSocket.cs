using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Transactions;

namespace trading_platform.KoreaInvestment;

public class KisWebSocket {
  public required string TransactionId { get; set; }
  public required string TransactionKey { get; set; } 
  public required ClientWebSocket Client { get; init; }
  public byte[]? AesIV { get; init; }
  public byte[]? AesKey { get; init; }

  public static (bool Encrypted, string TransactionId, List<string[]> Message) SplitWebSocketMessage(string content) {
    var tokens = content.Split('|');
    if (tokens.Length != 4) return (false, "", []);
    var encryption = tokens[0] != "0";
    var transId = tokens[1];
    var recordCount = int.Parse(tokens[2]);
    var contentTokens = tokens[3].Split();
    var propertyCount = contentTokens.Length / recordCount;
    var message = new List<string[]>();
    for (int i = 0; i < recordCount; i++) {
      var line = new string[propertyCount];
      for (int j = 0; j < propertyCount; j++) {
        line[j] = contentTokens[propertyCount * i + j];
      }
      message.Add(line);
    }
    return (encryption, transId, message);
  }
}

public partial class ApiClient {
  private readonly static List<KisWebSocket> WebSockets = [];
  public static async Task<KisWebSocket?> OpenWebSocket(string relUri, string transId, string transKey) {
    ClientWebSocket ws = new();
    object body = new {
      header = new {
        approval_key = WebSocketAccessToken,
        custtype = Personal ? "P" : "B",
        tr_type = "1",
        content_type = "utf-8"
      },
      body = new {
        input = new {
          tr_id = transId,
          tr_key = transKey,
        },
      },
    };
    var requestJsonString = JsonSerializer.Serialize(body);
    await ws.ConnectAsync(new($"ws://ops.koreainvestment.com:{(Simulation ? 31000 : 21000)}" + relUri), CancellationToken.None);
    await ws.SendAsync(Encoding.UTF8.GetBytes(requestJsonString), WebSocketMessageType.Text, true, CancellationToken.None);
    byte[] buffer = new byte[1024];
    var receiveResult = await ws.ReceiveAsync(buffer, CancellationToken.None);
    var bufferString = Encoding.UTF8.GetString(buffer[..receiveResult.Count]);
    var responseJson = JsonSerializer.Deserialize<JsonElement>(bufferString);
    string message;
    string iv, key;
    try {
      var responseBodyJson = responseJson.GetProperty("body");
      message = responseBodyJson.GetProperty("msg1").GetString() ?? "";
      var encrypted = responseBodyJson.TryGetProperty("output", out var responseBodyOutputJson);
      iv = encrypted ? responseBodyOutputJson.GetProperty("iv").GetString() ?? "" : "";
      key = encrypted ? responseBodyOutputJson.GetProperty("key").GetString() ?? "" : "";
    }
    catch (Exception ex) {
      Debug.WriteLine($"Failed to manipulating JSON object: {ex.Message}\n{ex.StackTrace}");
      return null;
    }
    if (message.Trim() != "SUBSCRIBE SUCCESS") {
      Debug.WriteLine($"Failed to establish WebSocket Connection: {message}\nSent JSON:\n{requestJsonString}");
      return null;
    }
    KisWebSocket item = new() {
      TransactionId = transId,
      TransactionKey = transKey,
      Client = ws,
      AesIV = Convert.FromBase64String(iv),
      AesKey = Convert.FromBase64String(key)
    };
    WebSockets.Add(item);
    return item;
  }
  public static async Task CloseWebSocket(KisWebSocket? socket) {
    if (socket == null) return;
    var body = new {
      header = new {
        approval_key = WebSocketAccessToken,
        custtype = Personal ? "P" : "B",
        tr_type = "2",
        content_type = "utf-8"
      },
      body = new {
        input = new {
          tr_id = socket.TransactionId,
          tr_key = socket.TransactionKey,
        },
      },
    };
    var requestJsonString = JsonSerializer.Serialize(body);
    await socket.Client.SendAsync(Encoding.UTF8.GetBytes(requestJsonString), WebSocketMessageType.Text, true, CancellationToken.None);
    if (socket.Client.State == WebSocketState.Open || socket.Client.State == WebSocketState.CloseReceived) {
      await socket.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, ":3", CancellationToken.None);
    }
    socket.Client.Dispose();
    WebSockets.Remove(socket);
  }
}