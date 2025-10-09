using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model;

public partial class ChartOHLC : ObservableObject, IComparable<ChartOHLC> {
  [ObservableProperty]
  public partial DateTime Date { get; set; }
  [ObservableProperty]
  public partial decimal Open { get; set; } = default!;
  [ObservableProperty]
  public partial decimal High { get; set; } = default!;
  [ObservableProperty]
  public partial decimal Low { get; set; } = default!;
  [ObservableProperty]
  public partial decimal Close { get; set; } = default!;
  [ObservableProperty]
  public partial decimal Volume { get; set; } = default!;
  [ObservableProperty]
  public partial decimal Amount { get; set; } = default!;
  public ChartOHLC() {
    Date = DateTime.UnixEpoch;
    Open = default!;
    High = default!;
    Low = default!;
    Close = default!;
    Volume = default!;
  }
  public ChartOHLC(decimal open, decimal high, decimal low, decimal close) {
    Date = DateTime.UnixEpoch;
    (Open, High, Low, Close) = (open, high, low, close);
  }
  int IComparable<ChartOHLC>.CompareTo(ChartOHLC? other) {
    return Date.CompareTo(other?.Date);
  }
  public void CopyOHLCFrom(ChartOHLC ohlc) {
    (Open, High, Low, Close) = (ohlc.Open, ohlc.High, ohlc.Low, ohlc.Close);
    (Volume, Amount) = (ohlc.Volume, ohlc.Amount);
  }
}