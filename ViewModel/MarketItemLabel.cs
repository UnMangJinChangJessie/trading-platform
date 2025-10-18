using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class MarketItemLabel : ObservableObject {
  [ObservableProperty]
  public partial string Ticker { get; set; } = "";
  [ObservableProperty]
  public partial string Name { get; set; } = "";
}