using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class Api : UserControl {
  public Api() {
    InitializeComponent();
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
  }
  private void AppPublicKey_TextChanged(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    ApiClient.AppPublicKey = AppPublicKey.Text ?? "";
  }
  private void AppSecretKey_TextChanged(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    ApiClient.AppSecretKey = AppSecretKey.Text ?? "";
  }
  private async void IssueButton_Click(object? sender, RoutedEventArgs args) {
    var success = await ApiClient.IssueToken();
    if (success) {
      AccessKeyTextBox.Text = ApiClient.AccessToken;
      AccessKeyExpireTextBox.Text = ApiClient.AccessTokenExpire.ToString("yyyy-MM-ddThh:mm:sszzz");
    }
    success = await ApiClient.IssueWebSocketToken();
    if (success) {
      WebSocketAccessKeyTextBox.Text = ApiClient.WebSocketAccessToken;
    }
  }
  private async void RevokeButton_Click(object? sender, RoutedEventArgs args) {
    var success = await ApiClient.RevokeToken();
    if (success) {
      AccessKeyTextBox.Text = "";
      AccessKeyExpireTextBox.Text = "";
    }
  }
  private void RevealButton_Click(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    AccessKeyTextBox.RevealPassword = !AccessKeyTextBox.RevealPassword;
    WebSocketAccessKeyTextBox.RevealPassword = !WebSocketAccessKeyTextBox.RevealPassword;
  }
}