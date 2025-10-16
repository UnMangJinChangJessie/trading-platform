using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class ProfitLoss : ObservableObject, IRefresh {
  public partial class Item : ObservableObject {
    [ObservableProperty]
    public partial string Ticker { get; set; } = "";
    [ObservableProperty]
    public partial string Name { get; set; } = "";
    [ObservableProperty]
    public partial decimal EntryAmount { get; set; }
    [ObservableProperty]
    public partial decimal Quantity { get; set; }
    [ObservableProperty]
    public partial decimal AveragePrice { get; set; }
    [ObservableProperty]
    public partial decimal CurrentEvaluation { get; set; }
    [ObservableProperty]
    public partial decimal CurrentProfitLoss { get; set; }
    [ObservableProperty]
    public partial float CurrentProfitLossRate { get; set; }
    public virtual void ChangeDependentProperties() {
      AveragePrice = Quantity == 0 ? 0 : EntryAmount / Quantity;
      CurrentProfitLoss = CurrentEvaluation - EntryAmount;
      CurrentProfitLossRate = EntryAmount != 0 ? (float)CurrentProfitLoss / (float)EntryAmount : 0.0F;
    }
  }
  [ObservableProperty]
  public partial decimal TotalEntryAmount { get; set; }
  [ObservableProperty]
  public partial decimal TotalEvaluation { get; set; }
  [ObservableProperty]
  public partial decimal TotalProfitLoss { get; set; }
  [ObservableProperty]
  public partial float TotalProfitLossRate { get; set; }
  public ObservableCollection<Item> ProfitLosses { get; protected set; } = new();
  
  public virtual void ChangeDependentProperties() {
    TotalEntryAmount = ProfitLosses.Sum(x => x.EntryAmount);
    TotalEvaluation = ProfitLosses.Sum(x => x.CurrentEvaluation);
    TotalProfitLoss = TotalEvaluation - TotalEntryAmount;
    TotalProfitLossRate = TotalEntryAmount != 0 ? (float)TotalProfitLoss / (float)TotalEntryAmount : 0;
  }
  public abstract Task RefreshAsync(IDictionary<string, object> dict);
}