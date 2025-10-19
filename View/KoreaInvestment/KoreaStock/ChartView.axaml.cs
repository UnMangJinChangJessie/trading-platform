using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model;
using trading_platform.ViewModel;

namespace trading_platform.View.KoreaInvestment.KoreaStock;

public partial class ChartView : UserControl {
  private ViewModel.KoreaInvestment.KoreaStock.MarketItem? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KoreaStock.MarketItem;
  public ChartView() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.RefreshAsync();
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    if (StockMarketInformation.KRXStock.SearchByTicker(ticker) is not StockMarketInformation.KRXStockInformation info) return;
    CastedDataContext.ItemLabel.Ticker = info.Ticker;
    CastedDataContext.ItemLabel.Name = info.Name;
    await CastedDataContext.RefreshAsync();
  }
}