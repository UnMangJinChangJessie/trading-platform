using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace trading_platform.Dialogs;

public partial class OkDialog {
  /// <summary>
  /// Message StyledProperty definition
  /// indicates the dialog text.
  /// </summary>
  public static readonly StyledProperty<string> MessageProperty =
      AvaloniaProperty.Register<OkDialog, string>(nameof(Message));

  /// <summary>
  /// Gets or sets the Message property. This StyledProperty
  /// indicates the dialog text.
  /// </summary>
  public string Message {
    get => this.GetValue(MessageProperty);
    set => SetValue(MessageProperty, value);
  }
}
public partial class OkDialog : Window {
  public OkDialog(string title, string text) {
    InitializeComponent();
    DialogTextBlock.Text = text;
    Title = title;
  }
  private void OkButton_Click(object? sender, RoutedEventArgs args) {
    Close(true);
  }
  private void Window_Closing(object? sender, WindowClosingEventArgs args) {
    Close(true);
  }
}