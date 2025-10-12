using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform.View.KoreaInvestment;

public partial class KoreaStockQuickOrder : UserControl {
  private ViewModel.KoreaInvestment.DomesticStockQuickOrder? CastedDataContext => DataContext as ViewModel.KoreaInvestment.DomesticStockQuickOrder;
  public KoreaStockQuickOrder() {
    InitializeComponent();
  }
  public async void InquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext is null) return;
    await CastedDataContext.RefreshAsync(ViewModel.QuickOrder.NullArguments);
  }
}