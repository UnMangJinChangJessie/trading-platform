using System.Text.Json;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Tmds.DBus.Protocol;
using trading_platform.Model;
using trading_platform.Model.Charts;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockMarketData : MarketData {
  private Exchange CurrentExchange { get; set; } = Exchange.None;
  [ObservableProperty]
  public partial int DecimalDigitCount { get; private set; } = 0;
  [ObservableProperty]
  public partial float EarningsPerShare { get; private set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceBookValueRate { get; private set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceEarningsRate { get; private set; } = 0.0F;

  public OverseaStockMarketData() {
    CurrentOrderBook = new OverseaStockOrderBook();
    CurrentOrder = new OverseaStockOrder();
    PriceChart.AvailableCandlePeriod = [
      CandlestickChartData.CandlePeriod.Daily,
      CandlestickChartData.CandlePeriod.Weekly,
      CandlestickChartData.CandlePeriod.Monthly,
      CandlestickChartData.CandlePeriod.Yearly,
    ];
    PriceChart.Span = CandlestickChartData.CandlePeriod.Daily;
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
      PreviousClose = (args.Message[^1][12] == "5" || args.Message[^1][12] == "4" ? 1 : -1) * decimal.Parse(args.Message[^1][13]) + CurrentClose;
      if (
        DateOnly.TryParseExact(args.Message[^1][4], "yyyyMMdd", out var date) &&
        TimeOnly.TryParseExact(args.Message[^1][5], "HHmmss", out var time)
      ) {
        CurrentDateTime = new(date, time);
        PriceChart.InsertCandle(
          new(CurrentOpen, CurrentHigh, CurrentLow, CurrentClose) {
            Volume = CurrentVolume,
            Amount = CurrentAmount,
            Date = CurrentDateTime
          }
        );
      }
      ChangeDependentValues();
    };

    if (Design.IsDesignMode) {
      var generated = Generators.Series.GenerateBrownianOHLC(24600.0, 0.01, 0.15, TimeSpan.FromDays(1), DateTime.Today, 300);
      PriceChart.InsertCandleRange(generated);
      CurrentClose = PriceChart[0]!.Close;
      CurrentHigh = PriceChart[0]!.High;
      CurrentLow = PriceChart[0]!.Low;
      CurrentOpen = PriceChart[0]!.Open;
      Currency = "pt";
      Name = "NASDAQ 100";
      PreviousClose = PriceChart[1]!.Close;
    }
  }
  private void OnReceiveData(string jsonString) {
    OverseaStockInquireChartResult result;
    try {
      result = JsonSerializer.Deserialize<OverseaStockInquireChartResult>(jsonString, ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (result!.Chart != null) foreach (var candle in result!.Chart) {
        PriceChart.InsertCandle(new(candle.Open, candle.High, candle.Low, candle.Close) {
          Volume = candle.Volume,
          Amount = candle.Amount,
          Date = candle.Date.ToDateTime(TimeOnly.MinValue)
        });
    }
  }
  /// <param name="args">
  /// exchange[Exchange]: 거래소
  /// ticker[string]: 종목코드
  /// </param>
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("exchange", out var exchangeObject) || exchangeObject is not Exchange exchange) return;
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker) is not OverseaStockInformation information) return;
    CurrentExchange = information.Exchange;
    Ticker = ticker;
    Name = information.Name;
    (CurrentOrder as OverseaStockOrder)?.StockExchange = information.Exchange;
    (CurrentOrder as OverseaStockOrder)?.Ticker = information.Ticker;
    (CurrentOrder as OverseaStockOrder)?.Name = information.Name;
    PriceChart.Clear();
    var inquireTo = PriceChart.ChartDateEnd ?? DateTimeOffset.Now.Date;
    var inquireFrom = PriceChart.ChartDateStart ?? DateTimeOffset.Now.Date.AddYears(-5);
    while (inquireTo >= inquireFrom) {
      Model.KoreaInvestment.OverseaStock.GetChart(
        new() {
          Ticker = information.Ticker,
          Exchange = information.Exchange,
          EndDate = DateOnly.FromDateTime(inquireTo.Date),
          CandlePeriod = PriceChart.Span.ToKisCandlePeriod(),
          Adjusted = true
        },
        OnReceiveData
      );
      inquireTo = inquireTo.AddDays(-140);
    }
  }

  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("exchange", out var exchangeObject) || exchangeObject is not Exchange exchange) return;
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker) is not OverseaStockInformation information) return;
    CurrentExchange = information.Exchange;
    Ticker = information.Ticker;
    Name = information.Name;
    (CurrentOrder as OverseaStockOrder)?.StockExchange = information.Exchange;
    (CurrentOrder as OverseaStockOrder)?.Ticker = information.Ticker;
    (CurrentOrder as OverseaStockOrder)?.Name = information.Name;
    DecimalDigitCount = information.DecimalDigitCount;
    var task_1 = ApiClient.KisWebSocket.Subscribe("HDFSCNT0", $"D{CurrentExchange.GetCode()}{ticker}");
    var task_2 = CurrentOrderBook?.StartRefreshRealtimeAsync(args) ?? Task.CompletedTask;
    await task_1;
    await task_2;
  }

  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (CurrentExchange == Exchange.None) return;
    var task_1 = ApiClient.KisWebSocket.Unsubscribe("HDFSCNT0", $"D{CurrentExchange.GetCode()}{Ticker}");
    var task_2 = CurrentOrderBook?.EndRefreshRealtimeAsync(args) ?? Task.CompletedTask;
    await task_1;
    await task_2;
  }
}