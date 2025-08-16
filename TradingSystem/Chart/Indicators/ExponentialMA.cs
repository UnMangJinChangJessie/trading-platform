namespace TradingSystem.Chart.Indicators;

public class ExponentialMA : Indicator {
  public int Period { get; private set; }
  private List<double> Average { get; set; }
  private double Interpolation => 2.0 / (1.0 + Period);
  public ExponentialMA(int period) {
    Period = period;
    Average = new();
  }
  protected override void Append(object sender, Candle candle) {
    if (BaseChart == null) return;
    if (BaseChart.Count == 0) Average.Add((double)candle.Close);
    else {
      double delta = (double)(candle.Close - BaseChart[^Period].Close);
      Average.Add(double.Lerp(Average[^1], (double)candle.Close, Interpolation));
    }
  }
  protected override void Prepend(object sender, Candle candle) {
    if (BaseChart == null) return;
    if (BaseChart.Count == 0) Average.Insert(0, (double)candle.Close);
    else Average[Period - 1] = (double)BaseChart[..Period].Average(c => c.Close);
  }
  protected override void Update(object sender, Candle prev, Candle post) {
    if (BaseChart == null) return;
    if (BaseChart.Count + 1 < Period) return;
    double delta = (double)(post.Close - BaseChart[^Period].Close);
    Average[^1] = Math.FusedMultiplyAdd(delta, 1.0 / Period, Average[^2]);
  }
  protected override void PopBack(object sender, Candle candle) {
    Average.RemoveAt(Average.Count - 1);
  }
  protected override void PopFront(object sender, Candle candle) {
    Average.RemoveAt(0);
  }
  protected override void Initialize() {
    if (BaseChart == null) return;
    Average.Clear();
    Average.Capacity = BaseChart.Count;
    for (int i = 0; i < BaseChart.Count; i++) {
      if (i < Period) Average.Add(double.NaN);
      else if (i == Period) Average.Add((double)BaseChart[..Period].Average(c => c.Close));
      else {
        double delta = (double)(BaseChart[i].Close - BaseChart[i - Period].Close);
        Average.Add(Math.FusedMultiplyAdd(delta, 1.0 / Period, Average[^1]));
      }
    }
  }
  public override double this[Index i] {
    get => Average[i];
  }
  public override IEnumerable<double> this[Range r] {
    get => Average[r];
  }
  public override void Dispose() {
    if (BaseChart == null) return;
    BaseChart.CandleAppended -= Append;
    BaseChart.CandlePrepended -= Prepend;
    BaseChart.CandlePoppedBack -= PopBack;
    BaseChart.CandlePoppedFront -= PopFront;
    BaseChart.LastCandleUpdated -= Update;
  }
  public override string GetIndicatorName() {
    return $"EMA({Period})";
  }
}