
using System.Text.Json;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockOrderBook : OrderBook {
  private Exchange CurrentExchange { get; set; } = Exchange.None;
  public OverseaStockOrderBook() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "HDFSASP0" && args.TransactionId != "HDFSASP1") return;
      if (args.Message.Count == 0) return;
      if (args.Message[^1][1] != Ticker) return;
      BidPrice[0].Value = decimal.Parse(args.Message[^1][11]);
      AskPrice[0].Value = decimal.Parse(args.Message[^1][12]);
      BidQuantity[0].Value = decimal.Parse(args.Message[^1][13]);
      AskQuantity[0].Value = decimal.Parse(args.Message[^1][14]);
      for (int i = 1; i < 10; i++) {
        AskPrice[i].Value = 0;
        BidPrice[i].Value = 0;
        AskQuantity[i].Value = 0;
        BidQuantity[i].Value = 0;
      }
      IntermediatePrice = null;
      IntermediateAskQuantity = null;
      IntermediateBidQuantity = null;
      ConclusionTime = TimeOnly.ParseExact(args.Message[^1][4], "HHmmss");
      HighestQuantity = Math.Max(BidQuantity[0].Value, AskQuantity[0].Value);
      OnPropertyChanged(propertyName: null);
    };
    if (Design.IsDesignMode) {
      for (int i = 0; i < 10; i++) {
        AskPrice[i].Value = 24600.00M + i * 0.25M;
        BidPrice[i].Value = 24600.00M - (i + 1) * 0.25M;
        AskQuantity[i].Value = Random.Shared.Next(1, 50 - 5 * i);
        BidQuantity[i].Value = Random.Shared.Next(1, 50 - 5 * i);
      }
      HighestQuantity = Math.Max(AskQuantity.Max(x => x.Value), BidQuantity.Max(x => x.Value));
    }
  }
  private void OnReceiveMessage(string jsonString) {
    OverseaStockOrderBookResult result;
    try {
      result = JsonSerializer.Deserialize<OverseaStockOrderBookResult>(jsonString, ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    AskPrice[0].Value = result!.OrderBook?.FirstAskPrice ?? 0;
    BidPrice[0].Value = result.OrderBook?.FirstBidPrice ?? 0;
    AskQuantity[0].Value = result.OrderBook?.FirstAskQuantity ?? 0;
    BidQuantity[0].Value = result.OrderBook?.FirstBidQuantity ?? 0;
    HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
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