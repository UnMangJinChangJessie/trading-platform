using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;
using System.ComponentModel;

namespace trading_platform.Model.Charts.Indicators;

[Description("이동평균수렴확산지표(MACD)")]
public class MovingAverageConvergenceDivergence : Indicator {
  public override string LegendText => $"MACD({Lookback_1}, {Lookback_2})";
  public class MacdResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
    public double Average_1 { get; set; }
    public double Average_2 { get; set; }
    public double Value { get; set; }
  };
  [IndicatorParameter(ParameterName = "기간 1")]
  public int Lookback_1 {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles]});
        OnPropertyChanged(nameof(Lookback_1));
        OnPropertyChanged(nameof(LegendText));
      }
    }
  }
  [IndicatorParameter(ParameterName = "기간 2")]
  public int Lookback_2 {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles] });
        OnPropertyChanged(nameof(Lookback_2));
        OnPropertyChanged(nameof(LegendText));
      }
    }
  }
  [IndicatorParameter(ParameterName = "막대 모양새")]
  public BarStyle BarStyle { get; private set; } = new();

  public List<MacdResult> BaseResults { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  public MovingAverageConvergenceDivergence(CandlestickChartData chart, int lookback_1, int lookback_2) : base(chart) {
    RenderingResults = [];
    BaseResults = [];
    Lookback_1 = lookback_1;
    Lookback_2 = lookback_2;
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (BaseResults) RenderingResults = [.. BaseResults];
      ResetUpdateTime();
    }
    if (RenderingResults.Length == 0) return;
    var bars = RenderingResults
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return range.Min - margin <= date && date <= range.Max + margin;
      })
      .Select(x => new Bar() { Value = x.Value, Position = x.Date.ToOADate(), Size = x.Span.TotalDays });
    if (!bars.Any()) return;
    double? previousValue = null;
    foreach (Bar bar in bars) {
      var positive = bar.Value > 0;
      var notDecreased = !previousValue.HasValue || (previousValue.Value <= bar.Value);
      bar.FillStyle = positive ? (notDecreased ? BarStyle.PositiveBarIncreasingFill : BarStyle.PositiveBarDecreasingFill) :
        (notDecreased ? BarStyle.NegativeBarIncreasingFill : BarStyle.NegativeBarDecreasingFill);
      bar.LineStyle = positive ? (notDecreased ? BarStyle.PositiveBarIncreasingLine : BarStyle.PositiveBarDecreasingLine) :
        (notDecreased ? BarStyle.NegativeBarIncreasingLine : BarStyle.NegativeBarDecreasingLine);
      bar.RenderBody(rp, Axes, rp.Paint);
      previousValue = bar.Value;
    }
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (BaseResults) {
      BaseResults.Clear();
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var candle = chart[i];
        var close = (double)candle.Close;
        var result = GetExtensionMacd(i, close);
        result.Date = candle.Date;
        result.Span = candle.Span;
        BaseResults.Add(result);
      }
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (BaseResults) {
      if (BaseResults.Count == 0) return;
      var count = BaseResults.Count;
      var close = (double)args.Candle.Close;
      var date = args.Candle.Date;
      if (args.IsAppending) {
        var result = GetExtensionMacd(count, close);
        result.Span = args.Candle.Span;
        result.Date = date;
        BaseResults.Add(result);
      }
      else {
        BaseResults[^1].Value = GetExtensionMacd(count - 1, close).Value;
        BaseResults[^1].Span = args.Candle.Span;
      }
    }
    base.UpdateEnd(sender, args);
  }
  internal MacdResult GetExtensionMacd(int insertingIndex, double close) {
    if (insertingIndex == 0) return new() { Average_1 = close, Average_2 = close, Value = 0.0 };
    else {
      var meow_1 = double.Lerp(BaseResults[insertingIndex - 1].Average_1, close, 2.0 / (1 + Lookback_1));
      var meow_2 = double.Lerp(BaseResults[insertingIndex - 1].Average_2, close, 2.0 / (1 + Lookback_2));
      return new() {
        Average_1 = meow_1,
        Average_2 = meow_2,
        Value = meow_1 - meow_2
      };
    }
  }
}