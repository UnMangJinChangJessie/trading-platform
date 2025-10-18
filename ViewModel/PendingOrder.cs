using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class PendingOrder : ObservableObject {
  [ObservableProperty]
  public partial MarketItemLabel Label { get; set; }
  [ObservableProperty]
  public partial object? Method { get; set; }
  [ObservableProperty]
  public partial decimal ConcludedAmount { get; set; }
  [ObservableProperty]
  public partial decimal OrderedQuantity { get; set; }
  [ObservableProperty]
  public partial decimal ConcludedQuantity { get; set; }
  [ObservableProperty]
  public partial decimal ModifiableQuantity { get; set; }
  [ObservableProperty]
  public partial decimal UnitPrice { get; set; }
}