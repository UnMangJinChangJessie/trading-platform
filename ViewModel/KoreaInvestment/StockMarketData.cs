using Avalonia.Controls;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public class StockMarketData : MarketData {
  public KisWebSocket Socket { get; private set; } = new();
  public float EarningsPerShare { get; private set; } = 0.0F;
  public float PriceBookValueRate { get; private set; } = 0.0F;
  public float PriceEarningsRate { get; private set; } = 0.0F;

  public StockMarketData() {
    Socket.OnMessage += OnPropertyChangedRealTime;
    // if (true) {
    if (Design.IsDesignMode) {
      PriceChart = new(
        Generators.Series.GenerateBrownianOHLC(450.0, -0.01, 0.2, TimeSpan.FromDays(1), DateTime.Today, 300)
          .Select(x => new Reactive<OHLC<decimal>>(x))
      );
      CurrentClose = PriceChart[^1].Value.Close;
      CurrentHigh = PriceChart[^1].Value.High;
      CurrentLow = PriceChart[^1].Value.Low;
      CurrentOpen = PriceChart[^1].Value.Open;
      Currency = "pt";
      Name = "KOSPI200";
      PreviousClose = PriceChart[^2].Value.Close;
    }
  }
  public void OnPropertyChangedRealTime(object? sender, string id, List<string[]> tokens) {
    if (tokens.Count == 0) return;
    CurrentClose = decimal.Parse(tokens[^1][2]);
    CurrentOpen = decimal.Parse(tokens[^1][7]);
    CurrentHigh = decimal.Parse(tokens[^1][8]);
    CurrentLow = decimal.Parse(tokens[^1][9]);
    CurrentVolume = decimal.Parse(tokens[^1][13]);
    CurrentAmount = decimal.Parse(tokens[^1][14]);
    // DateTime = new(DateOnly.ParseExact(tokens[^1][33], "yyyyMMdd"), TimeOnly.ParseExact(tokens[^1][1], "hhmmss"));
    if (
      TimeOnly.TryParseExact(tokens[^1][1], "hhmmss", out var time) &&
      DateOnly.TryParseExact(tokens[^1][33], "yyyyMMdd", out var date)
    ) {
      DateTime = new(date, time);
    }
    
  }
  public override async ValueTask<bool> RequestRefreshAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is not KRXStockInformation stockInformation) return false;
    PriceChart.Clear();
    var inquireFrom = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));
    var inquireTo = DateOnly.FromDateTime(DateTime.Today);
    while (inquireTo < inquireFrom) {
      var (status, result) = await DomesticStock.InquireStockChart(new() {
        Ticker = ticker,
        Exchange = Exchange.DomesticUnified,
        From = inquireTo.AddDays(-140), // 봉 최대 100건 조회. 평일 휴일을 고려하지 않을 때 한 번에 최대 100 * 7 / 5 = 140일 조회 가능.
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
      foreach (var (candle, idx) in result.Chart.Select((x, i) => (x, i))) {
        PriceChart.Insert(0, new(new(candle.Open, candle.High, candle.Low, candle.Close)));
        VolumeChart.Insert(0, new(candle.Volume));
        AmountChart.Insert(0, new(candle.Amount));
      }
      inquireTo = inquireTo.AddDays(-140);
    }
    return true;
  }

  public override async ValueTask<bool> RequestRefreshRealTimeAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is null) return false;
    RealTimeRefresh = await Socket.StartReceivingAsync("/tryitout/H0UNCNT0", "H0UNCNT0", ticker);
    return RealTimeRefresh;
    
  }

  public override async Task EndRefreshRealTimeAsync() {
    await Socket.StopReceivingAsync();
    RealTimeRefresh = false;
  }
}