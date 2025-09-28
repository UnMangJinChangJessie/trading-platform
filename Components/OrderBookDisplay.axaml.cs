using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
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
  private ViewModel.KoreaInvestment.StockOrderBook? CastedDataContext => DataContext as ViewModel.KoreaInvestment.StockOrderBook;
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



  public OrderBookDisplay() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    LongBrush ??= new SolidColorBrush(Colors.Pink);
    ShortBrush ??= new SolidColorBrush(Colors.SkyBlue);
    NeutralBrush ??= new SolidColorBrush(Colors.Black);
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    var blocks = PART_Grid.Children.OfType<TextBlock>();
    foreach (var block in blocks) {
      int rowIdx = Grid.GetRow(block);
      int viewIdx = Math.Abs(rowIdx - 9) + (rowIdx < 10 ? 0 : -1);
      TextBlock[] controls = rowIdx < 10 ? ref SellingPriceTextBlocks : ref BuyingPriceTextBlocks;
      controls[viewIdx] = block;
    }
    var quantities = PART_Grid.Children.OfType<OrderBookQuantityBlock>();
    foreach (var block in quantities) {
      int rowIdx = Grid.GetRow(block);
      int viewIdx = Math.Abs(rowIdx - 9) + (rowIdx < 10 ? 0 : -1);
      OrderBookQuantityBlock[] controls = rowIdx < 10 ? ref SellingQuantityBlocks : ref BuyingQuantityBlocks;
      controls[viewIdx] = block;
    }
  }
  public void UpdatePriceBlocks(object? sender, PropertyChangedEventArgs args) {
    if (CastedDataContext == null) return;
    IBrush? PickColor(decimal price, decimal close) {
      return (price - close) switch {
        > 0 => LongBrush,
        < 0 => ShortBrush,
        0 => NeutralBrush
      };
    }
    for (int i = 0; i < 10; i++) {
      TextElement.SetForeground(SellingPriceTextBlocks[i], PickColor(CastedDataContext.AskPrice[i].Value, 0.0M));
      TextElement.SetForeground(BuyingPriceTextBlocks[i], PickColor(CastedDataContext.BidPrice[i].Value, 0.0M));
    }
  }
}