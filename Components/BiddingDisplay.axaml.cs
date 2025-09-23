using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;

namespace trading_platform.Components;

public partial class BiddingDisplay : UserControl {
  private readonly TextBlock[] SellingPriceTextBlocks = new TextBlock[10];
  private readonly TextBlock[] BuyingPriceTextBlocks = new TextBlock[10];
  private readonly TextBlock[] SellingQuantityTextBlocks = new TextBlock[10];
  private readonly TextBlock[] BuyingQuantityTextBlocks = new TextBlock[10];

  public BiddingDisplay() {
    InitializeComponent();
    if (DataContext is not ViewModel.Bidding context) {
      context = new ViewModel.Bidding();
      DataContext = context;
    }
    for (int i = 0; i < 10; i++) {
      SellingPriceTextBlocks[i] = new() {
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
      };
      SellingQuantityTextBlocks[i] = new() {
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
      };
      Grid.SetRow(SellingPriceTextBlocks[i], 9 - i);
      Grid.SetColumn(SellingPriceTextBlocks[i], 1);
      Grid.SetRow(SellingQuantityTextBlocks[i], 9 - i);
      Grid.SetColumn(SellingQuantityTextBlocks[i], 0);
      BiddingGrid.Children.Add(SellingPriceTextBlocks[i]);
      BiddingGrid.Children.Add(SellingQuantityTextBlocks[i]);
      BuyingPriceTextBlocks[i] = new() {
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
      };
      BuyingQuantityTextBlocks[i] = new() {
        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
      };
      Grid.SetRow(BuyingPriceTextBlocks[i], 10 + i);
      Grid.SetColumn(BuyingPriceTextBlocks[i], 1);
      Grid.SetRow(BuyingQuantityTextBlocks[i], 10 + i);
      Grid.SetColumn(BuyingQuantityTextBlocks[i], 2);
      BiddingGrid.Children.Add(BuyingPriceTextBlocks[i]);
      BiddingGrid.Children.Add(BuyingQuantityTextBlocks[i]);
      
      SellingPriceTextBlocks[i].Bind(TextBlock.TextProperty, new Binding($"Selling[{i}].Price"));
      SellingQuantityTextBlocks[i].Bind(TextBlock.TextProperty, new Binding($"Selling[{i}].Quantity"));
      BuyingPriceTextBlocks[i].Bind(TextBlock.TextProperty, new Binding($"Buying[{i}].Price"));
      BuyingQuantityTextBlocks[i].Bind(TextBlock.TextProperty, new Binding($"Buying[{i}].Quantity"));
    }
    if (Design.IsDesignMode) SetupDesignTimeData(context);
  }

  private static void SetupDesignTimeData(ViewModel.Bidding context) {
    for (int i = 0; i < 10; i++) {
      (context.Selling[i].Price, context.Selling[i].Quantity) = (70000 + 100 * i, 1000);
      (context.Buying[i].Price, context.Buying[i].Quantity) = (70000 - 100 * (i + 1), 1000);
    }
  }
}