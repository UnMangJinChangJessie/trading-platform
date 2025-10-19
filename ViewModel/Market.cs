using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.View;

namespace trading_platform.ViewModel;

public abstract partial class Market(MarketItem first) : ObservableObject, IRefresh {
  [ObservableProperty]
  public partial MarketItem?[] InspectingItems { get; set; } = [ first ];
  [ObservableProperty]
  public partial Order Order { get; set; }
  [ObservableProperty]
  public partial Balance Balance { get; set; }

  public void ResizeItem(int count) {
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(count, 0, nameof(count));
    MarketItem?[] newInspectingItems = new MarketItem?[count];
    lock (InspectingItems) {
      for (int i = 0; i < Math.Min(count, InspectingItems.Length); i++) {
        newInspectingItems[i] = InspectingItems[i];
      }
      InspectingItems = newInspectingItems;
    }
  }
  public abstract void Refresh();
  public abstract Task RefreshAsync();
}