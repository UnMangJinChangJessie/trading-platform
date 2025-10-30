using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public partial class AverageTrueRange : Indicator {
  internal class AtrResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
    public double Value { get; set; }

    public double Close { get; set; }
    public double TrueRange { get; set; }
  }
  private List<AtrResult> BaseResults { get; set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  public int Lookback {
    get => field;
    set {
      if (field != value) {
        field = value;
        OnPropertyChanged(nameof(Lookback));
      }
    }
  }
  public LineStyle LineStyle { get; private set; }
  public override string LegendText => $"ATR({Lookback})";
  public AverageTrueRange(CandlestickChartData chart, int lookback) : base(chart) {
    RenderingResults = [];
    BaseResults = [];
    Lookback = lookback;
    LineStyle = new(1.0F, Colors.DarkRed);
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
      lock (BaseResults) RenderingResults = [.. BaseResults];
      ResetUpdateTime();
    }
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
    Clear();
    lock (BaseResults) {
      for (int i = 0; i < args.WholeCandles.Count; i++) {
        var candle = args.WholeCandles[i];
        var tr = GetExtensionTrueRange((double)candle.High, (double)candle.Low, i == 0 ? null : BaseResults[i - 1].Close);
        BaseResults.Add(new() {
          Date = candle.Date,
          Span = candle.Span,
          Close = (double)candle.Close,
          TrueRange = tr,
          Value = GetExtensionAverageTrueRange(i, tr)
        });
      }
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (BaseResults) {
      var count = BaseResults.Count;
      if (count == 0) {
        var tr = GetExtensionTrueRange((double)args.Candle.High, (double)args.Candle.Low, null);
        BaseResults.Add(new() {
          Date = args.Candle.Date,
          Span = args.Candle.Span,
          Close = (double)args.Candle.Close,
          TrueRange = tr,
          Value = GetExtensionAverageTrueRange(0, tr)
        });
      }
      if (args.Candle.Date == BaseResults[^1].Date) {
        BaseResults[^1].TrueRange = GetExtensionTrueRange((double)args.Candle.High, (double)args.Candle.Low, BaseResults[^2].Close);
        BaseResults[^1].Value = GetExtensionAverageTrueRange(count - 1, BaseResults[^1].TrueRange);
      }
    }
    base.UpdateEnd(sender, args);
  }
  public override void Clear() {
    lock (BaseResults) {
      BaseResults.Clear();
    }
    base.Clear();
  }
  private static double GetExtensionTrueRange(double high, double low, double? prevClose = null) {
    if (prevClose == null) return high - low;
    else return Enumerable.Max<double>([high - low, Math.Abs(high - prevClose.Value), Math.Abs(low - prevClose.Value)]);
  }
  private double GetExtensionAverageTrueRange(int insertingIndex, double tr) {
    return insertingIndex == 0 ? tr : double.Lerp(BaseResults[insertingIndex - 1].Value, tr, 1.0 / Lookback);
  }
}