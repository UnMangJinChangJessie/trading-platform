using System.Collections.Immutable;
using Avalonia;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class MovingAverageConvergenceDivergence(CandlestickChartData chart, int lookback_1, int lookback_2) : Indicator(chart) {
  public override string LegendText => $"MACD({Lookback_1}, {Lookback_2})";
  public struct MacdResult {
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
  } = lookback_1;
  public int Lookback_2 {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles]});
      }
    }
  } = lookback_2;
  public List<MacdResult> Results { get; private set; } = [];
  
  public ImmutableArray<MacdResult> Snapshot() {
    lock (Results) {
      return [..Results];
    }
  }
  public override AxisLimits GetAxisLimits() {
    if (Results.Count == 0) return AxisLimits.Unset;
    else return new(
      left: Results[0].Date.ToOADate(),
      right: Results[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: Results.Min(x => x.Value), Results.Max(x => x.Value)
    );
  }
  public override void ContinuousAutoscaleAction(RenderPack rp) {
    var snapshot = Snapshot();
    if (snapshot.Length == 0) return;
    var xRange = rp.Plot.Axes.GetLimits().HorizontalRange;
    xRange = new(Math.Max(xRange.Min, snapshot[0].Date.ToOADate()), Math.Min(xRange.Min, snapshot[^1].Date.ToOADate()));
    var startIdx = snapshot.BinarySearch(DateTime.FromOADate(xRange.Min), x => x.Date);
    var endIdx = snapshot.BinarySearch(DateTime.FromOADate(xRange.Max), x => x.Date);
    if (startIdx < 0) startIdx = ~startIdx;
    if (endIdx < 0) endIdx = ~endIdx;
    if (startIdx == endIdx) return;
    var (min, max) = snapshot[startIdx..endIdx]
      .Aggregate((Minimum: snapshot[0].Value, Maximum: snapshot[0].Value), (prev, x) => (Math.Min(prev.Minimum, x.Value), Math.Max(prev.Maximum, x.Value)));
    rp.Plot.Axes.SetLimitsY(min * (1 + PaddingRate) - max * PaddingRate, max * (1 + PaddingRate) - min * PaddingRate);
  }
  public override void Render(RenderPack rp) {
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    // Want to assume that the candles are already sorted by dates but...
    // Also, the base collection can be modified by another thread.
    ImmutableList<MacdResult> snapshot = [.. Results];
    var rectValues = snapshot
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
      pt1: rp.Plot.GetPixel(coordinates: new(snapshot[0].Date.ToOADate(), 0)),
      pt2: rp.Plot.GetPixel(coordinates: new(snapshot[^1].Date.ToOADate(), 0)),
      color: Colors.Black
    );
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (Results) {
      Results.Clear();
      double alpha_1 = 2.0 / (Lookback_1 + 1);
      double alpha_2 = 2.0 / (Lookback_2 + 1);
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var candle = chart[i];
        var close = (double)candle.Close;
        if (i == 0) {
          Results.Add(new() {
            Date = candle.Date,
            Average_1 = close,
            Average_2 = close,
            Value = 0.0
          });
        }
        else {
          var average_1 = Results[i - 1].Average_1 * (1 - alpha_1) + close * alpha_1;
          var average_2 = Results[i - 1].Average_2 * (1 - alpha_2) + close * alpha_2;
          Results.Add(new() {
            Date = candle.Date,
            Average_1 = average_1,
            Average_2 = average_2,
            Value = average_1 - average_2
          });
        }
      }
    }
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    double alpha_1 = 2.0 / (Lookback_1 + 1);
    double alpha_2 = 2.0 / (Lookback_2 + 1);
    lock (Results) {
      if (Results.Count == 0) return;
      var close = (double)args.Candle.Close;
      var date = args.Candle.Date;
      if (Results[^1].Date == date) {
        if (Results.Count == 1) {
          Results[0] = new() {
            Date = date,
            Average_1 = close,
            Average_2 = close,
            Value = 0.0
          };
        }
        else {
          var average_1 = Ema(Results[^1].Average_1, close, alpha_1);
          var average_2 = Ema(Results[^1].Average_2, close, alpha_2);
          Results[^1] = new() {
            Date = date,
            Average_1 = average_1,
            Average_2 = average_2,
            Value = average_1 - average_2
          };
        }
      }
      else {
        var average_1 = Ema(Results[^1].Average_1, close, alpha_1);
        var average_2 = Ema(Results[^1].Average_2, close, alpha_2);
        Results.Add(new() {
          Date = date,
          Average_1 = average_1,
          Average_2 = average_2,
          Value = average_1 - average_2
        });
      }
    }
  }
  internal static double Ema(double x, double y, double a) => y * a + x * (1 - a);
}