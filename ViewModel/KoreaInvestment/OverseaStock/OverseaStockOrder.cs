using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockOrder : Order, IAccount {
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [ObservableProperty]
  public partial string AccountCode { get; set; }
  [ObservableProperty]
  public partial Model.KoreaInvestment.Exchange StockExchange { get; set; }
  public OverseaStockOrder() {
    StockExchange = Model.KoreaInvestment.Exchange.None;
    AccountBase = "";
    AccountCode = "";
    MethodsList = new List<Model.KoreaInvestment.OrderMethod> {
      Model.KoreaInvestment.OrderMethod.Limit,
      Model.KoreaInvestment.OrderMethod.LimitOnOpen,
      Model.KoreaInvestment.OrderMethod.LimitOnClose,
      Model.KoreaInvestment.OrderMethod.MarketOnOpen,
      Model.KoreaInvestment.OrderMethod.MarketOnClose,
      Model.KoreaInvestment.OrderMethod.VolumeWeightedAveragePrice,
      Model.KoreaInvestment.OrderMethod.TimeWeightedAveragePrice,
    };
  }
  public override async Task Long() {
    if (SelectedMethod == null) return;
    Model.KoreaInvestment.OverseaStock.Order(new() {
      Position = Model.KoreaInvestment.OrderPosition.Long,
      Exchange = StockExchange,
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      Ticker = Ticker,
      UnitPrice = UnitPrice,
      Quantity = (ulong)Quantity,
      Method = (Model.KoreaInvestment.OrderMethod)SelectedMethod
    }, null);
    await Task.CompletedTask;
  }
  public override async Task Short() {
    if (SelectedMethod == null) return;
    Model.KoreaInvestment.OverseaStock.Order(new() {
      Position = Model.KoreaInvestment.OrderPosition.Short,
      Exchange = StockExchange,
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      Ticker = Ticker,
      UnitPrice = UnitPrice,
      Quantity = (ulong)Quantity,
      Method = (Model.KoreaInvestment.OrderMethod)SelectedMethod
    }, null);
    await Task.CompletedTask;
  }
}