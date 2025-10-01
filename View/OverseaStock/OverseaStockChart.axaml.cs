using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View;

public partial class OverseaStockChart : UserControl {
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  private static readonly OrderMethod[] ALLOWED_ORDER_METHODS = [
    OrderMethod.Limit, OrderMethod.LimitOnClose, OrderMethod.LimitOnOpen,
    OrderMethod.MarketOnOpen, OrderMethod.MarketOnClose,
    OrderMethod.VolumeWeightedAveragePrice, OrderMethod.TimeWeightedAveragePrice
  ];
  private static string GetCountryExchangeCode(string input) => input switch {
      "미국" => "USA",
      "중국" => "CHN",
      "일본" => "TKS",
      "베트남" => "VNM",
      _ => throw new InvalidDataException($"{input} is not a valid country option.")
    };
  public OverseaStockChart() {
    InitializeComponent();
    if (OrderView.DataContext is ViewModel.Order orderContext) {
      orderContext.MethodsAllowed.Clear();
      foreach (var item in ALLOWED_ORDER_METHODS) orderContext.MethodsAllowed.Add(item);
      TickerExchangeComboBox.ItemsSource = new List<string>() { "미국", "중국", "일본", "베트남" };
    }
    if (CastedDataContext == null) return;
    CastedDataContext.PropertyChanged += OnDataContextChanged;
  }
  private void OnDataContextChanged(object? sender, PropertyChangedEventArgs args) {
    var orderContext = OrderView.DataContext as ViewModel.Order;
    if (orderContext?.Ticker != null) {
      orderContext.Ticker = CastedDataContext.Ticker;
      orderContext.Name = CastedDataContext.Name;
    }
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is not string selectedCountry) return;
    var exchangeCode = GetCountryExchangeCode(selectedCountry);
    var task_1 = CastedDataContext.RefreshAsync(exchangeCode + CastedDataContext.Ticker);
    var task_2 = CastedDataContext.StartRefreshRealTimeAsync(exchangeCode + CastedDataContext.Ticker);
    await task_1;
    await task_2;
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is not string selectedCountry) return;
    var exchangeCode = GetCountryExchangeCode(selectedCountry);
    await CastedDataContext.EndRefreshRealTimeAsync(exchangeCode + CastedDataContext.Ticker);
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    if (TickerExchangeComboBox.SelectedItem is not string selectedCountry) return;
    string ticker = TickerTextBox.Text ?? "";
    var exchangeCode = GetCountryExchangeCode(selectedCountry);
    await CastedDataContext.EndRefreshRealTimeAsync(CastedDataContext.Ticker);
    var task_1 = CastedDataContext.RefreshAsync(exchangeCode + ticker);
    var task_2 = CastedDataContext.StartRefreshRealTimeAsync(exchangeCode + ticker);
    await task_1;
    await task_2;
  }
}