namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using MarketBase = Market;

public partial class KoreaStockMarket
: MarketBase {
  [ObservableProperty]
  public partial MarketItem MainItem { get; set; } 
  public override ObservableCollection<ViewModel.MarketItem?> InspectingItems { get; set; }
  public override ViewModel.Order Order { get; set; }
  public override ViewModel.Balance Balance { get; set; }
  public KisClients Api {
    get => field;
    set {
      if (field != value) {
        lock (InspectingItems) {
          foreach (var item in InspectingItems) {
            if (item == null) continue;
            lock (item) {
              if (item is MarketItem marketItem) marketItem.Api = value;
            }
          }
          if (Balance is Balance balance) balance.Api = value;
          if (Order is Order order) order.Api = value;
        }
        field = value;
      }
    }
  }
  public KoreaStockMarket([MaybeNull] KisClients api) {
    MainItem = new MarketItem(api);
    InspectingItems = [MainItem];
    Order = new Order(api, MainItem.ItemLabel);
    Balance = new Balance(api);
  }
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