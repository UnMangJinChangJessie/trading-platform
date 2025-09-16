using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace trading_platform.Components;

public partial class AccountForm : ContentControl
{
  public AccountForm()
  {
    InitializeComponent();
  }
  public string GetAccount() {
    return AccountBase.Text + AccountCode.Text;
  }
}