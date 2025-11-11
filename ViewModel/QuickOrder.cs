using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class QuickOrderViewModel(MarketItem item, OrderBook depth, OrderForm form) : ObservableObject {
  [ObservableProperty]
  public partial MarketItem CurrentItem { get; set; } = item;
  [ObservableProperty]
  public partial OrderBook CurrentOrderBook { get; set; } = depth;
  [ObservableProperty]
  public partial OrderForm CurrentOrderForm { get; set; } = form;
  [ObservableProperty]
  public partial int StopLossTick { get; set; } = 1;
  [ObservableProperty]
  public partial decimal Amount { get; set; } = 0;
}