using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;
using trading_platform.ViewModel;

namespace trading_platform.View.KoreaInvestment.KoreaStock;

public partial class MainView : UserControl {
  private ViewModel.KoreaInvestment.KoreaStock.KoreaStockMarket? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KoreaStock.KoreaStockMarket;
  public MainView() {
    InitializeComponent();
    DataContextChanged += (sender, args) => {
      if (CastedDataContext == null) return;
      KoreaStockQuickOrderView.SetValue(DataContextProperty, new QuickOrderViewModel(
        CastedDataContext.MainItem,
        CastedDataContext.MainItem.ItemOrderBook,
        CastedDataContext.Order.Form
      ));
    };
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) { }
}