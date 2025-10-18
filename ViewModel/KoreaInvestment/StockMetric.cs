using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class StockMetric : ObservableObject {
  // 비율은 정밀한 수치 저장이 필요 없음
  // 단, 주당순이익, 주당순자산가치는 절대적인 값이므로 정확한 수치가 필요할 수 있음
  // (주당순이익이 8자리가 넘고 그럴 거 같진 않다만 혹시라도...)

  [ObservableProperty]
  [Description("주당순자산가치")]
  public partial decimal BookValuePerShare { get; set; }
  [ObservableProperty]
  [Description("주당순이익")]
  public partial decimal EarningPerShare { get; set; }
  [ObservableProperty]
  [Description("주가순이익비율")]
  public partial float PriceEarningRate { get; set; }
  [ObservableProperty]
  [Description("주가순자산비율")]
  public partial float PriceBookValueRate { get; set; }
  [ObservableProperty]
  [Description("자기자본이익률")]
  public partial float ReturnOnEquity { get; set; }
  [ObservableProperty]
  [Description("부채 비율")]
  public partial float DebtRate { get; set; }
}