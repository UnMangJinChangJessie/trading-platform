namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using MarketBase = ViewModel.Market;

public partial class Market : MarketBase {
  public override void Refresh() {
    lock (InspectingItems) {
      foreach (var item in InspectingItems) {
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