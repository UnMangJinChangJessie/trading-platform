using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform.Dialogs;

public partial class YesNoDialog : Window {
  /// <summary>
  /// Content StyledProperty definition
  /// indicates the content inside of the dialog.
  /// </summary>
  public static readonly StyledProperty<object?> DialogContentProperty =
    AvaloniaProperty.Register<YesNoDialog, object?>(nameof(Content));

  /// <summary>
  /// Gets or sets the Content property. This StyledProperty
  /// indicates the content inside of the dialog.
  /// </summary>
  public object? DialogContent {
    get => GetValue(ContentProperty);
    set => SetValue(ContentProperty, value);
  }

  public YesNoDialog() {
    InitializeComponent();
  }
  public void YesButton_Click(object? sender, RoutedEventArgs args) {
    Close(true);
  }
  public void NoButton_Click(object? sender, RoutedEventArgs args) {
    Close(false);
  }
}