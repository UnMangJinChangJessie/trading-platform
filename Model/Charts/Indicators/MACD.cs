using System.Collections.Immutable;
using Avalonia;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public class MovingAverageConvergenceDivergence : Indicator {
  public struct MacdResult {
    public DateTime Date { get; set; }
    public double Average_1 { get; set; }
    public double Average_2 { get; set; }
    public double Value { get; set; }
    public double Close { get; set; }
  };
  public BarStyle BarStyle { get; private set; }
  private int _Lookback_1;
  private int _Lookback_2;
  public int Lookback_1 {
    get => _Lookback_1;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (value != _Lookback_1) {
        LegendText = $"MACD({_Lookback_1}, {_Lookback_2})";
        _Lookback_1 = value;
        Invalidate();
      }
    }
  }
  public int Lookback_2 {
    get => _Lookback_2;
    set {
      ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(value, 0, nameof(value));
      if (value != _Lookback_2) {
        LegendText = $"MACD({_Lookback_1}, {_Lookback_2})";
        _Lookback_2 = value;
        Invalidate();
      }
    }
  }
  public List<MacdResult> Results { get; private set; }
  
  public MovingAverageConvergenceDivergence(CandlestickChartData data, int lookback_1, int lookback_2) : base(data) {
    Results = [];
    Lookback_1 = lookback_1;
    Lookback_2 = lookback_2;
    BarStyle = new();
    Invalidate();
  }
  public ImmutableArray<MacdResult> Snapshot() {
    bool entered = Monitor.TryEnter(Results);
    ImmutableArray<MacdResult> result = [.. Results];
    if (entered) Monitor.Exit(Results);
    return result;
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
    var startIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Min));
    var endIdx = SearchIndexByDate(snapshot, DateTime.FromOADate(xRange.Max));
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
  protected override void OnCandleChanged(object? sender, ChartOHLC candle) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx < 0) return; // 캔들의 변경인데 시각이 존재하지 않으면 안 됨.
      Reevaluate(idx, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleInserted(object? sender, ChartOHLC candle) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), candle.Date);
      if (idx >= 0) return; // 캔들의 삽입인데 시각이 이미 존재하면 안 됨.
      idx = ~idx;
      Results.Insert(idx, new() { Date = candle.Date });
      Reevaluate(idx, withClose: (double)candle.Close);
    }
  }
  protected override void OnCandleRemoved(object? sender, DateTime dt) {
    lock (Results) {
      int idx = SearchIndexByDate(Snapshot(), dt);
      if (idx < 0) return; // 캔들의 삭제인데 시각이 존재하지 않으면 안 됨.
      Results.RemoveAt(idx);
      Reevaluate(idx); // 이론상으로는 캔들 하나가 바뀌면 그 뒤의 모든 이동평균은 다시 계산해야 함.
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
    LinkedList<double> closes = [];
    for (int i = 0; i < snapshot.Count; i++) {
      Results.Add(new() { Date = snapshot[i].Date, Close = (double)snapshot[i].Close });
      if (i == 0) Results[i] = Results[i] with {
        Average_1 = Results[i].Close,
        Average_2 = Results[i].Close,
        Value = 0.0
      };
      else {
        var avg_1 = double.Lerp(Results[i - 1].Average_1, Results[i].Close, 2.0 / (1.0 + Lookback_1));
        var avg_2 = double.Lerp(Results[i - 1].Average_2, Results[i].Close, 2.0 / (1.0 + Lookback_2));
        Results[i] = Results[i] with {
          Average_1 = avg_1,
          Average_2 = avg_2,
          Value = avg_1 - avg_2
        };
      }
    }
  }
  protected void Reevaluate(int begin, int end = int.MaxValue, double? withClose = null) {
    if (withClose != null) Results[begin] = Results[begin] with { Close = withClose.Value };
    for (int i = begin; i < Math.Min(Results.Count, end); i++) {
      if (i == 0) Results[i] = Results[i] with {
        Average_1 = Results[i].Close,
        Average_2 = Results[i].Close,
        Value = 0.0
      };
      else {
        var avg_1 = double.Lerp(Results[i - 1].Average_1, Results[i].Close, 2.0 / (1.0 + Lookback_1));
        var avg_2 = double.Lerp(Results[i - 1].Average_2, Results[i].Close, 2.0 / (1.0 + Lookback_2));
        Results[i] = Results[i] with {
          Average_1 = avg_1,
          Average_2 = avg_2,
          Value = avg_1 - avg_2
        };
      }
    }
  }
  private int SearchIndexByDate(ImmutableArray<MacdResult> snapshot, DateTime date) {
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