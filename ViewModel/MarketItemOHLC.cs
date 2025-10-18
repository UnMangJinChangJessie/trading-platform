using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class MarketItemOHLC : ObservableObject {
  [ObservableProperty]
  public partial DateTime CurrentDateTime { get; set; } = DateTime.Now;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(OpenChangeRate))]
  public partial decimal CurrentOpen { get; set; } = 0.0M;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(HighChangeRate))]
  public partial decimal CurrentHigh { get; set; } = 0.0M;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(LowChangeRate))]
  public partial decimal CurrentLow { get; set; } = 0.0M;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(CloseChangeRate))]
  public partial decimal CurrentClose { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentVolume { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentAmount { get; set; } = 0.0M;
  [ObservableProperty]
  [NotifyPropertyChangedFor(nameof(OpenChangeRate), nameof(HighChangeRate), nameof(LowChangeRate), nameof(CloseChangeRate), nameof(Change))]
  public partial decimal PreviousClose { get; set; } = 0.0M;
  public float OpenChangeRate => GetChangeRate(PreviousClose, CurrentOpen);
  public float HighChangeRate => GetChangeRate(PreviousClose, CurrentHigh);
  public float LowChangeRate => GetChangeRate(PreviousClose, CurrentLow);
  public float CloseChangeRate => GetChangeRate(PreviousClose, CurrentClose);
  public decimal Change => CurrentClose - PreviousClose;
  internal static float GetChangeRate(decimal from, decimal to) {
    if (from <= 0) return float.NaN;
    else return (float)(to - from) / (float)from;
  }
}