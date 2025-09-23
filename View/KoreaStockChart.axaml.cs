using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  public KoreaStockChart() {
    DataContext ??= new ViewModel.KoreaStockChart(); 
    InitializeComponent();
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    var (status, result) = await DomesticStock.InquireStockPrice(new() {
      Ticker = (DataContext as ViewModel.KoreaStockChart)!.Ticker,
      Exchange = Exchange.DomesticUnified
    });
    if (result == null) return;
    if (status != System.Net.HttpStatusCode.OK || result.Information == null) {
      return;
    }
    UpdateChart(result.Information);
  }
  public void UpdateChart(StockDetailInformation info) {
    if (DataContext is not ViewModel.KoreaStockChart context) return;
    context.Name = "";
    context.Ticker = info.Ticker;
    context.CurrentPrice = info.Close;
    context.PreviousClose = info.Close - info.PriceChange;
    context.DailyVolume = info.Volume;
  }
}