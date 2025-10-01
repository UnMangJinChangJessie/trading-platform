using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using trading_platform.ViewModel;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  private static readonly OrderMethod[] ALLOWED_ORDER_METHODS = [
    OrderMethod.Limit, OrderMethod.Market, OrderMethod.ConditionalLimit, OrderMethod.BestOffer, OrderMethod.TopPriority
  ];
  public KoreaStockChart() {
    InitializeComponent();
    var orderContext = OrderView.DataContext as ViewModel.Order;
    if (orderContext != null) {
      orderContext.MethodsAllowed.Clear();
      foreach (var item in ALLOWED_ORDER_METHODS) orderContext.MethodsAllowed.Add(item);
    }
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    CastedDataContext.PropertyChanged += OnDataContextChanged;
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
    Dispatcher.UIThread.Post(() => {
      var orderContext = OrderView.DataContext as ViewModel.Order;
      var orderBookContext = OrderBookDisplayView.DataContext as OrderBook;
      if (orderContext?.Ticker != null) orderContext.Ticker = CastedDataContext?.Ticker ?? "";
      if (args.PropertyName == nameof(CastedDataContext.PreviousClose)) {
        orderBookContext?.PreviousClose = CastedDataContext?.PreviousClose ?? 0;
      }
      if (args.PropertyName == nameof(CastedDataContext.CurrentClose)) {
        orderBookContext?.CurrentClose = CastedDataContext?.CurrentClose ?? 0;
      }
    });
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
    await CastedDataContext.EndRefreshRealTimeAsync(CastedDataContext.Ticker);
    if (OrderBookDisplayView.DataContext is ViewModel.KoreaInvestment.StockOrderBook orderContext) {
      await orderContext.EndRefreshRealTimeAsync(orderContext.Ticker);
    }
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    await CastedDataContext.EndRefreshRealTimeAsync(ticker);
    await CastedDataContext.RefreshAsync(ticker);
    await CastedDataContext.StartRefreshRealTimeAsync(ticker);
    if (OrderBookDisplayView.DataContext is ViewModel.KoreaInvestment.StockOrderBook orderContext) {
      await CastedDataContext.EndRefreshRealTimeAsync(ticker);
      await orderContext.RefreshAsync(ticker);
      await orderContext.StartRefreshRealTimeAsync(ticker);
    }
  }
}