using Avalonia.Controls;
using Avalonia.Interactivity;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class DomesticStockOrderView : UserControl {
  public string ProductName { get; set; } = "";
  private static Dictionary<string, OrderMethod> ORDER_METHOD_NAME_DICT = new() {
    ["지정가"] = OrderMethod.Limit,
    ["지정가(IOC)"] = OrderMethod.IocLimit,
    ["지정가(FOK)"] = OrderMethod.FokLimit,
    ["시장가"] = OrderMethod.Market,
    ["시장가(IOC)"] = OrderMethod.IocMarket,
    ["시장가(FOK)"] = OrderMethod.FokMarket,
    ["조건부지정가"] = OrderMethod.ConditionalLimit,
    ["최유리지정가"] = OrderMethod.BestOffer,
    ["최유리지정가(IOC)"] = OrderMethod.IocBestOffer,
    ["최유리지정가(FOK)"] = OrderMethod.FokBestOffer,
    ["최우선지정가"] = OrderMethod.TopPriority,
    ["장전전일종가매매"] = OrderMethod.PreMarket,
    ["장후종가매매"] = OrderMethod.PostMarket,
    ["시간외단일가매매"] = OrderMethod.AfterMarket,
    ["중간가"] = OrderMethod.Intermediate,
    ["중간가(IOC)"] = OrderMethod.FokIntermediate,
    ["중간가(FOK)"] = OrderMethod.FokIntermediate,
    ["손실제한주문"] = OrderMethod.StopLossLimit
  };
  public DomesticStockOrderView() {
    InitializeComponent();
    OrderView.ClickLong += Buy;
    OrderView.ClickShort += Sell;
  }
  public void OrderView_Loaded(object? sender, RoutedEventArgs args) {
    var context = OrderView.DataContext as ViewModel.Order;
    if (context == null) return;
    context.MethodsAllowed = ORDER_METHOD_NAME_DICT;
    context.SelectedMethod = ORDER_METHOD_NAME_DICT.First().Key;
  }
  public async void Buy(object? sender, RoutedEventArgs args) {
    if (OrderView.DataContext is not ViewModel.Order context) return;
    var method = ORDER_METHOD_NAME_DICT[context.SelectedMethod!];
    var (status, result) = await DomesticStock.OrderCash(new() {
      AccountBase = context.AccountBase,
      AccountCode = context.AccountCode,
      Position = OrderPosition.Buy,
      Ticker = context.Ticker,
      OrderDivision = method,
      UnitPrice = context.UnitPrice,
      Quantity = (ulong)context.Quantity,
      StopLossLimit = method == OrderMethod.StopLossLimit ? context.StopLossPrice : null
    });
    if (result == null) {
      MessageTextBox.Text = $"매수 주문 전송 실패: \nHTTPS status code {(int)status}({status}).";
      return;
    }
    MessageTextBox.Text = $"[{result.ResponseMessageCode}]\n{result.ResponseMessage}";
  }
  public async void Sell(object? sender, RoutedEventArgs args) {
    if (OrderView.DataContext is not ViewModel.Order context) return;
    var method = ORDER_METHOD_NAME_DICT[context.SelectedMethod!];
    var (status, result) = await DomesticStock.OrderCash(new() {
      AccountBase = context.AccountBase,
      AccountCode = context.AccountCode,
      Position = OrderPosition.Sell,
      SellType = OrderSelling.Ordinary,
      Ticker = context.Ticker,
      OrderDivision = method,
      UnitPrice = context.UnitPrice,
      Quantity = (ulong)context.Quantity,
      StopLossLimit = method == OrderMethod.StopLossLimit ? context.StopLossPrice : null
    });
    if (result == null) {
      MessageTextBox.Text = $"매수 주문 전송 실패: \nHTTPS status code {(int)status}({status}).";
      return;
    }
    MessageTextBox.Text = $"[{result.ResponseMessageCode}]\n{result.ResponseMessage}";
  }
}