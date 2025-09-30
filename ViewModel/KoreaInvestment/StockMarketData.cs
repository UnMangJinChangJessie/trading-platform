using Avalonia.Controls;
using Tmds.DBus.Protocol;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public class StockMarketData : MarketData {
  public float EarningsPerShare { get; private set; } = 0.0F;
  public float PriceBookValueRate { get; private set; } = 0.0F;
  public float PriceEarningsRate { get; private set; } = 0.0F;

  public StockMarketData() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "H0UNCNT0") return;
      if (args.Message.Count == 0) return;
      if (args.Message[^1][0] != Ticker) return;
      CurrentClose = decimal.Parse(args.Message[^1][2]);
      CurrentOpen = decimal.Parse(args.Message[^1][7]);
      CurrentHigh = decimal.Parse(args.Message[^1][8]);
      CurrentLow = decimal.Parse(args.Message[^1][9]);
      CurrentVolume = decimal.Parse(args.Message[^1][13]);
      CurrentAmount = decimal.Parse(args.Message[^1][14]);
      if (
        TimeOnly.TryParseExact(args.Message[^1][1], "HHmmss", out var time) &&
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
      var generated = Generators.Series.GenerateBrownianOHLC(450.0, -0.01, 0.2, TimeSpan.FromDays(1), DateTime.Today, 300);
      PriceChart = [..generated];
      CurrentClose = PriceChart[^1].Close;
      CurrentHigh = PriceChart[^1].High;
      CurrentLow = PriceChart[^1].Low;
      CurrentOpen = PriceChart[^1].Open;
      Currency = "pt";
      Name = "KOSPI200";
      PreviousClose = PriceChart[^2].Close;
    }
  }
  public override async ValueTask<bool> RequestRefreshAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is not KRXStockInformation stockInformation) return false;
    ClearChart();
    var inquireFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
    var inquireTo = DateOnly.FromDateTime(DateTime.Today);
    while (inquireTo >= inquireFrom) {
      var (status, result) = await DomesticStock.InquireStockChart(new() {
        Ticker = ticker,
        Exchange = Exchange.KoreaExchange,
        From = inquireTo.AddDays(-139), // 봉 최대 100건 조회. 평일 휴일을 고려하지 않을 때 한 번에 최대 100 * 7 / 5 = 140일 조회 가능.
        To = inquireTo,
        CandlePeriod = CandlePeriod.Daily,
        Adjusted = true
      });
      if (status != System.Net.HttpStatusCode.OK || result == null) return false;
      if (result.Information == null || result.Chart == null) return false;
      Ticker = result.Information.Ticker;
      Name = result.Information.Name;
      CurrentOpen = result.Information?.Open ?? 0;
      CurrentHigh = result.Information?.High ?? 0;
      CurrentLow = result.Information?.Low ?? 0;
      CurrentClose = result.Information?.Close ?? 0;
      CurrentVolume = result.Information?.Volume ?? 0;
      CurrentAmount = result.Information?.Amount ?? 0;
      PreviousClose = result.Information?.PreviousClose ?? 0;
      EarningsPerShare = result.Information?.EarningsPerShare ?? 0;
      PriceEarningsRate = result.Information?.PriceEarningsRate ?? 0;
      PriceBookValueRate = result.Information?.PriceBookValueRatio ?? 0;
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
    Ticker = ticker;
    await ApiClient.KisWebSocket.Subscribe("H0UNCNT0", ticker);
    return ApiClient.KisWebSocket.ClientState == System.Net.WebSockets.WebSocketState.Open;
    
  }

  public override async Task EndRefreshRealTimeAsync(string ticker) {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNCNT0", ticker);
  }
}