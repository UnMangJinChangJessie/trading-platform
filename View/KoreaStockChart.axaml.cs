using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  public KoreaStockChart() {
    InitializeComponent();
    CastedDataContext?.Currency = "원";
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    var ticker = CastedDataContext.Ticker.Trim();
    var searchResult = Model.StockMarketInformation.KRXStock.Data.Where(x => x.Ticker == ticker);
    if (!searchResult.Any()) return;
    CastedDataContext.Name = searchResult.First().Name;
    var (status, result) = await DomesticStock.InquireStockPrice(new() {
      Ticker = CastedDataContext.Ticker,
      Exchange = Exchange.DomesticUnified
    });
    if (result == null) return;
    if (status != System.Net.HttpStatusCode.OK || result.Information == null) {
      return;
    }
    CastedDataContext.CurrentOpen = result.Information.Open;
    CastedDataContext.CurrentHigh = result.Information.High;
    CastedDataContext.CurrentLow = result.Information.Low;
    CastedDataContext.CurrentClose = result.Information.Close;
    CastedDataContext.CurrentVolume = result.Information.Volume;
    CastedDataContext.CurrentAmount = result.Information.Amount;
    CastedDataContext.EarningPerShare = result.Information.EarningsPerShare;
    CastedDataContext.PriceBookValueRatio = result.Information.PriceBookValueRatio;
    CastedDataContext.PriceEarningsRatio = result.Information.PriceEarningsRate;
  }
}