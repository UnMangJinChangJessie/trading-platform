using System.Collections.Immutable;
using Avalonia;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class MovingAverageConvergenceDivergence : Indicator {
  public override string LegendText => $"MACD({Lookback_1}, {Lookback_2})";
  public class MacdResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public double Average_1 { get; set; }
    public double Average_2 { get; set; }
    public double Value { get; set; }
  };
  public BarStyle BarStyle { get; private set; } = new();
  public int Lookback_1 {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles]});
      }
    }
  }
  public int Lookback_2 {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles]});
      }
    }
  }
  public List<MacdResult> BaseResults { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  public MovingAverageConvergenceDivergence(CandlestickChartData chart, int lookback_1, int lookback_2) : base(chart) {
    RenderingResults = [];
    BaseResults = [];
    Lookback_1 = lookback_1;
    Lookback_2 = lookback_2;
  }
  public override AxisLimits GetAxisLimits() {
    if (RenderingResults.Length == 0) return AxisLimits.Unset;
    else return new(
      left: RenderingResults[0].Date.ToOADate(),
      right: RenderingResults[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: RenderingResults.Min(x => x.Value), RenderingResults.Max(x => x.Value)
    );
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (BaseResults) RenderingResults = [.. BaseResults];
      ResetUpdateTime();
    }
    if (RenderingResults.Length == 0) return;
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    var rectValues = RenderingResults
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return range.Min - margin <= date && date <= range.Max + margin;
      })
      .Select(x => {
        var pixelTopLeft = rp.Plot.GetPixel(new Coordinates(x.Date.ToOADate() - BaseChart.TimeSpan.TotalDays / 2, Math.Max(0.0, x.Value)));
        var pixelBottomRight = rp.Plot.GetPixel(new Coordinates(x.Date.ToOADate() + BaseChart.TimeSpan.TotalDays / 2, Math.Min(0.0, x.Value)));
        return (
          new ScottPlot.PixelRect(left: pixelTopLeft.X, right: pixelBottomRight.X, top: pixelTopLeft.Y, bottom: pixelBottomRight.Y),
          x.Value
        );
      });
    if (!rectValues.Any()) return;
    var previousValue = 0.0;
    foreach (var (rect, value) in rectValues) {
      var fill = value >= 0 ?
        (previousValue < value ? BarStyle.PositiveBarIncreasingFill : BarStyle.PositiveBarDecreasingFill) :
        (previousValue < value ? BarStyle.NegativeBarIncreasingFill : BarStyle.NegativeBarDecreasingFill);
      var line = value >= 0 ?
        (previousValue < value ? BarStyle.PositiveBarIncreasingLine : BarStyle.PositiveBarDecreasingLine) :
        (previousValue < value ? BarStyle.NegativeBarIncreasingLine : BarStyle.NegativeBarDecreasingLine);
      Drawing.FillRectangle(rp.Canvas, rect, rp.Paint, fill);
      Drawing.DrawPath(rp.Canvas, rp.Paint, [rect.BottomLeft, rect.BottomRight, rect.TopRight, rect.TopLeft], line, close: true);
    }
    Drawing.DrawLine(
      canvas: rp.Canvas,
      paint: rp.Paint,
      pt1: rp.Plot.GetPixel(coordinates: new(RenderingResults[0].Date.ToOADate(), 0)),
      pt2: rp.Plot.GetPixel(coordinates: new(RenderingResults[^1].Date.ToOADate(), 0)),
      color: Colors.Black
    );
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (BaseResults) {
      BaseResults.Clear();
      double alpha_1 = 2.0 / (Lookback_1 + 1);
      double alpha_2 = 2.0 / (Lookback_2 + 1);
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var candle = chart[i];
        var close = (double)candle.Close;
        var result = GetExtensionMacd(i, close);
        result.Date = candle.Date;
        BaseResults.Add(result);
      }
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    double alpha_1 = 2.0 / (Lookback_1 + 1);
    double alpha_2 = 2.0 / (Lookback_2 + 1);
    lock (BaseResults) {
      if (BaseResults.Count == 0) return;
      var count = BaseResults.Count;
      var close = (double)args.Candle.Close;
      var date = args.Candle.Date;
      if (args.IsAppending) {
        var result = GetExtensionMacd(count, close);
        result.Date = date;
        BaseResults.Add(result);
      }
      else {
        BaseResults[^1].Value = GetExtensionMacd(count - 1, close).Value;
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