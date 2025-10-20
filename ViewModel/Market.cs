using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.View;

namespace trading_platform.ViewModel;

public abstract partial class Market : ObservableObject, IRefresh {
  public virtual ObservableCollection<MarketItem?> InspectingItems { get; set; }
  [ObservableProperty]
  public virtual partial Order Order { get; set; }
  [ObservableProperty]
  public virtual partial Balance Balance { get; set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}