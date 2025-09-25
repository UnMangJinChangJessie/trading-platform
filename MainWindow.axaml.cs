using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform;

public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();
  }
  public static async void Window_Loaded(object? sender, RoutedEventArgs args) {
    var loadedKrx = await Model.StockMarketInformation.KRXStock.Load();
    if (!loadedKrx) {
      Debug.WriteLine("Failed to fetch KRX listings data.");
    }
    if (File.Exists("kis-developers-key.json")) {
      var json = await JsonSerializer.DeserializeAsync<JsonElement>(File.OpenRead("kis-developers-key.json"));
      if (json.TryGetProperty("public_key", out var pubKey)) ApiClient.AppPublicKey = pubKey.GetString() ?? "";
      if (json.TryGetProperty("secret_key", out var secretKey)) ApiClient.AppSecretKey = secretKey.GetString() ?? "";
    }
  }
}