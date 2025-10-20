using System.Collections.Immutable;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts.Indicators;

public class SimpleMovingAverage(CandlestickChartData data, int lookback) : Indicator(data) {
  public override string LegendText => $"SMA({Lookback})";
  public struct SmaResult {
    public DateTime Date { get; set; }
    public double? Value { get; set; }
    public double Close { get; set; }
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
  } = lookback;
  public List<SmaResult> MovingAverage { get; private set; } = [];
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public ImmutableArray<SmaResult> Snapshot() {
    lock (MovingAverage) return [.. MovingAverage];
  }
  public override AxisLimits GetAxisLimits() {
    if (MovingAverage.Count == 0) return AxisLimits.Unset;
    var snapshot = Snapshot();
    ImmutableArray<SmaResult> notNull = [.. MovingAverage.Where(x => x.Value.HasValue)];
    if (!notNull.Any()) return AxisLimits.Default;
    else return new(
      left: MovingAverage[0].Date.ToOADate(),
      right: MovingAverage[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: notNull.Min(x => x.Value!.Value), notNull.Max(x => x.Value!.Value)
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
      .Where(x => x.Value.HasValue)
      .Aggregate(
        (Minimum: snapshot[0].Value!.Value, Maximum: snapshot[0].Value!.Value),
        (prev, x) => (Math.Min(prev.Minimum, x.Value!.Value), Math.Max(prev.Maximum, x.Value!.Value))
      );
    rp.Plot.Axes.SetLimitsY(min * (1 + PaddingRate) - max * PaddingRate, max * (1 + PaddingRate) - min * PaddingRate);
  }
  public override void Render(RenderPack rp) {
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    // Want to assume that the candles are already sorted by dates but...
    // Also, the base collection can be modified by another thread.
    var snapshot = Snapshot();
    IEnumerable<Pixel> pixels = snapshot
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return range.Min - margin <= date && date <= range.Max + margin;
      })
      .Where(x => x.Value.HasValue)
      .Select(x => rp.Plot.GetPixel(
        new Coordinates((double)x.Date.ToOADate(), (double)x.Value!.Value),
        rp.Plot.Axes.Bottom,
        rp.Plot.Axes.Left
      ));
    Drawing.DrawLines(rp.Canvas, rp.Paint, pixels, LineStyle);
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (MovingAverage) {
      MovingAverage.Clear();
      var chart = args.WholeCandles;
      for (int i = 0; i < chart.Count; i++) {
        var date = chart[i].Date;
        var close = (double)chart[i].Close;
        if (i + 1 < Lookback) MovingAverage.Add(new() { Date = date, Close = close, Value = null });
        else if (i + 1 == Lookback) {
          MovingAverage.Add(new() {
            Date = chart[i].Date,
            Close = close,
            Value = chart.Take(Lookback).Average(x => (double)x.Close)
          });
        }
        else {
          var average = Math.FusedMultiplyAdd(MovingAverage[^1].Value!.Value, Lookback, close - MovingAverage[i - Lookback].Close) / Lookback;
          MovingAverage.Add(new() { Date = date, Close = close, Value = average });
        }
      }
    }
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    if (MovingAverage.Count == 0) return;
    var date = args.Candle.Date;
    var close = (double)args.Candle.Close;
    lock (MovingAverage) {
      var count = MovingAverage.Count;
      if (MovingAverage[^1].Date == args.Candle.Date) {
        var average = Math.FusedMultiplyAdd(MovingAverage[^1].Value!.Value, Lookback, close - MovingAverage[^1].Close);
        MovingAverage[^1] = new() { Date = date, Close = close, Value = average };
      }
      else {
        var average = Math.FusedMultiplyAdd(MovingAverage[^1].Value!.Value, Lookback, close - MovingAverage[^Lookback].Close);
        MovingAverage.Add(new() { Date = date, Close = close, Value = average });
      }
    }
  }
}