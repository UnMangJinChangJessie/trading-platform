
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class StockOrder : ViewModel.Order {
  [ObservableProperty]
  public partial string AccountBase { get; set; } = "";
  [ObservableProperty]
  public partial string AccountCode { get; set; } = "";
  public StockOrder() {
    MethodsList = new System.Collections.ArrayList() {
      Model.KoreaInvestment.OrderMethod.Limit,
      Model.KoreaInvestment.OrderMethod.IocLimit,
      Model.KoreaInvestment.OrderMethod.FokLimit,
      Model.KoreaInvestment.OrderMethod.Market,
      Model.KoreaInvestment.OrderMethod.IocLimit,
      Model.KoreaInvestment.OrderMethod.FokMarket,
      Model.KoreaInvestment.OrderMethod.BestOffer,
      Model.KoreaInvestment.OrderMethod.IocBestOffer,
      Model.KoreaInvestment.OrderMethod.FokBestOffer,
      Model.KoreaInvestment.OrderMethod.Intermediate,
      Model.KoreaInvestment.OrderMethod.IocIntermediate,
      Model.KoreaInvestment.OrderMethod.FokIntermediate,
      Model.KoreaInvestment.OrderMethod.ConditionalLimit,
      Model.KoreaInvestment.OrderMethod.TopPriority,
      Model.KoreaInvestment.OrderMethod.StopLossLimit
    };
  }
  public override async ValueTask<bool> Long() {
    if (SelectedMethod == null) return false;
    var (_, body) = await Model.KoreaInvestment.DomesticStock.OrderCash(new() {
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      Position = Model.KoreaInvestment.OrderPosition.Buy,
      Ticker = Ticker,
      UnitPrice = (ulong)UnitPrice,
      Quantity = (ulong)Quantity,
      OrderDivision = (Model.KoreaInvestment.OrderMethod)SelectedMethod,
      StopLossLimit = (ulong?)StopLossPrice
    });
    return body?.ReturnCode == 0;
  }
  public override async ValueTask<bool> Short() {
    if (SelectedMethod == null) return false;
    var (_, body) = await Model.KoreaInvestment.DomesticStock.OrderCash(new() {
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      SellType = Model.KoreaInvestment.OrderSelling.Ordinary,
      Position = Model.KoreaInvestment.OrderPosition.Buy,
      Ticker = Ticker,
      UnitPrice = (ulong)UnitPrice,
      Quantity = (ulong)Quantity,
      OrderDivision = (Model.KoreaInvestment.OrderMethod)SelectedMethod,
      StopLossLimit = (ulong?)StopLossPrice
    });
    return body?.ReturnCode == 0;
  }
}