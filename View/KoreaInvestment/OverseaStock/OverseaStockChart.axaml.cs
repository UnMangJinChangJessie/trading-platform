using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View.KoreaInvestment;

public partial class OverseaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  private static Exchange GetCountryExchangeCode(string input) => input switch {
      "미국" => Exchange.UnitedStates,
      "중국" => Exchange.China,
      "일본" => Exchange.Tokyo,
      "베트남" => Exchange.Vietnam,
      _ => throw new InvalidDataException($"{input} is not a valid country option.")
    };
  public OverseaStockChart() {
    InitializeComponent();
    TickerExchangeComboBox.ItemsSource = new List<string>() { "미국", "중국", "일본", "베트남" };
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is not string selectedCountry) return;
    var meow = new Dictionary<string, object>() {
      ["exchange"] = GetCountryExchangeCode(selectedCountry),
      ["ticker"] = TickerTextBox.Text ?? ""
    };
    await CastedDataContext.RefreshAsync(meow);
    await CastedDataContext.StartRefreshRealtimeAsync(meow);
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is null) return;
    await CastedDataContext.EndRefreshRealtimeAsync(ViewModel.OrderBook.NullArguments);
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is not string selectedCountry) return;
    var meow = new Dictionary<string, object>() {
      ["exchange"] = GetCountryExchangeCode(selectedCountry),
      ["ticker"] = TickerTextBox.Text ?? ""
    };
    await CastedDataContext.EndRefreshRealtimeAsync(ViewModel.OrderBook.NullArguments);
    await CastedDataContext.RefreshAsync(meow);
    await CastedDataContext.StartRefreshRealtimeAsync(meow);
  }
}