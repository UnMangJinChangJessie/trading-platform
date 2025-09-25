using System.ComponentModel;
using Avalonia.Controls;
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
  [ObservableProperty]
  public partial decimal PreviousClose { get; set; } = 0.0M;
  [ObservableProperty]
  public partial decimal? LastConclusion { get; set; } = null;
  public double SellingVisualBarRatio(int idx) {
    decimal max = Math.Max(SellingQuantities.Max(), BuyingQuantities.Max());
    if (max == 0) return 0.0;
    else return (double)(SellingQuantities[idx] / max);
  }
  public double BuyingVisualBarRatio(int idx) {
    decimal max = Math.Max(SellingQuantities.Max(), BuyingQuantities.Max());
    if (max == 0) return 0.0;
    else return (double)(BuyingQuantities[idx] / max);
  }
  public OrderBook() {
    if (Design.IsDesignMode) SetupDesignTimeData();
    // if (true) SetupDesignTimeData();
  }
  private void SetupDesignTimeData() {
    for (int i = 0; i < 10; i++) {
      (SellingPrices[i], SellingQuantities[i]) = (450.00M + 0.05M * i, Random.Shared.Next(1, 10));
      (BuyingPrices[i], BuyingQuantities[i]) = (450.00M - 0.05M * (i + 1), Random.Shared.Next(1, 10));
      PreviousClose = 450.00M + 0.05M * Random.Shared.Next(-5, 5);
      LastConclusion = 450.00M + 0.05M * Random.Shared.Next(-1, 2);
    }
  }
}