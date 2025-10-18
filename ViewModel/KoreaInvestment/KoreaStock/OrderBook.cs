namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Diagnostics;
using System.Threading.Tasks;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;

using OrderBookBase = ViewModel.OrderBook;

public partial class OrderBook(MarketItemLabel label) : OrderBookBase(label) {
  /// <summary>
  /// WebSocket의 연결 해제를 위해 저장하는 종목코드
  /// </summary>
  private string? WebSocketTicker = null;
  private void OnReceivedRealtimeOrderBook(object? sender, ApiClient.KisWebSocket.MessageReceivedEventArgs args) {
    if (args.Tokens.Length == 0) return;
    lock (CurrentOrders) {
      for (int i = 0; i < 10; i++) {
        InsertOrder(ulong.Parse(args.Tokens[^1][3 + i]), ulong.Parse(args.Tokens[^1][23 + i]), 0);
        InsertOrder(ulong.Parse(args.Tokens[^1][13 + i]), 0, ulong.Parse(args.Tokens[^1][33 + i]));
      }
    }
  }
  private async void OnReceivedOrderBook(string jsonString, bool hasNextData, object? args) {
    var result = ApiClient.DeserializeJson<OrderBookResult>(jsonString);
    if (result == null) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedOrderBook)}] {result.ResponseMessage}");
      return;
    }
    lock (CurrentOrders) {
      #region 10호가 삽입
      InsertOrder(result.Output!.AskPrice_1, result.Output!.AskQuantity_1, 0);
      InsertOrder(result.Output!.AskPrice_2, result.Output!.AskQuantity_2, 0);
      InsertOrder(result.Output!.AskPrice_3, result.Output!.AskQuantity_3, 0);
      InsertOrder(result.Output!.AskPrice_4, result.Output!.AskQuantity_4, 0);
      InsertOrder(result.Output!.AskPrice_5, result.Output!.AskQuantity_5, 0);
      InsertOrder(result.Output!.AskPrice_6, result.Output!.AskQuantity_6, 0);
      InsertOrder(result.Output!.AskPrice_7, result.Output!.AskQuantity_7, 0);
      InsertOrder(result.Output!.AskPrice_8, result.Output!.AskQuantity_8, 0);
      InsertOrder(result.Output!.AskPrice_9, result.Output!.AskQuantity_9, 0);
      InsertOrder(result.Output!.AskPrice_10, result.Output!.AskQuantity_10, 0);
      InsertOrder(result.Output!.BidPrice_1, 0, result.Output!.BidQuantity_1);
      InsertOrder(result.Output!.BidPrice_2, 0, result.Output!.BidQuantity_2);
      InsertOrder(result.Output!.BidPrice_3, 0, result.Output!.BidQuantity_3);
      InsertOrder(result.Output!.BidPrice_4, 0, result.Output!.BidQuantity_4);
      InsertOrder(result.Output!.BidPrice_5, 0, result.Output!.BidQuantity_5);
      InsertOrder(result.Output!.BidPrice_6, 0, result.Output!.BidQuantity_6);
      InsertOrder(result.Output!.BidPrice_7, 0, result.Output!.BidQuantity_7);
      InsertOrder(result.Output!.BidPrice_8, 0, result.Output!.BidQuantity_8);
      InsertOrder(result.Output!.BidPrice_9, 0, result.Output!.BidQuantity_9);
      InsertOrder(result.Output!.BidPrice_10, 0, result.Output!.BidQuantity_10);
      #endregion
    }
    if (WebSocketTicker != null) {
      await ApiClient.KisWebSocket.Unsubscribe("H0UNASP0", WebSocketTicker);
    }
    WebSocketTicker = Label.Ticker;
    // 실시간 데이터 수신 요청(KRX/NXT 통합)
    await ApiClient.KisWebSocket.Subscribe("H0UNASP0", Label.Ticker, OnReceivedRealtimeOrderBook);
  }
  public override void Refresh() {
    GetOrderBook(new OrderBookQueries() {
      MarketClassification = Exchange.DomesticUnified,
      Ticker = Label.Ticker,
    }, OnReceivedOrderBook, null);
  }
  public override Task RefreshAsync() {
    throw new NotImplementedException();
  }
}