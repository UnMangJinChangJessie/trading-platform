namespace TradingSystem.Chart;

public abstract class Indicator : IDisposable {
  public CandlestickChart? BaseChart { get; private set; } = null;
  public required bool Overlay { get; set; }
  public abstract string GetIndicatorName();
  protected abstract void Initialize();
  protected abstract void Update(object sender, Candle prevCandle, Candle postCandle);
  protected abstract void Append(object sender, Candle candle);
  protected abstract void Prepend(object sender, Candle candle);
  protected abstract void PopBack(object sender, Candle candle);
  protected abstract void PopFront(object sender, Candle candle);
  public void BindChart(CandlestickChart chart) {
    if (BaseChart != null) {
      BaseChart.LastCandleUpdated -= Update;
      BaseChart.CandleAppended -= Append;
      BaseChart.CandlePrepended -= Prepend;
      BaseChart.CandlePoppedBack -= PopBack;
      BaseChart.CandlePoppedFront -= PopFront;
    }
    BaseChart = chart;
    BaseChart.LastCandleUpdated += Update;
    BaseChart.CandleAppended += Append;
    BaseChart.CandlePrepended += Prepend;
    BaseChart.CandlePoppedBack += PopBack;
    BaseChart.CandlePoppedFront += PopFront;
    Initialize();
  }
  public abstract double this[Index i] {
    get;
  }
  public abstract IEnumerable<double> this[Range r] {
    get;
  }
  public abstract void Dispose();
}