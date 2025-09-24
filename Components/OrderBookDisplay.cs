using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace trading_platform.Components;

public partial class OrderBookDisplay : UserControl {
  private readonly TextBlock[] SellingPriceTextBlocks = new TextBlock[10];
  private readonly TextBlock[] BuyingPriceTextBlocks = new TextBlock[10];
  private readonly OrderBookQuantityBlock[] SellingQuantityBlocks = new OrderBookQuantityBlock[10];
  private readonly OrderBookQuantityBlock[] BuyingQuantityBlocks = new OrderBookQuantityBlock[10];

  public OrderBookDisplay() {
    InitializeComponent();
    if (Design.IsDesignMode) SetupDesignTimeData(DataContext as ViewModel.OrderBook);
  }

  private static void SetupDesignTimeData(ViewModel.OrderBook context) {
    for (int i = 0; i < 10; i++) {
      (context.SellingPrices[i], context.SellingQuantities[i]) = (450.00M + 0.05M * i, Random.Shared.Next(1, 10));
      (context.BuyingPrices[i], context.BuyingQuantities[i]) = (450.00M - 0.05M * (i + 1), Random.Shared.Next(1, 10));
    }
  }
}