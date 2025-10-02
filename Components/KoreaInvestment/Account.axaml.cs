using Avalonia;
using Avalonia.Controls;

namespace trading_platform.Components.KoreaInvestment;

public partial class Account : UserControl {
  /// <summary>
  /// AccountBase StyledProperty definition
  /// indicates the first 8 digits of an account number.
  /// </summary>
  public static readonly StyledProperty<string> AccountBaseProperty =
      AvaloniaProperty.Register<Account, string>(nameof(AccountBase));

  /// <summary>
  /// Gets or sets the AccountBase property. This StyledProperty
  /// indicates the first 8 digits of an account number.
  /// </summary>
  public string AccountBase {
    get => GetValue(AccountBaseProperty);
    set => SetValue(AccountBaseProperty, value);
  }
  /// <summary>
  /// AccountCode StyledProperty definition
  /// indicates the last 2 digits of an account number.
  /// </summary>
  public static readonly StyledProperty<string> AccountCodeProperty =
      AvaloniaProperty.Register<Account, string>(nameof(AccountCode));

  /// <summary>
  /// Gets or sets the AccountCode property. This StyledProperty
  /// indicates the last 2 digits of an account number.
  /// </summary>
  public string AccountCode {
    get => GetValue(AccountCodeProperty);
    set => SetValue(AccountCodeProperty, value);
  }

  public Account() {
    InitializeComponent();
  }
}