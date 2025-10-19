using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform.View.KoreaInvestment.KoreaStock;

public partial class BalanceView : UserControl {
  public ViewModel.KoreaInvestment.KoreaStock.Balance? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KoreaStock.Balance;
  public BalanceView() {
    InitializeComponent();
  }
  public async void AccountButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.RefreshAsync();
  }
}