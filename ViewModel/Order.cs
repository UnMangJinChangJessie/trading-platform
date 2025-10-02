using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public abstract partial class Order : ObservableObject {
  [ObservableProperty]
  public partial string Name { get; set; } = "";
  [ObservableProperty]
  public partial string Ticker { get; set; } = "";
  [ObservableProperty]
  public partial IEnumerable MethodsList { get; protected set; }
  [ObservableProperty]
  public partial object? SelectedMethod { get; set; }
  [ObservableProperty]
  public partial decimal UnitPrice { get; set; } = 0.0M;
  [ObservableProperty]
  public partial bool BlockPriceInput { get; set; } = false;
  [ObservableProperty]
  public partial decimal Quantity { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal StopLossPrice { get; set; } = 0.0M;

  public abstract ValueTask<bool> Long();
  public abstract ValueTask<bool> Short();
}