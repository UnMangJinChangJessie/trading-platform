using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScottPlot;
using trading_platform.Model.Charts;
using trading_platform.Model.Charts.Indicators;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  private Model.Charts.CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  private int? DraggingDividerIndex;
  public CandlestickChart() {
    InitializeComponent();
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.Menu?.Add("Show/Hide Grid", plot => {
      plot.Grid.XAxisStyle.IsVisible = !plot.Grid.XAxisStyle.IsVisible;
      plot.Grid.YAxisStyle.IsVisible = !plot.Grid.YAxisStyle.IsVisible;
    });
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    PriceChart.Multiplot.AddPlots(3);
    PriceChart.Multiplot.CollapseVertically();
    ConfigureCandleChart();
    ConfigureVolumeChart();
    var macd = new MovingAverageConvergenceDivergence(CastedDataContext, 12, 26);
    PriceChart.Multiplot.GetPlot(2).Add.Plottable(macd);
    PriceChart.Multiplot.GetPlot(2).Axes.ContinuouslyAutoscale = true;
    PriceChart.Multiplot.GetPlot(2).Axes.ContinuousAutoscaleAction = macd.ContinuouslyAutoscaleAction;
    PriceChart.Multiplot.GetPlot(2).Grid.YAxis = PriceChart.Multiplot.GetPlot(2).Axes.Right;
    ConfigureLayout();
    ConfigureBottomAxis();
    PriceChart.Multiplot.SharedAxes.ShareX(PriceChart.Multiplot.GetPlots());
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  public void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  private void ConfigureCandleChart() {
    if (CastedDataContext == null) return;
    var plot = PriceChart.Multiplot.GetPlot(0);
    var candles = new CandlestickPlot(CastedDataContext);
    plot.Add.Plottable(candles);

    plot.DataBackground.Color = Colors.Transparent;
    plot.FigureBackground.Color = Colors.Transparent;
    plot.Grid.YAxis = plot.Axes.Right;
    plot.Axes.ContinuouslyAutoscale = true;
    plot.Axes.ContinuousAutoscaleAction = candles.ContinuouslyAutoscaleAction;

    candles.RisingColor = Colors.LightPink;
    candles.FallingColor = Colors.LightBlue;
    candles.Axes.XAxis = plot.Axes.Bottom;
    candles.Axes.YAxis = plot.Axes.Right;

    int[] periods = [10, 20, 30, 60, 120, 200];
    foreach (var period in periods) {
      var sma = new SimpleMovingAverage(CastedDataContext, period);
      plot.Add.Plottable(sma);
    }
  }
  private void ConfigureVolumeChart() {
    if (CastedDataContext == null) return;
    var plot = PriceChart.Multiplot.GetPlot(1);
    var volume = new Volume(CastedDataContext);
    plot.Add.Plottable(volume);
    plot.Grid.YAxis = plot.Axes.Right;
    plot.Axes.ContinuouslyAutoscale = true;
    plot.Axes.ContinuousAutoscaleAction = volume.ContinuouslyAutoscaleAction;
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
      plot.Layout.Fixed(padding: new(5, 80, 0, 0));
    }
    PriceChart.Multiplot.GetPlots()[^1].Layout.Fixed(padding: new(5, 80, 60, 0));
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