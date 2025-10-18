using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class MarketItem : ObservableObject, IRefresh {
  [ObservableProperty]
  public partial MarketItemLabel ItemLabel { get; protected set; } = new();
  [ObservableProperty]
  public partial MarketItemOHLC ItemOHLC { get; protected set; } = new();
  [ObservableProperty]
  public partial Model.Charts.CandlestickChartData ItemChart { get; protected set; } = new();
  [ObservableProperty]
  public partial string Currency { get; protected set; } = "";
  [ObservableProperty]
  /// <summary>
  /// 호가 정보
  /// </summary>
  public partial OrderBook ItemOrderBook { get; protected set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}