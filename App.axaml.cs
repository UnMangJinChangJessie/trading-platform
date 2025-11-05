using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace trading_platform;

public partial class App : Application {
  private static async Task LoadMasters() {
    var loadedKrx = await Model.StockMarketInformation.KRXStock.Load();
    if (!loadedKrx) {
      Debug.WriteLine("Failed to fetch KRX listings data.");
    }
    var loadedOversea = await Model.StockMarketInformation.OverseaStock.Load();
    if (!loadedOversea) {
      Debug.WriteLine("Failed to fetch oversea listings data.");
    }
    var loadedKrxFutures = await Model.StockMarketInformation.KRXFutures.Load();
    if (!loadedKrxFutures) {
      Debug.WriteLine("Failed to fetch KRX futures listings.");
    }
  }
  public override void Initialize() {
    AvaloniaXamlLoader.Load(this);
    Avalonia.Threading.Dispatcher.UIThread.Post(async () => { await LoadMasters(); });
    Avalonia.Threading.DispatcherTimer.Run(
      () => {
        Avalonia.Threading.Dispatcher.UIThread.Post(async () => { await LoadMasters(); });
        return true;
      },
      TimeSpan.FromMinutes(90)
    );
  }

  public override void OnFrameworkInitializationCompleted() {
    if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
      desktop.MainWindow = new MainWindow();
    }
    base.OnFrameworkInitializationCompleted();
  }
}