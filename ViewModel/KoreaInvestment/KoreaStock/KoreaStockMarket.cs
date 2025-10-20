namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Diagnostics.CodeAnalysis;
using MarketBase = Market;

public partial class KoreaStockMarket([MaybeNull] KisClients api) : MarketBase(new MarketItem(api)) {
  public KisClients Api {
    get => field;
    set {
      if (field != value) {
        lock (InspectingItems) {
          foreach (var item in InspectingItems) {
            if (item == null) continue;
            lock (item) {
              if (item is MarketItem marketItem) marketItem.Api = value;
              if (Balance is Balance balance) balance.Api = value;
            }
          }
        }
        field = value;
      }
    }
  } = api;
  public override void Refresh() {
    lock (InspectingItems) {
      foreach (var item in InspectingItems) {
        if (item == null) continue;
        lock (item) {
          item.Refresh();
        }
      }
    }
  }
  public override Task RefreshAsync() {
    Refresh();
    return Task.CompletedTask;
  }
}