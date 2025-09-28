using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public partial class Order : ObservableObject {
  [ObservableProperty]
  public partial string Name { get; set; } = "";
  [ObservableProperty]
  public partial string AccountBase { get; set; } = "";
  [ObservableProperty]
  public partial string AccountCode { get; set; } = "";
  [ObservableProperty]
  public partial IList<OrderMethod> MethodsAllowed { get; set; } = [];
  [ObservableProperty]
  public partial OrderMethod? SelectedMethod { get; set; } = null;
  [ObservableProperty]
  public partial string Ticker { get; set; } = "";
  [ObservableProperty]
  public partial decimal UnitPrice { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal Quantity { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal StopLossPrice { get; set; } = 0.0M;
}