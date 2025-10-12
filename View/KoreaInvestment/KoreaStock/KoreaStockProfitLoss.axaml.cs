using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform.View.KoreaInvestment;

public partial class KoreaStockProfitLoss : UserControl {
  public ViewModel.KoreaInvestment.DomesticStockProfitLoss? CastedDataContext => DataContext as ViewModel.KoreaInvestment.DomesticStockProfitLoss;
  public KoreaStockProfitLoss() {
    InitializeComponent();
  }
  public async void AccountButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.RefreshAsync(ViewModel.MarketData.NullArguments);
  }
}