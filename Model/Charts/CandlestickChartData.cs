using System.ComponentModel;

namespace trading_platform.Model.Charts;

public class CandlestickChartData {
  public enum CandlePeriod {
    [Description("1분")]
    Minutes_1,
    [Description("5분")]
    Minutes_5,
    [Description("10분")]
    Minutes_10,
    [Description("15분")]
    Minutes_15,
    [Description("30분")]
    Minutes_30,
    [Description("1시간")]
    Hourly,
    [Description("일")]
    Daily,
    [Description("주")]
    Weekly,
    [Description("월")]
    Monthly,
    [Description("년")]
    Yearly,
  }
  public readonly Lock CandlesLock;
  public List<ChartOHLC> Candles { get; private set; }
  public DateTimeOffset? ChartDateStart { get; set; }
  public DateTimeOffset? ChartDateEnd { get; set; }
  public CandlePeriod Span { get; set; }
  public TimeSpan TimeSpan => ToTimeSpan(Span);
  public List<CandlePeriod> AvailableCandlePeriod { get; set; }
  public event EventHandler<ChartOHLC> CandleChanged;
  public event EventHandler<ChartOHLC> CandleInserted;
  public event EventHandler<DateTime> CandleRemoved;
  public event EventHandler Cleared;

  public CandlestickChartData() {
    Span = CandlePeriod.Daily;
    AvailableCandlePeriod = [];
    CandlesLock = new();
    Candles = [];
    ChartDateStart = DateTimeOffset.Now.Date.AddDays(-180);
    ChartDateEnd = DateTimeOffset.Now.Date.AddDays(1).AddMilliseconds(-1);
    CandleChanged = default!;
    CandleInserted = default!;
    CandleRemoved = default!;
    Cleared = default!;
  }
  public void InsertCandle(ChartOHLC ohlc) {
    var floorDate = Floor(ohlc.Date, Span);
    ohlc.Date = floorDate;
    lock (CandlesLock) {
      var candleIndex = Candles.BinarySearch(new() { Date = floorDate }); // 시각이 같은 캔들을 찾음
      if (candleIndex < 0) {
        // 캔들을 삽입할 자리는 이미 BinarySearch가 찾아줌.
        Candles.Insert(~candleIndex, ohlc);
        CandleInserted?.Invoke(this, ohlc);
      }
      else {
        var candle = Candles[candleIndex];
        var candleEqual =
          candle.Open == ohlc.Open &&
          candle.High == ohlc.High &&
          candle.Low == ohlc.Low &&
          candle.Close == ohlc.Close &&
          candle.Volume == ohlc.Volume &&
          candle.Amount == ohlc.Amount;
        if (!candleEqual) {
          candle.CopyOHLCFrom(ohlc);
          CandleChanged?.Invoke(this, candle);
        }
      }
    }
  }
  public void InsertCandleRange(IEnumerable<ChartOHLC> ohlcs) {
    List<ChartOHLC> changedCandles = [];
    List<ChartOHLC> insertedCandles = [];
    lock (CandlesLock) {
      foreach (var ohlc in ohlcs) {
        var floorDate = Floor(ohlc.Date, Span);
        var candleIndex = Candles.BinarySearch(new() { Date = floorDate }); // 시각이 같은 캔들을 찾음
        ohlc.Date = floorDate;
        if (candleIndex < 0) {
          // 캔들을 삽입할 자리는 이미 BinarySearch가 찾아줌.
          Candles.Insert(~candleIndex, ohlc);
          CandleInserted?.Invoke(this, ohlc);
        }
        else {
          var candle = Candles[candleIndex];
          var candleEqual =
            candle.Open == ohlc.Open &&
            candle.High == ohlc.High &&
            candle.Low == ohlc.Low &&
            candle.Close == ohlc.Close &&
            candle.Volume == ohlc.Volume &&
            candle.Amount == ohlc.Amount;
          if (!candleEqual) {
            candle.CopyOHLCFrom(ohlc);
            CandleChanged?.Invoke(this, candle);
          }
        }
      }
    }
  }
  public void RemoveCandle(ChartOHLC ohlc) {
    lock (CandlesLock) {
      if (Candles.Remove(ohlc)) {
        CandleRemoved?.Invoke(this, ohlc.Date);
      }
    }
  }
  public void RemoveCandle(DateTime date) {
    lock (CandlesLock) {
      if (Candles.Remove(new() { Date = Floor(date, Span) })) {
        CandleRemoved?.Invoke(this, date);
      }
    }
  }
  public void RemoveCandle(double oaDate) {
    lock (CandlesLock) {
      if (Candles.Remove(new() { Date = Floor(DateTime.FromOADate(oaDate), Span) })) {
        CandleRemoved?.Invoke(this, DateTime.FromOADate(oaDate));
      }
    }
  }
  public void Clear() {
    lock (CandlesLock) {
      Candles.Clear();
    }
    Cleared?.Invoke(this, EventArgs.Empty);
  }
  /// <summary>
  /// 최근 캔들부터 차례대로 0, 1, 2, 3, ...과 같이 접근할 수 있습니다.
  /// </summary>
  public ChartOHLC? this[int idx] {
    get => Candles.SkipLast(idx).LastOrDefault();
  }
  private static DateTime Floor(DateTime dt, CandlePeriod period) {
    return period switch {
      CandlePeriod.Minutes_1 => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute, second: 0),
      CandlePeriod.Minutes_5 => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 5 * 5, second: 0),
      CandlePeriod.Minutes_10 => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 10 * 10, second: 0),
      CandlePeriod.Minutes_15 => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 15 * 15, second: 0),
      CandlePeriod.Minutes_30 => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 30 * 30, second: 0),
      CandlePeriod.Hourly => new(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: 0, second: 0),
      CandlePeriod.Daily => dt.Date,
      CandlePeriod.Weekly => GetMostRecentMonday(dt),
      CandlePeriod.Monthly => new(year: dt.Year, month: dt.Month, day: 1),
      CandlePeriod.Yearly => new(year: dt.Year, month: 1, day: 1),
      _ => throw new ArgumentException("Invalid CandlePeriod value.")
    };
    static DateTime GetMostRecentMonday(DateTime date) {
      var result = date.Date;
      while (result.DayOfWeek != DayOfWeek.Monday) {
        result = result.AddDays(-1);
      }
      return result;
    }
  }
  private static TimeSpan ToTimeSpan(CandlePeriod period) {
    return period switch {
      CandlePeriod.Minutes_1 => TimeSpan.FromMinutes(1),
      CandlePeriod.Minutes_5 => TimeSpan.FromMinutes(5),
      CandlePeriod.Minutes_10 => TimeSpan.FromMinutes(10),
      CandlePeriod.Minutes_15 => TimeSpan.FromMinutes(15),
      CandlePeriod.Minutes_30 => TimeSpan.FromMinutes(30),
      CandlePeriod.Hourly => TimeSpan.FromHours(1),
      CandlePeriod.Daily => TimeSpan.FromDays(1),
      CandlePeriod.Weekly => TimeSpan.FromDays(7),
      CandlePeriod.Monthly => TimeSpan.FromDays(365.23 / 12),
      CandlePeriod.Yearly => TimeSpan.FromDays(365.23),
      _ => throw new ArgumentException("Invalid CandlePeriod value")
    };
  }
}