using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model;

public partial class OHLC<T> : ObservableObject
where T : INumber<T> {
  [ObservableProperty]
  public partial DateTime DateTime { get; set; }
  [ObservableProperty]
  public partial TimeSpan TimeSpan { get; set; }
  [ObservableProperty]
  public partial T Open { get; set; } = default!;
  [ObservableProperty]
  public partial T High { get; set; } = default!;
  [ObservableProperty]
  public partial T Low { get; set; } = default!;
  [ObservableProperty]
  public partial T Close { get; set; } = default!;

  public OHLC() {
    DateTime = DateTime.UnixEpoch;
    TimeSpan = TimeSpan.Zero;
    Open = default!;
    High = default!;
    Low = default!;
    Close = default!;
  }
  public OHLC(T open, T high, T low, T close) {
    DateTime = DateTime.UnixEpoch;
    TimeSpan = TimeSpan.Zero;
    (Open, High, Low, Close) = (open, high, low, close);
  }

  public ScottPlot.OHLC ToScottPlotOHLC() {
    return new(
      double.CreateChecked(Open),
      double.CreateChecked(High),
      double.CreateChecked(Low),
      double.CreateChecked(Close)
    ) {
      DateTime = DateTime, TimeSpan = TimeSpan
    };
  }
}