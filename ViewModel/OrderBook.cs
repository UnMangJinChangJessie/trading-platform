using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.ViewModel;

public partial class OrderBookItem(decimal price, decimal ask, decimal bid) : ObservableObject {
  [ObservableProperty]
  public partial decimal Price { get; set; } = price;
  [ObservableProperty]
  public partial decimal AskQuantity { get; set; } = ask;
  [ObservableProperty]
  public partial decimal BidQuantity { get; set; } = bid;
}

public abstract partial class OrderBook : ObservableObject, IRefresh {
  public readonly static Dictionary<string, object> NullArguments = [];
  [ObservableProperty]
  public partial TimeOnly ConclusionTime { get; protected set; } = TimeOnly.MinValue;
  [ObservableProperty]
  public partial string Ticker { get; protected set; } = "";
  [ObservableProperty]
  public partial decimal CurrentClose { get; set; }
  [ObservableProperty]
  public partial decimal PreviousClose { get; set; }
  public ObservableCollection<OrderBookItem> CurrentOrders { get; set; } = [];
  [ObservableProperty]
  public partial decimal? IntermediatePrice { get; protected set; }
  [ObservableProperty]
  public partial decimal? IntermediateAskQuantity { get; protected set; }
  [ObservableProperty]
  public partial decimal? IntermediateBidQuantity { get; protected set; }
  [ObservableProperty]
  public partial decimal HighestQuantity { get; protected set; } = 0;
  public bool RealTimeRefresh { get; protected set; } = false;

  public OrderBook() {
    if (Design.IsDesignMode || Debugger.IsAttached) {
      for (int i = 0; i < 50; i++) {
        var price = 450.00M + (25 - i) * 0.05M;
        var askQuantity = i < 25 ? Random.Shared.Next(1, 50) : 0;
        var bidQuantity = i < 25 ? 0 : Random.Shared.Next(1, 50);
        InsertOrder(price, askQuantity, bidQuantity);
      }
      HighestQuantity = CurrentOrders.Max(x => Math.Max(x.AskQuantity, x.BidQuantity));
      PreviousClose = 450.00M;
      CurrentClose = 450.05M;
    }
  }
  protected void InsertOrder(decimal price, decimal ask, decimal bid) {
    int index = BinarySearch(price);
    if (index < 0) CurrentOrders.Insert(~index, new(price, ask, bid));
    else CurrentOrders[index] = new(price, ask, bid);
  }
  protected int BinarySearch(decimal price) {
    int lo = 0, hi = CurrentOrders.Count;
    while (lo != hi) {
      var mid = lo + (hi - lo) / 2;
      if (-CurrentOrders[mid].Price < -price) lo = mid + 1; // 내림차순으로 삽입하려고 한다.
      else hi = mid;
    }
    if (lo == CurrentOrders.Count || CurrentOrders[lo].Price != price) return ~lo;
    else return lo;
  }
  protected void ZeroOutOutOfRange(decimal min, decimal max) {
    int index;
    index = BinarySearch(max);
    index = (index < 0 ? ~index : index) - 1;
    for (int i = index; i >= 0; i--) CurrentOrders[i] = new(CurrentOrders[i].Price, 0, 0);
    index = BinarySearch(min);
    index = index < 0 ? ~index : (index + 1);
    for (int i = index; i < CurrentOrders.Count; i++) CurrentOrders[i] = new(CurrentOrders[i].Price, 0, 0);
  }

  public abstract Task RefreshAsync(IDictionary<string, object> args);
}