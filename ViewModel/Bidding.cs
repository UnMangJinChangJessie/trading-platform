using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class Bidding : ObservableObject {
  public partial class Bid : ObservableObject {
    [ObservableProperty]
    public partial decimal Price { get; set; }
    [ObservableProperty]
    public partial decimal Quantity { get; set; }
  }
  [ObservableProperty]
  public partial Bid[] Selling { get; set; } = Enumerable.Range(0, 10).Select(_ => new Bid()).ToArray();
  [ObservableProperty]
  public partial Bid[] Buying { get; set; } = Enumerable.Range(0, 10).Select(_ => new Bid()).ToArray();
  [ObservableProperty] public partial decimal Volume { get; set; } = 0.0M;
  
}