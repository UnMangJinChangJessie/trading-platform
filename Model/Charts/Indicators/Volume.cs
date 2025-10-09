using System.Collections.Immutable;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public class Volume : Indicator {
  public struct VolumeResult {
    public DateTime Date { get; set; }
    public double Value { get; set; }
  };
  public BarStyle BarStyle { get; private set; }
  public List<VolumeResult> Results { get; private set; }
  
  public Volume(CandlestickChartData data) : base(data) {
    Results = [];
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
    Invalidate();
  }
  public ImmutableArray<VolumeResult> Snapshot() {
    bool entered = Monitor.TryEnter(Results);
    ImmutableArray<VolumeResult> result = [.. Results];
    if (entered) Monitor.Exit(Results);
    return result;
  }
  public override AxisLimits GetAxisLimits() {
    var snapshot = Snapshot();
    if (snapshot.Length == 0) return AxisLimits.Unset;
    else return new(
      left: snapshot[0].Date.ToOADate(),
      right: snapshot[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: snapshot.Min(x => x.Value), snapshot.Max(x => x.Value)
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
      .Aggregate((Minimum: snapshot[0].Value, Maximum: snapshot[0].Value), (prev, x) => (Math.Min(prev.Minimum, x.Value), Math.Max(prev.Maximum, x.Value)));
    rp.Plot.Axes.SetLimitsY(min, max * (1 + PaddingRate));
    rp.Plot.Grid.YAxis.Range.Set(min, max * (1 + PaddingRate));
  }
  public override void Render(RenderPack rp) {
    if (rp.Plot.Axes.ContinuouslyAutoscale) {
      rp.Plot.Axes.ContinuousAutoscaleAction.Invoke(rp);
    }
    // Want to assume that the candles are already sorted by dates but...
    // Also, the base collection can be modified by another thread.
    var snapshot = Snapshot();
    // filter only the necessary candles
    var rectValues = snapshot
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return range.Min - margin <= date && date <= range.Max + margin;
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
  protected override void OnCandleChanged(object? sender, ChartOHLC candle) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx < 0) return; // 캔들의 변경인데 시각이 존재하지 않으면 안 됨.
      Results[idx] = Results[idx] with { Value = (double)candle.Volume };
    }
  }
  protected override void OnCandleInserted(object? sender, ChartOHLC candle) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx >= 0) return; // 캔들의 삽입인데 시각이 이미 존재하면 안 됨.
      idx = ~idx;
      Results.Insert(idx, new() { Date = candle.Date, Value = (double)candle.Volume });
    }
  }
  protected override void OnCandleRemoved(object? sender, DateTime dt) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), dt);
      if (idx < 0) return; // 캔들의 삭제인데 시각이 존재하지 않으면 안 됨.
      Results.RemoveAt(idx);
    }
  }
  protected override void OnCleared(object? sender, EventArgs args) {
    lock (Results) {
      Results.Clear();
    }
  }
  protected override void Invalidate() {
    var snapshot = BaseChart.Candles.ToImmutableList();
    Results.Clear();
    for (int i = 0; i < snapshot.Count; i++) {
      Results.Add(new() { Date = snapshot[i].Date, Value = (double)snapshot[i].Volume });
    }
  }
  private int SearchIndexByDate(ImmutableArray<VolumeResult> snapshot, DateTime date) {
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