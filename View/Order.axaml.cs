using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform.View;

public partial class Order : UserControl {
  private ViewModel.OrderForm? CastedDataContext => DataContext as ViewModel.OrderForm;
  public Order() {
    InitializeComponent();
  }
  public async void LongButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    CastedDataContext.Long();
  }
  public async void ShortButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    CastedDataContext.Short();
  }
  public void UnitPrice_Spinned(object? sender, SpinEventArgs args) {
    if (sender is not NumericUpDown upDown) return;
    if (CastedDataContext == null) return;
    if (args.Direction == SpinDirection.Increase) {
      CastedDataContext.UnitPrice -= upDown.Increment;
      CastedDataContext.IncreaseUnitPriceTick();
    }
    else {
      CastedDataContext.UnitPrice += upDown.Increment;
      CastedDataContext.DecreaseUnitPriceTick();
    }
  }
  public void Quantity_Spinned(object? sender, SpinEventArgs args) {
    if (sender is not NumericUpDown upDown) return;
    if (CastedDataContext == null) return;
    if (args.Direction == SpinDirection.Increase) {
      CastedDataContext.Quantity -= upDown.Increment;
      CastedDataContext.IncreaseQuantityTick();
    }
    else {
      CastedDataContext.Quantity += upDown.Increment;
      CastedDataContext.DecreaseQuantityTick();
    }
  }
  public void StopLossPrice_Spinned(object? sender, SpinEventArgs args) {
    if (sender is not NumericUpDown upDown) return;
    if (CastedDataContext == null) return;
    if (args.Direction == SpinDirection.Increase) {
      CastedDataContext.StopLossPrice -= upDown.Increment;
      CastedDataContext.IncreaseStopLossPriceTick();
    }
    else {
      CastedDataContext.UnitPrice += upDown.Increment;
      CastedDataContext.DecreaseStopLossPriceTick();
    }
  }
}