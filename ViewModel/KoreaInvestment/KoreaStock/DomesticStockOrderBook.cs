
using System.Text.Json;
using Avalonia.Controls;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using ScottPlot;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class StockOrderBook : OrderBook {
  public StockOrderBook() : base() {
    ApiClient.KisWebSocket.MessageReceived += OnReceiveRealtime;
  }
  ~StockOrderBook() {
    ApiClient.KisWebSocket.MessageReceived -= OnReceiveRealtime;
  }
  public void OnReceiveRealtime(object? sender, (string TransactionId, List<string[]> Message) args) {
    if (args.TransactionId != "H0UNASP0" && args.TransactionId != "H0STASP0" && args.TransactionId != "H0NXASP0") return; // 통합 | KRX | NexTrade
    if (args.Message.Count == 0) return;
    if (args.Message[^1][0] != Ticker) return;
    if (args.TransactionId == "H0UNASP0" || args.TransactionId == "H0NXASP0") {
      lock (CurrentOrders) {
        CurrentOrders.Clear();
        for (int i = 0; i < 10; i++) {
          ulong askPrice = ulong.Parse(args.Message[^1][3 + i]);
          ulong bidPrice = ulong.Parse(args.Message[^1][13 + i]);
          ulong askQuantity = ulong.Parse(args.Message[^1][23 + i]);
          ulong bidQuantity = ulong.Parse(args.Message[^1][33 + i]);
          InsertOrder(askPrice, askQuantity, 0);
          InsertOrder(bidPrice, 0, bidQuantity);
        }
        HighestQuantity = CurrentOrders.Max(x => Math.Max(x.AskQuantity, x.BidQuantity));
      }
      decimal krxIntermediateQuantity = decimal.Parse(args.Message[^1][60]);
      bool krxIntermediateExists = args.Message[^1][61] != "0";
      bool krxIntermediateAsking = args.Message[^1][61] == "1";
      decimal nxtIntermediateQuantity = decimal.Parse(args.Message[^1][63]);
      bool nxtIntermediateExists = args.Message[^1][64] != "0";
      bool nxtIntermediateAsking = args.Message[^1][64] == "1";
      decimal intermediatePrice = decimal.Parse(args.Message[^1][59]);
      bool intermediateDoesNotExist = !nxtIntermediateExists && !krxIntermediateExists;
      if (intermediateDoesNotExist) {
        IntermediatePrice = null;
        IntermediateAskQuantity = null;
        IntermediateBidQuantity = null;
      }
      else {
        IntermediatePrice = intermediatePrice;
        decimal ask = (krxIntermediateAsking ? krxIntermediateQuantity : 0) + (nxtIntermediateAsking ? nxtIntermediateQuantity : 0);
        decimal bid = (krxIntermediateAsking ? 0 : krxIntermediateQuantity) + (nxtIntermediateAsking ? 0 : nxtIntermediateQuantity);
        IntermediateAskQuantity = ask == 0 ? null : ask;
        IntermediateBidQuantity = bid == 0 ? null : bid;
      }
      ConclusionTime = TimeOnly.ParseExact(args.Message[^1][1], "HHmmss");
    }
    else {
      lock (CurrentOrders) {
        for (int i = 0; i < 10; i++) {
          // thanks ScottPlot
          ulong askPrice = ulong.Parse(args.Message[^1][3 + i]);
          ulong bidPrice = ulong.Parse(args.Message[^1][13 + i]);
          ulong askQuantity = ulong.Parse(args.Message[^1][23 + i]);
          ulong bidQuantity = ulong.Parse(args.Message[^1][33 + i]);
          InsertOrder(askPrice, ask: askQuantity, bid: 0);
          InsertOrder(bidPrice, bid: bidQuantity, ask: 0);
          if (i + 1 == 10) ZeroOutOutOfRange(bidPrice, askPrice);
        }
        HighestQuantity = CurrentOrders.Max(x => Math.Max(x.AskQuantity, x.BidQuantity));
      }
      IntermediatePrice = null;
      IntermediateAskQuantity = null;
      IntermediateBidQuantity = null;
      ConclusionTime = TimeOnly.ParseExact(args.Message[^1][1], "HHmmss");
    }
  }
  public async void OnReceiveMessage(string jsonString, object? args) {
    OrderBookResult json;
    try {
      json = JsonSerializer.Deserialize<OrderBookResult>(jsonString, ApiClient.JsonSerializerOption)!;
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (json!.ReturnCode != 0) return;
    var result = json.Output!;
    lock (CurrentOrders) {
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
    HighestQuantity = CurrentOrders.Max(x => Math.Max(x.AskQuantity, x.BidQuantity));
    ConclusionTime = result.Time;
    PreviousClose = json.Information!.PreviousClose;
    CurrentClose = json.Information!.CurrentClose;
    await EndRefreshRealtimeAsync();
    await StartRefreshRealtimeAsync();
  }
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (KRXStock.SearchByTicker(ticker) is not KRXStockInformation info) return;
    Ticker = info.Ticker;
    lock (CurrentOrders) {
      CurrentOrders.Clear();
    }
    GetOrderBook(
      new() {
        Ticker = ticker,
        MarketClassification = info.Exchange
      },
      OnReceiveMessage, null
    );
  }
  private async Task StartRefreshRealtimeAsync() {
    await ApiClient.KisWebSocket.Subscribe("H0UNASP0", Ticker);
    RealTimeRefresh = true;
  }
  private async Task EndRefreshRealtimeAsync() {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNASP0", Ticker);
    RealTimeRefresh = false;
  }
}