using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

internal class DomesticStockQuickOrderItem(decimal price, decimal ask, decimal bid) : QuickOrderItem(price, ask, bid) {
  public List<PendingOrder> LongOrders { get; set; } = [];
  public List<PendingOrder> ShortOrders { get; set; } = [];
}

public partial class DomesticStockQuickOrder : QuickOrder, IAccount {
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [ObservableProperty]
  public partial string AccountCode { get; set; }
  private KRXSecuritiesType SecuritiesType { get; set; }

  public DomesticStockQuickOrder() : base() {
    AccountBase = "";
    AccountCode = "";
    NextTickGenerator = x => KRXStock.GetTickIncrement(x, SecuritiesType);
    PreviousTickGenerator = x => KRXStock.GetTickDecrement(x, SecuritiesType);
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
  public override async Task MoveAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("from_price", out var fromPriceObject) || fromPriceObject is not decimal fromPrice) return;
    if (!args.TryGetValue("to_price", out var toPriceObject) || toPriceObject is not decimal toPrice) return;
    if (!args.TryGetValue("from_position", out var fromPositionObject) || fromPositionObject is not Model.Position fromPosition) return;
    if (!args.TryGetValue("to_position", out var toPositionObject) || toPositionObject is not Model.Position toPosition) return;
    // 정정주문 총량
    ulong totalQuantity = 0;
    var orderRowIdx = BinarySearch(fromPrice);
    if (orderRowIdx < 0) return; // 주문이 등록이 되어있어서 있어야 하지만 혹시라도...
    List<PendingOrder> modifying = fromPosition == Model.Position.Long ?
      ((DomesticStockQuickOrderItem)CurrentOrders[orderRowIdx]).LongOrders :
      ((DomesticStockQuickOrderItem)CurrentOrders[orderRowIdx]).ShortOrders;
    totalQuantity = modifying.Aggregate(0UL, (prev, x) => prev + x.ModifiableQuantity);
    await CancelAsync(args);
    // 지정가 재주문
    if (toPosition == Model.Position.Long) {
      await LongAsync(new Dictionary<string, object>() {
        ["ticker"] = Ticker,
        ["price"] = toPrice,
        ["quantity"] = totalQuantity,
      });
    }
    else {
      await ShortAsync(new Dictionary<string, object>() {
        ["ticker"] = Ticker,
        ["price"] = toPrice,
        ["quantity"] = totalQuantity,
      });
    }
  }
  public override async Task CancelAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("from_price", out var fromPriceObject) || fromPriceObject is not decimal fromPrice) return;
    if (!args.TryGetValue("from_position", out var fromPositionObject) || fromPositionObject is not Model.Position fromPosition) return;
    var orderRowIdx = BinarySearch(fromPrice);
    if (orderRowIdx < 0) return; // 호가가 등록이 되어있어서 있어야 하지만 혹시라도...
    List<PendingOrder> modifying = fromPosition == Model.Position.Long ?
      ((DomesticStockQuickOrderItem)CurrentOrders[orderRowIdx]).LongOrders :
      ((DomesticStockQuickOrderItem)CurrentOrders[orderRowIdx]).ShortOrders; // 이러면 참조로 되나?
    // 굳이 그러나 싶지만 같은 가격에 주문을 나누는 경우가 있을 수 있으니,,,,
    foreach (var order in modifying) {
      ModifyOrder(new ModifyOrderBody() {
        AccountBase = AccountBase,
        AccountCode = AccountCode,
        ModificationType = Modification.Cancel,
        OrganizationNumber = order.OrganizationNumber,
        OrderNumber = order.OrderNumber,
        ModifyEntirely = true,
        Quantity = order.ModifiableQuantity,
        UnitPrice = order.UnitPrice,
        OrderDivision = order.OrderDivision
      }, (jsonString) => { }); // callback에서 할 게 없음
    }
    modifying.Clear();
  }
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (KRXStock.SearchByTicker(Ticker) is not KRXStockInformation information) return;
    SecuritiesType = information.SecuritiesType;
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