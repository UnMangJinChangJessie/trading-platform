using System.Collections.Immutable;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public class SimpleMovingAverage : Indicator {
  public struct SmaResult {
    public DateTime Date { get; set; }
    public double? Value { get; set; }
    public double Close { get; set; }
  };
  private int _Lookback;
  public int Lookback {
    get => _Lookback;
    set {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
      if (_Lookback != value) {
        _Lookback = value;
        LegendText = $"SMA({_Lookback})";
        Invalidate();
      }
    }
  }
  public List<SmaResult> MovingAverage { get; private set; }
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public SimpleMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    MovingAverage = [];
    Lookback = lookback;
    // Invalidate(); will be called at the lookback allocation.
  }
  public ImmutableArray<SmaResult> Snapshot() {
    bool entered = Monitor.TryEnter(MovingAverage);
    ImmutableArray<SmaResult> result = [.. MovingAverage];
    if (entered) Monitor.Exit(MovingAverage);
    return result;
  }
  public override AxisLimits GetAxisLimits() {
    if (MovingAverage.Count == 0) return AxisLimits.Unset;
    var notNull = MovingAverage.Where(x => x.Value.HasValue);
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
    var startIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Min));
    var endIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Max));
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
    ImmutableArray<SmaResult> snapshot = Snapshot();
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
  protected override void OnCandleChanged(object? sender, ChartOHLC candle) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx < 0) return; // 캔들의 변경인데 시각이 존재하지 않으면 안 됨.
      Reevaluate(idx, idx + Lookback, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleInserted(object? sender, ChartOHLC candle) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx >= 0) return; // 캔들의 삽입인데 시각이 이미 존재하면 안 됨.
      idx = ~idx;
      MovingAverage.Insert(idx, new() { Date = candle.Date });
      Reevaluate(idx, idx + Lookback, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleRemoved(object? sender, DateTime dt) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), dt);
      if (idx < 0) return; // 캔들의 삭제인데 시각이 존재하지 않으면 안 됨.
      MovingAverage.RemoveAt(idx);
      Reevaluate(idx, idx + Lookback);
    }
  }
  protected override void OnCleared(object? sender, EventArgs args) {
    lock (MovingAverage) {
      MovingAverage.Clear();
    }
  }
  protected override void Invalidate() {
    var snapshot = BaseChart.Candles.ToImmutableList();
    MovingAverage.Clear();
    LinkedList<double> closes = [];
    foreach (var candle in snapshot) {
      closes.AddLast((double)candle.Close);
      double? value;
      if (closes.Count < Lookback) value = null;
      else if (closes.Count == Lookback) value = closes.Average();
      else {
        value = Math.FusedMultiplyAdd(MovingAverage[^1].Value!.Value, Lookback, closes.Last() - closes.First()) / Lookback;
        closes.RemoveFirst();
      }
      MovingAverage.Add(new() { Close = (double)candle.Close, Date = candle.Date, Value = value });
    }
  }
  protected void Reevaluate(int begin, int end = int.MaxValue, double? withClose = null) {
    if (withClose != null) MovingAverage[begin] = MovingAverage[begin] with { Close = withClose.Value };
    for (int i = begin; i < Math.Min(MovingAverage.Count, end); i++) {
      if (i + 1 == Lookback) MovingAverage[i] = MovingAverage[i] with { Value = MovingAverage[..Lookback].Average(x => x.Close) };
      else if (i + 1 > Lookback) MovingAverage[i] = MovingAverage[i] with {
        Value = Math.FusedMultiplyAdd(
          MovingAverage[i - 1].Value!.Value, Lookback, MovingAverage[i].Close - MovingAverage[i - Lookback].Close
        ) / Lookback
      };
    }
  }
  private int SearchIndexByDate(ImmutableArray<SmaResult> snapshot, DateTime date) {
    if (snapshot.Length == 0) return -1;
    int lo = 0;
    int hi = snapshot.Length;
    while (lo != hi) {
      int mid = lo + (hi - lo) / 2;
      if (date < snapshot[mid].Date) hi = Math.Max(mid, 0);
      else if (date > snapshot[mid].Date) lo = Math.Min(mid + 1, snapshot.Length);
      else return mid;
    }
    if (lo == snapshot.Length) return ~lo;
    return snapshot[lo].Date == date ? lo : ~lo;
  }
}