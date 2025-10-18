using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class OrderBookItem(decimal price, decimal ask, decimal bid) : ObservableObject {
  [ObservableProperty]
  public partial decimal Price { get; set; } = price;
  [ObservableProperty]
  public partial decimal AskQuantity { get; set; } = ask;
  [ObservableProperty]
  public partial decimal BidQuantity { get; set; } = bid;
}

public abstract partial class OrderBook(MarketItemLabel label) : ObservableObject, IRefresh {
  [ObservableProperty]
  public partial MarketItemLabel Label { get; set; } = label;
  public ObservableCollection<OrderBookItem> CurrentOrders { get; set; } = [];
  protected void InsertOrder(decimal price, decimal ask, decimal bid) {
    if (!Monitor.IsEntered(CurrentOrders)) {
      throw new SynchronizationLockException();
    }
    int index = CurrentOrders.BinarySearch(price, x => x.Price);
    if (index < 0) CurrentOrders.Insert(~index, new(price, ask, bid));
    else CurrentOrders[index] = new(price, ask, bid);
  }
  protected void ZeroOutOutOfRange(decimal min, decimal max) {
    if (!Monitor.IsEntered(CurrentOrders)) {
      throw new SynchronizationLockException();
    }
    int maxIndex = CurrentOrders.BinarySearch(max, x => x.Price);
    int minIndex = CurrentOrders.BinarySearch(min, x => x.Price);
    maxIndex = maxIndex < 0 ? ~maxIndex : (maxIndex + 1);
    minIndex = (minIndex < 0 ? ~minIndex : minIndex) - 1;
    for (int i = maxIndex; i < CurrentOrders.Count; i++) {
      CurrentOrders[i] = new(CurrentOrders[i].Price, 0, 0);
    }
    for (int i = minIndex; i >= 0; i--) {
      CurrentOrders[i] = new(CurrentOrders[i].Price, 0, 0);
    }
  }
  public abstract void Refresh();
  public abstract Task RefreshAsync();
}