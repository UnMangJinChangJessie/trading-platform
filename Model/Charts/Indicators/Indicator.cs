using ScottPlot;

namespace trading_platform.Model.Charts;

public abstract class Indicator(CandlestickChartData chart) : IPlottable, IHasLegendText {
  public virtual string LegendText { get; set; } = "Indicator";
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = new Axes();
  public IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public double PaddingRate { get; set; }
  public CandlestickChartData BaseChart { get; } = chart;
  public abstract void Render(RenderPack rp);
  public virtual AxisLimits GetAxisLimits() {
    return AxisLimits.Default;
  }
  public abstract void ContinuousAutoscaleAction(RenderPack rp);
  public abstract void Reset();
  public abstract void UpdateEnd();
}