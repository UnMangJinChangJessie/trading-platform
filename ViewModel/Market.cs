using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.View;

namespace trading_platform.ViewModel;

public abstract partial class Market : ObservableObject, IRefresh {
  public ObservableCollection<MarketItem> InspectingItems { get; protected set; } = [];
  [ObservableProperty]
  public partial Order Order { get; protected set; }
  [ObservableProperty]
  public partial Balance Balance { get; protected set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}