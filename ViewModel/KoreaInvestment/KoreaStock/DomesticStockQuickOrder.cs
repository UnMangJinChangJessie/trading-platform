using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class DomesticStockQuickOrder : QuickOrder, IAccount {
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [ObservableProperty]
  public partial string AccountCode { get; set; }

  public DomesticStockQuickOrder() : base() {
    AccountBase = "";
    AccountCode = "";
    NextTickGenerator = KRXStock.GetTickIncrement;
    PreviousTickGenerator = KRXStock.GetTickDecrement;
  }
  public void OnReceivedLong(string jsonString) {
    throw new NotImplementedException();
  }
  public void OnReceivedShort(string jsonString) {
    throw new NotImplementedException();
  }
  public void OnReceivedOrderBook(string jsonString) {
    OrderBookResult json;
    try {
      json = JsonSerializer.Deserialize<OrderBookResult>(jsonString, ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (json!.ReturnCode != 0) return;
    var result = json.Output!;
    var info = json.Information!;
    CurrentClose = info.CurrentClose;
    PreviousClose = info.PreviousClose;
    lock (CurrentOrders) {
      // 상하한가 호가 생성 (하나씩 삽입할 이유는 없음)
      ulong loopCounter = 0;
      ulong unitPrice = info.PreviousClose;
      while (loopCounter < 1_000) {
        ulong newPrice = (ulong)NextTickGenerator(unitPrice);
        if (newPrice * 10 <= PreviousClose * 13) unitPrice = newPrice;
        else break;
        loopCounter++;
      }
      loopCounter = 0;
      CurrentOrders.Clear();
      while (loopCounter < 2_000) {
        if (unitPrice * 10 < PreviousClose * 7) break;
        CurrentOrders.Add(new(unitPrice, 0, 0) { AskQuantity = 0, BidQuantity = 0 });
        unitPrice = (ulong)PreviousTickGenerator(unitPrice);
        loopCounter++;
      }
      //호가 삽입
      InsertOrder(result.AskPrice_1, ask: result.AskQuantity_1, bid: 0);
      InsertOrder(result.AskPrice_2, ask: result.AskQuantity_2, bid: 0);
      InsertOrder(result.AskPrice_3, ask: result.AskQuantity_3, bid: 0);
      InsertOrder(result.AskPrice_4, ask: result.AskQuantity_4, bid: 0);
      InsertOrder(result.AskPrice_5, ask: result.AskQuantity_5, bid: 0);
      InsertOrder(result.AskPrice_6, ask: result.AskQuantity_6, bid: 0);
      InsertOrder(result.AskPrice_7, ask: result.AskQuantity_7, bid: 0);
      InsertOrder(result.AskPrice_8, ask: result.AskQuantity_8, bid: 0);
      InsertOrder(result.AskPrice_9, ask: result.AskQuantity_9, bid: 0);
      InsertOrder(result.AskPrice_10, ask: result.AskQuantity_10, bid: 0);
      InsertOrder(result.BidPrice_1, bid: result.BidQuantity_1, ask: 0);
      InsertOrder(result.BidPrice_2, bid: result.BidQuantity_2, ask: 0);
      InsertOrder(result.BidPrice_3, bid: result.BidQuantity_3, ask: 0);
      InsertOrder(result.BidPrice_4, bid: result.BidQuantity_4, ask: 0);
      InsertOrder(result.BidPrice_5, bid: result.BidQuantity_5, ask: 0);
      InsertOrder(result.BidPrice_6, bid: result.BidQuantity_6, ask: 0);
      InsertOrder(result.BidPrice_7, bid: result.BidQuantity_7, ask: 0);
      InsertOrder(result.BidPrice_8, bid: result.BidQuantity_8, ask: 0);
      InsertOrder(result.BidPrice_9, bid: result.BidQuantity_9, ask: 0);
      InsertOrder(result.BidPrice_10, bid: result.BidQuantity_10, ask: 0);
      ZeroOutOutOfRange(result.BidPrice_10, result.AskPrice_10);
    }
  }
  public override async Task LongAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (!args.TryGetValue("price", out var priceObject) || priceObject is not ulong price) return;
    if (!args.TryGetValue("quantity", out var quantityObject) || quantityObject is not ulong quantity) return;
    OrderCash(new() {
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      Position = OrderPosition.Long,
      Ticker = ticker,
      Exchange = DomesticOrderRoute.SmartOrderRouting,
      Method = OrderMethod.Limit,
      UnitPrice = price,
      Quantity = quantity,
    }, OnReceivedLong);
  }
  public override async Task ShortAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (!args.TryGetValue("price", out var priceObject) || priceObject is not ulong price) return;
    if (!args.TryGetValue("quantity", out var quantityObject) || quantityObject is not ulong quantity) return;
    OrderCash(new() {
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      Position = OrderPosition.Short,
      Ticker = ticker,
      Exchange = DomesticOrderRoute.SmartOrderRouting,
      Method = OrderMethod.Limit,
      UnitPrice = price,
      Quantity = quantity,
    }, OnReceivedLong);
  }

  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (KRXStock.SearchByTicker(Ticker) is null) return;
    lock (CurrentOrders) CurrentOrders.Clear();
    GetOrderBook(new() {
      MarketClassification = Exchange.DomesticUnified,
      Ticker = Ticker
    }, OnReceivedOrderBook);
  }

  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (KRXStock.SearchByTicker(Ticker) is not KRXStockInformation stockInformation) return;
    // 실시간 호가
    await ApiClient.KisWebSocket.Subscribe("H0UNASP0", Ticker);
  }
  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> args) {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNASP0", Ticker);
  }
}