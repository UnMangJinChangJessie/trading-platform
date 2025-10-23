using System.Collections.Immutable;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public class SimpleMovingAverage : Indicator {
  public override string LegendText => $"SMA({Lookback})";
  public class SmaResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public double Close { get; set; }
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
  public List<SmaResult> MovingAverage { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => MovingAverage;
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public SimpleMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    MovingAverage = [];
    Lookback = lookback;
  }
  public ImmutableArray<SmaResult> Snapshot() {
    lock (MovingAverage) return [.. MovingAverage];
  }
  public override AxisLimits GetAxisLimits() {
    if (MovingAverage.Count == 0) return AxisLimits.Unset;
    var snapshot = Snapshot();
    ImmutableArray<SmaResult> notNull = [.. MovingAverage.Where(x => double.IsFinite(x.Value))];
    if (notNull.Length == 0) return AxisLimits.Default;
    else return new(
      left: MovingAverage[0].Date.ToOADate(),
      right: MovingAverage[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: notNull.Min(x => x.Value), notNull.Max(x => x.Value)
    );
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
      .Where(x => double.IsFinite(x.Value))
      .Select(x => rp.Plot.GetPixel(
        new Coordinates((double)x.Date.ToOADate(), (double)x.Value),
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
        var result = GetExtensionSma(i, (double)chart[i].Close);
        result.Date = chart[i].Date;
        MovingAverage.Add(result);
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
        MovingAverage[^1] = GetExtensionSma(count - 1, close);
        MovingAverage[^1].Date = date;
      }
      else {
        MovingAverage.Add(GetExtensionSma(count, close));
        MovingAverage[^1].Date = date;
      }
    }
  }
  // 정보: 수열을 확장할 때 삽입 전 insertingIndex에 현재 MovingAverage의 크기를 넣으면 됨.
  private SmaResult GetExtensionSma(int insertingIndex, double close) {
    // 자원을 점유했을 때만 접근해야 다른 스레드에서의 MovingAverage 수정을 막을 수 있음
    if (!Monitor.IsEntered(MovingAverage)) {
      throw new SynchronizationLockException();
    }
    if (insertingIndex + 1 == Lookback) {
      double meow = MovingAverage.Sum(x => x.Close) + close;
      return new SmaResult() { Close = close, Value = meow / Lookback };
    }
    else if (insertingIndex + 1 > Lookback) {
      double meow = Math.FusedMultiplyAdd(MovingAverage[insertingIndex - 1].Value, Lookback, close - MovingAverage[insertingIndex - Lookback].Close);
      return new SmaResult() { Close = close, Value = meow / Lookback };
    }
    else return new SmaResult() { Close = close, Value = double.NaN };
  }
}