using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.ViewModel;

public partial class QuickOrderItem(decimal price, decimal ask, decimal bid) : ObservableObject {
  [ObservableProperty]
  public partial decimal Price { get; set; } = price;
  [ObservableProperty]
  public partial decimal AskQuantity { get; set; } = ask;
  [ObservableProperty]
  public partial decimal BidQuantity { get; set; } = bid;
  [ObservableProperty]
  public partial decimal ShortQuantity { get; set; } = 0;
  [ObservableProperty]
  public partial decimal LongQuantity { get; set; } = 0;
}

public abstract partial class QuickOrder : ObservableObject, IRefresh {
  public readonly static Dictionary<string, object> NullArguments = [];
  public ObservableCollection<decimal> QuantityUnits { get; set; }
  [ObservableProperty]
  public partial string Ticker { get; set; } = "";
  [ObservableProperty]
  public partial decimal Quantity { get; set; }
  [ObservableProperty]
  public partial decimal CurrentClose { get; set; }
  [ObservableProperty]
  public partial decimal PreviousClose { get; set; }
  public ObservableCollection<QuickOrderItem> CurrentOrders { get; set; } = [];
  [ObservableProperty]
  public partial decimal HighestQuantity { get; protected set; } = 0;
  public bool RealTimeRefresh { get; protected set; } = false;
  public Func<decimal, decimal> NextTickGenerator { get; protected set; }
  public Func<decimal, decimal> PreviousTickGenerator { get; protected set; } 

  public QuickOrder() {
    QuantityUnits = [1, 2, 3, 4, 5, 10, 20, 50, 100];
    NextTickGenerator = val => val + 0.05M;
    PreviousTickGenerator = val => val - 0.05M;
    if (Design.IsDesignMode || Debugger.IsAttached) {
      for (int i = 0; i < 50; i++) {
        var price = 450.00M + (25 - i) * 0.05M;
        var askQuantity = i < 25 ? Random.Shared.Next(1, 50) : 0;
        var bidQuantity = i < 25 ? 0 : Random.Shared.Next(1, 50);
        InsertOrder(price, askQuantity, bidQuantity);
      }
      PreviousClose = 450.00M;
      HighestQuantity = CurrentOrders.Max(x => Math.Max(x.AskQuantity, x.BidQuantity));
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
  public abstract Task LongAsync(IDictionary<string, object> args);
  public abstract Task ShortAsync(IDictionary<string, object> args);
  // 주문을 다른 가격의 (주로 지정가) 주문으로 묶어 재주문하는 함수
  // 숏 주문을 롱으로, 반대로 롱 주문을 숏 주문으로 전환하는 것도 가능하도록 해야 한다.
  public abstract Task MoveAsync(IDictionary<string, object> args);
  // 주문 취소 함수
  public abstract Task CancelAsync(IDictionary<string, object> args);
}