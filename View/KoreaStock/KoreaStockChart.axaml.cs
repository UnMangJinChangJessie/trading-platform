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
    OrderMethod.Limit,
    OrderMethod.IocLimit,
    OrderMethod.FokLimit,
    OrderMethod.Market,
    OrderMethod.IocLimit,
    OrderMethod.FokMarket,
    OrderMethod.BestOffer,
    OrderMethod.IocBestOffer,
    OrderMethod.FokBestOffer,
    OrderMethod.Intermediate,
    OrderMethod.IocIntermediate,
    OrderMethod.FokIntermediate,
    OrderMethod.ConditionalLimit,
    OrderMethod.TopPriority,
    OrderMethod.StopLossLimit
  ];
  public KoreaStockChart() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    CastedDataContext.PropertyChanged += OnDataContextChanged;
    if (OrderView.DataContext is ViewModel.Order orderContext) {
      orderContext.MethodsAllowed.Clear();
      foreach (var item in ALLOWED_ORDER_METHODS) orderContext.MethodsAllowed.Add(item);
    }
    OrderView.UnitPriceChanged += (sender, args) => {
      if (sender is not NumericUpDown castedSender) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) castedSender.Value = StockMarketInformation.KRXStock.GetTickIncrement(castedSender.Value.Value - castedSender.Increment);
      else castedSender.Value = StockMarketInformation.KRXStock.GetTickDecrement(castedSender.Value.Value + castedSender.Increment);
    };
    OrderView.StopLossPriceChanged += (sender, args) => {
      if (sender is not NumericUpDown castedSender) return;
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
      if (orderContext?.Ticker != null) {
        orderContext.Ticker = CastedDataContext.Ticker;
        orderContext.Name = CastedDataContext.Name;
      }
    });
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    var task_1 = CastedDataContext.RefreshAsync(CastedDataContext.Ticker);
    var task_2 = CastedDataContext.StartRefreshRealTimeAsync(CastedDataContext.Ticker);
    await task_1;
    await task_2;
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.EndRefreshRealTimeAsync(CastedDataContext.Ticker);
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    await CastedDataContext.EndRefreshRealTimeAsync(CastedDataContext.Ticker);
    var task_1 = CastedDataContext.RefreshAsync(ticker);
    var task_2 = CastedDataContext.StartRefreshRealTimeAsync(ticker);
    await task_1;
    await task_2;
  }
}