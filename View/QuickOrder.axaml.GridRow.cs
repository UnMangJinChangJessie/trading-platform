using System.Globalization;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;

namespace trading_platform.View;

public partial class QuickOrderView {
  private void AddOrderBookGridRow(Grid grid, int rowIdx) {
    Button offerOrder = new();
    TextBlock offerRemain = new();
    TextBlock price = new();
    TextBlock bidRemain = new();
    Button bidOrder = new();
    offerOrder.SetValue(DataContextProperty, false); // 마우스로 끌고 있는지 여부(주문 취소 기능)
    offerOrder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch); // 마우스로 끌고 있는지 여부(주문 취소 기능)
    offerOrder.Classes.AddRange(["Flat","Accent"]);
    bidOrder.SetValue(DataContextProperty, false);
    bidOrder.SetValue(VerticalAlignmentProperty, VerticalAlignment.Stretch); // 마우스로 끌고 있는지 여부(주문 취소 기능)
    bidOrder.Classes.AddRange(["Flat","Accent"]);
    offerRemain.SetValue(TextBlock.TextProperty, "0");
    offerRemain.SetValue(DataContextProperty, 0M);
    offerRemain.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
    offerRemain.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
    bidOrder.SetValue(TextBlock.TextProperty, "0");
    bidRemain.SetValue(TextBlock.TextProperty, "0");
    bidRemain.SetValue(DataContextProperty, 0M);
    bidRemain.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
    bidRemain.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
    price.SetValue(TextBlock.TextProperty, "0");
    price.SetValue(DataContextProperty, 0M);
    price.SetValue(HorizontalAlignmentProperty, HorizontalAlignment.Center);
    price.SetValue(VerticalAlignmentProperty, VerticalAlignment.Center);
    Border priceBorder = new();
    EventHandler<PointerEventArgs> enterEvent = (sender, args) => {
      priceBorder.SetValue(IsVisibleProperty, true);
    }; 
    EventHandler<PointerEventArgs> exitEvent = (sender, args) => {
      priceBorder.SetValue(IsVisibleProperty, false);
    }; 
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
    _orderBookGridRows.Add([offerOrder, offerRemain, price, priceBorder, bidRemain, bidOrder]);
    grid.Children.AddRange(_orderBookGridRows[^1]);
  }
  private void RotatePriceDown() {
    if (CastedDataContext == null) return;
    for (int i = 0; i < _orderBookGridRows.Count; i++) {
      decimal price;
      if (i + 1 == _orderBookGridRows.Count) {
        price = (decimal)_orderBookGridRows[i][2].DataContext!;
        price = CastedDataContext.CurrentOrderForm.GetPreviousPriceTick(price);
      }
      else {
        price = (decimal)_orderBookGridRows[i + 1][2].DataContext!;
      }
      _orderBookGridRows[i][2].SetValue(DataContextProperty, price);
      ((TextBlock)_orderBookGridRows[i][2]).SetValue(TextBlock.TextProperty, price.ToString(new NumberFormatInfo() {
        NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits
      }));
    }
  }
  private void RotatePriceUp() {
    if (CastedDataContext == null) return;
    for (int i = 1; i <= _orderBookGridRows.Count; i++) {
      decimal price;
      if (i == _orderBookGridRows.Count) {
        price = (decimal)_orderBookGridRows[^i][2].DataContext!;
        price = CastedDataContext.CurrentOrderForm.GetNextPriceTick(price);
      }
      else {
        price = (decimal)_orderBookGridRows[^(i + 1)][2].DataContext!;
      }
      _orderBookGridRows[^i][2].SetValue(DataContextProperty, price);
      ((TextBlock)_orderBookGridRows[^i][2]).SetValue(TextBlock.TextProperty, price.ToString(new NumberFormatInfo() {
        NumberDecimalDigits = CastedDataContext.CurrentItem.PriceDecimalDigits
      }));
    }
  }
}