
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockOrderBook : OrderBook {
  private Exchange CurrentExchange { get; set; } = Exchange.None;
  [ObservableProperty]
  public partial decimal HighestQuantity { get; private set; } = 0;
  public OverseaStockOrderBook() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId == "HDFSASP0" || args.TransactionId == "HDFSASP1") return;
      if (args.Message.Count == 0) return;
      if (args.Message[^1][4] != Ticker) return;
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
      ConclusionTime = TimeOnly.ParseExact(args.Message[^1][6], "HHmmss");
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
  public override async ValueTask<bool> RequestRefreshAsync(string ticker) {
    var exchange = StockMarketInformation.OverseaStock.GetExchange(ticker[..3]);
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker[3..]) is not OverseaStockInformation information) return false;
    var (status, result) = await Model.KoreaInvestment.OverseaStock.InquireOrderBook(new() {
      ExchangeCode = Exchange.DomesticUnified,
      Ticker = ticker[3..]
    });
    if (status != System.Net.HttpStatusCode.OK || result == null) return false;
    AskPrice[0].Value = result.OrderBook?.FirstAskPrice ?? 0;
    BidPrice[0].Value = result.OrderBook?.FirstBidPrice ?? 0;
    AskQuantity[0].Value = result.OrderBook?.FirstAskQuantity ?? 0;
    BidQuantity[0].Value = result.OrderBook?.FirstBidQuantity ?? 0;
    HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
    ConclusionTime = result.Information?.CurrentTime ?? TimeOnly.MinValue;
    CurrentClose = result.Information?.CurrentClose ?? 0;
    PreviousClose = result.Information?.PreviousClose ?? 0;
    return true;
  }
  public override async ValueTask<bool> RequestRefreshRealTimeAsync(string ticker) {
    var exchange = StockMarketInformation.OverseaStock.GetExchange(ticker[..3]);
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker[3..]) is not OverseaStockInformation information) return false;
    CurrentExchange = information.Exchange;
    Ticker = ticker[3..];
    await ApiClient.KisWebSocket.Subscribe("HDFSASP0", $"D{CurrentExchange.GetCode()}{ticker}");
    RealTimeRefresh = true;
    return RealTimeRefresh;
  }
  public override async Task EndRefreshRealTimeAsync(string ticker) {
    await ApiClient.KisWebSocket.Unsubscribe("HDFSASP0", $"D{CurrentExchange.GetCode()}{ticker}");
  }
}