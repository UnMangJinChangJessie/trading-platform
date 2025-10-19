namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Diagnostics.CodeAnalysis;
using MarketBase = ViewModel.Market;

public partial class Market([MaybeNull] KisClients api) : MarketBase(new MarketItem(api)) {
  public KisClients Api { get; set; } = api;
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