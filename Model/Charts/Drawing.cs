using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class DrawingParameterAttribute : Attribute {
  public required string ParameterName;
}

public abstract class Drawing : ObservableObject, IPlottable, IHasLegendText {
  public bool IsVisible { get; set; } = true;
  public bool IsCoordinatesVisible { get; set; } = false;
  public IAxes Axes { get; set; } = new Axes();
  public virtual string LegendText { get; set; } = "";
  public virtual IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public virtual AxisLimits GetAxisLimits() => AxisLimits.Default;
  public virtual int CoordinateCount => 0;
  public virtual IEnumerable<Coordinates> DrawingCoordinates => [];
  public abstract void SetCoordinates(IEnumerable<Coordinates> coordinates);
  public virtual void Render(RenderPack rp) {
    if (IsCoordinatesVisible) {
      var pixel = DrawingCoordinates.Select(p => rp.Plot.GetPixel(p));
      ScottPlot.Drawing.DrawMarkers(rp.Canvas, rp.Paint, pixel, MarkerStyle.Default);
    }
  }
}