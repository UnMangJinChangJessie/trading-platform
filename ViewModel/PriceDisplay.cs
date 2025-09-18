using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class PriceDisplay : ObservableObject {
  [ObservableProperty]
  public partial string TickerName { get; set; } = "";

  [ObservableProperty]
  public partial decimal Price { get; set; } = 0.0M;

  [ObservableProperty]
  public partial string Currency { get; set; } = "";
}