using System.Collections.Immutable;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class Volume : Indicator {
  public class VolumeResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public double Value { get; set; }
  };
  public override string LegendText => $"Volume";
  public BarStyle BarStyle { get; private set; }
  public List<VolumeResult> BaseResults { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  
  public Volume(CandlestickChartData data) : base(data) {
    RenderingResults = [];
    BaseResults = [];
    BarStyle = new() {
      PositiveBarIncreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.7) },
      PositiveBarDecreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.7) },
      PositiveBarIncreasingLine = new() { Color = Colors.LightPink, Width = 2 },
      PositiveBarDecreasingLine = new() { Color = Colors.LightPink, Width = 2 },
      NegativeBarIncreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.7) },
      NegativeBarDecreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.7) },
      NegativeBarIncreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 },
      NegativeBarDecreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 },
    };
    lock (BaseChart.Candles) {
      if (BaseChart.Candles.Count != 0) {
        Reset(this, new() { WholeCandles = [.. BaseChart.Candles] });
      }
    }
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
    if (!ShouldUpdateResult) {
      lock (BaseResults) RenderingResults = [.. BaseResults];
    }
    _lastResultUpdateTime = DateTime.UtcNow;
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    var horizontalRange = rp.Plot.Axes.GetLimits().HorizontalRange;
    var rectValues = RenderingResults
      .Where(x => {
        var date = x.Date.ToOADate();
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return horizontalRange.Min - margin <= date && date <= horizontalRange.Max + margin;
      })
      .Select(x => {
        var pixelTopLeft = rp.Plot.GetPixel(
          new Coordinates(x.Date.ToOADate() - BaseChart.TimeSpan.TotalDays / 2, Math.Max(0.0, x.Value)),
          rp.Plot.Axes.Bottom,
          rp.Plot.Axes.Left
        );
        var pixelBottomRight = rp.Plot.GetPixel(
          new Coordinates(x.Date.ToOADate() + BaseChart.TimeSpan.TotalDays / 2, Math.Min(0.0, x.Value)),
          rp.Plot.Axes.Bottom,
          rp.Plot.Axes.Left
        );
        return (
          new ScottPlot.PixelRect(left: pixelTopLeft.X, right: pixelBottomRight.X, top: pixelTopLeft.Y, bottom: pixelBottomRight.Y),
          x.Value
        );
      });
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
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (BaseResults) {
      BaseResults = [.. args.WholeCandles.Select(x => new VolumeResult() { Date = x.Date, Value = (double)x.Volume })];
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (BaseResults) {
      if (BaseResults[^1].Date == args.Candle.Date) BaseResults[^1] = new() { Date = args.Candle.Date, Value = (double)args.Candle.Volume };
      else BaseResults.Add(new() { Date = args.Candle.Date, Value = (double)args.Candle.Volume });
    }
    base.UpdateEnd(sender, args);
  }
}