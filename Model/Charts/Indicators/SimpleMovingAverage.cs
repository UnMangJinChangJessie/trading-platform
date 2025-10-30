using System.Collections.Immutable;
using System.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

[Description("단순이동평균")]
public class SimpleMovingAverage : Indicator {
  public override string LegendText => $"SMA({Lookback})";
  public class SmaResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
    public double Close { get; set; }
    public double Value { get; set; }
  };
  [IndicatorParameter(ParameterName = "기간")]
  public int Lookback {
    get => field;
    set {
      ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
      if (field != value) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles] });
        OnPropertyChanged(nameof(Lookback));
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
  public override bool IsPriceOverlay => true;
  public SimpleMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    RenderingResults = [];
    MovingAverage = [];
    Lookback = lookback;
  }
  public override AxisLimits GetAxisLimits() {
    if (RenderingResults.Length == 0) return AxisLimits.Unset;
    IEnumerable<IIndicatorResult> notNull = RenderingResults.Where(x => double.IsFinite(((SmaResult)x).Value));
    if (!notNull.Any()) return AxisLimits.Default;
    else return new(
      left: RenderingResults[0].Date.ToOADate(),
      right: RenderingResults[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: notNull.Min(x => x.Value), notNull.Max(x => x.Value)
    );
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (MovingAverage) RenderingResults = [.. MovingAverage];
      ResetUpdateTime();
    }
    if (RenderingResults.Length == 0) return;
    // Want to assume that the candles are already sorted by dates but...
    // Also, the base collection can be modified by another thread.
    IEnumerable<Pixel> pixels = RenderingResults.Where(x => double.IsFinite(x.Value))
      .Select(x => rp.Plot.GetPixel(
        new Coordinates((double)x.Date.ToOADate(), (double)x.Value)
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
    base.Reset(sender, args);
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
    base.UpdateEnd(sender, args);
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