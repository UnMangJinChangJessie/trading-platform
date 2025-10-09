using ScottPlot;

namespace trading_platform.Model.Charts.Indicators;

public struct BarStyle {
  public LineStyle PositiveBarIncreasingLine { get; set; }
  public LineStyle PositiveBarDecreasingLine { get; set; }
  public LineStyle NegativeBarIncreasingLine { get; set; }
  public LineStyle NegativeBarDecreasingLine { get; set; }
  public FillStyle PositiveBarIncreasingFill { get; set; }
  public FillStyle PositiveBarDecreasingFill { get; set; }
  public FillStyle NegativeBarIncreasingFill { get; set; }
  public FillStyle NegativeBarDecreasingFill { get; set; }

  public BarStyle() {
    PositiveBarIncreasingLine = new() { Color = Colors.LightPink, Width = 2 };
    PositiveBarDecreasingLine = new() { Color = Colors.LightPink, Width = 2 };
    NegativeBarIncreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 };
    NegativeBarDecreasingLine = new() { Color = Colors.LightSkyBlue, Width = 2 };
    PositiveBarIncreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.3) };
    PositiveBarDecreasingFill = new() { Color = Colors.LightPink.WithAlpha(0.7) };
    NegativeBarIncreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.3)};
    NegativeBarDecreasingFill = new() { Color = Colors.LightSkyBlue.WithAlpha(0.7) };
  }
}