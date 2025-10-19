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
}