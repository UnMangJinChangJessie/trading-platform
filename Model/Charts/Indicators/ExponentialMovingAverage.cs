using System.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

[Description("지수이동평균")]
public class ExponentialMovingAverage : Indicator {
  public override string LegendText => $"EMA({Lookback})";
  public class EmaResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
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
  public List<EmaResult> MovingAverage { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => MovingAverage;
  public LineStyle LineStyle { get; set; } = new LineStyle() {
    Color = Colors.DarkBlue,
    Pattern = LinePattern.Solid,
    AntiAlias = true,
    Width = 1,
  };
  public override bool IsPriceOverlay => true;
  public ExponentialMovingAverage(CandlestickChartData data, int lookback) : base(data) {
    RenderingResults = [];
    MovingAverage = [];
    Lookback = lookback;
  }
  public override AxisLimits GetAxisLimits() {
    if (RenderingResults.Length == 0) return AxisLimits.Unset;
    return new(
      left: RenderingResults[0].Date.ToOADate(),
      right: RenderingResults[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: RenderingResults.Min(x => x.Value),
      top: RenderingResults.Max(x => x.Value)
    );
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (MovingAverage) RenderingResults = [.. MovingAverage];
      ResetUpdateTime();
    }
    _lastResultUpdateTime = DateTime.UtcNow;
    IEnumerable<Pixel> pixels = RenderingResults
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
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
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
    base.UpdateEnd(sender, args);
  }
  internal EmaResult GetExtensionEma(int insertingIndex, double close) {
    if (insertingIndex == 0) return new EmaResult() { Value = close };
    else return new EmaResult() { Value = double.Lerp(MovingAverage[insertingIndex - 1].Value, close, 2.0 / (1 + Lookback)) };
  }
}