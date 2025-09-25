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
    var blocks = PART_Grid.Children.OfType<TextBlock>();
    foreach (var block in blocks) {
      int rowIdx = Grid.GetRow(block);
      int viewIdx = Math.Abs(rowIdx - 9) + (rowIdx < 10 ? 0 : -1);
      TextBlock[] controls = rowIdx < 10 ? SellingPriceTextBlocks : BuyingPriceTextBlocks;
      controls[viewIdx] = block;
    }
    var quantities = PART_Grid.Children.OfType<OrderBookQuantityBlock>();
    foreach (var block in quantities) {
      int rowIdx = Grid.GetRow(block);
      int viewIdx = Math.Abs(rowIdx - 9) + (rowIdx < 10 ? 0 : -1);
      OrderBookQuantityBlock[] controls = rowIdx < 10 ? SellingQuantityBlocks : BuyingQuantityBlocks;
      controls[viewIdx] = block;
    }
    UpdateColours(sender, args);
  }
  public void UpdateColours(object? sender, EventArgs args) {
    var context = DataContext as ViewModel.OrderBook;
    IBrush? colorPicker(decimal a, decimal b) {
      return a.CompareTo(b) switch {
        > 0 => LongBrush,
        < 0 => ShortBrush,
        0 => NeutralBrush
      };
    }
    for (int i = 0; i < 10; i++) {
      SellingPriceTextBlocks[i].Foreground = colorPicker(context!.SellingPrices[i], context!.PreviousClose);
      BuyingPriceTextBlocks[i].Foreground = colorPicker(context!.BuyingPrices[i], context!.PreviousClose);
    }
    // 마지막 체결가 표시
    if (context!.LastConclusion == null) {
      PART_ConclusionBorder.IsVisible = false;
    }
    else {
      int index = Array.FindIndex(context.SellingPrices, x => x == context.LastConclusion);
      if (index >= 0) {
        Grid.SetRow(PART_ConclusionBorder, 9 - index);
        PART_ConclusionBorder.IsVisible = true;
      }
      else {
        index = Array.FindIndex(context.BuyingPrices, x => x == context.LastConclusion);
        if (index >= 0) {
          Grid.SetRow(PART_ConclusionBorder, index + 10);
        }
      }
      PART_ConclusionBorder.IsVisible = index >= 0;
    }
  }
}