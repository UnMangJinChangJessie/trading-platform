using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

public partial class FinancialStatements : ObservableObject {
  [ObservableProperty]
  public partial DateOnly StatementPoint { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalAssets))]
  [NotifyPropertyChangedFor(nameof(TotalCapital))]
  public partial decimal CurrentAssets { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalAssets))]
  [NotifyPropertyChangedFor(nameof(TotalCapital))]
  public partial decimal NonCurrentAssets { get; set; }
  public decimal TotalAssets => CurrentAssets + NonCurrentAssets;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalDebts))]
  [NotifyPropertyChangedFor(nameof(TotalCapital))]
  public partial decimal CurrentLiabilities { get; set; }
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalDebts))]
  [NotifyPropertyChangedFor(nameof(TotalCapital))]
  public partial decimal NonCurrentLiabilities { get; set; }
  public decimal TotalDebts => CurrentLiabilities + NonCurrentLiabilities;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(TotalCapital))]
  public partial decimal CapitalFunds { get; set; }
  public decimal TotalCapital => TotalAssets - TotalDebts + CapitalFunds;
}