using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class MarketData : ObservableObject {
  [ObservableProperty]
  public partial List<Model.OHLC> PriceChart { get; set; } = [];
  [ObservableProperty]
  public partial List<decimal> VolumeChart { get; set; } = [];
  [ObservableProperty]
  public partial List<decimal> AmountChart { get; set; } = [];
  [ObservableProperty]
  public partial string Ticker { get; set; } = "";
  [ObservableProperty]
  public partial string Name { get; set; } = "";
  [ObservableProperty]
  public partial float EarningPerShare { get; set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceBookValueRatio { get; set; } = 0.0F;
  [ObservableProperty]
  public partial float PriceEarningRatio { get; set; } = 0.0F;
  [ObservableProperty]
  public partial float ReturnOnEquity { get; set; } = 0.0F;

  public MarketData() {
    if (Design.IsDesignMode) {
      PriceChart = [ ..Model.Generators.Series.GenerateBrownianOHLC(450.00, 0.01, 2.0, TimeSpan.FromHours(1), DateTime.Now, 300) ];
    }
  }
}