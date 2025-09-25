using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

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
  /// DecimalPointCount StyledProperty definition
  /// indicates the number of digits below the decimal point.
  /// </summary>
  public static readonly StyledProperty<int> DecimalPointCountProperty =
      AvaloniaProperty.Register<OrderBookQuantityBlock, int>(nameof(DecimalPointCount));

  /// <summary>
  /// Gets or sets the DecimalPointCount property. This StyledProperty
  /// indicates the number of digits below the decimal point.
  /// </summary>
  public int DecimalPointCount {
    get => this.GetValue(DecimalPointCountProperty);
    set => SetValue(DecimalPointCountProperty, value);
  }
  /// <summary>
  /// LongColor StyledProperty definition
  /// indicates long color.
  /// </summary>
  public static readonly StyledProperty<IBrush?> LongColorProperty =
      AvaloniaProperty.Register<OrderBookQuantityBlock, IBrush?>(nameof(LongColor));

  /// <summary>
  /// Gets or sets the LongColor property. This StyledProperty
  /// indicates long color.
  /// </summary>
  public IBrush? LongColor {
    get => this.GetValue(LongColorProperty);
    set => SetValue(LongColorProperty, value);
  }
  /// <summary>
  /// ShortColor StyledProperty definition
  /// indicates short color.
  /// </summary>
  public static readonly StyledProperty<IBrush?> ShortColorProperty =
      AvaloniaProperty.Register<OrderBookQuantityBlock, IBrush?>(nameof(ShortColor));

  /// <summary>
  /// Gets or sets the ShortColor property. This StyledProperty
  /// indicates short color.
  /// </summary>
  public IBrush? ShortColor {
    get => this.GetValue(ShortColorProperty);
    set => SetValue(ShortColorProperty, value);
  }
  public OrderBookQuantityBlock() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    ShortColor ??= new SolidColorBrush(Colors.LightSkyBlue);
    LongColor ??= new SolidColorBrush(Colors.LightPink);
    PART_Rectangle.HorizontalAlignment = IsSelling ? Avalonia.Layout.HorizontalAlignment.Right : Avalonia.Layout.HorizontalAlignment.Left;
    PART_Rectangle.Fill = IsSelling ? ShortColor : LongColor;
    PART_Rectangle.Height = Bounds.Height * 0.95;
    var context = DataContext as ViewModel.OrderBook;
    DataContextChanged += UpdateBar;
    UpdateBar(sender, args);
  }
  public void UpdateBar(object? sender, EventArgs args) {
    var boundWidth = Bounds.Width;
    var context = DataContext as ViewModel.OrderBook;

    var percentage = IsSelling ? context!.SellingVisualBarRatio(OrderViewIndex) : context!.BuyingVisualBarRatio(OrderViewIndex);
    PART_Rectangle.Width = percentage * boundWidth;
    var quantity = IsSelling ? context!.SellingQuantities[OrderViewIndex] : context!.BuyingQuantities[OrderViewIndex];
    PART_TextBlock.Text = string.Format($"{{0:F{DecimalPointCount}}}", quantity);
  }
}