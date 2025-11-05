using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Skia;
using ScottPlot;
using trading_platform.Dialogs;
using trading_platform.Model.Charts;
using trading_platform.Model.Charts.Drawings;
using trading_platform.Model.Charts.Indicators;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  private CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  private CandlestickChartPlot? _candlestickPlot;
  private ScottPlot.MultiplotLayouts.DraggableRows _chartLayout;
  private int? _draggingDividerIndex;
  private bool _drawingMode;
  private Model.Charts.Drawing? _drawing;
  private List<Coordinates> _drawingCoordinates;
  public CandlestickChart() {
    InitializeComponent();
    _chartLayout = new();
    _drawingMode = false;
    _drawingCoordinates = [];
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.Menu?.Add("Toggle Grid", plot => {
      foreach (var mpPlot in PriceChart.Multiplot.GetPlots()) {
        mpPlot.Grid.XAxisStyle.IsVisible = !mpPlot.Grid.XAxisStyle.IsVisible;
        mpPlot.Grid.YAxisStyle.IsVisible = !mpPlot.Grid.YAxisStyle.IsVisible;
      }
    });
    PriceChart.Menu?.Add("Toggle Automatic Scaling", plot => {
      plot.Axes.ContinuouslyAutoscale = !plot.Axes.ContinuouslyAutoscale;
      PriceChart.UserInputProcessor.LeftClickDragPan(true, horizontal: true, vertical: !plot.Axes.ContinuouslyAutoscale);
    });
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    CastedDataContext.AddIndicator(new Volume(CastedDataContext));
    ConfigureCandleChart();
    ConfigureIndicatorPlots();
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
    Color[] colors = [Colors.Red, Colors.OrangeRed, Colors.Yellow, Colors.GreenYellow, Colors.Indigo, Colors.Violet];
    _candlestickPlot.Axes.ContinuouslyAutoscale = true;
    _candlestickPlot.Axes.ContinuousAutoscaleAction = _candlestickPlot.ContinuouslyAutoscaleAction;
    _candlestickPlot.ShowLegend(Alignment.UpperLeft, Orientation.Horizontal);
  }
  private void ConfigureIndicatorPlots() {
    if (CastedDataContext == null) return;
    foreach (var indicator in CastedDataContext.Indicators) {
      if (indicator.IsPriceOverlay) {
        _candlestickPlot!.Add.Plottable(indicator);
      }
      else {
        var plot = PriceChart.Multiplot.AddPlot();
        plot.Axes.Left.RemoveTickGenerator();
        plot.Grid.XAxis = plot.Axes.Bottom;
        plot.Grid.YAxis = plot.Axes.Right;
        indicator.Axes.XAxis = plot.Axes.Bottom;
        indicator.Axes.YAxis = plot.Axes.Right;
        plot.Add.Plottable(indicator);
        plot.Axes.ContinuouslyAutoscale = true;
        plot.Axes.ContinuousAutoscaleAction = indicator.ContinuouslyAutoscaleAction;
      }
    }
  }
  private void ConfigureLayout() {
    _chartLayout = new ScottPlot.MultiplotLayouts.DraggableRows() {
      ExpandingPlotIndex = 0,
      SnapDistance = 2,
      MinimumHeight = 50,
    };
    PriceChart.PointerPressed += (sender, args) => {
      var y = (float)args.GetPosition(PriceChart).Y;
      var divider = _chartLayout.GetDivider(y);
      _draggingDividerIndex = divider;
      PriceChart.UserInputProcessor.IsEnabled = divider is null;
    };
    PriceChart.PointerReleased += (sender, args) => {
      _draggingDividerIndex = null;
      PriceChart.UserInputProcessor.IsEnabled = true;
    };
    PriceChart.PointerMoved += (sender, args) => {
      var y = (float)args.GetPosition(PriceChart).Y;
      if (_draggingDividerIndex != null) {
        _chartLayout.SetDivider(_draggingDividerIndex.Value, y);
        PriceChart.Refresh();
      }
      else {
        var divider = _chartLayout.GetDivider(y);
        Cursor = new Avalonia.Input.Cursor(
          divider != null ? Avalonia.Input.StandardCursorType.SizeNorthSouth : Avalonia.Input.StandardCursorType.Arrow
        );
      }
    };
    PriceChart.Multiplot.Layout = _chartLayout;
    var background = new Color((Background as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
    foreach (var plot in PriceChart.Multiplot.GetPlots()) {
      plot.Layout.Fixed(padding: new(10, 60, 0, 0));
      plot.Grid.MajorLineColor = new((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
      plot.Grid.MinorLineColor = new((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black);
      plot.Grid.MajorLineColor = Color.InterpolateRgb(plot.Grid.MajorLineColor, background, 0.7);
      plot.Grid.MinorLineColor = Color.InterpolateRgb(plot.Grid.MajorLineColor, background, 0.99);
    }
    PriceChart.Multiplot.GetPlots()[^1].Layout.Fixed(padding: new(10, 60, 60, 0));
    PriceChart.Multiplot.GetPlots()[^1].Axes.AutoScale();
  }
  private void ConfigureBottomAxis() {
    var plots = PriceChart.Multiplot.GetPlots();
    var lastPlot = plots[^1];
    lastPlot.Axes.DateTimeTicksBottom();
    foreach (var plot in plots) {
      plot.Axes.Color(new Color((Foreground as Avalonia.Media.SolidColorBrush)?.Color.ToSKColor() ?? SkiaSharp.SKColors.Black));
    }
    PriceChart.Multiplot.SharedAxes.ShareX(plots);
    PriceChart.Multiplot.CollapseVertically();
    PriceChart.UserInputProcessor.LeftClickDragPan(enable: true, horizontal: true, vertical: false);
  }
  private void VolumeToggle_Checked(object? sender, RoutedEventArgs args) {
    
  }
  private void AmountToggle_Checked(object? sender, RoutedEventArgs args) {
    
  }
  private async void ConfigureIndicators(object? sender, RoutedEventArgs args) {
    var dialog = new ChartIndicatorDialog() {
      DataContext = CastedDataContext,
      Title = "보조지표 설정",
    };
    if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime) {
      await dialog.ShowDialog<int>(lifetime.MainWindow!);
    }
    if (CastedDataContext == null) return;
    ConfigureCandleChart();
    ConfigureIndicatorPlots();
    ConfigureLayout();
    ConfigureBottomAxis();
    PriceChart.InvalidateVisual();
  }
  private async void SaveFull(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    var top = TopLevel.GetTopLevel(this)!;
    var savePath = await top.StorageProvider.SaveFilePickerAsync(
      new FilePickerSaveOptions() {
        SuggestedFileName = "data",
        FileTypeChoices = [
          new FilePickerFileType("Comma-separated values") { MimeTypes = [ "text/csv" ], Patterns = [ "*.csv" ] }
        ]
      }
    );
    if (savePath is not null) {
      using var stream = await savePath.OpenWriteAsync();
      CastedDataContext.WriteEverything(stream);
    }
  }
  private async void SaveBacktestPy(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    var top = TopLevel.GetTopLevel(this)!;
    var savePath = await top.StorageProvider.SaveFilePickerAsync(
      new FilePickerSaveOptions() {
        SuggestedFileName = "data",
        FileTypeChoices = [
          new FilePickerFileType("Comma-separated values") { MimeTypes = [ "text/csv" ], Patterns = [ "*.csv" ] }
        ]
      }
    );
    if (savePath is not null) {
      using var stream = await savePath.OpenWriteAsync();
      CastedDataContext.WriteBacktestPy(stream);
    }
  }
  private void StartDrawing(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    if (sender is not MenuItem item) return;
    if (item.DataContext is not DrawingName name) return;
    CancelDrawing(interrupted: true);
    switch (name) {
      case DrawingName.TrendLine:
        _drawing = new TrendLine();
        break;
    }
    if (_drawing != null) {
      _candlestickPlot?.AddDrawing(_drawing);
      EnableDrawing();
    }
  }
  private void PriceChart_PointerPressed(object? sender, PointerPressedEventArgs args) {
    
  }
  private void PriceChart_PointerReleased(object? sender, PointerReleasedEventArgs args) {
    if (_drawingMode) {
      // if (args.) return;
      if (_candlestickPlot == null) return;
      var position = args.GetPosition(PriceChart);
      Pixel pixel = new(position.X, position.Y);
      var plot = PriceChart.Multiplot.GetPlotAtPixel(pixel);
      if (plot != _candlestickPlot) return;
      bool isLeft = args.InitialPressMouseButton == MouseButton.Left;
      bool isRight = args.InitialPressMouseButton == MouseButton.Right;
      if (isLeft) {
        var finished = RegisterDrawingCoordinate(_candlestickPlot.GetCoordinates(pixel));
        if (finished) CancelDrawing(interrupted: false);
      }
      else if (isRight) CancelDrawing(interrupted: true);
    }
  }
  private void PriceChart_PointerMoved(object? sender, PointerEventArgs args) {
    if (_drawingMode) {
      if (_candlestickPlot == null) return;
      var position = args.GetPosition(PriceChart);
      Pixel pixel = new(position.X, position.Y);
      var plot = PriceChart.Multiplot.GetPlotAtPixel(pixel);
      if (plot != _candlestickPlot) return;
      // 미리보기
      _drawing?.SetCoordinates(_drawingCoordinates.Append(_candlestickPlot.GetCoordinates(pixel)));
      PriceChart.InvalidateVisual();
    }
  }
  internal bool RegisterDrawingCoordinate(Coordinates coordinates) {
    _drawingCoordinates.Add(coordinates);
    _drawing?.SetCoordinates(_drawingCoordinates);
    return _drawingCoordinates.Count == _drawing?.CoordinateCount;
  }
  internal void EnableDrawing() {
    DrawingMenuItem.IsEnabled = false;
    _drawingMode = true;
    // 처음에 이상하게 추세선이 보이지 않도록 초기값을 NaN으로 모두 설정해버리자
    _drawing?.SetCoordinates(Enumerable.Repeat<Coordinates>(new(double.NaN, double.NaN), _drawing.CoordinateCount));
  }
  internal void CancelDrawing(bool interrupted = false) {
    if (!_drawingMode) return;
    _drawingMode = false;
    if (_drawing != null && interrupted) {
      _candlestickPlot?.Remove(_drawing);
    }
    _drawing = null;
    _drawingCoordinates.Clear();
    DrawingMenuItem.IsEnabled = true;
  }
}