using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

public partial class FinancialProfitLoss : ObservableObject {
  [ObservableProperty]
  public partial DateOnly StatementPoint { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(SaleProfit))]
  public partial decimal Sales { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(SaleProfit))]
  public partial decimal SalesCost { get; set; }
  public decimal SaleProfit => Sales - SalesCost;
  [ObservableProperty]
  public partial decimal Profit { get; set; }
  [ObservableProperty]
  public partial decimal OrdinaryProfit { get; set; }
  [ObservableProperty]
  public partial decimal SpecialProfit { get; set; }
  [ObservableProperty]
  public partial decimal SpecialLoss { get; set; }
  [ObservableProperty]
  public partial decimal NetProfit { get; set; }
}