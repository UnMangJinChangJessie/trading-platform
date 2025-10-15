using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using trading_platform.Model;

namespace trading_platform.Components;

public partial class QuickOrder {
  /// <summary>
  /// LongBrush StyledProperty definition
  /// indicates long brush.
  /// </summary>
  public static readonly StyledProperty<IBrush?> LongBrushProperty =
    AvaloniaProperty.Register<QuickOrder, IBrush?>(nameof(LongBrush));
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
    AvaloniaProperty.Register<QuickOrder, IBrush?>(nameof(ShortBrush));
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
    AvaloniaProperty.Register<QuickOrder, IBrush?>(nameof(NeutralBrush));
  /// <summary>
  /// Gets or sets the NeutralBrush property. This StyledProperty
  /// indicates neutral(i.e. previous close == current price) brush.
  /// </summary>
  public IBrush? NeutralBrush {
    get => GetValue(NeutralBrushProperty);
    set => SetValue(NeutralBrushProperty, value);
  }
  
  private ViewModel.QuickOrderItem? DraggingItem { get; set; }
  private ViewModel.QuickOrderItem? TargetItem { get; set; }
  private Position? TargetPosition { get; set; } = null;
}

public partial class QuickOrder : UserControl {
  private ViewModel.QuickOrder? CastedDataContext => DataContext as ViewModel.QuickOrder;
  public QuickOrder() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    LongBrush ??= new SolidColorBrush(Colors.Pink);
    ShortBrush ??= new SolidColorBrush(Colors.SkyBlue);
    NeutralBrush ??= new SolidColorBrush(Colors.Black);
  }
  public async void LongButton_PointerPressed(object? sender, PointerPressedEventArgs args) {
    if (CastedDataContext == null) return;
    // 의미 없는 클릭으로 간주
    if (args.ClickCount > 2) return;
    // 신규 롱 주문 진행
    if (args.ClickCount == 2) {
      await CastedDataContext.LongAsync(ViewModel.QuickOrder.NullArguments);
      return;
    }
    // sender는 선택한 QuickOrderItem 형식이지만 일단 확인한다.
    if (sender is not ViewModel.QuickOrderItem clickedOrder) return;
    DraggingItem = clickedOrder;
  }
  public async void LongButton_PointerReleased(object? sender, PointerReleasedEventArgs args) {
    if (CastedDataContext == null) return;
    if (DraggingItem != null && TargetItem != null && TargetPosition != null && DraggingItem != TargetItem) {
      await CastedDataContext.MoveAsync(new Dictionary<string, object>() {
        ["from_price"] = DraggingItem.Price,
        ["from_position"] = Position.Long,
        ["to_price"] = TargetItem.Price,
        ["to_position"] = TargetPosition
      });
    }
    DraggingItem = null;
    TargetItem = null;
  }
  public void LongButton_PointerEntered(object? sender, PointerEventArgs args) {
    if (sender is not ViewModel.QuickOrderItem hoverOrder) return;
    TargetItem = hoverOrder;
    TargetPosition = Position.Long;
  }
  public async void ShortButton_PointerPressed(object? sender, PointerPressedEventArgs args) {
    if (CastedDataContext == null) return;
    // 의미 없는 클릭으로 간주
    if (args.ClickCount > 2) return;
    // 신규 롱 주문 진행
    if (args.ClickCount == 2) {
      await CastedDataContext.ShortAsync(ViewModel.QuickOrder.NullArguments);
      return;
    }
    // sender는 선택한 QuickOrderItem 형식이지만 일단 확인한다.
    if (sender is not ViewModel.QuickOrderItem clickedOrder) return;
    DraggingItem = clickedOrder;
  }
  public async void ShortButton_PointerReleased(object? sender, PointerReleasedEventArgs args) {
    if (CastedDataContext == null) return;
    if (DraggingItem != null && TargetItem != null && TargetPosition != null && DraggingItem != TargetItem) {
      await CastedDataContext.MoveAsync(new Dictionary<string, object>() {
        ["from_price"] = DraggingItem.Price,
        ["from_position"] = Position.Short,
        ["to_price"] = TargetItem.Price,
        ["to_position"] = TargetPosition
      });
    }
    DraggingItem = null;
    TargetItem = null;
  }
  public void ShortButton_PointerEntered(object? sender, PointerEventArgs args) {
    if (sender is not ViewModel.QuickOrderItem hoverOrder) return;
    TargetItem = hoverOrder;
    TargetPosition = Position.Short;
  }
}