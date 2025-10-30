using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Skia;
using ScottPlot;
using trading_platform.Dialogs;
using trading_platform.Model.Charts;
using trading_platform.Model.Charts.Indicators;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  public static readonly ObservableCollection<IndicatorName> AvailableTechnicalIndicators = [
    IndicatorName.ExponentialMovingAverage,
    IndicatorName.MovingAverageConvergenceDivergence,
    IndicatorName.SimpleMovingAverage,
  ];
  private CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  private Plot? _volumePlot;
  private CandlestickChartPlot? _candlestickPlot;
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
    ConfigureCandleChart();
    ConfigureVolumeChart();
    ConfigureLayout();
    ConfigureBottomAxis();
  }
  private void ConfigureCandleChart() {
    if (CastedDataContext == null) return;
    _candlestickPlot = new CandlestickChartPlot(CastedDataContext);
    PriceChart.Multiplot.Reset(_candlestickPlot);
    _candlestickPlot.PlotControl = PriceChart;
    _candlestickPlot.RisingFillStyle.Color = Colors.LightPink;
    _candlestickPlot.RisingLineStyle.Color = Colors.LightPink;
    _candlestickPlot.FallingFillStyle.Color = Colors.LightBlue;
    _candlestickPlot.FallingLineStyle.Color = Colors.LightBlue;
    _candlestickPlot.DataBackground.Color = Colors.Transparent;
    _candlestickPlot.FigureBackground.Color = Colors.Transparent;
    int[] periods = [10, 20, 30, 60, 120, 200];
    Color[] colors = [Colors.Red, Colors.OrangeRed, Colors.Yellow, Colors.GreenYellow, Colors.Indigo, Colors.Violet]; 
    _candlestickPlot.Axes.ContinuouslyAutoscale = true;
    _candlestickPlot.Axes.ContinuousAutoscaleAction = _candlestickPlot.ContinuouslyAutoscaleAction;
  }
  private void ConfigureVolumeChart() {
    if (CastedDataContext == null) return;
    _volumePlot = PriceChart.Multiplot.AddPlot();
    _volumePlot.Grid.XAxis = _volumePlot.Axes.Bottom;
    _volumePlot.Grid.YAxis = _volumePlot.Axes.Right;
    _volumePlot.Axes.DefaultGrid = _volumePlot.Grid;
    var plottable = new Volume(CastedDataContext);
    _volumePlot.Add.Plottable(plottable);
    _volumePlot.Axes.ContinuouslyAutoscale = true;
    _volumePlot.Axes.ContinuousAutoscaleAction = plottable.ContinuouslyAutoscaleAction;
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
    var background = new Color((Background as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
    foreach (var plot in PriceChart.Multiplot.GetPlots()) {
      plot.Layout.Fixed(padding: new(10, 60, 0, 0));
      plot.Grid.MajorLineColor = new((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
      plot.Grid.MinorLineColor = new((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
      plot.Grid.MajorLineColor = Color.InterpolateRgb(plot.Grid.MajorLineColor, background, 0.7);
      plot.Grid.MinorLineColor = Color.InterpolateRgb(plot.Grid.MajorLineColor, background, 0.99);
    }
    PriceChart.Multiplot.GetPlots()[0].Layout.Fixed(padding: new(10, 60, 60, 0));
    PriceChart.Multiplot.GetPlots()[0].Axes.AutoScale();
  }
  private void ConfigureBottomAxis() {
    var plots = PriceChart.Multiplot.GetPlots();
    var lastPlot = plots[^1];
    lastPlot.Axes.DateTimeTicksBottom();
    foreach (var plot in plots.SkipLast(1)) {
      plot.Grid.XAxis = lastPlot.Axes.Bottom;
    }
    foreach (var plot in plots) {
      plot.Axes.Color(new Color((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black));
    }
    PriceChart.Multiplot.SharedAxes.ShareX(PriceChart.Multiplot.GetPlots());
    PriceChart.Multiplot.CollapseVertically();
    PriceChart.UserInputProcessor.LeftClickDragPan(enable: true, horizontal: true, vertical: false);
  }
  private void VolumeToggle_Checked(object? sender, RoutedEventArgs args) {
    
  }
  private void AmountToggle_Checked(object? sender, RoutedEventArgs args) {
    
  }
  private void ConfigureIndicators(object? sender, RoutedEventArgs args) {
    var dialog = new ChartIndicatorDialog() {
      DataContext = CastedDataContext,
      Title = "보조지표 설정",
    };
    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
      dialog.ShowDialog<int>(lifetime.MainWindow!);
    }
    if (CastedDataContext == null) return;
    PriceChart.Multiplot.Reset();
    ConfigureCandleChart();
    ConfigureVolumeChart();
    foreach (var indicator in CastedDataContext.Indicators) {
      if (indicator.IsPriceOverlay) {
        _candlestickPlot!.Add.Plottable(indicator);
      }
      else {
        var plot = PriceChart.Multiplot.AddPlot();
        plot.Add.Plottable(indicator);
      }
    }
    ConfigureLayout();
    ConfigureBottomAxis();
  }
  private void PrintHello(object? sender, RoutedEventArgs args) {
    Debug.WriteLine("Meow :3");
  }
}