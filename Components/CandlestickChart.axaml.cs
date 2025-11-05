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

internal enum DrawingMode {
  None,
  Add,
  Modify
}

public partial class CandlestickChart : UserControl {
  private CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  private CandlestickChartPlot _candlestickPlot;
  private ScottPlot.MultiplotLayouts.DraggableRows _chartLayout;
  private int? _draggingDividerIndex;
  private DrawingMode _drawingMode = DrawingMode.None;
  private Model.Charts.Drawing? _drawing = null;
  private bool _drawingRegisterOnRelease = false;
  private readonly List<Coordinates> _drawingCoordinates = [];
  private int? _drawingModifyingIndex = null;
  private ScottPlot.Plottables.Crosshair _cursorCrosshair;
  private bool _cursorCrosshairVisible;
  public CandlestickChart() {
    InitializeComponent();
    _cursorCrosshair = new();
    _cursorCrosshairVisible = true;
    _chartLayout = new();
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.UserInputProcessor.DoubleLeftClickBenchmark(false);
    PriceChart.Menu?.Add("격자 보이기/숨기기", plot => {
      foreach (var mpPlot in PriceChart.Multiplot.GetPlots()) {
        mpPlot.Grid.XAxisStyle.IsVisible = !mpPlot.Grid.XAxisStyle.IsVisible;
        mpPlot.Grid.YAxisStyle.IsVisible = !mpPlot.Grid.YAxisStyle.IsVisible;
      }
    });
    PriceChart.Menu?.Add("상하 범위 자동 조정", plot => {
      plot.Axes.ContinuouslyAutoscale = !plot.Axes.ContinuouslyAutoscale;
    });
    PriceChart.Menu?.Add("십자선 보이기/숨기기", plot => {
      _cursorCrosshairVisible = !_cursorCrosshairVisible;
      _cursorCrosshair.IsVisible = _cursorCrosshairVisible;
    });
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    _candlestickPlot = new CandlestickChartPlot(CastedDataContext);
    if (!CastedDataContext.Indicators.OfType<Volume>().Any()) {
      CastedDataContext.AddIndicator(new Volume(CastedDataContext));
    }
    _cursorCrosshair = _candlestickPlot.Add.Crosshair(0, 0);
    _cursorCrosshair.Axes.XAxis = _candlestickPlot.Axes.GetXAxes().First();
    _cursorCrosshair.Axes.YAxis = _candlestickPlot.Axes.GetYAxes().First();
    ConfigureCandleChart();
    ConfigureIndicatorPlots();
    ConfigureLayout();
    ConfigureBottomAxis();
  }
  private void ConfigureCandleChart() {
    if (CastedDataContext == null) return;
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
    PriceChart.UserInputProcessor.LeftClickDragPan(enable: true, horizontal: true, vertical: true);
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
    var position = args.GetPosition(PriceChart);
    var pixel = new Pixel(position.X, position.Y);
    // Select drawing if it's close enough
    if (_candlestickPlot != null) {
      if (_drawingMode == DrawingMode.Add) return;
      var drawings = _candlestickPlot.GetPlottables<Model.Charts.Drawing>();
      bool clicked = false;
      if (_drawingMode == DrawingMode.None) {
        foreach (var drawing in drawings) {
          var (pixelOnDrawing, pixelIndex) = IsPixelOnDrawing(drawing, pixel);
          if (!pixelOnDrawing) continue;
          clicked = true;
          EnableModification(drawing);
          return;
        }
      }
      else if (_drawingMode == DrawingMode.Modify) {
        var (pixelOnDrawing, pixelIndex) = IsPixelOnDrawing(_drawing, pixel);
        clicked = pixelOnDrawing;
        if (_drawingModifyingIndex != null) {
          _drawingModifyingIndex = null;
        }
        else if (args.ClickCount == 2) {
          _drawingModifyingIndex = pixelIndex;
        }
      }
      if (clicked) return;
      CancelModification();
    }
    var y = (float)args.GetPosition(PriceChart).Y;
    var divider = _chartLayout.GetDivider(y);
    _draggingDividerIndex = divider;
    PriceChart.UserInputProcessor.IsEnabled = divider is null;
  }
  private void PriceChart_PointerReleased(object? sender, PointerReleasedEventArgs args) {
    var position = args.GetPosition(PriceChart);
    Pixel pixel = new(position.X, position.Y);
    bool isLeft = args.InitialPressMouseButton == MouseButton.Left;
    bool isRight = args.InitialPressMouseButton == MouseButton.Right;
    if (_drawingMode == DrawingMode.Add) {
      // if (args.) return;
      if (_candlestickPlot == null) return;
      if (!_drawingRegisterOnRelease) {
        _drawingRegisterOnRelease = true;
        return;
      }
      var plot = PriceChart.Multiplot.GetPlotAtPixel(pixel);
      if (plot != _candlestickPlot) return;
      if (isLeft) {
        var finished = RegisterDrawingCoordinate(_candlestickPlot.GetCoordinates(pixel));
        if (finished) CancelDrawing(interrupted: false);
      }
      else if (isRight) CancelDrawing(interrupted: true);
    }
    else if (_draggingDividerIndex != null) {
      _draggingDividerIndex = null;
      PriceChart.UserInputProcessor.IsEnabled = true;
    }
  }
  private void PriceChart_PointerMoved(object? sender, PointerEventArgs args) {
    var position = args.GetPosition(PriceChart);
    Pixel pixel = new(position.X, position.Y);
    if (_drawingMode == DrawingMode.Add) {
      if (args.Properties.IsLeftButtonPressed) _drawingRegisterOnRelease = false;
      if (_candlestickPlot == null) return;
      var plot = PriceChart.Multiplot.GetPlotAtPixel(pixel);
      if (plot != _candlestickPlot) return;
      // 미리보기
      _drawing?.SetCoordinates(_drawingCoordinates.Append(_candlestickPlot.GetCoordinates(pixel)));
      PriceChart.InvalidateVisual();
    }
    else if (_drawingMode == DrawingMode.Modify) {
      if (_candlestickPlot == null) return;
      if (_drawingModifyingIndex != null) {
        _drawingCoordinates[_drawingModifyingIndex.Value] = _candlestickPlot.GetCoordinates(pixel);
        _drawing?.SetCoordinates(_drawingCoordinates);
        PriceChart.InvalidateVisual();
      }
    }

    if (_draggingDividerIndex != null) {
      _chartLayout.SetDivider(_draggingDividerIndex.Value, (float)position.Y);
      PriceChart.Refresh();
    }
    var divider = _chartLayout.GetDivider((float)position.Y);
    Cursor = new Cursor(
      divider != null ? StandardCursorType.SizeNorthSouth : StandardCursorType.Arrow
    );
    if (_cursorCrosshairVisible) {
      _cursorCrosshair.Position = _candlestickPlot.GetCoordinates(pixel);
      PriceChart.InvalidateVisual();
    }
  }
  private void PriceChart_PointerEntered(object? sender, PointerEventArgs args) {
    _cursorCrosshair.IsVisible = _cursorCrosshairVisible;
  }
  private void PriceChart_PointerExited(object? sender, PointerEventArgs args) {
    _cursorCrosshair.IsVisible = false;
  }
  private void PriceChart_KeyDown(object? sender, KeyEventArgs args) {
    if (_drawingMode == DrawingMode.Modify) {
      if (_drawing != null) {
        if (args.Key == Key.Escape || (args.Key == Key.X && args.KeyModifiers == KeyModifiers.Control)) {
          _candlestickPlot.RemoveDrawing(_drawing);
          CancelModification();
        }
        else if (args.Key == Key.C || args.KeyModifiers == KeyModifiers.Control) {
          var clone = _drawing.Clone();
          clone.SetCoordinates(clone.DrawingCoordinates.Select(x => x with { X = x.X + 1 }));
          _candlestickPlot.AddDrawing(clone);
          CancelModification();
          EnableModification(clone);
        }
      }
    }
  }
  internal bool RegisterDrawingCoordinate(Coordinates coordinates) {
    _drawingCoordinates.Add(coordinates);
    _drawing?.SetCoordinates(_drawingCoordinates);
    return _drawingCoordinates.Count == _drawing?.CoordinateCount;
  }
  internal void EnableDrawing() {
    DrawingMenuItem.IsEnabled = false;
    _drawingMode = DrawingMode.Add;
    _drawingRegisterOnRelease = true;
    _drawingCoordinates.Clear();
    // 처음에 이상하게 추세선이 보이지 않도록 초기값을 NaN으로 모두 설정해버리자
    _drawing?.SetCoordinates(Enumerable.Repeat<Coordinates>(new(double.NaN, double.NaN), _drawing.CoordinateCount));
  }
  internal void EnableModification(Model.Charts.Drawing drawing) {
    _drawingMode = DrawingMode.Modify;
    _drawing = drawing;
    _drawing.IsCoordinatesVisible = true;
    _drawingCoordinates.Clear();
    _drawingCoordinates.AddRange(drawing.DrawingCoordinates);
    _drawingRegisterOnRelease = true;
    PriceChart.UserInputProcessor.LeftClickDragPan(false, false, false);
    PriceChart.Refresh();
  }
  internal void CancelDrawing(bool interrupted = false) {
    if (_drawingMode != DrawingMode.Add) return;
    _drawingMode = DrawingMode.None;
    if (_drawing != null && interrupted) {
      _candlestickPlot?.Remove(_drawing);
    }
    _drawing = null;
    _drawingCoordinates.Clear();
    _drawingRegisterOnRelease = true;
    DrawingMenuItem.IsEnabled = true;
    PriceChart.Refresh();
  }
  internal void CancelModification() {
    if (_drawingMode != DrawingMode.Modify) return;
    _drawing?.IsCoordinatesVisible = false;
    _drawingMode = DrawingMode.None;
    _drawing = null;
    _drawingCoordinates.Clear();
    _drawingRegisterOnRelease = true;
    PriceChart.UserInputProcessor.LeftClickDragPan(true, true, true);
    PriceChart.Refresh();
  }
  private static readonly (bool OnDrawing, int? CoordinatesIndex) IS_PIXEL_ON_DRAWING_DEFAULT_VALUE = (false, null);
  internal (bool OnDrawing, int? CoordinatesIndex) IsPixelOnDrawing(Model.Charts.Drawing drawing, Pixel pixel, double grace = 5.0) {
    if (_candlestickPlot == null) return IS_PIXEL_ON_DRAWING_DEFAULT_VALUE;
    var iter = drawing.DrawingCoordinates.GetEnumerator();
    if (!iter.MoveNext()) return IS_PIXEL_ON_DRAWING_DEFAULT_VALUE ;
    var prevPixel = _candlestickPlot.GetPixel(iter.Current);
    var prevIndex = 0;
    bool pixelOnDrawing = false;
    if (prevPixel.DistanceFrom(pixel) < grace) return (true, prevIndex);
    while (iter.MoveNext()) {
      var currentPixel = _candlestickPlot.GetPixel(iter.Current);
      var directionVector = currentPixel - prevPixel;
      var directionVectorLength = directionVector.DistanceFrom(Pixel.Zero);
      if (directionVectorLength < 1e-10) continue;
      var currentPixelVector = pixel - prevPixel;
      var currentPixelVectorLength = currentPixelVector.DistanceFrom(Pixel.Zero);
      if (currentPixelVectorLength < 1e-10) return (true, prevIndex); // 점이 어떤 픽셀과 가까이 있는 것이니 바로 반환
      var innerProduct = directionVector.X * currentPixelVector.X + directionVector.Y * currentPixelVector.Y;
      var vectorSine = Math.Sin(Math.Acos(innerProduct / currentPixelVectorLength / directionVectorLength));
      var distance = currentPixelVector.DistanceFrom(Pixel.Zero) * vectorSine;
      if (distance < grace) {
        pixelOnDrawing = true;
        break;
      }
      prevPixel = currentPixel;
      prevIndex++;
    }
    if (prevPixel.DistanceFrom(pixel) < grace) {
      pixelOnDrawing = true;
    }
    int? index = pixelOnDrawing ? drawing.DrawingCoordinates.Select((x, i) => (coord: x, distance: _candlestickPlot.GetPixel(x).DistanceFrom(pixel), index: i))
      .Where(t => t.distance < grace)
      .FirstOrDefault()
      .index : null;
    return (pixelOnDrawing, index);
  }
}