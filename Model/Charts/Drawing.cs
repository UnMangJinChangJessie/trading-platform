using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class DrawingParameterAttribute : Attribute {
  public required string ParameterName;
}

public abstract class Drawing : ObservableObject, IPlottable, IHasLegendText {
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = new Axes();
  public virtual string LegendText { get; set; } = "";
  public virtual IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public virtual AxisLimits GetAxisLimits() => AxisLimits.Default;
  public virtual int CoordinateCount => 0;
  public abstract void SetCoordinates(IEnumerable<Coordinates> coordinates);
  public abstract void Render(RenderPack rp);
}