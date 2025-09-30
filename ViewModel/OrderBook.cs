using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public abstract partial class OrderBook : ObservableObject {
  [ObservableProperty]
  public partial TimeOnly ConclusionTime { get; protected set; } = TimeOnly.MinValue;
  [ObservableProperty]
  public partial string Ticker { get; protected set; } = "";
  [ObservableProperty]
  public partial decimal CurrentClose { get; set; }
  [ObservableProperty]
  public partial decimal PreviousClose { get; set; }
  public List<Reactive<decimal>> AskPrice { get; protected set; } = [..Enumerable.Repeat(0, 10).Select(_ => new Reactive<decimal>(0.0M))];
  public List<Reactive<decimal>> BidPrice { get; protected set; } = [..Enumerable.Repeat(0, 10).Select(_ => new Reactive<decimal>(0.0M))];
  public List<Reactive<decimal>> AskQuantity { get; protected set; } = [..Enumerable.Repeat(0, 10).Select(_ => new Reactive<decimal>(0.0M))];
  public List<Reactive<decimal>> BidQuantity { get; protected set; } = [..Enumerable.Repeat(0, 10).Select(_ => new Reactive<decimal>(0.0M))];
  [ObservableProperty]
  public partial decimal? IntermediatePrice { get; protected set; }
  [ObservableProperty]
  public partial decimal? IntermediateAskQuantity { get; protected set; }
  [ObservableProperty]
  public partial decimal? IntermediateBidQuantity { get; protected set; }
  public bool RealTimeRefresh { get; protected set; } = false;

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