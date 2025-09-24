using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel;

public partial class OrderBook : ObservableObject {
  [ObservableProperty]
  public partial decimal[] SellingPrices { get; set; } = new decimal[10];
  [ObservableProperty]
  public partial decimal[] BuyingPrices { get; set; } = new decimal[10];
  [ObservableProperty]
  public partial decimal[] SellingQuantities { get; set; } = new decimal[10];
  [ObservableProperty]
  public partial decimal[] BuyingQuantities { get; set; } = new decimal[10];
  [ObservableProperty]
  public partial decimal Volume { get; set; } = 0.0M;
  public double[] SellingVisualBarRatio { get; set; } = new double[10];
  public double[] BuyingVisualBarRatio { get; set; } = new double[10];
  void UpdateQuantityBarRatio(object? sender, PropertyChangedEventArgs args) {
    decimal max = Math.Max(SellingQuantities.Max(), BuyingQuantities.Max());
    if (max == 0) {
      Array.Fill(SellingVisualBarRatio, 0.0);
      Array.Fill(BuyingVisualBarRatio, 0.0);
    }
    else {
      for (int i = 0; i < 10; i++) {
        SellingVisualBarRatio[i] = (double)(SellingQuantities[i] / max);
        BuyingVisualBarRatio[i] = (double)(BuyingQuantities[i] / max);
      }
    }
  }
  public OrderBook() {
    PropertyChanged += UpdateQuantityBarRatio;
  }
}