using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts.Drawings;

public partial class TrendLine : Drawing {
  [DrawingParameter(ParameterName = "시점")]
  [ObservableProperty]
  public partial Coordinates StartCoordinate { get; set; }
  [DrawingParameter(ParameterName = "종점")]
  [ObservableProperty]
  public partial Coordinates EndCoordinate { get; set; }
  [DrawingParameter(ParameterName = "우측 연장")]
  [ObservableProperty]
  public partial bool ExtendRight { get; set; } = true;
  [DrawingParameter(ParameterName = "좌측 연장")]
  [ObservableProperty]
  public partial bool ExtendLeft { get; set; } = false;
  [DrawingParameter(ParameterName = "추세선 모양새")]
  [ObservableProperty]
  public partial LineStyle TrendLineStyle { get; set; }
  public override int CoordinateCount => 2;

  public TrendLine() {
    StartCoordinate = new();
    EndCoordinate = new();
    TrendLineStyle = new() {
      Color = Colors.White,
      Width = 1.5F,
    };
  }
  public override void SetCoordinates(IEnumerable<Coordinates> coordinates) {
    IEnumerator<Coordinates> iterator = coordinates.GetEnumerator();
    if (!iterator.MoveNext()) return;
    StartCoordinate = iterator.Current;
    if (!iterator.MoveNext()) return;
    EndCoordinate = iterator.Current;
  }
  public override void Render(RenderPack rp) {
    // get the pixel positions
    Pixel start = rp.Plot.GetPixel(StartCoordinate);
    Pixel end = rp.Plot.GetPixel(EndCoordinate);
    if (Math.Abs(end.X - start.X) < 1e-10) return;
    AxisLimits axisLimits = rp.Plot.Axes.GetLimits();
    if (ExtendLeft && axisLimits.Right < start.X) {
      float alpha = (float)((end.X - axisLimits.Left) / (end.X - start.X));
      start += (end - start) * alpha;
    }
    var copy = rp.Paint.Clone();
    ScottPlot.Drawing.DrawLine(rp.Canvas, rp.Paint, start, end, TrendLineStyle);
  }
}