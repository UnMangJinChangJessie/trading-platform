using System.Collections.Immutable;
using ScottPlot;
using ScottPlot.AxisPanels;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class ExponentialMovingAverage : Indicator {
  public override string LegendText => $"EMA({Lookback})";
  public class EmaResult : IIndicatorResult {
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
  public override IEnumerable<IIndicatorResult> Results => MovingAverage;
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public ExponentialMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    MovingAverage = [];
    Lookback = lookback;
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
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    double alpha = 2.0 / (1.0 + Lookback);
    lock (MovingAverage) {
      MovingAverage.Clear();
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var candle = chart[i];
        var date = candle.Date;
        var close = (double)candle.Close;
        MovingAverage.Add(GetExtensionEma(i, close));
        MovingAverage[^1].Date = date;
      }
    }
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    double alpha = 2.0 / (1.0 + Lookback);
    lock (MovingAverage) {
      if (MovingAverage.Count == 0) return;
      var date = args.Candle.Date;
      var close = (double)args.Candle.Close;
      var count = MovingAverage.Count;
      if (MovingAverage[^1].Date == date) {
        MovingAverage[^1] = GetExtensionEma(count - 1, close);
        MovingAverage[^1].Date = date;
      }
      else {
        MovingAverage.Add(GetExtensionEma(count, close));
        MovingAverage[^1].Date = date;
      }
    }
  }
  internal EmaResult GetExtensionEma(int insertingIndex, double close) {
    if (insertingIndex == 0) return new EmaResult() { Value = close };
    else return new EmaResult() { Value = double.Lerp(MovingAverage[insertingIndex].Value, close, 2.0 / (1 + Lookback)) };
  }
}