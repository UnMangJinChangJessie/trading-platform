using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScottPlot;
using trading_platform.Model.Charts;
using trading_platform.Model.Charts.Indicators;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  private CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  private int? DraggingDividerIndex;
  public CandlestickChart() {
    InitializeComponent();
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.Menu?.Add("Show/Hide Grid", plot => {
      plot.Grid.XAxisStyle.IsVisible = !plot.Grid.XAxisStyle.IsVisible;
      plot.Grid.YAxisStyle.IsVisible = !plot.Grid.YAxisStyle.IsVisible;
    });
    // PriceChart.UserInputProcessor.DoubleLeftClickBenchmark(false);
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    ConfigureCandleChart();
    ConfigureVolumeChart();
    ConfigureLayout();
    ConfigureBottomAxis();
    PriceChart.Multiplot.SharedAxes.ShareX(PriceChart.Multiplot.GetPlots());
    PriceChart.Multiplot.CollapseVertically();
    PriceChart.Refresh();
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  public void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  private void ConfigureCandleChart() {
    if (CastedDataContext == null) return;
    var meow = new CandlestickChartPlot(CastedDataContext);
    PriceChart.Multiplot.Reset(meow);
    meow.PlotControl = PriceChart;
    meow.MainPlot.RisingColor = Colors.LightPink;
    meow.MainPlot.FallingColor = Colors.LightBlue;
    meow.MainPlot.Axes.XAxis = meow.Axes.Bottom;
    meow.MainPlot.Axes.YAxis = meow.Axes.Right;
    meow.PriceHorizontalLine.Axes.XAxis = meow.Axes.Bottom;
    meow.PriceHorizontalLine.Axes.YAxis = meow.Axes.Right;
    meow.PriceHorizontalLine.LabelRotation = 0;
    meow.DataBackground.Color = Colors.Transparent;
    meow.FigureBackground.Color = Colors.Transparent;
    // lock 횟수를 줄여 성능을 개선하기 전까지 Y축 조정을 비활성화 함
    meow.Axes.ContinuouslyAutoscale = true;
    meow.Axes.ContinuousAutoscaleAction = meow.ContinuouslyAutoscaleAction;
    int[] periods = [10, 20, 30, 60, 120, 200];
    Color[] colors = [Colors.Red, Colors.OrangeRed, Colors.Yellow, Colors.GreenYellow, Colors.Indigo, Colors.Violet]; 
    foreach (var (period, color) in periods.Zip(colors)) {
      var sma = new SimpleMovingAverage(CastedDataContext, period);
      sma.LineStyle.Color = color;
      sma.Axes.XAxis = meow.Axes.Bottom;
      sma.Axes.YAxis = meow.Axes.Right;
      meow.Add.Plottable(sma);
    }
  }
  private void ConfigureVolumeChart() {
    if (CastedDataContext == null) return;
    var plot = PriceChart.Multiplot.AddPlot();
    var volume = new Volume(CastedDataContext);
    plot.Add.Plottable(volume);
    plot.Grid.YAxis = plot.Axes.Right;
    // plot.Axes.ContinuouslyAutoscale = true;
    // plot.Axes.ContinuousAutoscaleAction = volume.ContinuouslyAutoscaleAction;
    volume.Axes.XAxis = plot.Axes.Bottom;
    volume.Axes.YAxis = plot.Axes.Right;
  }
  private void ConfigureLayout() {
    var layout = new ScottPlot.MultiplotLayouts.DraggableRows() {
      ExpandingPlotIndex = 0,
      SnapDistance = 2,
      MinimumHeight = 50,
    };
    PriceChart.PointerPressed += (sender, args) => {
      var y = (float)args.GetPosition(PriceChart).Y;
      var divider = layout.GetDivider(y);
      DraggingDividerIndex = divider;
      PriceChart.UserInputProcessor.IsEnabled = divider is null;
    };
    PriceChart.PointerReleased += (sender, args) => {
      DraggingDividerIndex = null;
      PriceChart.UserInputProcessor.IsEnabled = true;
    };
    PriceChart.PointerMoved += (sender, args) => {
      var y = (float)args.GetPosition(PriceChart).Y;
      if (DraggingDividerIndex != null) {
        layout.SetDivider(DraggingDividerIndex.Value, y);
        PriceChart.Refresh();
      }
      else {
        var divider = layout.GetDivider(y);
        Cursor = new Avalonia.Input.Cursor(
          divider != null ? Avalonia.Input.StandardCursorType.SizeNorthSouth : Avalonia.Input.StandardCursorType.Arrow
        );
      }
    };
    PriceChart.Multiplot.Layout = layout;
    foreach (var plot in PriceChart.Multiplot.GetPlots()) {
      plot.Layout.Fixed(padding: new(10, 60, 0, 0));
    }
    PriceChart.Multiplot.GetPlots()[^1].Layout.Fixed(padding: new(10, 60, 60, 0));
  }
  private void ConfigureBottomAxis() {
    var plots = PriceChart.Multiplot.GetPlots();
    var lastPlot = plots[^1];
    lastPlot.Axes.DateTimeTicksBottom();
    foreach (var plot in plots.SkipLast(1)) {
      plot.Grid.XAxis = lastPlot.Axes.Bottom;
    }
    lastPlot.Axes.DateTimeTicksBottom();
  }
}