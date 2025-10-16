using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model;
using trading_platform.ViewModel;

namespace trading_platform.View.KoreaInvestment;

public partial class KoreaStockChart : UserControl {
  private ViewModel.KoreaInvestment.StockMarketData? CastedDataContext => DataContext as ViewModel.KoreaInvestment.StockMarketData;
  private const int UNIT_PRICE_SPINNER = 0;
  private const int STOP_LOSS_SPINNER = 1;
  private int ModifyingPriceSpinner = 0;
  public KoreaStockChart() {
    InitializeComponent();
    OrderView.PriceSpinner.Spinned += PriceSpinner_Spined;
    OrderView.PriceSpinner.GotFocus += (sender, args) => ModifyingPriceSpinner = 0;
    OrderView.StopLossSpinner.Spinned += PriceSpinner_Spined;
    OrderView.StopLossSpinner.GotFocus += (sender, args) => ModifyingPriceSpinner = 1;
    OrderBookDisplayView.ClickPrice += OnClickPrice;
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
  }
  private void OnClickPrice(object? sender, RoutedEventArgs args) {
    if (args.Source is not OrderBookItem item) return;
    if (ModifyingPriceSpinner == UNIT_PRICE_SPINNER) OrderView.PriceSpinner.Value = item.Price;
    else if (ModifyingPriceSpinner == STOP_LOSS_SPINNER) OrderView.StopLossSpinner.Value = item.Price;
  }
  private void PriceSpinner_Spined(object? sender, SpinEventArgs args) {
      if (sender is not NumericUpDown castedSender) return;
      if (CastedDataContext == null) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) {
        castedSender.Value = StockMarketInformation
          .KRXStock
          .GetTickIncrement(castedSender.Value.Value - castedSender.Increment, CastedDataContext.SecuritiesType);
      }
      else {
        castedSender.Value = StockMarketInformation
          .KRXStock
          .GetTickDecrement(castedSender.Value.Value + castedSender.Increment, CastedDataContext.SecuritiesType);
      }
    }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    var meow = new Dictionary<string, object>() {
      ["ticker"] = CastedDataContext.Ticker
    };
    await CastedDataContext.RefreshAsync(meow);
    await CastedDataContext.StartRefreshRealtimeAsync(meow);
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.EndRefreshRealtimeAsync(ViewModel.MarketData.NullArguments);
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    string ticker = TickerTextBox.Text ?? "";
    await CastedDataContext.EndRefreshRealtimeAsync(ViewModel.MarketData.NullArguments);
    var meow = new Dictionary<string, object>() {
      ["ticker"] = ticker
    };
    await CastedDataContext.RefreshAsync(meow);
    await CastedDataContext.StartRefreshRealtimeAsync(meow);
  }
}