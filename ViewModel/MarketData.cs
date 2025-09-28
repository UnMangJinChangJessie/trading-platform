using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class MarketData : ObservableObject {
  public ObservableCollection<Reactive<Model.OHLC<decimal>>> PriceChart { get; protected set; } = [];
  public ObservableCollection<Reactive<decimal>> VolumeChart { get; protected set; } = [];
  public ObservableCollection<Reactive<decimal>> AmountChart { get; protected set; } = [];
  [ObservableProperty]
  public partial DateTime DateTime { get; protected set; } = DateTime.Now;
  [ObservableProperty]
  public partial decimal CurrentOpen { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentHigh { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentLow { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentClose { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentVolume { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal CurrentAmount { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial decimal PreviousClose { get; protected set; } = 0.0M;
  [ObservableProperty]
  public partial string Currency { get; protected set; } = "";
  [ObservableProperty]
  public partial string Ticker { get; protected set; } = "";
  [ObservableProperty]
  public partial string Name { get; protected set; } = "";
  [ObservableProperty]
  public partial bool RealTimeRefresh { get; protected set; } = false;

  public abstract ValueTask<bool> RequestRefreshAsync(string ticker);
  public async Task RefreshAsync(string ticker) {
    await RequestRefreshAsync(ticker);
  }
  public abstract ValueTask<bool> RequestRefreshRealTimeAsync(string ticker);
  public async Task StartRefreshRealTimeAsync(string ticker) {
    if (RealTimeRefresh && ticker == Ticker) return;
    await EndRefreshRealTimeAsync();
    await RequestRefreshRealTimeAsync(ticker);
  }
  public abstract Task EndRefreshRealTimeAsync();
}