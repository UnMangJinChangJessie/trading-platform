using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.Charts.Indicators;

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
  public ObservableCollection<Indicator> Indicators { get; private set; }
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
    Indicators = [];
    ChartDateBegin = DateTimeOffset.Now.Date.AddDays(-180);
    ChartDateEnd = DateTimeOffset.Now.Date.AddDays(1).AddTicks(-1);
    if (Avalonia.Controls.Design.IsDesignMode || Debugger.IsAttached) {
      // 예시 데이터: 로그 정규분포 곡선
      ChartDateBegin = DateTime.Today.AddDays(-499);
      ChartDateEnd = DateTime.Today;
      var logNormal = Generators.Series.GenerateBrownianOHLC(100.0, 0.0274, 2.0, TimeSpan.FromDays(1), ChartDateBegin.Value.DateTime, 500);
      foreach (var candle in logNormal) {
        Candles.Add(candle);
      }
    }
  }
  public void NotifyLoadComplete() {
    var args = new LoadedEventArgs() { WholeCandles = [.. Candles] };
    lock (Candles) {
      Loaded?.Invoke(this, args);
    }
    foreach (var indicator in Indicators) {
      indicator.Reset(this, args);
    }
  }
  public void ExtendBegin(ChartOHLC ohlc) {
    lock (Candles) {
      if (Candles.Count != 0 && ohlc.Date >= Candles[0].Date) return;
      Candles.Insert(0, ohlc);
    }
  }
  public void ExtendBegin(IEnumerable<ChartOHLC> ohlcs, bool assumeSorted = false) {
    ImmutableList<ChartOHLC> sorted = assumeSorted ? [.. ohlcs
      .Select(x => {
        var candle = new ChartOHLC(x.Open, x.High, x.Low, x.Close) { Volume = x.Volume, Amount = x.Amount, Date = Floor(x.Date, Span) };
        candle.Span = Ceiling(x.Date, Span) - candle.Date;
        return candle;
      })] :
      [.. ohlcs
        .Select(x => {
          var candle = new ChartOHLC(x.Open, x.High, x.Low, x.Close) { Volume = x.Volume, Amount = x.Amount, Date = Floor(x.Date, Span) };
          candle.Span = Ceiling(x.Date, Span) - candle.Date;
          return candle;
        })
        .OrderByDescending(x => x.Date)];
    lock (Candles) {
      for (int i = 0; i < sorted.Count; i++) {
        Candles.Insert(0, sorted[i]);
      }
    }
  }
  public void UpdateEnd(ChartOHLC ohlc) {
    bool inserted;
    lock (Candles) {
      ChartOHLC inserting = new(ohlc.Open, ohlc.High, ohlc.Low, ohlc.Close) { Volume = ohlc.Volume, Amount = ohlc.Amount, Date = Floor(ohlc.Date, Span) };
      inserting.Span = Ceiling(ohlc.Date, Span) - inserting.Date;
      if (Candles[^1].Date == inserting.Date) {
        Candles[^1] = inserting;
        inserted = false;
      }
      else {
        Candles.Add(inserting);
        inserted = true;
      }
    }
    var args = new UpdatedEndEventArgs(ohlc) { IsAppending = inserted };
    UpdatedEnd?.Invoke(this, args);
    foreach (var indicator in Indicators) {
      indicator.UpdateEnd(this, args);
    }
  }
  public void Clear() {
    lock (Candles) {
      Candles.Clear();
    }
    foreach (var indicator in Indicators) {
      indicator.Clear();
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
  private static DateTime Ceiling(DateTime dt, CandlePeriod period) {
    return period switch {
      CandlePeriod.Minutes_1 => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute + 1, second: 0).AddMinutes(1).AddTicks(-1),
      CandlePeriod.Minutes_5 => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 5 * 5, second: 0).AddMinutes(5).AddTicks(-1),
      CandlePeriod.Minutes_10 => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 10 * 10, second: 0).AddMinutes(10).AddTicks(-1),
      CandlePeriod.Minutes_15 => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 15 * 15, second: 0).AddMinutes(15).AddTicks(-1),
      CandlePeriod.Minutes_30 => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: dt.Minute / 30 * 30, second: 0).AddMinutes(30).AddTicks(-1),
      CandlePeriod.Hourly => new DateTime(year: dt.Year, month: dt.Month, day: dt.Day, hour: dt.Hour, minute: 0, second: 0).AddHours(1).AddTicks(-1),
      CandlePeriod.Daily => dt.Date.AddDays(1),
      CandlePeriod.Weekly => GetMostRecentMonday(dt).AddDays(7).AddTicks(-1),
      CandlePeriod.Monthly => new DateTime(year: dt.Year + dt.Month / 12, month: dt.Month % 12 + 1, day: 1).AddTicks(-1), 
      CandlePeriod.Yearly => new DateTime(year: dt.Year + 1, month: 1, day: 1).AddTicks(-1),
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
  public void AddIndicator(Indicator newIndicator) {
    Indicators.Add(newIndicator);
    ImmutableList<ChartOHLC> candles;
    lock (Candles) {
      candles = [.. Candles];
    }
    newIndicator.Reset(this, new() { WholeCandles = candles });
  }
  public bool WriteBacktestPy(Stream stream) {
    if (!stream.CanWrite) return false;
    using var writer = new StreamWriter(stream);
    writer.WriteLine($"Date,Open,High,Low,Close,Volume");
    lock (Candles) {
      for (int i = 0; i < Candles.Count; i++) {
        var candle = Candles[i];
        var dateString = candle.Date.ToString("yyyy-MM-dd");
        var open = candle.Open.ToString(CultureInfo.InvariantCulture);
        var high = candle.High.ToString(CultureInfo.InvariantCulture);
        var low = candle.Low.ToString(CultureInfo.InvariantCulture);
        var close = candle.Close.ToString(CultureInfo.InvariantCulture);
        var volume = candle.Volume.ToString(CultureInfo.InvariantCulture);
        writer.WriteLine($"{dateString},{open},{high},{low},{close},{volume}");
      }
    }
    return true;
  }
  public bool WriteEverything(Stream stream) {
    if (!stream.CanWrite) return false;
    using var writer = new StreamWriter(stream);
    writer.Write($"영업일자,시가,고가,저가,종가,거래량,거래대금,평균단가");
    foreach (var indicator in Indicators) {
      // 거래량이나 거래대금의 경우에는 이미 열이 존재하므로 무시함.
      if (indicator is Volume) continue;
      writer.Write($",{indicator.LegendText}");
    }
    writer.WriteLine();
    lock (Candles) {
      for (int i = 0; i < Candles.Count; i++) {
        var candle = Candles[i];
        var dateString = $"{candle.Date:yyyy-MM-dd}T{candle.Date:hh:mm:sszzz}";
        var averagePrice = candle.Volume == 0 ? 0 : candle.Amount / candle.Volume;
        var open = candle.Open.ToString(CultureInfo.InvariantCulture);
        var high = candle.High.ToString(CultureInfo.InvariantCulture);
        var low = candle.Low.ToString(CultureInfo.InvariantCulture);
        var close = candle.Close.ToString(CultureInfo.InvariantCulture);
        var volume = candle.Volume.ToString(CultureInfo.InvariantCulture);
        var amount = candle.Amount.ToString(CultureInfo.InvariantCulture);
        writer.Write($"{dateString},{open},{high},{low},{close},{volume},{amount},{averagePrice}");
        foreach (var indicator in Indicators) {
          if (indicator is Volume) continue;
          var indicatorValue = indicator.RenderingResults[i].Value;
          if (double.IsFinite(indicatorValue)) writer.Write($",{indicatorValue}");
          else writer.Write(",");
        }
        writer.WriteLine();
      }
    }
    return true;
  }
}