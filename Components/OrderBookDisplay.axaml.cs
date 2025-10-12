using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace trading_platform.Components;

public partial class OrderBookDisplay : UserControl {
  private ViewModel.OrderBook? CastedDataContext => DataContext as ViewModel.OrderBook;
  /// <summary>
  /// LongBrush StyledProperty definition
  /// indicates long brush.
  /// </summary>
  public static readonly StyledProperty<IBrush?> LongBrushProperty =
    AvaloniaProperty.Register<OrderBookDisplay, IBrush?>(nameof(LongBrush));
  /// <summary>
  /// Gets or sets the LongBrush property. This StyledProperty
  /// indicates long brush.
  /// </summary>
  public IBrush? LongBrush {
    get => GetValue(LongBrushProperty);
    set => SetValue(LongBrushProperty, value);
  }
  /// <summary>
  /// ShortBrush StyledProperty definition
  /// indicates short brush.
  /// </summary>
  public static readonly StyledProperty<IBrush?> ShortBrushProperty =
    AvaloniaProperty.Register<OrderBookDisplay, IBrush?>(nameof(ShortBrush));
  /// <summary>
  /// Gets or sets the ShortBrush property. This StyledProperty
  /// indicates short brush.
  /// </summary>
  public IBrush? ShortBrush {
    get => GetValue(ShortBrushProperty);
    set => SetValue(ShortBrushProperty, value);
  }
  /// <summary>
  /// NeutralBrush StyledProperty definition
  /// indicates neutral(i.e. previous close == current price) brush.
  /// </summary>
  public static readonly StyledProperty<IBrush?> NeutralBrushProperty =
    AvaloniaProperty.Register<OrderBookDisplay, IBrush?>(nameof(NeutralBrush));
  /// <summary>
  /// Gets or sets the NeutralBrush property. This StyledProperty
  /// indicates neutral(i.e. previous close == current price) brush.
  /// </summary>
  public IBrush? NeutralBrush {
    get => GetValue(NeutralBrushProperty);
    set => SetValue(NeutralBrushProperty, value);
  }
  /// <summary>
  /// PriceDecimalPoint StyledProperty definition
  /// indicates the digits under the fraction point of prices.
  /// </summary>
  public static readonly StyledProperty<int> PriceDecimalPointProperty =
    AvaloniaProperty.Register<OrderBookDisplay, int>(nameof(PriceDecimalPoint));
  /// <summary>
  /// Gets or sets the PriceDecimalPoint property. This StyledProperty
  /// indicates the digits under the fraction point of prices.
  /// </summary>
  public int PriceDecimalPoint {
    get => this.GetValue(PriceDecimalPointProperty);
    set => SetValue(PriceDecimalPointProperty, value);
  }
  /// <summary>
  /// QuantityDecimalPoint StyledProperty definition
  /// indicates the digits under the fraction points of quantities.
  /// </summary>
  public static readonly StyledProperty<int> QuantityDecimalPointProperty =
    AvaloniaProperty.Register<OrderBookDisplay, int>(nameof(QuantityDecimalPoint));
  /// <summary>
  /// Gets or sets the QuantityDecimalPoint property. This StyledProperty
  /// indicates the digits under the fraction points of quantities.
  /// </summary>
  public int QuantityDecimalPoint {
    get => this.GetValue(QuantityDecimalPointProperty);
    set => SetValue(QuantityDecimalPointProperty, value);
  }
  /// <summary>
  /// Transparent StyledProperty definition
  /// indicates transparent brush because I couldn't bind to the predefined one.
  /// </summary>
  public static readonly StyledProperty<IBrush> TransparentProperty =
      AvaloniaProperty.Register<OrderBookDisplay, IBrush>(nameof(Transparent));

  /// <summary>
  /// Gets or sets the Transparent property. This StyledProperty
  /// indicates transparent brush because I couldn't bind to the predefined one.
  /// </summary>
  public IBrush Transparent {
    get => this.GetValue(TransparentProperty);
    set => SetValue(TransparentProperty, value);
  }

  public OrderBookDisplay() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    LongBrush ??= new SolidColorBrush(Colors.Pink);
    ShortBrush ??= new SolidColorBrush(Colors.SkyBlue);
    NeutralBrush ??= new SolidColorBrush(Colors.Black);
  }
}