using System.Numerics;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model;

public class OHLC<T> where T : INumber<T> {
  public DateTime DateTime { get; set; }
  public TimeSpan TimeSpan { get; set; }
  public T Open { get; set; } = default!;
  public T High { get; set; } = default!;
  public T Low { get; set; } = default!;
  public T Close { get; set; } = default!;

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