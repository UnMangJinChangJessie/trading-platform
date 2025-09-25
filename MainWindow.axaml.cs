using System.Diagnostics;
using Avalonia.Controls;

namespace trading_platform;

public partial class MainWindow : Window {
  public MainWindow() {
    InitializeComponent();
    var loadedKrx = Model.StockMarketInformation.KRXStock.Load().Result;
    if (!loadedKrx) {
      Debug.WriteLine("Failed to fetch KRX listings data.");
    }
  }
}