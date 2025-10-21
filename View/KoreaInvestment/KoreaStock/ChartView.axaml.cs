using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model;
using trading_platform.ViewModel;

namespace trading_platform.View.KoreaInvestment.KoreaStock;

public partial class ChartView {
  /// <summary>
  /// OrderDataContext StyledProperty definition
  /// indicates the data context for balance/order forms.
  /// </summary>
  public static readonly StyledProperty<ViewModel.KoreaInvestment.KoreaStock.Order> OrderDataContextProperty =
      AvaloniaProperty.Register<ChartView, ViewModel.KoreaInvestment.KoreaStock.Order>(nameof(OrderDataContext));
  
  /// <summary>
  /// Gets or sets the OrderDataContext property. This StyledProperty
  /// indicates the data context for balance/order forms.
  /// </summary>
  public ViewModel.KoreaInvestment.KoreaStock.Order OrderDataContext
  {
     get => this.GetValue(OrderDataContextProperty);
     set => SetValue(OrderDataContextProperty, value);
  }
  
}

public partial class ChartView : UserControl {
  private ViewModel.KoreaInvestment.KoreaStock.MarketItem? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KoreaStock.MarketItem;
  public ChartView() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (PriceChart.DataContext is Model.Charts.CandlestickChartData data) {
      foreach (var item in new Model.Charts.CandlestickChartData.CandlePeriod[] {
        Model.Charts.CandlestickChartData.CandlePeriod.Daily,
        Model.Charts.CandlestickChartData.CandlePeriod.Weekly,
        Model.Charts.CandlestickChartData.CandlePeriod.Monthly,
        Model.Charts.CandlestickChartData.CandlePeriod.Yearly,
      }) {
        data.AvailableCandlePeriod.Add(item);
      }
    }
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