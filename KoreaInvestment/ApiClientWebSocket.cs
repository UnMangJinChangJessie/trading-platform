using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace trading_platform.KoreaInvestment;

public class KisWebSocket {
  public required ClientWebSocket Client { get; init; }
  public byte[]? AesIV { get; init; }
  public byte[]? AesKey { get; init; }

  public static (string TransactionId, string[,] Contents) Parse(string rawMessage) {
    var token = rawMessage.Split('|');
    string transId = token[0];
    int count = Convert.ToInt32(token[1]);
    string[] rawContents = token[2].Split("^");
    int propCount = rawContents.Length / count;
    string[,] contents = new string[count, propCount];
    for (int i = 0; i < count; i++) {
      for (int j = 0; j < propCount; j++) {
        contents[i, j] = rawContents[i * propCount + j];
      }
    }
    return (transId, contents);
  }
}

public partial class ApiClient {
  private static readonly WebSocketCreationOptions WebSocketCreationOption = new() {
  };
  private readonly static List<KisWebSocket> WebSockets = [];
  public static async Task<KisWebSocket?> OpenWebSocket(string relUri, string transId, string transKey) {
    ClientWebSocket ws = new();
    string body = "{" +
      $"\"approval_key\":\"{WebSocketAccessToken}\"," +
      $"\"custtype\":\"{(Personal ? "P" : "B")}\"," +
      $"\"tr_type\":\"1\"," +
      $"\"content-type\":\"utf-8\"," +
      "\"input\": {" +
      $"\"tr_id\": \"{transId}\"," +
      $"\"tr_key\": \"{transKey}\"" +
      "}" +
      "}";
    await ws.ConnectAsync(new($"ws://ops.koreainvestment.com:{(Simulation ? 31000 : 21000)}" + relUri), CancellationToken.None);
    await ws.SendAsync(Encoding.UTF8.GetBytes(body), WebSocketMessageType.Text, true, CancellationToken.None);
    byte[] buffer = new byte[1024];
    await ws.ReceiveAsync(buffer, CancellationToken.None);
    var responseBody = JsonSerializer.Deserialize<dynamic>(buffer);
    if (responseBody == null) return null;
    bool encrypted = (bool)responseBody.header.encrypt;
    if (((string)responseBody.body.msg1).Trim() != "SUBSCRIBE SUCCESS") return null;
    KisWebSocket item;
    if (!encrypted) {
      item = new() { Client = ws, AesIV = null, AesKey = null };
    }
    else {
      var ivBase64 = (string)responseBody.body.output.iv;
      var keyBase64 = (string)responseBody.body.output.key;
      item = new() { Client = ws, AesIV = Convert.FromBase64String(ivBase64), AesKey = Convert.FromBase64String(keyBase64) };
    }
    WebSockets.Add(item);
    return item;
  }
  public static async Task CloseWebSocket(KisWebSocket socket) {
    if (socket.Client.State == WebSocketState.Open || socket.Client.State == WebSocketState.CloseReceived) {
      await socket.Client.CloseAsync(WebSocketCloseStatus.NormalClosure, ":3", CancellationToken.None);
    }
    socket.Client.Dispose();
    WebSockets.Remove(socket);
  }
}