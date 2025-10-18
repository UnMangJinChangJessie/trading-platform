namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.ComponentModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using OrderFormBase = ViewModel.OrderForm;

public partial class OrderForm(Account account) : OrderFormBase {
  [ObservableProperty]
  public partial Account Account { get; set; } = account;
  public event EventHandler<OrderInformation> SucceedLong;
  public event EventHandler<OrderInformation> SucceedShort;
  protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
    if (e.PropertyName == nameof(OrderMethod) && OrderMethod != null) {
      BlockPriceInput = ((OrderMethod)OrderMethod).IsPriceMarket();
    }
    base.OnPropertyChanged(e);
  }
  public void OnReceivedLong(string jsonString, bool hasNextData, object? args) {
    var result = ApiClient.DeserializeJson<CashOrderResult>(jsonString);
    if (result == null) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedLong)}] {result.ResponseMessage}");
      return;
    }
    SucceedLong?.Invoke(this, result.Response!);
  }
  public void OnReceivedShort(string jsonString, bool hasNextData, object? args) {
    var result = ApiClient.DeserializeJson<CashOrderResult>(jsonString);
    if (result == null) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedShort)}] {result.ResponseMessage}");
      return;
    }
    SucceedShort?.Invoke(this, result.Response!);
  }
  public override void Long() {
    if (OrderMethod is not OrderMethod method) return;
    OrderCash(
      new CashOrderBody() {
        AccountBase = Account.AccountBase,
        AccountCode = Account.AccountCode,
        Method = method,
        Position = OrderPosition.Long,
        Quantity = (ulong)decimal.Round(Quantity),
        StopLossLimit = (ulong)decimal.Round(StopLossPrice),
        Ticker = ItemLabel.Ticker,
        UnitPrice = (ulong)decimal.Round(UnitPrice),
      },
      OnReceivedLong,
      null
    );
  }
  public override void Short() {
    if (OrderMethod is not OrderMethod method) return;
    OrderCash(
      new CashOrderBody() {
        AccountBase = Account.AccountBase,
        AccountCode = Account.AccountCode,
        Method = method,
        Position = OrderPosition.Short,
        Quantity = (ulong)decimal.Round(Quantity),
        SellType = OrderSelling.Ordinary,
        StopLossLimit = (ulong)decimal.Round(StopLossPrice),
        Ticker = ItemLabel.Ticker,
        UnitPrice = (ulong)decimal.Round(UnitPrice),
      },
      OnReceivedShort,
      null
    );
  }
  public override Task LongAsync() {
    Long();
    return Task.CompletedTask;
  }
  public override Task ShortAsync() {
    Short();
    return Task.CompletedTask;
  }
}