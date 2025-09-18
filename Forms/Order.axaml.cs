using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform.Forms;

public partial class Order : UserControl {
  public Order() {
    InitializeComponent();
    DataContext = new ViewModel.Order();
  }
  public event EventHandler<RoutedEventArgs> ClickShort;
  public event EventHandler<RoutedEventArgs> ClickLong;
  public void LongButton_Click(object? sender, RoutedEventArgs args) {
    ClickLong?.Invoke(sender, args);
  }
  public void ShortButton_Click(object? sender, RoutedEventArgs args) {
    ClickShort?.Invoke(sender, args);
  }
  public void OrderMethodComboBox_SelectionChanged(object? sender, RoutedEventArgs args) {
    if (DataContext is not ViewModel.Order context) return;
    if (context.SelectedMethod == null) return;
    var method = context.MethodsAllowed[context.SelectedMethod];
    if (method.IsPriceMarket()) {
      UnitPriceNumericUpDown.IsReadOnly = true;
      context.UnitPrice = 0;
    }
    else {
      UnitPriceNumericUpDown.IsReadOnly = false;
    }
    StopLossNumericUpDown.IsReadOnly = method != OrderMethod.StopLossLimit;
  }
}