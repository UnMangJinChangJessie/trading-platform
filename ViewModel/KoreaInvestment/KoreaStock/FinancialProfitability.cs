using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

public partial class FinancialProfitability : ObservableObject {
  [ObservableProperty]
  public partial DateOnly StatementPoint { get; set; }
  [ObservableProperty]
  public partial float ProfitCapitalRate { get; set; }
  [ObservableProperty]
  public partial float ProfitOwnCapitalRate { get; set; }
  [ObservableProperty]
  public partial float ProfitSalesRate { get; set; }
  [ObservableProperty]
  public partial float NetProfitSalesRate { get; set; }
}