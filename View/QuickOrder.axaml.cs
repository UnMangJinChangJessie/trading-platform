using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using trading_platform.ViewModel;

namespace trading_platform.View;

public partial class QuickOrderView : UserControl {
  private QuickOrderViewModel? CastedDataContext => DataContext as QuickOrderViewModel;
  public QuickOrderView() {
    InitializeComponent();
    Loaded += (_, _) => BuildOrderBookGrid();
  }
  private void BuildOrderBookGrid() {
    for (int i = 1; i <= 20; i++) {
      OrderBookGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
      AddOrderBookGridRow(OrderBookGrid, i);
    }
  }
  private void StackPanel_PointerWheelChanged(object? sender, PointerWheelEventArgs args) {
    if (CastedDataContext == null) return;
    if (OrderBookGrid.IsPointerOver) {
      if (args.Delta.Y < 0) {
        for (int i = 0; i < -args.Delta.Y; i++) {
          RotateRowsDown();
          SetPriceGridCell(0, CastedDataContext.CurrentOrderForm.GetNextPriceTick((decimal)GetPriceGridCell(1)!.DataContext!));
        }
      }
      else if (args.Delta.Y > 0) {
        for (int i = 0; i < args.Delta.Y; i++) {
          RotateRowsUp();
          SetPriceGridCell(19, CastedDataContext.CurrentOrderForm.GetPreviousPriceTick((decimal)GetPriceGridCell(18)!.DataContext!));
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