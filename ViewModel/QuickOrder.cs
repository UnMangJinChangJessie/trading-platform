using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class QuickOrderItem(decimal price, decimal ask, decimal bid) : ObservableObject {
  [ObservableProperty]
  public partial decimal Price { get; set; } = price;
  [ObservableProperty]
  public partial decimal AskQuantity { get; set; } = ask;
  [ObservableProperty]
  public partial decimal BidQuantity { get; set; } = bid;
  [ObservableProperty]
  public partial decimal ShortQuantity { get; set; } = 0;
  [ObservableProperty]
  public partial decimal LongQuantity { get; set; } = 0;
}

public abstract partial class QuickOrder(MarketItem item, OrderBook depth, OrderForm form) : ObservableObject {
  [ObservableProperty]
  public partial MarketItem CurrentItem { get; set; } = item;
  [ObservableProperty]
  public partial OrderBook CurrentOrderBook { get; set; } = depth;
  [ObservableProperty]
  public partial OrderForm CurrentOrderForm { get; set; } = form;
  public Func<decimal, object?, decimal> NextTickGenerator { get; init; } = default!;
  public Func<decimal, object?, decimal> PreviousTickGenerator { get; init; } = default!;
}