using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model.Charts;

public partial class CandlestickChartData : ObservableObject {
  public class LoadedEventArgs() : EventArgs() {
    public required ImmutableList<ChartOHLC> WholeCandles { get; init; }
  }
  public class UpdatedEndEventArgs(ChartOHLC candle) : EventArgs() {
    public ChartOHLC Candle { get; init; } = candle;
    public required bool IsAppending { get; init; }
  }
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
  public ObservableCollection<ChartOHLC> Candles { get; private set; }
  [ObservableProperty]
  public partial DateTimeOffset? ChartDateBegin { get; set; }
  [ObservableProperty]
  public partial DateTimeOffset? ChartDateEnd { get; set; }
  [ObservableProperty]
  public partial CandlePeriod Span { get; set; }
  public TimeSpan TimeSpan => ToTimeSpan(Span);
  public ObservableCollection<CandlePeriod> AvailableCandlePeriod { get; set; }

  public event EventHandler<LoadedEventArgs> Loaded = default!;
  public event EventHandler<UpdatedEndEventArgs> UpdatedEnd = default!;

  public CandlestickChartData() {
    Span = CandlePeriod.Daily;
    AvailableCandlePeriod = [];
    Candles = [];
    ChartDateBegin = DateTimeOffset.Now.Date.AddDays(-180);
    ChartDateEnd = DateTimeOffset.Now.Date.AddDays(1).AddMilliseconds(-1);
  }
  public void NotifyLoadComplete() {
    lock (Candles) {
      Loaded?.Invoke(this, new() { WholeCandles = [.. Candles] });
    }
  }
  public void ExtendBegin(ChartOHLC ohlc) {
    lock (Candles) {
      if (Candles.Count != 0 && ohlc.Date >= Candles[0].Date) return;
      Candles.Insert(0, ohlc);
    }
  }
  public void ExtendBegin(IEnumerable<ChartOHLC> ohlcs, bool assumeSorted = false) {
    ImmutableList<ChartOHLC> sorted = assumeSorted ? [.. ohlcs] : [.. ohlcs
      .Select(x => {
        var candle = new ChartOHLC() { Date = Floor(x.Date, Span) };
        candle.CopyOHLCFrom(x);
        return candle;
      })
      .OrderByDescending(x => x.Date)
    ];
    lock (Candles) {
      if (Candles.Count != 0 && sorted[^1].Date >= Candles[0].Date) return;
      for (int i = 0; i < sorted.Count; i++) {
        Candles.Insert(0, sorted[i]);
      }
    }
  }
  public void UpdateEnd(ChartOHLC ohlc) {
    bool inserted;
    lock (Candles) {
      ChartOHLC inserting = new() { Date = Floor(ohlc.Date, Span) };
      inserting.CopyOHLCFrom(ohlc);
      if (Candles[^1].Date == inserting.Date) {
        Candles[^1].CopyOHLCFrom(ohlc);
        inserted = false;
      }
      else {
        Candles.Add(inserting);
        inserted = true;
      }
    }
    UpdatedEnd?.Invoke(this, new(ohlc) { IsAppending = inserted });
  }
  public void Clear() {
    lock (Candles) {
      Candles.Clear();
    }
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