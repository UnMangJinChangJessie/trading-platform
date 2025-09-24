using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;

namespace trading_platform.Components;

public partial class OrderBookQuantityBlock : UserControl {
  /// <summary>
  /// IsSelling StyledProperty definition
  /// indicates if the quantity block indicates selling ones.
  /// </summary>
  public static readonly StyledProperty<bool> IsSellingProperty =
    AvaloniaProperty.Register<OrderBookQuantityBlock, bool>(nameof(IsSelling));
  /// <summary>
  /// Gets or sets the IsSelling property. This StyledProperty
  /// indicates if the quantity block indicates selling ones.
  /// </summary>
  public bool IsSelling {
    get => GetValue(IsSellingProperty);
    set => SetValue(IsSellingProperty, value);
  }
  /// <summary>
  /// OrderViewIndex StyledProperty definition
  /// indicates which order price and quantity the control will watch for.
  /// </summary>
  public static readonly StyledProperty<int> OrderViewIndexProperty =
    AvaloniaProperty.Register<OrderBookQuantityBlock, int>(nameof(OrderViewIndex));

  /// <summary>
  /// Gets or sets the OrderViewIndex property. This StyledProperty
  /// indicates which order price and quantity the control will watch for.
  /// </summary>
  public int OrderViewIndex {
    get => GetValue(OrderViewIndexProperty);
    set => SetValue(OrderViewIndexProperty, value);
  }
  /// <summary>
  /// Quantity StyledProperty definition
  /// indicates quantity in decimal.
  /// </summary>
  public static readonly StyledProperty<decimal> QuantityProperty =
    AvaloniaProperty.Register<OrderBookQuantityBlock, decimal>(nameof(Quantity));
  /// <summary>
  /// Gets or sets the Quantity property. This StyledProperty
  /// indicates quantity in decimal.
  /// </summary>
  public decimal Quantity {
    get => GetValue(QuantityProperty);
    set => SetValue(QuantityProperty, value);
  }

  public OrderBookQuantityBlock() {
    InitializeComponent();
  }
}