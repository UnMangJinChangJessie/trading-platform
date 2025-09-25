using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model;

public partial class OHLC : ObservableObject {
  [ObservableProperty]
  public partial DateTime Date { get; set; } = DateTime.UnixEpoch;
  [ObservableProperty]
  public partial decimal Open { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal High { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal Low { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal Close { get; set; } = 0.0M;
}