using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using Avalonia.Interactivity;
using trading_platform.ViewModel;

namespace trading_platform.View;

public partial class QuickOrderView : UserControl {
  private QuickOrderViewModel? CastedDataContext => DataContext as QuickOrderViewModel;
  private List<Control[]> _orderBookGridRows;
  public QuickOrderView() {
    InitializeComponent();
    _orderBookGridRows = [];
    Initialized += (_, _) => BuildOrderBookGrid();
  }
  private void BuildOrderBookGrid() {
    for (int i = 1; i <= 20; i++) {
      OrderBookGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
      AddOrderBookGridRow(OrderBookGrid, 2 * i - 1);
      // 가로선 추가
      OrderBookGrid.RowDefinitions.Add(new RowDefinition(1.0, GridUnitType.Pixel));
      var separator = new Separator() {
        Background = Foreground,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Stretch,
        Margin = new(0)
      };
      Grid.SetColumn(separator, 0);
      Grid.SetColumnSpan(separator, 5);
      Grid.SetRow(separator, 2 * i);
      OrderBookGrid.Children.Add(separator);
    }
  }
  private void StackPanel_PointerWheelChanged(object? sender, PointerWheelEventArgs args) {
    if (CastedDataContext == null) return;
    if (OrderBookGrid.IsPointerOver) {
      if (args.Delta.Y < 0) {
        for (int i = 0; i < -args.Delta.Y; i++) {
          RotatePriceDown();
        }
      }
      else if (args.Delta.Y > 0) {
        for (int i = 0; i < args.Delta.Y; i++) {
          RotatePriceUp();
        }
      }
    }
  }
  private void QuantityNumericUpDown_Spined(object? sender, SpinEventArgs args) {
    
  }
  private void StopLossCheckBox_Click(object? sender, RoutedEventArgs args) {
    StopLossNumericUpDown.SetValue(InputElement.IsEnabledProperty, StopLossCheckBox.IsChecked ?? false);
  }
  private void QuantityRadioButton_Checked(object? sender, RoutedEventArgs args) {
    QuantityNumericUpDown.SetValue(InputElement.IsEnabledProperty, QuantityRadioButton.IsChecked ?? false);
  }
}