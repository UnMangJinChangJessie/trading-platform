using System.Diagnostics;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

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
  }
}