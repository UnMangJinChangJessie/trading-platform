using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScottPlot;
using ScottPlot.Plottables;
using trading_platform.Model;
using trading_platform.Model.Charts.Indicators;
using trading_platform.ViewModel;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  private Model.Charts.CandlestickChartData? CastedDataContext => DataContext as Model.Charts.CandlestickChartData;
  private Plot CandlePlot;
  private List<OHLC> CandleSource;
  private Plot VolumePlot;
  private int? DraggingDividerIndex;
  public CandlestickChart() {
    InitializeComponent();
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.Menu?.Add("Show/Hide Grid", plot => {
      plot.Grid.XAxisStyle.IsVisible = !plot.Grid.XAxisStyle.IsVisible;
      plot.Grid.YAxisStyle.IsVisible = !plot.Grid.YAxisStyle.IsVisible;
    });
    CandleSource = [];
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (System.Diagnostics.Debugger.IsAttached || Design.IsDesignMode) {
      foreach (var candle in CastedDataContext.Candles) AddCandle(candle);
      PriceChart.Refresh();
    }
    CastedDataContext?.CandleChanged += (s, candle) => {
      Dispatcher.UIThread.Post(() => {
        AddCandle(candle);
        PriceChart.InvalidateVisual();
      });
    };
    CastedDataContext?.CandleInserted += (s, candle) => {
      Dispatcher.UIThread.Post(() => {
        AddCandle(candle);
        PriceChart.InvalidateVisual();
      });
    };
    CastedDataContext?.Cleared += (s, candles) => {
      Dispatcher.UIThread.Post(() => {
        CandleSource.Clear();
        PriceChart.InvalidateVisual();
      });
    };
    CastedDataContext?.CandleRemoved += (s, date) => {
      Dispatcher.UIThread.Post(() => {
        CandleSource.RemoveAll(x => x.DateTime == date);
        PriceChart.InvalidateVisual();
      });
    };
    PriceChart.Multiplot.AddPlots(3);
    PriceChart.Multiplot.CollapseVertically();
    ConfigureCandleChart();
    ConfigureVolumeChart();
    var macd = new MovingAverageConvergenceDivergence(CastedDataContext, 12, 26);
    PriceChart.Multiplot.GetPlot(2).Add.Plottable(macd);
    PriceChart.Multiplot.GetPlot(2).Axes.ContinuouslyAutoscale = true;
    PriceChart.Multiplot.GetPlot(2).Axes.ContinuousAutoscaleAction = macd.ContinuousAutoscaleAction;
    PriceChart.Multiplot.GetPlot(2).Grid.YAxis = PriceChart.Multiplot.GetPlot(2).Axes.Right;
    ConfigureLayout();
    ConfigureBottomAxis();
    PriceChart.Multiplot.SharedAxes.ShareX(PriceChart.Multiplot.GetPlots());
  }
  public void PriceChart_ContinuousAutoscale(RenderPack rp) {
    if (CandleSource.Count == 0) return;
    var plot = rp.Plot;
    var currentRange = plot.Axes.GetLimits().HorizontalRange;
    var begin = DateTime.FromOADate(currentRange.Min);
    var end = DateTime.FromOADate(currentRange.Max);
    var dateTimeComparer = Comparer<OHLC>.Create((x, y) => x.DateTime.CompareTo(y.DateTime));
    var candleBeginIdx = CandleSource.BinarySearch(new OHLC { DateTime = begin }, dateTimeComparer);
    if (candleBeginIdx < 0) candleBeginIdx = ~candleBeginIdx;
    var candleEndIdx = CandleSource.BinarySearch(new OHLC { DateTime = end }, dateTimeComparer);
    if (candleEndIdx < 0) candleEndIdx = Math.Min(~candleEndIdx + 1, CandleSource.Count);
    if (candleEndIdx == candleBeginIdx) return;
    var slice = CandleSource[candleBeginIdx..candleEndIdx]; // it's shallow copy lol
    var lowerLimit = slice.Min(x => x.Low);
    var upperLimit = slice.Max(x => x.High);
    foreach (var plottable in CandlePlot.PlottableList) {
      if (plottable == null) continue;
      plottable.Axes.YAxis.Range.Set(lowerLimit * 1.05 - upperLimit * 0.05, upperLimit * 1.05 - lowerLimit * 0.05);
      plottable.Axes.XAxis.Range.Set(currentRange.Min, currentRange.Max);
    }
  }
  public void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  public void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
  }
  private void AddCandle(ChartOHLC candle) {
    int idx;
    {
      int lo = 0, hi = CandleSource.Count;
      while (lo != hi) {
        int mid = lo + (hi - lo) / 2;
        if (candle.Date == CandleSource[mid].DateTime) {
          lo = mid;
          break;
        }
        else if (candle.Date < CandleSource[mid].DateTime) hi = mid;
        else lo = mid + 1;
      }
      idx = lo;
    }
    var ohlc = new OHLC((double)candle.Open, (double)candle.High, (double)candle.Low, (double)candle.Close) {
      DateTime = candle.Date,
      TimeSpan = CastedDataContext.TimeSpan
    };
    var bar = new Bar() { Value = (double)candle.Volume, Position = candle.Date.ToOADate() };
    if (idx < CandleSource.Count && CandleSource[idx].DateTime == candle.Date) {
      CandleSource[idx] = ohlc;
    }
    else {
      CandleSource.Insert(idx, ohlc);
    }
  }
  private void ConfigureCandleChart() {
    CandlePlot = PriceChart.Multiplot.GetPlot(0);
    CandlePlot.DataBackground.Color = Colors.Transparent;
    CandlePlot.FigureBackground.Color = Colors.Transparent;
    CandlePlot.Grid.YAxis = CandlePlot.Axes.Right;
    CandlePlot.Axes.ContinuouslyAutoscale = true;
    CandlePlot.Axes.ContinuousAutoscaleAction = PriceChart_ContinuousAutoscale;

    CandleSource = [..CastedDataContext.Candles.Select(x => new OHLC((double)x.Open, (double)x.High, (double)x.Low, (double)x.Close) {
      DateTime = x.Date, TimeSpan = CastedDataContext.TimeSpan
    })];
    var candles = CandlePlot.Add.Candlestick(CandleSource);
    candles.Axes.YAxis = CandlePlot.Axes.Right;
    candles.RisingColor = Colors.LightPink;
    candles.FallingColor = Colors.LightBlue;
    var sma_10  = new SimpleMovingAverage(CastedDataContext, 10);
    sma_10.LineStyle.Color = Colors.DarkRed;
    var sma_20  = new SimpleMovingAverage(CastedDataContext, 20);
    sma_20.LineStyle.Color = Colors.DarkOrange;
    var sma_60  = new SimpleMovingAverage(CastedDataContext, 60);
    sma_60.LineStyle.Color = Colors.Gold;
    var sma_120 = new SimpleMovingAverage(CastedDataContext, 120);
    sma_120.LineStyle.Color = Colors.Green;
    var sma_200 = new SimpleMovingAverage(CastedDataContext, 200);
    sma_200.LineStyle.Color = Colors.DarkBlue;
    CandlePlot.Add.Plottable(sma_10);
    CandlePlot.Add.Plottable(sma_20);
    CandlePlot.Add.Plottable(sma_60);
    CandlePlot.Add.Plottable(sma_120);
    CandlePlot.Add.Plottable(sma_200);
  }
  private void ConfigureVolumeChart() {
    VolumePlot = PriceChart.Multiplot.GetPlot(1);
    var volume = new Volume(CastedDataContext);
    VolumePlot.Add.Plottable(volume);
    VolumePlot.Grid.YAxis = VolumePlot.Axes.Right;
    VolumePlot.Axes.ContinuouslyAutoscale = true;
    VolumePlot.Axes.ContinuousAutoscaleAction = volume.ContinuousAutoscaleAction;
    volume.Axes.XAxis = VolumePlot.Axes.Bottom;
    volume.Axes.YAxis = VolumePlot.Axes.Right;
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