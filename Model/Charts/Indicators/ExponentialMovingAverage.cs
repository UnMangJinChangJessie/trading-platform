using System.Collections.Immutable;
using ScottPlot;
using ScottPlot.AxisPanels;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class ExponentialMovingAverage : Indicator {
  public override string LegendText => $"EMA({Lookback})";
  public struct EmaResult {
    public DateTime Date { get; set; }
    public double Value { get; set; }
  };
  public int Lookback {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles]});
      }
    }
  }
  public List<EmaResult> MovingAverage { get; private set; }
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public ExponentialMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    MovingAverage = [];
    Lookback = lookback;
    // Invalidate(); will be called at the lookback allocation.
  }
  public override AxisLimits GetAxisLimits() {
    if (MovingAverage.Count == 0) return AxisLimits.Unset;
    var snapshot = Snapshot();
    return new(
      left: MovingAverage[0].Date.ToOADate(),
      right: MovingAverage[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: snapshot.Min(x => x.Value), snapshot.Max(x => x.Value)
    );
  }
  public ImmutableArray<EmaResult> Snapshot() {
    bool entered = Monitor.TryEnter(MovingAverage);
    ImmutableArray<EmaResult> result = [.. MovingAverage];
    if (entered) Monitor.Exit(MovingAverage);
    return result;
  }
  public override void Render(RenderPack rp) {
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    // Want to assume that the candles are already sorted by dates but...
    // Also, the base collection can be modified by another thread.
    ImmutableArray<EmaResult> snapshot = Snapshot();
    IEnumerable<Pixel> pixels = snapshot
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return range.Min - margin <= date && date <= range.Max + margin;
      })
      .Select(x => rp.Plot.GetPixel(
        new Coordinates((double)x.Date.ToOADate(), (double)x.Value),
        rp.Plot.Axes.Bottom,
        rp.Plot.Axes.Left
      ));
    Drawing.DrawLines(rp.Canvas, rp.Paint, pixels, LineStyle);
  }
  public override void ContinuousAutoscaleAction(RenderPack rp) {
    var snapshot = Snapshot();
    if (snapshot.Length == 0) return;
    var xRange = rp.Plot.Axes.GetLimits().HorizontalRange;
    xRange = new(Math.Max(xRange.Min, snapshot[0].Date.ToOADate()), Math.Min(xRange.Min, snapshot[^1].Date.ToOADate()));
    var startIdx = MovingAverage.BinarySearch(DateTime.FromOADate(xRange.Min), x => x.Date);
    var endIdx = MovingAverage.BinarySearch(DateTime.FromOADate(xRange.Max), x => x.Date);
    if (startIdx < 0) startIdx = ~startIdx;
    if (endIdx < 0) endIdx = ~endIdx;
    if (startIdx == endIdx) return;
    var (min, max) = snapshot[startIdx..endIdx]
      .Aggregate(
        (Minimum: snapshot[0].Value, Maximum: snapshot[0].Value),
        (prev, x) => (Math.Min(prev.Minimum, x.Value), Math.Max(prev.Maximum, x.Value))
      );
    rp.Plot.Axes.SetLimitsY(min * (1 + PaddingRate) - max * PaddingRate, max * (1 + PaddingRate) - min * PaddingRate);
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    double alpha = 2.0 / (1.0 + Lookback);
    lock (MovingAverage) {
      MovingAverage.Clear();
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var candle = chart[i];
        var date = candle.Date;
        var close = (double)candle.Close;
        if (i == 0) MovingAverage[i] = new() { Date = date, Value = close };
        else MovingAverage[i] = new() { Date = date, Value = Ema(MovingAverage[^1].Value, close, alpha) };
      }
    }
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    double alpha = 2.0 / (1.0 + Lookback);
    lock (MovingAverage) {
      if (MovingAverage.Count == 0) return;
      if (args.Candle is not ChartOHLC candle) return;
      var date = candle.Date;
      var close = (double)candle.Close;
      if (MovingAverage[^1].Date == date) {
        if (MovingAverage.Count == 1) MovingAverage[^1] = new() { Date = date, Value = close };
        else MovingAverage[^1] = new() { Date = date, Value = Ema(MovingAverage[^2].Value, close, alpha) };
      }
      else MovingAverage.Add(new() { Date = date, Value = Ema(MovingAverage[^1].Value, close, alpha) });
    }
  }
  internal double Ema(double x, double y, double a) => x * (1 - a) + y * a;
}