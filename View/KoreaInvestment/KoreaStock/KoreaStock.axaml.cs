using Avalonia.Controls;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View.KoreaInvestment;

public partial class KoreaStock : UserControl {
  private ViewModel.KoreaInvestment.StockMarketData? CastedDataContext => DataContext as ViewModel.KoreaInvestment.StockMarketData;
  public KoreaStock() {
    InitializeComponent();
  }
}