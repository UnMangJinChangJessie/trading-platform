using System.Collections.Immutable;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public class ExponentialMovingAverage : Indicator {
  public struct EmaResult {
    public DateTime Date { get; set; }
    public double Value { get; set; }
    public double Close { get; set; }
  };
  private int _Lookback;
  public int Lookback {
    get => _Lookback;
    set {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
      if (_Lookback != value) {
        _Lookback = value;
        LegendText = $"EMA({_Lookback})";
        Invalidate();
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
    var startIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Min));
    var endIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Max));
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
  protected override void OnCandleChanged(object? sender, ChartOHLC candle) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx < 0) return; // 캔들의 변경인데 시각이 존재하지 않으면 안 됨.
      Reevaluate(idx, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleInserted(object? sender, ChartOHLC candle) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx >= 0) return; // 캔들의 삽입인데 시각이 이미 존재하면 안 됨.
      idx = ~idx;
      MovingAverage.Insert(idx, new() { Date = candle.Date });
      Reevaluate(idx, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleRemoved(object? sender, DateTime dt) {
    lock (MovingAverage) {
      int idx = SearchIndexByDate(Snapshot(), dt);
      if (idx < 0) return; // 캔들의 삭제인데 시각이 존재하지 않으면 안 됨.
      MovingAverage.RemoveAt(idx);
      Reevaluate(idx); // 이론상으로는 캔들 하나가 바뀌면 그 뒤의 모든 이동평균은 다시 계산해야 함.
    }
  }
  protected override void OnCleared(object? sender, EventArgs args) {
    lock (MovingAverage) {
      MovingAverage.Clear();
    }
  }
  protected override void Invalidate() {
    var snapshot = BaseChart.Candles.ToImmutableList();
    if (snapshot.Count == 0) return;
    MovingAverage.Clear();
    double? val = null;
    double alpha = 2.0 / (1.0 + Lookback);
    foreach (var candle in snapshot) {
      if (val == null) val = (double)candle.Close;
      else val = val * (1 - alpha) + (double)candle.Close * alpha;
      MovingAverage.Add(new() { Close = (double)candle.Close, Date = candle.Date, Value = val.Value });
    }
  }
  protected void Reevaluate(int begin, int end = int.MaxValue, double? withClose = null) {
    if (withClose != null) MovingAverage[begin] = MovingAverage[begin] with { Close = withClose.Value };
    for (int i = begin; i < Math.Min(MovingAverage.Count, end); i++) {
      if (i == 0) MovingAverage[i] = MovingAverage[i] with { Value = MovingAverage[i].Close };
      else MovingAverage[i] = MovingAverage[i] with {
        Value = double.Lerp(MovingAverage[i - 1].Value, MovingAverage[i].Close, 2.0 / (1.0 + Lookback))
      };
    }
  }
  private int SearchIndexByDate(ImmutableArray<EmaResult> snapshot, DateTime date) {
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