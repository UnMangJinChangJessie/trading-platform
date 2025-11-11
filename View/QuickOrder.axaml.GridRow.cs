using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace trading_platform.View;

public partial class QuickOrderView {
  private void AddOrderBookGridRow(Grid grid, int rowIdx) {
    Button offerOrder = new();
    TextBlock offerRemain = new();
    TextBlock price = new();
    TextBlock bidRemain = new();
    Button bidOrder = new();
    offerOrder.SetValue(DataContextProperty, false); // 마우스로 끌고 있는지 여부(주문 취소 기능)
    offerOrder.Classes.AddRange(["Flat","Accent"]);
    bidOrder.SetValue(DataContextProperty, false);
    bidOrder.Classes.AddRange(["Flat","Accent"]);
    offerRemain.SetValue(TextBlock.TextProperty, "0");
    offerRemain.SetValue(DataContextProperty, 0M);
    offerRemain.SetValue(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);
    bidOrder.SetValue(TextBlock.TextProperty, "0");
    bidRemain.SetValue(TextBlock.TextProperty, "0");
    bidRemain.SetValue(DataContextProperty, 0M);
    bidRemain.SetValue(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);
    price.SetValue(TextBlock.TextProperty, "0");
    price.SetValue(DataContextProperty, 0M);
    price.SetValue(HorizontalAlignmentProperty, Avalonia.Layout.HorizontalAlignment.Center);
    Border priceBorder = new();
    EventHandler<PointerEventArgs> enterEvent = (sender, args) => {
      priceBorder.SetValue(IsVisibleProperty, true);
    }; 
    EventHandler<PointerEventArgs> exitEvent = (sender, args) => {
      priceBorder.SetValue(IsVisibleProperty, false);
    }; 
    priceBorder.Padding = new(2);
    priceBorder.BorderThickness = new Avalonia.Thickness(1);
    priceBorder.BorderBrush = Foreground;
    priceBorder.IsVisible = false;
    offerOrder.PointerEntered += enterEvent;
    offerOrder.PointerExited += exitEvent;
    bidOrder.PointerEntered += enterEvent;
    bidOrder.PointerExited += exitEvent;
    Grid.SetRow(offerOrder, rowIdx);
    Grid.SetRow(offerRemain, rowIdx);
    Grid.SetRow(price, rowIdx);
    Grid.SetRow(priceBorder, rowIdx);
    Grid.SetRow(bidRemain, rowIdx);
    Grid.SetRow(bidOrder, rowIdx);
    Grid.SetColumn(offerOrder, 0);
    Grid.SetColumn(offerRemain, 1);
    Grid.SetColumn(price, 2);
    Grid.SetColumn(priceBorder, 2);
    Grid.SetColumn(bidRemain, 3);
    Grid.SetColumn(bidOrder, 4);
    grid.Children.AddRange([offerOrder, offerRemain, price, priceBorder, bidRemain, bidOrder]);
  }
  private TextBlock? GetPriceGridCell(int idx) {
    return OrderBookGrid.Children.OfType<Control>()
      .Where(x => Grid.GetRow(x) == idx + 1 && Grid.GetColumn(x) == 2)
      .OfType<TextBlock>()
      .SingleOrDefault();
  }
  private void SetPriceGridCell(int idx, decimal price) {
    if (GetPriceGridCell(idx) is not TextBlock cell) return;
    if (CastedDataContext == null) return;
    cell.Text = price.ToString(new NumberFormatInfo() {
      NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits,
    });
    cell.DataContext = price;
  }
  private void IncrementPriceGridCell(int idx) {
    if (GetPriceGridCell(idx) is not TextBlock cell) return;
    if (CastedDataContext == null) return;
    if (cell.DataContext is not decimal price) return;
    price = CastedDataContext.CurrentOrderForm.GetNextPriceTick(price);
    cell.Text = price.ToString(new NumberFormatInfo() {
      NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits,
    });
    cell.DataContext = price;
  }
  private void DecrementPriceGridCell(int idx) {
    if (GetPriceGridCell(idx) is not TextBlock cell) return;
    if (CastedDataContext == null) return;
    if (cell.DataContext is not decimal price) return;
    price = CastedDataContext.CurrentOrderForm.GetPreviousPriceTick(price);
    cell.Text = price.ToString(new NumberFormatInfo() {
      NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits,
    });
    cell.DataContext = price;
  }
  private TextBlock? GetQuantityGridCell(int idx, bool isOffer) {
    int colIdx = isOffer ? 1 : 3;
    return OrderBookGrid.Children.OfType<Control>()
      .Where(x => Grid.GetRow(x) == idx + 1 && Grid.GetColumn(x) == colIdx)
      .SingleOrDefault() as TextBlock;
  }
  private void SetQuantityGridCell(int idx, bool isOffer, decimal quantity) {
    if (GetQuantityGridCell(idx, isOffer) is not TextBlock cell) return;
    if (CastedDataContext == null) return;
    cell.Text = quantity.ToString(new NumberFormatInfo() {
      NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits,
    });
    cell.DataContext = quantity;
  }
  private void RotateRowsDown() {
    foreach (var cell in OrderBookGrid.Children.OfType<Control>().Where(x => Grid.GetRow(x) > 0)) {
      Grid.SetRow(cell, Grid.GetRow(cell) % 20 + 1);
    }
  }
  private void RotateRowsUp() {
    foreach (var cell in OrderBookGrid.Children.OfType<Control>().Where(x => Grid.GetRow(x) != 0)) {
      Grid.SetRow(cell, (Grid.GetRow(cell) + 18) % 20 + 1);
    }
  }
}