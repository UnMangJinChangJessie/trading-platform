using Avalonia.Controls;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.View.KoreaInvestment.KoreaStock;

public partial class MainView : UserControl {
  private ViewModel.KoreaInvestment.KoreaStock.Market? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KoreaStock.Market;
  public MainView() {
    InitializeComponent();
  }
}