using System.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class OrderForm : ObservableObject {
  [ObservableProperty]
  public partial MarketItemLabel ItemLabel { get; set; } = new();
  [ObservableProperty]
  public partial object? OrderMethod { get; set; }
  [ObservableProperty]
  public partial object? AvailableOrderMethod { get; set; }
  [ObservableProperty]
  public partial decimal UnitPrice { get; set; }
  [ObservableProperty]
  public partial decimal Quantity { get; set; }
  [ObservableProperty]
  public partial decimal StopLossPrice { get; set; }
  [ObservableProperty]
  public partial bool BlockPriceInput { get; set; }

  public virtual void Reset(string name = "", string ticker = "") {
    ItemLabel.Name = name;
    ItemLabel.Ticker = ticker;
    UnitPrice = 0.0M;
    Quantity = 0;
    StopLossPrice = 0;
    BlockPriceInput = false;
  }
  public abstract void Long();
  public abstract Task LongAsync();
  public abstract void Short();
  public abstract Task ShortAsync();
}