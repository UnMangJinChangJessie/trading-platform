using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public abstract partial class OrderForm(IEnumerable<object> methodsList, MarketItemLabel label) : ObservableObject {
  [ObservableProperty]
  public partial MarketItemLabel ItemLabel { get; set; } = label;
  [ObservableProperty]
  public partial object? OrderMethod { get; set; }
  public ObservableCollection<object> AvailableOrderMethod { get; set; } = new(methodsList);
  [ObservableProperty]
  public partial decimal UnitPrice { get; set; }
  [ObservableProperty]
  public partial decimal Quantity { get; set; }
  [ObservableProperty]
  public partial decimal? StopLossPrice { get; set; }
  [ObservableProperty]
  public partial bool BlockPriceInput { get; set; } = true;
  [ObservableProperty]
  public partial bool BlockStopLossPriceInput { get; set; } = true;

  public virtual void IncreaseUnitPriceTick() {
    UnitPrice += 1;
  }
  public virtual void IncreaseStopLossPriceTick() {
    StopLossPrice += 1;
  }
  public virtual void DecreaseUnitPriceTick() {
    UnitPrice -= 1;
  }
  public virtual void DecreaseStopLossPriceTick() {
    StopLossPrice += 1;
  }
  // 일본 주식이나 암호화폐와 같이 거래 단위가 1이 아닌 경우가 있음
  public virtual void IncreaseQuantityTick() {
    Quantity += 1;
  }
  public virtual void DecreaseQuantityTick() {
    Quantity -= 1;
  }

  public virtual void Reset(string name = "", string ticker = "") {
    ItemLabel.Name = name;
    ItemLabel.Ticker = ticker;
    UnitPrice = 0.0M;
    Quantity = 0;
    StopLossPrice = 0;
    BlockPriceInput = false;
  }
  public abstract void Long();
  public abstract Task LongAsync();
  public abstract void Short();
  public abstract Task ShortAsync();
}