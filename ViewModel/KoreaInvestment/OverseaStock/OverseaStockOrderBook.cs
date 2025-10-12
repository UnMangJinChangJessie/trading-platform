
using System.Text.Json;
using Avalonia.Controls;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;
using static trading_platform.Model.KoreaInvestment.OverseaStock;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockOrderBook : OrderBook {
  private Exchange CurrentExchange { get; set; } = Exchange.None;
  public OverseaStockOrderBook() : base() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "HDFSASP0" && args.TransactionId != "HDFSASP1") return;
      if (args.Message.Count == 0) return;
      if (args.Message[^1][1] != Ticker) return;
      lock (CurrentOrders) {
        decimal bidPrice = decimal.Parse(args.Message[^1][11]);
        ulong bidQuantity = ulong.Parse(args.Message[^1][13]);
        decimal askPrice = decimal.Parse(args.Message[^1][12]);
        ulong askQuantity = ulong.Parse(args.Message[^1][14]);
        InsertOrder(bidPrice, bidQuantity, 0);
        InsertOrder(askPrice, 0, askQuantity);
        ZeroOutOutOfRange(askPrice, bidPrice);
        HighestQuantity = CurrentOrders.Max(x => Math.Max(x.BidQuantity, x.AskQuantity));
      }
      IntermediatePrice = null;
      IntermediateAskQuantity = null;
      IntermediateBidQuantity = null;
      ConclusionTime = TimeOnly.ParseExact(args.Message[^1][4], "HHmmss");
      OnPropertyChanged(propertyName: null);
    };
  }
  private void OnReceiveMessage(string jsonString) {
    OrderBookResult result;
    try {
      result = JsonSerializer.Deserialize<OrderBookResult>(jsonString, ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    lock (CurrentOrders) {
      InsertOrder(result!.OrderBook?.FirstAskPrice ?? 0, result!.OrderBook?.FirstAskQuantity ?? 0, 0);
      InsertOrder(result!.OrderBook?.FirstBidPrice ?? 0, 0, result!.OrderBook?.FirstBidQuantity ?? 0);
      HighestQuantity = CurrentOrders.Max(x => Math.Max(x.BidQuantity, x.AskQuantity));
    }
    ConclusionTime = result.Information?.CurrentTime ?? TimeOnly.MinValue;
    CurrentClose = result.Information?.CurrentClose ?? 0;
    PreviousClose = result.Information?.PreviousClose ?? 0;
  }
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("exchange", out var exchangeObject) || exchangeObject is not Exchange exchange) return;
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker) is not OverseaStockInformation information) return;
    Model.KoreaInvestment.OverseaStock.GetOrderBook(new() {
      ExchangeCode = information.Exchange,
      Ticker = information.Ticker
    }, OnReceiveMessage);
  }
  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("exchange", out var exchangeObject) || exchangeObject is not Exchange exchange) return;
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker) is not OverseaStockInformation information) return;
    CurrentExchange = information.Exchange;
    Ticker = information.Ticker;
    await ApiClient.KisWebSocket.Subscribe("HDFSASP0", $"D{CurrentExchange.GetCode()}{Ticker}");
    RealTimeRefresh = true;
  }
  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (CurrentExchange == Exchange.None) return;
    await ApiClient.KisWebSocket.Unsubscribe("HDFSASP0", $"D{CurrentExchange.GetCode()}{Ticker}");
  }
}