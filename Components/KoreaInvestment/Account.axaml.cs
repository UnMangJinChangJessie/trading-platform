using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.Components.KoreaInvestment;

public partial class Account : UserControl {
  private ViewModel.KoreaInvestment.IAccount? CastedDataContext => DataContext as ViewModel.KoreaInvestment.IAccount;
  public Account() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    CastedDataContext.AccountBase = ApiClient.DefaultAccountBase;
    CastedDataContext.AccountCode = ApiClient.DefaultAccountCode;
  }
}