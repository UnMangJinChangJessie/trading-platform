using System.Buffers;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace trading_platform.Model.KoreaInvestment;

public partial class WebSocketModel {
  
  private async Task PollReceivedMessage() {
    WebSocketReceiveResult receiveResult;
    byte[] buffer = new byte[1024 * 4];
    bool closeSent = false;
    ArrayBufferWriter<byte> writer = new();
    while (Client.State == WebSocketState.Open) {
      try {
        writer.Clear();
        if (Cancellation.Token.IsCancellationRequested && !closeSent) {
          closeSent = true;
          await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, Cancellation.Token);
        }
        do {
          receiveResult = await Client.ReceiveAsync(buffer, Cancellation.Token);
          writer.Write(buffer.AsSpan()[..receiveResult.Count]);
        }
        while (!receiveResult.EndOfMessage);
        // 서버가 웹소켓을 닫았으면 반복문 종료
        if (receiveResult.MessageType == WebSocketMessageType.Close) {
          Debug.WriteLine("[{0}] Connection closed: {1}", System.Reflection.MethodBase.GetCurrentMethod(), receiveResult.CloseStatusDescription);
          if (Client.State == WebSocketState.Open) await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, null, Cancellation.Token); // closing handshake
          break;
        }
        // WebSocket 서버에서 보내는 메시지의 경우 subscribe가 성공했음을 알리는 JSON Object 혹은 실시간 데이터가 주어진다.
        // 실시간 데이터의 가장 앞 글자는 암호화 여부를 나타내는 0/1이 있으므로,
        // '{'로 시작하면 JSON으로 간주하고 그렇지 않으면 데이터로 간주한다.
        string message = Encoding.UTF8.GetString(writer.WrittenSpan);
        if (message[0] == '{') {
          var subscriptionSuccess = ParseResponseJson(message);
        }
        else {
          var tokens = message.Split('|');
          bool encrypted = tokens[0] != "0";
          string transId = tokens[1];
          int rowCount = int.Parse(tokens[2]);
          string rawData = tokens[3];
          var subscriptions = Subscriptions.Where(x => x.Key.TransactionId == transId);
          foreach (var kv in subscriptions) {
            var result = ParseTransactionData(
              kv.Key,
              input: rawData,
              encrypted,
              rowCount
            );
            kv.Value.InvokeCallback(transId, result);
          }
        }
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
  }
}