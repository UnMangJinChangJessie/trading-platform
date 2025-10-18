using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class Balance {
  public partial class Item : ObservableObject {
    [ObservableProperty]
    public partial MarketItemLabel Label { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentProfitLoss), nameof(CurrentProfitLossRate))]
    public partial decimal EntryAmount { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AveragePrice))]
    public partial decimal Quantity { get; set; }
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CurrentProfitLoss), nameof(CurrentProfitLossRate))]
    public partial decimal CurrentEvaluation { get; set; }

    public decimal CurrentProfitLoss => EntryAmount == 0 ? 0 : (CurrentEvaluation - EntryAmount);
    public float CurrentProfitLossRate => EntryAmount == 0 ? 0.0F : (float)(CurrentEvaluation / EntryAmount - 1);
    public decimal AveragePrice => Quantity == 0 ? 0 : EntryAmount / Quantity;
  }
}

public abstract partial class Balance : ObservableObject, IRefresh {
  public ObservableCollection<Item> HoldingItems { get; protected set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalFunds))]
  public partial decimal FreeFunds { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalProfitLoss), nameof(TotalProfitLossRate))]
  public partial decimal TotalEntryAmount { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalProfitLoss), nameof(TotalProfitLossRate))]
  public partial decimal TotalEvaluation { get; set; }
  
  public decimal TotalFunds => TotalEntryAmount + FreeFunds;
  public decimal TotalProfitLoss => TotalEntryAmount == 0 ? 0 : (TotalEvaluation - TotalEntryAmount);
  public float TotalProfitLossRate => TotalEntryAmount == 0 ? 0 : (float)(TotalEvaluation / TotalEntryAmount - 1);

  public Balance() {
    HoldingItems = [];
  }
  public abstract void Refresh();
  public abstract Task RefreshAsync();
}