using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View;

public partial class Order : UserControl {
  public Order() {
    InitializeComponent();
  }
  public event EventHandler<RoutedEventArgs> ClickShort;
  public event EventHandler<RoutedEventArgs> ClickLong;
  public event EventHandler<SpinEventArgs> UnitPriceChanged;
  public event EventHandler<SpinEventArgs> StopLossPriceChanged;
  public void LongButton_Click(object? sender, RoutedEventArgs args) {
    ClickLong?.Invoke(sender, args);
  }
  public void ShortButton_Click(object? sender, RoutedEventArgs args) {
    ClickShort?.Invoke(sender, args);
  }
  public void UnitPriceSpinner_ValueChanged(object? sender, SpinEventArgs args) {
    UnitPriceChanged?.Invoke(UnitPriceNumericUpDown, args);
  }
  public void StopLossPriceSpinner_ValueChanged(object? sender, SpinEventArgs args) {
    StopLossPriceChanged?.Invoke(StopLossNumericUpDown, args);
  }
}