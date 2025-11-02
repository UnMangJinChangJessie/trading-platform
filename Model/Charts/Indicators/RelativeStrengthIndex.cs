using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public partial class RelativeStrengthIndex : Indicator {
  internal class RsiResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
    public double Value { get; set; }
    
    public double Close { get; set; }
    public double Up { get; set; }
    public double Down { get; set; }
    public double UpAverage { get; set; }
    public double DownAverage { get; set; }
  }
  private List<RsiResult> BaseResults { get; set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  public override string LegendText => $"RSI({Lookback})";
  [IndicatorParameter(ParameterName = "기간")]
  public int Lookback {
    get => field;
    set {
      if (value != field) {
        field = value;
        if (BaseChart != null) Reset(this, new() { WholeCandles = [.. BaseChart.Candles] });
        OnPropertyChanged(nameof(Lookback));
        OnPropertyChanged(nameof(LegendText));
      }
    }
  }
  [IndicatorParameter(ParameterName = "곡선")]
  public LineStyle LineStyle { get; set; }
  public RelativeStrengthIndex(CandlestickChartData baseChart, int lookback) : base(baseChart) {
    BaseResults = [];
    RenderingResults = [];
    Lookback = lookback;
    LineStyle = new();
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (BaseResults) RenderingResults = [.. BaseResults];
      ResetUpdateTime();
    }
    IEnumerable<Pixel> pixels = RenderingResults
      .Where(x => {
        var date = x.Date.ToOADate();
        var range = rp.Plot.Axes.GetLimits().HorizontalRange;
        return range.Min <= date && date <= range.Max && double.IsFinite(x.Value);
      })
      .Select(x => rp.Plot.GetPixel(
        new Coordinates((double)x.Date.ToOADate(), (double)x.Value),
        rp.Plot.Axes.Bottom,
        rp.Plot.Axes.Left
      ));
    Drawing.DrawLines(rp.Canvas, rp.Paint, pixels, LineStyle);
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (BaseResults) {
      BaseResults.Clear();
      for (int i = 0; i < args.WholeCandles.Count; i++) {
        var candle = args.WholeCandles[i];
        BaseResults.Add(GetExtensionRsi(i, candle));
      }
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (BaseResults) {
      if (BaseResults.Count == 0) return;
      var count = BaseResults.Count;
      if (args.Candle.Date == BaseResults[^1].Date) {
        BaseResults[count - 1] = GetExtensionRsi(count - 1, args.Candle);
      }
      else {
        BaseResults.Add(GetExtensionRsi(count, args.Candle));
      }
    }
    base.UpdateEnd(sender, args);
  }
  public override void Clear() {
    lock (BaseResults) BaseResults.Clear();
    base.Clear();
  }
  private RsiResult GetExtensionRsi(int insertingIdx, ChartOHLC candle) {
    if (insertingIdx == 0) return new() {
      Date = candle.Date,
      Span = candle.Span,
      Value = double.NaN,
      Close = (double)candle.Close,
      Up = double.NaN,
      Down = double.NaN,
      UpAverage = double.NaN,
      DownAverage = double.NaN
    };
    var up = Math.Max((double)candle.Close - BaseResults[insertingIdx - 1].Close, 0.0);
    var down = Math.Max(BaseResults[insertingIdx - 1].Close - (double)candle.Close, 0.0);
    if (insertingIdx < Lookback) return new() {
      Date = candle.Date,
      Span = candle.Span,
      Value = double.NaN,
      Close = (double)candle.Close,
      Up = up,
      Down = down,
      UpAverage = double.NaN,
      DownAverage = double.NaN,
    };
    const double EPSILON = 1.11022302462515654042e-16;
    double upAverage, downAverage, value;
    if (insertingIdx == Lookback) {
      upAverage = (BaseResults[1..Lookback].Sum(x => x.Up) + up) / Lookback;
      downAverage = (BaseResults[1..Lookback].Sum(x => x.Down) + down) / Lookback;
    }
    else {
      upAverage = double.Lerp(BaseResults[insertingIdx - 1].UpAverage, up, 1.0 / Lookback);
      downAverage = double.Lerp(BaseResults[insertingIdx - 1].DownAverage, down, 1.0 / Lookback);
    }
    value = Math.Abs(upAverage) >= EPSILON || Math.Abs(downAverage) >= EPSILON ? 100.0 * upAverage / (upAverage + downAverage) : 50.0;
    return new() {
      Date = candle.Date,
      Span = candle.Span,
      Close = (double)candle.Close,
      Value = value,
      Up = up,
      Down = down,
      UpAverage = upAverage,
      DownAverage = downAverage,
    };
  }
}