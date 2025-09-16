using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using trading_platform.KoreaInvestment;

namespace trading_platform.AppScreen;

public partial class Api : UserControl {
  public Api() {
    InitializeComponent();
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
  }
  private void AppPublicKey_TextChanged(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    ApiClient.AppPublicKey = ((TextBox)sender).Text ?? "";
  }
  private void AppSecretKey_TextChanged(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    ApiClient.AppSecretKey = ((TextBox)sender).Text ?? "";
  }
  private static async void IssueButton_Click(object? sender, RoutedEventArgs args) {
    var success = await ApiClient.IssueToken();
  }
  private static async void RevokeButton_Click(object? sender, RoutedEventArgs args) {
    var success = await ApiClient.RevokeToken();
  }
}