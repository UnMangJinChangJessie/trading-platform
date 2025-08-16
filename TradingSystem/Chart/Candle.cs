namespace TradingSystem.Chart;

public class Candle : IComparable<Candle> {
  public DateTime Time { get; private set; }
  public decimal Open { get; private set; }
  public decimal High { get; private set; }
  public decimal Low { get; private set; }
  public decimal Close { get; private set; }
  public decimal Volume { get; private set; }
  public decimal Amount { get; private set; }
  public decimal AveragePrice => Volume == 0.0M ? decimal.Zero : Amount / Volume;

  public Candle() {
    Time = DateTime.UnixEpoch;
    Open = 0.0M;
    High = 0.0M;
    Low = 0.0M;
    Close = 0.0M;
    Volume = 0.0M;
    Amount = 0.0M;
  }
  public Candle(DateTime time, decimal open, decimal high, decimal low, decimal close, decimal volume, decimal amount) {
    Time = time;
    (Open, High, Low, Close) = (open, high, low, close);
    Volume = volume;
    Amount = amount;
  }
  int IComparable<Candle>.CompareTo(Candle? other) {
    return Time.CompareTo(other?.Time);
  }
}