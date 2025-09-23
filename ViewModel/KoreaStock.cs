using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class KoreaStock : ObservableObject {
  [ObservableProperty] public partial decimal CurrentPrice { get; set; } = 0.0M;
  [ObservableProperty] public partial decimal DailyVolume { get; set; } = 0.0M;
  [ObservableProperty] public partial decimal PreviousClose { get; set; } = 0.0M;
  [ObservableProperty] public partial string Ticker { get; set; } = "";
  [ObservableProperty] public partial string Name { get; set; } = "";
}