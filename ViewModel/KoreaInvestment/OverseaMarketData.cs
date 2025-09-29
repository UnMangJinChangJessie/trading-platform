using Avalonia.Controls;
using Tmds.DBus.Protocol;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public class OverseaStockMarketData : MarketData {
  public float EarningsPerShare { get; private set; } = 0.0F;
  public float PriceBookValueRate { get; private set; } = 0.0F;
  public float PriceEarningsRate { get; private set; } = 0.0F;

  public OverseaStockMarketData() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "H0UNCNT0") return;
      if (args.Message.Count == 0) return;
      CurrentClose = decimal.Parse(args.Message[^1][2]);
      CurrentOpen = decimal.Parse(args.Message[^1][7]);
      CurrentHigh = decimal.Parse(args.Message[^1][8]);
      CurrentLow = decimal.Parse(args.Message[^1][9]);
      CurrentVolume = decimal.Parse(args.Message[^1][13]);
      CurrentAmount = decimal.Parse(args.Message[^1][14]);
      if (
        TimeOnly.TryParseExact(args.Message[^1][1], "hhmmss", out var time) &&
        DateOnly.TryParseExact(args.Message[^1][33], "yyyyMMdd", out var date)
      ) {
        CurrentDateTime = new(date, time);
        if (PriceChart.Count == 0 || date != DateOnly.FromDateTime(PriceChart[^1].DateTime)) {
          InsertCandleEnd(CurrentOpen, CurrentHigh, CurrentLow, CurrentClose, CurrentVolume, CurrentAmount, CurrentDateTime, TimeSpan.FromDays(1));
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
  public override async ValueTask<bool> RequestRefreshAsync(Exchange exchange, string ticker) {
    if (StockMarketInformation.OverseaStock.SearchByTicker(exchange, ticker) is not OverseaStockInformation stockInformation) return false;
    ClearChart();
    var inquireFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
    var inquireTo = DateOnly.FromDateTime(DateTime.Today);
    while (inquireTo >= inquireFrom) {
      var (status, result) = await Model.KoreaInvestment.OverseaStock.InquireOverseaStockChart(new() {
        Ticker = ticker,
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
      inquireTo = inquireTo.AddDays(-140);
    }
    return true;
  }

  public override async ValueTask<bool> RequestRefreshRealTimeAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is null) return false;
    await ApiClient.KisWebSocket.Subscribe("H0UNCNT0", ticker);
    RealTimeRefresh = true;
    return RealTimeRefresh;
    
  }

  public override async Task EndRefreshRealTimeAsync(string ticker) {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNCNT0", ticker);
    RealTimeRefresh = false;
  }
}