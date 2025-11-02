using System.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

[Description("거래량")]
public class Volume : Indicator {
  public class VolumeResult : IIndicatorResult {
    public DateTime Date { get; set; }
    public TimeSpan Span { get; set; }
    public double Value { get; set; }
    public double PreviousValue { get; set; }
  };
  public override string LegendText => $"거래량";
  public BarStyle BarStyle { get; private set; }
  public List<VolumeResult> BaseResults { get; private set; }
  public override IEnumerable<IIndicatorResult> Results => BaseResults;
  
  public Volume(CandlestickChartData data) : base(data) {
    RenderingResults = [];
    BaseResults = [];
    BarStyle = new() {
      PositiveBarIncreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.5) },
      PositiveBarDecreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.9) },
      PositiveBarIncreasingLine = new() { Color = Colors.LightPink, Width = 2 },
      PositiveBarDecreasingLine = new() { Color = Colors.LightPink, Width = 2 },
      NegativeBarIncreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.5) },
      NegativeBarDecreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.9) },
      NegativeBarIncreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 },
      NegativeBarDecreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 },
    };
    lock (BaseChart.Candles) {
      if (BaseChart.Candles.Count != 0) {
        Reset(this, new() { WholeCandles = [.. BaseChart.Candles] });
      }
    }
  }
  public override void Render(RenderPack rp) {
    if (ShouldUpdateResult) {
      lock (BaseResults) RenderingResults = [.. BaseResults];
      ResetUpdateTime();
    }
    _lastResultUpdateTime = DateTime.UtcNow;
    var horizontalRange = rp.Plot.Axes.GetLimits().HorizontalRange;
    var bars = RenderingResults
      .Cast<VolumeResult>()
      .Where(x => {
        var date = x.Date.ToOADate();
        var margin = 5 * BaseChart.TimeSpan.TotalDays;
        return horizontalRange.Min - margin <= date && date <= horizontalRange.Max + margin;
      })
      .Select(x => {
        return new Bar() { Value = x.Value, Size = x.Span.TotalDays, Position = x.Date.ToOADate() };
      });
    double? previousValue = null;
    foreach (Bar bar in bars) {
      var positive = bar.Value > 0;
      var notDecreased = !previousValue.HasValue || (previousValue.Value <= bar.Value);
      bar.FillStyle = positive ? (notDecreased ? BarStyle.PositiveBarIncreasingFill : BarStyle.PositiveBarDecreasingFill) :
        (notDecreased ? BarStyle.NegativeBarIncreasingFill : BarStyle.NegativeBarDecreasingFill);
      bar.LineStyle = positive ? (notDecreased ? BarStyle.PositiveBarIncreasingLine : BarStyle.PositiveBarDecreasingLine) :
        (notDecreased ? BarStyle.NegativeBarIncreasingLine : BarStyle.NegativeBarDecreasingLine);
      bar.RenderBody(rp, Axes, rp.Paint);
      previousValue = bar.Value;
    }
  }
  public override void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (BaseResults) {
      BaseResults = [.. args.WholeCandles.Select(x => new VolumeResult() { Date = x.Date, Value = (double)x.Volume, Span = x.Span })];
      for (int i = 1; i < BaseResults.Count; i++) {
        BaseResults[i].PreviousValue = BaseResults[i - 1].Value;
      }
    }
    base.Reset(sender, args);
  }
  public override void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (BaseResults) {
      if (BaseResults.Count == 0) BaseResults.Add(new() { Date = args.Candle.Date, Value = (double)args.Candle.Volume, Span = args.Candle.Span, PreviousValue = 0 });
      else if (BaseResults[^1].Date == args.Candle.Date) {
        BaseResults[^1].Value = (double)args.Candle.Volume;
      }
      else BaseResults.Add(new() { Date = args.Candle.Date, Value = (double)args.Candle.Volume, Span = args.Candle.Span, PreviousValue = BaseResults[^1].Value });
    }
    base.UpdateEnd(sender, args);
  }
  public override void Clear() {
    lock (BaseResults) {
      BaseResults.Clear();
    }
    base.Clear();
  }
}