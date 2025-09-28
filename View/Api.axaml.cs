using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View;

public partial class Api : UserControl {
  public Api() {
    InitializeComponent();
  }
  public async void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (string.IsNullOrEmpty(ApiClient.AppPublicKey) && File.Exists("kis-developers-key.json")) {
      var json = await JsonSerializer.DeserializeAsync<JsonElement>(File.OpenRead("kis-developers-key.json"));
      if (json.TryGetProperty("public_key", out var pubKey)) ApiClient.AppPublicKey = pubKey.GetString() ?? "";
      if (json.TryGetProperty("secret_key", out var secretKey)) ApiClient.AppSecretKey = secretKey.GetString() ?? "";
    }
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
    AccessKeyTextBox.Text = ApiClient.AccessToken;
    AccessKeyExpireTextBox.Text = ApiClient.AccessTokenExpire.ToString("yyyy-MM-ddThh:mm:sszzz");
  }
  private void AppPublicKey_TextChanged(object? sender, RoutedEventArgs args) {
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
  }
}