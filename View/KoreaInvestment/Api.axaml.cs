using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View.KoreaInvestment;

public partial class Api : UserControl {
  public Api() {
    InitializeComponent();
  }
  public async void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (string.IsNullOrEmpty(ApiClient.AppPublicKey) && File.Exists("kis-developers-key.json")) {
      var json = await JsonSerializer.DeserializeAsync<JsonElement>(File.OpenRead("kis-developers-key.json"));
      if (json.TryGetProperty("public_key", out var pubKey)) ApiClient.AppPublicKey = pubKey.GetString() ?? "";
      if (json.TryGetProperty("secret_key", out var secretKey)) ApiClient.AppSecretKey = secretKey.GetString() ?? "";
      if (json.TryGetProperty("account_base", out var accountBase)) ApiClient.DefaultAccountBase = accountBase.GetString() ?? "";
      if (json.TryGetProperty("account_code", out var accountCode)) ApiClient.DefaultAccountCode = accountCode.GetString() ?? "";
      if (json.TryGetProperty("hts_id", out var brokerageId)) ApiClient.BrokerageId = brokerageId.GetString() ?? "";
    }
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
    BrokerageId.Text = ApiClient.BrokerageId;
    DefaultAccount.Text = ApiClient.DefaultAccountBase + ApiClient.DefaultAccountCode;
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    AppPublicKey.Text = ApiClient.AppPublicKey;
    AppSecretKey.Text = ApiClient.AppSecretKey;
    AccessKeyTextBox.Text = ApiClient.AccessToken;
    AccessKeyExpireTextBox.Text = ApiClient.AccessTokenExpire.ToString("yyyy-MM-ddThh:mm:sszzz");
  }
  private void AppPublicKey_TextChanged(object? sender, TextChangedEventArgs args) {
    ApiClient.AppPublicKey = AppPublicKey.Text ?? "";
  }
  private void AppSecretKey_TextChanged(object? sender, TextChangedEventArgs args) {
    ApiClient.AppSecretKey = AppSecretKey.Text ?? "";
  }
  private void DefaultAccount_TextChanged(object? sender, TextChangedEventArgs args) {
    ApiClient.DefaultAccountBase = DefaultAccount.Text?[..8] ?? "";
    ApiClient.DefaultAccountCode = DefaultAccount.Text?[8..] ?? "";
  }
  private void BrokerageId_TextChanged(object? sender, TextChangedEventArgs args) {
    ApiClient.BrokerageId = BrokerageId.Text ?? "";
  }
  private async void IssueButton_Click(object? sender, RoutedEventArgs args) {
    var success = await ApiClient.IssueToken();
    if (success) {
      AccessKeyTextBox.Text = ApiClient.AccessToken;
      AccessKeyExpireTextBox.Text = ApiClient.AccessTokenExpire.ToString("yyyy-MM-ddThh:mm:sszzz");
    }
    await ApiClient.KisWebSocket.Connect();
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