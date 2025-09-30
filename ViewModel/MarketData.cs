using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public abstract partial class MarketData : ObservableObject {
  public enum CandleUpdate {
    InsertEnd, InsertBegin, Clear, UpdateLast
  };
  [ObservableProperty]
  public partial List<Model.OHLC<decimal>> PriceChart { get; protected set; } = [];
  [ObservableProperty]
  public partial List<Reactive<decimal>> VolumeChart { get; protected set; } = [];
  [ObservableProperty]
  public partial List<Reactive<decimal>> AmountChart { get; protected set; } = [];
  [ObservableProperty]
  public partial DateTime CurrentDateTime { get; protected set; } = DateTime.Now;
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

  public event EventHandler<(CandleUpdate UpdateType, Model.OHLC<decimal>? Candle, Reactive<decimal>? Volume, Reactive<decimal>? Amount)> ChartChanging;

  protected void ClearChart() {
    ChartChanging?.Invoke(this, (CandleUpdate.Clear, null, null, null));
    PriceChart.Clear();
  }
  protected void InsertCandleEnd(decimal open, decimal high, decimal low, decimal close, decimal volume, decimal amount, DateTime dateTime, TimeSpan timeSpan) {
    PriceChart.Add(new(open, high, low, close) { DateTime = dateTime, TimeSpan = timeSpan });
    VolumeChart.Add(new(volume));
    AmountChart.Add(new(amount));
    ChartChanging?.Invoke(this, (CandleUpdate.InsertEnd, PriceChart[^1], VolumeChart[^1], AmountChart[^1]));
  }
  protected void InsertCandleBegin(decimal open, decimal high, decimal low, decimal close, decimal volume, decimal amount, DateTime dateTime, TimeSpan timeSpan) {
    PriceChart.Insert(0, new(open, high, low, close) { DateTime = dateTime, TimeSpan = timeSpan });
    VolumeChart.Insert(0, new(volume));
    AmountChart.Insert(0, new(amount));
    ChartChanging?.Invoke(this, (CandleUpdate.InsertBegin, PriceChart[0], VolumeChart[0], AmountChart[0]));
  }
  protected void UpdateLastCandle(decimal open, decimal high, decimal low, decimal close, decimal volume, decimal amount) {
    if (PriceChart.Count == 0) return;
    PriceChart[^1].Open = open;
    PriceChart[^1].High = high;
    PriceChart[^1].Low = low;
    PriceChart[^1].Close = close;
    VolumeChart[^1].Value = volume;
    AmountChart[^1].Value = amount;
    ChartChanging?.Invoke(this, (CandleUpdate.UpdateLast, PriceChart[^1], VolumeChart[^1], AmountChart[^1]));
  }
  public abstract ValueTask<bool> RequestRefreshAsync(string ticker);
  public async Task RefreshAsync(string ticker) {
    await RequestRefreshAsync(ticker);
  }
  public abstract ValueTask<bool> RequestRefreshRealTimeAsync(string ticker);
  public async Task StartRefreshRealTimeAsync(string ticker) {
    if (!await ApiClient.KisWebSocket.Connect()) return;
    if (ApiClient.KisWebSocket.ClientState != System.Net.WebSockets.WebSocketState.Open) return; 
    await RequestRefreshRealTimeAsync(ticker);
  }
  public abstract Task EndRefreshRealTimeAsync(string ticker);
}