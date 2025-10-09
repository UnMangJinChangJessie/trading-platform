using ScottPlot;

namespace trading_platform.Model.Charts;

public abstract class Indicator : IPlottable, IHasLegendText {
  public string LegendText { get; set; } = "Indicator";
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = new Axes();
  public IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public double PaddingRate { get; set; }
  public abstract void Render(RenderPack rp);
  public virtual AxisLimits GetAxisLimits() {
    return AxisLimits.Default;
  }
  public abstract void ContinuousAutoscaleAction(RenderPack rp);
  public CandlestickChartData BaseChart { get; } = new();
  public Indicator(CandlestickChartData chart) {
    BaseChart = chart;
    PaddingRate = 0.05;
    BaseChart.CandleChanged  += OnCandleChanged;
    BaseChart.CandleInserted += OnCandleInserted;
    BaseChart.CandleRemoved  += OnCandleRemoved;
    BaseChart.Cleared        += OnCleared;
  }
  protected abstract void OnCandleChanged(object? sender, ChartOHLC candle);
  protected abstract void OnCandleInserted(object? sender, ChartOHLC candle);
  protected abstract void OnCandleRemoved(object? sender, DateTime dt);
  protected abstract void OnCleared(object? sender, EventArgs args);
  protected abstract void Invalidate();
  ~Indicator() {
    BaseChart.CandleChanged  -= OnCandleChanged;
    BaseChart.CandleInserted -= OnCandleInserted;
    BaseChart.CandleRemoved  -= OnCandleRemoved;
    BaseChart.Cleared        -= OnCleared;
  }
}