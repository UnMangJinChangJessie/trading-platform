namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using OrderFormBase = ViewModel.OrderForm;

public partial class OrderFormViewModel([MaybeNull] KisClients api, MarketItemLabel label) : OrderFormBase([
  Model.KoreaInvestment.OrderMethod.Limit,
  Model.KoreaInvestment.OrderMethod.IocLimit,
  Model.KoreaInvestment.OrderMethod.FokLimit,
  Model.KoreaInvestment.OrderMethod.Market,
  Model.KoreaInvestment.OrderMethod.IocMarket,
  Model.KoreaInvestment.OrderMethod.FokMarket,
  Model.KoreaInvestment.OrderMethod.BestOffer,
  Model.KoreaInvestment.OrderMethod.IocBestOffer,
  Model.KoreaInvestment.OrderMethod.FokBestOffer,
  Model.KoreaInvestment.OrderMethod.ConditionalLimit,
  Model.KoreaInvestment.OrderMethod.StopLossLimit,
], label) {
  [ObservableProperty]
  public partial KisClients Api { get; set; } = api;
  public event EventHandler<OrderInformation> SucceedLong = default!;
  public event EventHandler<OrderInformation> SucceedShort = default!;
  private StockMarketInformation.KRXSecuritiesType SecuritiesType { get; set; }
  protected override void OnPropertyChanged(PropertyChangedEventArgs e) {
    if (e.PropertyName == nameof(OrderMethod) && OrderMethod != null) {
      BlockPriceInput = ((OrderMethod)OrderMethod).IsPriceMarket();
      if (BlockPriceInput) UnitPrice = 0;
      BlockStopLossPriceInput = ((OrderMethod)OrderMethod) != Model.KoreaInvestment.OrderMethod.StopLossLimit;
      if (BlockStopLossPriceInput) StopLossPrice = null;
    }
    base.OnPropertyChanged(e);
  }
  public void OnReceivedLong(string jsonString, bool hasNextData, object? args) {
    var result = ApiModel.DeserializeJson<CashOrderResult>(jsonString);
    if (result == null) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedLong)}] {result.ResponseMessage}");
      return;
    }
    SucceedLong?.Invoke(this, result.Response!);
  }
  public void OnReceivedShort(string jsonString, bool hasNextData, object? args) {
    var result = ApiModel.DeserializeJson<CashOrderResult>(jsonString);
    if (result == null) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedShort)}] {result.ResponseMessage}");
      return;
    }
    SucceedShort?.Invoke(this, result.Response!);
  }
  public override decimal GetNextPriceTick(decimal price) {
    return StockMarketInformation.KRXStock.GetTickIncrement(price, SecuritiesType);
  }
  public override decimal GetPreviousPriceTick(decimal price) {
    return StockMarketInformation.KRXStock.GetTickDecrement(price, SecuritiesType);
  }
  public override void Long() {
    if (Api == null) return;
    if (OrderMethod is not OrderMethod method) return;
    OrderCash(
      Api.ApiClient,
      new CashOrderBody() {
        AccountBase = Api.ApiClient.Account.AccountBase,
        AccountCode = Api.ApiClient.Account.AccountCode,
        Method = method,
        Position = OrderPosition.Long,
        Quantity = (ulong)decimal.Round(Quantity),
        StopLossLimit = StopLossPrice == null ? null : (ulong)decimal.Round(StopLossPrice.Value),
        Ticker = ItemLabel.Ticker,
        UnitPrice = (ulong)decimal.Round(UnitPrice),
      },
      OnReceivedLong,
      null
    );
  }
  public override void Short() {
    if (Api == null) return;
    if (OrderMethod is not OrderMethod method) return;
    OrderCash(
      Api.ApiClient,
      new CashOrderBody() {
        AccountBase = Api.ApiClient.Account.AccountBase,
        AccountCode = Api.ApiClient.Account.AccountCode,
        Method = method,
        Position = OrderPosition.Short,
        Quantity = (ulong)decimal.Round(Quantity),
        SellType = OrderSelling.Ordinary,
        StopLossLimit = StopLossPrice == null ? null : (ulong)decimal.Round(StopLossPrice.Value),
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