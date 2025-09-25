using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class Order : UserControl, IDataContextIsMarketData {
  public Order() {
    InitializeComponent();
  }
  public event EventHandler<RoutedEventArgs> ClickShort;
  public event EventHandler<RoutedEventArgs> ClickLong;
  public void LongButton_Click(object? sender, RoutedEventArgs args) {
    ClickLong?.Invoke(sender, args);
  }
  public void ShortButton_Click(object? sender, RoutedEventArgs args) {
    ClickShort?.Invoke(sender, args);
  }
}