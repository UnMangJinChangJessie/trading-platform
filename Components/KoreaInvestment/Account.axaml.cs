using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.Components.KoreaInvestment;

public partial class Account : UserControl {
  private Account? CastedDataContext => DataContext as Account;
  public Account() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {  }
}