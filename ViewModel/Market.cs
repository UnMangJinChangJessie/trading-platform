using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.View;

namespace trading_platform.ViewModel;

public abstract partial class Market(MarketItem first) : ObservableObject, IRefresh {
  public ObservableCollection<MarketItem?> InspectingItems { get; set; } = [ first ];
  [ObservableProperty]
  public partial Order Order { get; set; }
  [ObservableProperty]
  public partial Balance Balance { get; set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}