using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class MarketItem : ObservableObject, IRefresh {
  public MarketItemLabel ItemLabel {
    get => field;
    protected set {
      if (field != value) {
        field = value;
        ItemOHLC.ItemLabel = value;
        OnPropertyChanged(nameof(ItemLabel));
      }
    } 
  } = new();
  [ObservableProperty]
  public partial MarketItemOHLC ItemOHLC { get; protected set; } = new();
  [ObservableProperty]
  public partial Model.Charts.CandlestickChartData ItemChart { get; protected set; } = new();
  /// <summary>
  /// 호가 정보
  /// </summary>
  public abstract OrderBook ItemOrderBook { get; protected set; }

  public abstract void Refresh();
  public abstract Task RefreshAsync();
}