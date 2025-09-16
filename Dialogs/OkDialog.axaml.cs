using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace trading_platform.Dialogs;

public partial class OkDialog : Window {
  public OkDialog(string title, string text) {
    InitializeComponent();
    DialogTextBlock.Text = text;
    Title = title;
  }
  private void OkButton_Click(object? sender, RoutedEventArgs args) {
    Close();
  }
}