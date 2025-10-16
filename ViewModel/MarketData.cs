using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel;

public abstract partial class MarketData : ObservableObject, IRefresh {
  public enum CandleUpdate {
    InsertEnd, InsertBegin, Clear, UpdateLast
  };
  public readonly static Dictionary<string, object> NullArguments = [];
  [ObservableProperty]
  public partial Model.Charts.CandlestickChartData PriceChart { get; protected set; } = new();
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
  public partial float OpenChangeRate { get; protected set; } = 0.0F;
  [ObservableProperty]
  public partial float HighChangeRate { get; protected set; } = 0.0F;
  [ObservableProperty]
  public partial float LowChangeRate { get; protected set; } = 0.0F;
  [ObservableProperty]
  public partial float CloseChangeRate { get; protected set; } = 0.0F;
  [ObservableProperty]
  public partial decimal Change { get; protected set; } = 0;
  [ObservableProperty]
  public partial string Currency { get; protected set; } = "";
  [ObservableProperty]
  public partial string Ticker { get; protected set; } = "";
  [ObservableProperty]
  public partial string Name { get; protected set; } = "";
  // Now we are integrating the order book to the market data.
  [ObservableProperty]
  public partial OrderBook? CurrentOrderBook { get; protected set; }
  [ObservableProperty]
  public partial Order? CurrentOrder { get; protected set; }

  protected static float GetChangeRate(decimal from, decimal to) {
    if (from <= 0) return float.NaN;
    else return (float)(to - from) / (float)from;
  }
  protected void ChangeDependentValues() {
    OpenChangeRate = GetChangeRate(PreviousClose, CurrentOpen);
    HighChangeRate = GetChangeRate(PreviousClose, CurrentHigh);
    LowChangeRate = GetChangeRate(PreviousClose, CurrentLow);
    CloseChangeRate = GetChangeRate(PreviousClose, CurrentClose);
    OnPropertyChanged(nameof(OpenChangeRate));
    OnPropertyChanged(nameof(HighChangeRate));
    OnPropertyChanged(nameof(LowChangeRate));
    OnPropertyChanged(nameof(CloseChangeRate));
    Change = CurrentClose - PreviousClose;
    OnPropertyChanged(nameof(Change));
    CurrentOrderBook?.PreviousClose = PreviousClose;
    CurrentOrderBook?.CurrentClose = CurrentClose;
    CurrentOrder?.Name = Name;
    CurrentOrder?.Ticker = Ticker;
  }
  public abstract Task RefreshAsync(IDictionary<string, object> args);
}