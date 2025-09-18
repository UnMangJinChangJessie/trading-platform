using Avalonia.Controls;

namespace trading_platform.Components;

public partial class PriceDisplay : UserControl {
  public PriceDisplay() {
    InitializeComponent();
    var context = new ViewModel.PriceDisplay();
    DataContext = context;
    if (Design.IsDesignMode) {
      context.Price = 500.00M;
      context.Currency = "pt";
      context.TickerName = "코스피200";
    }
  }
}