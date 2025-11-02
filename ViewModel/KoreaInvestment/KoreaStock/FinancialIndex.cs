using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

public partial class FinancialIndex : ObservableObject {
  [ObservableProperty]
  public partial DateOnly StatementPoint { get; set; }
  [ObservableProperty]
  public partial float SalesChangeRate { get; set; }
  [ObservableProperty]
  public partial float EarningChangeRate { get; set; }
  [ObservableProperty]
  public partial float ProfitChangeRate { get; set; }
  [ObservableProperty]
  public partial float ReturnOnEquity { get; set; }
  [ObservableProperty]
  public partial decimal EarningPerShare { get; set; }
  [ObservableProperty]
  public partial decimal SalesPerShare { get; set; }
  [ObservableProperty]
  public partial decimal BookValuePerShare { get; set; }
  [ObservableProperty]
  public partial float RetentionRate { get; set; }
  [ObservableProperty]
  public partial float DebtRate { get; set; }
}