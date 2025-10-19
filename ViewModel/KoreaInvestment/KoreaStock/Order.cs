namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using OrderBase = ViewModel.Order;
using PendingOrderBase = PendingOrder;

public partial class Order : OrderBase {
  public partial class PendingOrder : PendingOrderBase {
    [ObservableProperty]
    public partial string BranchId { get; set; }
    [ObservableProperty]
    public partial string CurrentOrderId { get; set; }
    [ObservableProperty]
    public partial string InitialOrderId { get; set; }
  }
  private OrderForm CastedForm => (OrderForm)Form;
  public KisClients Api { get; set; }
  public Order(KisClients api, Account account) {
    PendingOrders = [];
    Api = api;
    Form = new OrderForm(api, account);
  }
  public void OnReceivedModifiable(string jsonString, bool hasNextData, object? args) {
    var result = ApiModel.DeserializeJson<GetModifiableResult>(jsonString);
    if (result == null) return;
    lock (PendingOrders) {
      foreach (var order in result.ModifiableList!) {
        PendingOrders.Add(new PendingOrder() {
          BranchId = order.BranchId,
          ConcludedAmount = order.ConcludedQuantity,
          ConcludedQuantity = order.ConcludedQuantity,
          CurrentOrderId = order.OrderId,
          InitialOrderId = order.InitialOrderId,
          ModifiableQuantity = order.ModifiableQuantity,
          OrderedQuantity = order.OrderedQuantity,
          UnitPrice = order.ConcludedQuantity,
        });
      }
    }
    if (hasNextData) {
      GetModifiableOrder(
        Api!.ApiClient,
        new GetModifiableQueries() {
          AccountBase = CastedForm.Account.AccountBase,
          AccountCode = CastedForm.Account.AccountCode,
          SellOrBuy = GetModifiableQueries.ALL,
          OrderOrTicker = GetModifiableQueries.ORDER,
          FirstConsecutiveContext = result.FirstConsecutiveContext!,
          SecondConsecutiveContext = result.SecondConsecutiveContext!,
        }, OnReceivedModifiable, null
      );
    }
  }
  // 현재 정정 가능한 주문 목록을 불러옵니다.
  public override void Refresh() {
    if (Api == null) return;
    lock (PendingOrders) {
      PendingOrders.Clear();
    }
    GetModifiableOrder(
      Api.ApiClient,
      new GetModifiableQueries() {
        AccountBase = CastedForm.Account.AccountBase,
        AccountCode = CastedForm.Account.AccountCode,
        SellOrBuy = GetModifiableQueries.ALL,
        OrderOrTicker = GetModifiableQueries.ORDER,
        FirstConsecutiveContext = "",
        SecondConsecutiveContext = "",
      },
      OnReceivedModifiable,
      null
    );
  }
  public override Task RefreshAsync() {
    Refresh();
    return Task.CompletedTask;
  }
}