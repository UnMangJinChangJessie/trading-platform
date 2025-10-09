using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  public KoreaStockChart() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    OrderView.PriceSpinner.Spinned += (sender, args) => {
      if (sender is not NumericUpDown castedSender) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) castedSender.Value = StockMarketInformation.KRXStock.GetTickIncrement(castedSender.Value.Value - castedSender.Increment);
      else castedSender.Value = StockMarketInformation.KRXStock.GetTickDecrement(castedSender.Value.Value + castedSender.Increment);
    };
    OrderView.StopLossSpinner.Spinned += (sender, args) => {
      if (sender is not NumericUpDown castedSender) return;
      if (!castedSender.Value.HasValue) {
        castedSender.Value = 0;
        return;
      }
      if (args.Direction == SpinDirection.Increase) castedSender.Value = StockMarketInformation.KRXStock.GetTickIncrement(castedSender.Value.Value - castedSender.Increment);
      else castedSender.Value = StockMarketInformation.KRXStock.GetTickDecrement(castedSender.Value.Value + castedSender.Increment);
    };
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