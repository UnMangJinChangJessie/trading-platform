using System.Collections.Immutable;
using ScottPlot;

namespace trading_platform.Model.Charts;

public interface IIndicatorResult {
  public DateTime Date { get; set; }
  public double Value { get; set; }
}

public abstract class Indicator : IPlottable, IHasLegendText {
  public virtual string LegendText { get; set; } = "Indicator";
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = new Axes();
  public IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public double PaddingRate { get; set; }
  public CandlestickChartData BaseChart { get; }
  public abstract IEnumerable<IIndicatorResult> Results { get; }
  public abstract void Render(RenderPack rp);
  public virtual AxisLimits GetAxisLimits() {
    return AxisLimits.Default;
  }
  public abstract void Reset(object? sender, CandlestickChartData.LoadedEventArgs args);
  public abstract void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args);
  public Indicator(CandlestickChartData chart) {
    BaseChart = chart;
    BaseChart.Loaded += Reset;
    BaseChart.UpdatedEnd += UpdateEnd;
  }
}