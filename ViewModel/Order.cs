using Avalonia;
using trading_platform.KoreaInvestment;

namespace trading_platform.ViewModel;

public class Order : AvaloniaObject {
  public static readonly StyledProperty<OrderMethod> SelectionProperty =
    AvaloniaProperty.Register<Order, OrderMethod>(nameof(Selection));
  public OrderMethod Selection {
    get => GetValue(SelectionProperty);
    set => SetValue(SelectionProperty, value);
  }
  public static readonly StyledProperty<decimal> UnitPriceProperty =
    AvaloniaProperty.Register<Order, decimal>(nameof(UnitPrice));
  public decimal UnitPrice {
    get => GetValue(UnitPriceProperty);
    set => SetValue(UnitPriceProperty, value);
  }
  public static readonly StyledProperty<decimal> QuantityProperty =
    AvaloniaProperty.Register<Order, decimal>(nameof(Quantity));
  public decimal Quantity {
    get => GetValue(QuantityProperty);
    set => SetValue(QuantityProperty, value);
  }
  public static readonly StyledProperty<decimal> StopLossPriceProperty =
    AvaloniaProperty.Register<Order, decimal>(nameof(StopLossPrice));
  public decimal StopLossPrice {
    get => GetValue(StopLossPriceProperty);
    set => SetValue(StopLossPriceProperty, value);
  }
}