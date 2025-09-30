using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Tmds.DBus.Protocol;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockMarketData : MarketData {
  private Exchange CurrentExchange { get; set; } = Exchange.None;
  [ObservableProperty]
  public partial float EarningsPerShare { get; private set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceBookValueRate { get; private set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceEarningsRate { get; private set; } = 0.0F;

  public OverseaStockMarketData() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "HDFSCNT0") return;
      if (args.Message.Count == 0) return;
      if (args.Message[^1][1] != Ticker) return;
      CurrentOpen = decimal.Parse(args.Message[^1][8]);
      CurrentHigh = decimal.Parse(args.Message[^1][9]);
      CurrentLow = decimal.Parse(args.Message[^1][10]);
      CurrentClose = decimal.Parse(args.Message[^1][11]);
      CurrentVolume = decimal.Parse(args.Message[^1][20]);
      CurrentAmount = decimal.Parse(args.Message[^1][21]);
      PreviousClose = -decimal.Parse(args.Message[^1][13]) + CurrentClose;
      if (
        TimeOnly.TryParseExact(args.Message[^1][7], "HHmmss", out var time) &&
        DateOnly.TryParseExact(args.Message[^1][8], "yyyyMMdd", out var date)
      ) {
        CurrentDateTime = new(date, time);
        if (PriceChart.Count == 0 || date != DateOnly.FromDateTime(PriceChart[^1].DateTime)) {
          InsertCandleEnd(CurrentOpen, CurrentHigh, CurrentLow, CurrentClose, CurrentVolume, CurrentAmount, CurrentDateTime.Date, TimeSpan.FromDays(1));
        }
        else {
          UpdateLastCandle(CurrentOpen, CurrentHigh, CurrentLow, CurrentClose, CurrentVolume, CurrentAmount);
        }
      }
    };
    // if (true) {
    if (Design.IsDesignMode) {
      var generated = Generators.Series.GenerateBrownianOHLC(24600.0, 0.01, 0.15, TimeSpan.FromDays(1), DateTime.Today, 300);
      PriceChart = [..generated];
      CurrentClose = PriceChart[^1].Close;
      CurrentHigh = PriceChart[^1].High;
      CurrentLow = PriceChart[^1].Low;
      CurrentOpen = PriceChart[^1].Open;
      Currency = "pt";
      Name = "NASDAQ 100";
      PreviousClose = PriceChart[^2].Close;
    }
  }
  // 종목명 앞에 거래소 코드 세 글자를 작성해주세요.
  public override async ValueTask<bool> RequestRefreshAsync(string ticker) {
    var exchange = StockMarketInformation.OverseaStock.GetExchange(ticker[..3]);
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker[3..]) is not OverseaStockInformation stockInformation) return false;
    ClearChart();
    var inquireFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
    var inquireTo = DateOnly.FromDateTime(DateTime.Today);
    while (inquireTo >= inquireFrom) {
      var (status, result) = await Model.KoreaInvestment.OverseaStock.InquireStockChart(new() {
        Ticker = ticker[3..],
        Exchange = stockInformation.Exchange,
        EndDate = inquireTo,
        CandlePeriod = CandlePeriod.Daily,
        Adjusted = true
      });
      if (status != System.Net.HttpStatusCode.OK || result == null) return false;
      if (result.Chart == null) return false;
      Ticker = stockInformation.Ticker;
      Name = stockInformation.Name;
      foreach (var candle in result.Chart) {
        InsertCandleBegin(
          candle.Open, candle.High, candle.Low, candle.Close,
          candle.Volume, candle.Amount,
          candle.Date.ToDateTime(TimeOnly.MinValue), TimeSpan.FromDays(1)
        );
      }
      if (!result.Chart.Any()) break;
      inquireTo = result.Chart.Last()!.Date.AddDays(-1);
    }
    return true;
  }

  public override async ValueTask<bool> RequestRefreshRealTimeAsync(string ticker) {
    var exchange = StockMarketInformation.OverseaStock.GetExchange(ticker[..3]);
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker[3..]) is not OverseaStockInformation information) return false;
    CurrentExchange = information.Exchange;
    Ticker = ticker[3..];
    await ApiClient.KisWebSocket.Subscribe("HDFSCNT0", $"D{CurrentExchange.GetCode()}{ticker[3..]}");
    return ApiClient.KisWebSocket.ClientState == System.Net.WebSockets.WebSocketState.Open;
    
  }

  public override async Task EndRefreshRealTimeAsync(string ticker) {
    if (CurrentExchange == Exchange.None) return;
    await ApiClient.KisWebSocket.Unsubscribe("HDFSCNT0", $"D{CurrentExchange.GetCode()}{ticker[3..]}");
  }
}