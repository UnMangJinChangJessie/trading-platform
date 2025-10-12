using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace trading_platform.Dialogs;

public partial class OkDialog {
  /// <summary>
  /// Title StyledProperty definition
  /// indicates the title of the dialog.
  /// </summary>
  public static readonly StyledProperty<string> TitleProperty =
      AvaloniaProperty.Register<OkDialog, string>(nameof(Title));

  /// <summary>
  /// Gets or sets the Title property. This StyledProperty
  /// indicates the title of the dialog.
  /// </summary>
  public string Title {
    get => this.GetValue(TitleProperty);
    set => SetValue(TitleProperty, value);
  }
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