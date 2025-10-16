using System.Text.Json;
using Avalonia.Controls;
using trading_platform.Model;
using trading_platform.Model.Charts;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;
using static trading_platform.Model.KoreaInvestment.DomesticStock;

namespace trading_platform.ViewModel.KoreaInvestment;

public class StockMarketData : MarketData {
  public float EarningsPerShare { get; private set; } = 0.0F;
  public float PriceBookValueRate { get; private set; } = 0.0F;
  public float PriceEarningsRate { get; private set; } = 0.0F;
  public KRXSecuritiesType SecuritiesType { get; private set; } = KRXSecuritiesType.Unknown;
  public Exchange TickerExchange { get; private set; }

  public StockMarketData() {
    CurrentOrderBook = new StockOrderBook();
    CurrentOrder = new StockOrder();
    PriceChart.Span = CandlestickChartData.CandlePeriod.Daily;
    PriceChart.AvailableCandlePeriod = [
      CandlestickChartData.CandlePeriod.Daily,
      CandlestickChartData.CandlePeriod.Weekly,
      CandlestickChartData.CandlePeriod.Monthly,
      CandlestickChartData.CandlePeriod.Yearly,
    ];
    ApiClient.KisWebSocket.MessageReceived += OnReceivedRealtime;
    if (Design.IsDesignMode) {
      var generated = Generators.Series.GenerateBrownianOHLC(450.0, -0.01, 0.2, TimeSpan.FromDays(1), DateTime.Today, 300);
      PriceChart.InsertCandleRange(generated);
      CurrentClose = PriceChart[0]!.Close;
      CurrentHigh = PriceChart[0]!.High;
      CurrentLow = PriceChart[0]!.Low;
      CurrentOpen = PriceChart[0]!.Open;
      CurrentVolume = PriceChart[0]!.Volume;
      Currency = "pt";
      Name = "KOSPI200";
      PreviousClose = PriceChart[1]!.Close;
    }
  }
  ~StockMarketData() {
    ApiClient.KisWebSocket.MessageReceived -= OnReceivedRealtime;
  }
  private void OnReceivedRealtime(object? sender, (string TransactionId, List<string[]> Message) args) {
      if (args.TransactionId != "H0UNCNT0" && args.TransactionId != "H0STCNT0" && args.TransactionId != "H0NXCNT0") return; // 통합 | KRX | NexTrade
      if (args.Message.Count == 0) return;
      for (int i = args.Message.Count - 1; i < args.Message.Count; i++) { // 현재는 체결 목록을 저장하지 않기에 마지막 행만 읽음. 
        var row = args.Message[i];
        if (row[0] != Ticker) return;
        // 필요한 데이터 순서는 셋 다 동일함.
        CurrentClose  = decimal.Parse(row[2]);
        CurrentOpen   = decimal.Parse(row[7]);
        CurrentHigh   = decimal.Parse(row[8]);
        CurrentLow    = decimal.Parse(row[9]);
        CurrentVolume = decimal.Parse(row[13]);
        CurrentAmount = decimal.Parse(row[14]);
        PreviousClose = -decimal.Parse(row[4]) + CurrentClose;
        if (
          TimeOnly.TryParseExact(row[1], "HHmmss", out var time) &&
          DateOnly.TryParseExact(row[33], "yyyyMMdd", out var date)
        ) {
          CurrentDateTime = new(date, time);
          PriceChart.InsertCandle(
            new(CurrentOpen, CurrentHigh, CurrentLow, CurrentClose) {
              Date = CurrentDateTime,
              Volume = CurrentVolume,
              Amount = CurrentAmount
            }
          );
        }
      }
      ChangeDependentValues();
      Currency = "원";
    }
  private async void OnRequestSuccess(string jsonString, object? args) {
    ChartResult? body;
    try {
      body = JsonSerializer.Deserialize<ChartResult>(jsonString, options: ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (body == null || body.ReturnCode != 0) return;
    foreach (var candle in body.Chart!) {
      PriceChart.InsertCandle(new(candle.Open, candle.High, candle.Low, candle.Close) {
        Date = candle.Date.ToDateTime(TimeOnly.MinValue),
        Volume = candle.Volume,
        Amount = candle.Amount
      });
    }
    CurrentOpen = PriceChart[0]?.Open ?? 0;
    CurrentHigh = PriceChart[0]?.High ?? 0;
    CurrentLow = PriceChart[0]?.Low ?? 0;
    CurrentClose = PriceChart[0]?.Close ?? 0;
    CurrentVolume = PriceChart[0]?.Volume ?? 0;
    CurrentAmount = PriceChart[0]?.Amount ?? 0;
    PreviousClose = PriceChart[1]?.Close ?? 0;
    Name = body.Information!.Name;
    PriceEarningsRate = body.Information!.PriceEarningsRate;
    PriceBookValueRate = body.Information!.PriceBookValueRatio;
    EarningsPerShare = body.Information!.EarningsPerShare;
    ChangeDependentValues();
    if (args is bool v && v) {
      await StartRefreshRealtimeAsync();
    }
  }
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (KRXStock.SearchByTicker(ticker) is not KRXStockInformation information) return;
    Ticker = information.Ticker;
    TickerExchange = information.Exchange;
    SecuritiesType = information.SecuritiesType;
    var inquireFrom = PriceChart.ChartDateStart ?? DateTimeOffset.Now.AddYears(-5);
    var inquireTo = PriceChart.ChartDateEnd ?? DateTimeOffset.Now;
    PriceChart.Clear();
    while (inquireTo >= inquireFrom) {
      var minDate = inquireTo.AddDays(-139);
      var nextInquireTo = inquireTo.AddDays(-140);
      var isLast = nextInquireTo < inquireFrom;
      GetChart(new() {
        Ticker = ticker,
        Exchange = information.Exchange,
        From = DateOnly.FromDateTime(minDate > inquireFrom ? minDate.Date : inquireFrom.Date), // 봉 최대 100건 조회. 평일 휴일을 고려하지 않을 때 한 번에 최대 100 * 7 / 5 = 140일 조회 가능.
        To = DateOnly.FromDateTime(inquireTo.Date),
        CandlePeriod = PriceChart.Span.ToKisCandlePeriod(),
        Adjusted = true
      }, OnRequestSuccess, isLast);
      inquireTo = nextInquireTo;
    };
    if (CurrentOrderBook != null) await CurrentOrderBook.RefreshAsync(args);
  }
  private async Task StartRefreshRealtimeAsync() {
    var task = TickerExchange switch {
      Exchange.NexTrade => ApiClient.KisWebSocket.Subscribe("H0NXCNT0", Ticker),
      Exchange.KoreaExchange => ApiClient.KisWebSocket.Subscribe("H0STCNT0", Ticker),
      _ => ApiClient.KisWebSocket.Subscribe("H0UNCNT0", Ticker),
    };
    await task;
  }
  private async Task EndRefreshRealtimeAsync() {
    var task = TickerExchange switch {
      Exchange.NexTrade => ApiClient.KisWebSocket.Unsubscribe("H0NXCNT0", Ticker),
      Exchange.KoreaExchange => ApiClient.KisWebSocket.Unsubscribe("H0STCNT0", Ticker),
      _ => ApiClient.KisWebSocket.Unsubscribe("H0UNCNT0", Ticker),
    };
    await task;
  }
}