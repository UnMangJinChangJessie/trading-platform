using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Avalonia.Data.Converters;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.KoreaInvestment;

namespace trading_platform.ViewModel;

public partial class Order : ObservableObject {
  [ObservableProperty] public partial string AccountBase { get; set; } = "";
  [ObservableProperty] public partial string AccountCode { get; set; } = "";
  [ObservableProperty] public partial IDictionary<string, OrderMethod> MethodsAllowed { get; set; } = new Dictionary<string, OrderMethod>();
  [ObservableProperty] public partial string? SelectedMethod { get; set; }
  [ObservableProperty] public partial string Ticker { get; set; } = "";
  [ObservableProperty] public partial decimal UnitPrice { get; set; } = 0.0M;
  [ObservableProperty] public partial decimal Quantity { get; set; } = 0.0M;
  [ObservableProperty] public partial decimal StopLossPrice { get; set; } = 0.0M;
}