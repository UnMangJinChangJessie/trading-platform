using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View.KoreaInvestment;

public partial class OverseaStockProfitLoss : UserControl {
  public ViewModel.KoreaInvestment.OverseaStockProfitLoss? CastedDataContext => DataContext as ViewModel.KoreaInvestment.OverseaStockProfitLoss;
  private readonly List<(string Label, Exchange Exchange)> Exchanges = [
    ("미국(나스닥/NYSE/NYSE America)", Exchange.UnitedStates),
    ("홍콩", Exchange.HongKong),
    ("선전", Exchange.Shenzhen),
    ("상하이", Exchange.Shanghai),
    ("일본", Exchange.Tokyo),
    ("하노이", Exchange.Hanoi),
    ("호치민", Exchange.HoChiMinh),
  ];
  public OverseaStockProfitLoss() {
    InitializeComponent();
    ExchangeComboBox.ItemsSource = Exchanges.Select(x => x.Label);
  }
  public void ExchangeComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs args) {
    if (CastedDataContext == null) return;
    if (args.AddedItems.Count == 0 || args.AddedItems[0] is not string label) return;
    CastedDataContext.Exchange = Exchanges.Where(x => x.Label == label).SingleOrDefault().Exchange;
  }
  public async void AccountButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.RefreshAsync(ViewModel.MarketData.NullArguments);
  }
}