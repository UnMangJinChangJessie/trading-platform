using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public abstract partial class OrderBook : ObservableObject, IRefresh, IRefreshRealtime {
  public readonly static Dictionary<string, object> NullArguments = [];
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
  [ObservableProperty]
  public partial decimal HighestQuantity { get; protected set; } = 0;
  public bool RealTimeRefresh { get; protected set; } = false;

  public abstract Task RefreshAsync(IDictionary<string, object> args);
  public abstract Task StartRefreshRealtimeAsync(IDictionary<string, object> args);
  public abstract Task EndRefreshRealtimeAsync(IDictionary<string, object> args);
}