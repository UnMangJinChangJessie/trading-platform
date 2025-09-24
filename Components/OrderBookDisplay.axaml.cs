using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace trading_platform.Components;

public partial class OrderBookDisplay : UserControl {
  private readonly TextBlock[] SellingPriceTextBlocks = new TextBlock[10];
  private readonly TextBlock[] BuyingPriceTextBlocks = new TextBlock[10];
  private readonly OrderBookQuantityBlock[] SellingQuantityBlocks = new OrderBookQuantityBlock[10];
  private readonly OrderBookQuantityBlock[] BuyingQuantityBlocks = new OrderBookQuantityBlock[10];

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
  public IBrush? NeutralBrush
  {
    get => GetValue(NeutralBrushProperty);
    set => SetValue(NeutralBrushProperty, value);
  }

  public OrderBookDisplay() {
    InitializeComponent();
    DataContextChanged += UpdateColours;
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    LongBrush = new SolidColorBrush(Colors.Pink);
    ShortBrush = new SolidColorBrush(Colors.SkyBlue);
    NeutralBrush = new SolidColorBrush(Colors.Black);
    UpdateColours(sender, args);
  }
  public void UpdateColours(object? sender, EventArgs args) {
    var blocks = PART_Grid.Children.OfType<TextBlock>();
    var context = DataContext as ViewModel.OrderBook;
    foreach (var block in blocks) {
      int rowIdx = Grid.GetRow(block);
      int viewIdx = Math.Abs(rowIdx - 9) + (rowIdx < 10 ? 0 : -1);
      decimal[] view = rowIdx < 10 ? context!.SellingPrices : context!.BuyingQuantities;
      block.Foreground = context!.PreviousClose.CompareTo(view[viewIdx]) switch {
        > 0 => ShortBrush,
        < 0 => LongBrush,
        0 => NeutralBrush
      };
    }
  }
}