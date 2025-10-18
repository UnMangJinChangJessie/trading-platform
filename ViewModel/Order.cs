using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;


public abstract partial class Order : ObservableObject, IRefresh {
  [ObservableProperty]
  public partial OrderForm Form { get; set; }
  public ObservableCollection<PendingOrder> PendingOrders { get; set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}