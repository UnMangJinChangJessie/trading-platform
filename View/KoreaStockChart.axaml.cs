using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  public KoreaStockChart() {
    InitializeComponent();
    var orderContext = OrderView.DataContext as ViewModel.Order;
    orderContext?.MethodsAllowed = [
      OrderMethod.Limit, OrderMethod.Market, OrderMethod.ConditionalLimit, OrderMethod.BestOffer,
    ];
    CastedDataContext?.PropertyChanged += OnDataContextChanged;
    OrderView.UnitPriceChanged += (sender, args) => {
      var castedSender = sender as NumericUpDown;
      if (castedSender == null) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) castedSender.Value = StockMarketInformation.KRXStock.GetTickIncrement(castedSender.Value.Value - castedSender.Increment);
      else castedSender.Value = StockMarketInformation.KRXStock.GetTickDecrement(castedSender.Value.Value + castedSender.Increment);
    };
    OrderView.StopLossPriceChanged += (sender, args) => {
      var castedSender = sender as NumericUpDown;
      if (castedSender == null) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) castedSender.Value = StockMarketInformation.KRXStock.GetTickIncrement(castedSender.Value.Value - castedSender.Increment);
      else castedSender.Value = StockMarketInformation.KRXStock.GetTickDecrement(castedSender.Value.Value + castedSender.Increment);
    };
  }
  private void OnDataContextChanged(object? sender, PropertyChangedEventArgs args) {
    var orderContext = OrderView.DataContext as ViewModel.Order;
    orderContext?.Ticker = CastedDataContext?.Ticker ?? "";
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    await CastedDataContext.RefreshAsync(ticker);
    await CastedDataContext.StartRefreshRealTimeAsync(ticker);
    if (OrderBookDisplayView.DataContext is ViewModel.KoreaInvestment.StockOrderBook orderContext) {
      await orderContext.RefreshAsync(ticker);
      await orderContext.StartRefreshRealTimeAsync(ticker);
    }
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.EndRefreshRealTimeAsync();
    if (OrderBookDisplayView.DataContext is ViewModel.KoreaInvestment.StockOrderBook orderContext) {
      await orderContext.EndRefreshRealTimeAsync();
    }
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    await CastedDataContext.RefreshAsync(ticker);
    await CastedDataContext.StartRefreshRealTimeAsync(ticker);
    if (OrderBookDisplayView.DataContext is ViewModel.KoreaInvestment.StockOrderBook orderContext) {
      await orderContext.RefreshAsync(ticker);
      await orderContext.StartRefreshRealTimeAsync(ticker);
    }
  }
}