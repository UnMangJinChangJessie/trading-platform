using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using ScottPlot;
using trading_platform.ViewModel;

namespace trading_platform.Components;

public partial class CandlestickChart : UserControl {
  /// <summary>
  /// ZoomSensitivity StyledProperty definition
  /// indicates how fast the plot zooms in/out during pointer scrolls.
  /// </summary>
  public static readonly StyledProperty<double> ZoomSensitivityProperty =
    AvaloniaProperty.Register<CandlestickChart, double>(nameof(ZoomSensitivity));

  /// <summary>
  /// Gets or sets the ZoomSensitivity property. This StyledProperty
  /// indicates how fast the plot zooms in/out during pointer scrolls.
  /// </summary>
  public double ZoomSensitivity {
    get => GetValue(ZoomSensitivityProperty);
    set => SetValue(ZoomSensitivityProperty, value);
  }
  private ScottPlot.Plottables.OhlcPlot? OHLCChart { get; set; }
  private List<OHLC> OHLCChartSource { get; set; } = [];
  private MarketData? CastedDataContext => DataContext as MarketData;

  private double BottomLimitRateStart { get; set; } = 0.0;
  private double BottomLimitRateEnd { get; set; } = 1.0;
  public CandlestickChart() {
    InitializeComponent();
  }
  public void UserControl_Loaded(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    OHLCChartSource = [.. CastedDataContext.PriceChart.Select(x => x.ToScottPlotOHLC())];
    OHLCChart = PriceChart.Plot.Add.OHLC(OHLCChartSource);
    PriceChart.Plot.Font.Set("Gowun Dodum");
    PriceChart.Plot.Axes.ContinuouslyAutoscale = true;
    PriceChart.Plot.Axes.ContinuousAutoscaleAction = PriceChart_ContinuousAutoscale;
    var dtAxis = PriceChart.Plot.Axes.DateTimeTicksBottom();
    dtAxis.ClipLabel = true;
    PriceChart.Refresh();
    // 테마 설정
    // 현재 창에 맞는 색으로 설정
    PriceChart.Plot.FigureBackground.Color = new Color(System.Drawing.Color.Transparent);
    PriceChart.Plot.DataBackground.Color = Color.FromARGB(0x00000000U);
    OHLCChart.RisingStyle.Color = Colors.LightPink;
    OHLCChart.FallingStyle.Color = Colors.LightBlue;
    CastedDataContext.ChartChanging += (sender, args) => {
      Dispatcher.UIThread.Post(() => {
        if (args.UpdateType == MarketData.CandleUpdate.Clear) {
          OHLCChartSource.Clear();
        }
        else if (args.UpdateType == MarketData.CandleUpdate.InsertBegin) {
          OHLCChartSource.Insert(0, args.Candle!.ToScottPlotOHLC());
        }
        else if (args.UpdateType == MarketData.CandleUpdate.InsertEnd) {
          OHLCChartSource.Insert(0, args.Candle!.ToScottPlotOHLC());
        }
        else if (args.UpdateType == MarketData.CandleUpdate.UpdateLast) {
          OHLCChartSource[^1] = args.Candle!.ToScottPlotOHLC();
        }
        PriceChart.Refresh();
      });
    };
  }
  public void PriceChart_ContinuousAutoscale(RenderPack rp) {
    if (OHLCChartSource.Count == 0) return;
    var plot = rp.Plot;
    if (plot == null) return;
    var dataLimits = plot.Axes.GetDataLimits();
    var limits = plot.Axes.GetLimits();
    var (leftCoordinate, rightCoordinate) = (
      (limits.Left - dataLimits.Left) / dataLimits.XRange.Length * OHLCChartSource.Count,
      (limits.Right - dataLimits.Left) / dataLimits.XRange.Length * OHLCChartSource.Count
    );
    leftCoordinate = Math.Clamp(leftCoordinate, -10, OHLCChartSource.Count);
    rightCoordinate = Math.Clamp(rightCoordinate, leftCoordinate + 10, OHLCChartSource.Count + 10);
    plot.Axes.SetLimitsX(
      leftCoordinate / OHLCChartSource.Count * dataLimits.XRange.Length + dataLimits.Left,
      rightCoordinate / OHLCChartSource.Count * dataLimits.XRange.Length + dataLimits.Left
    );
    var dataIndexRange = new System.Range(
      (int)Math.Floor(Math.Clamp(leftCoordinate, 0, OHLCChartSource.Count - 1)),
      (int)Math.Ceiling(Math.Clamp(rightCoordinate, 0, OHLCChartSource.Count - 1)));
    if (dataIndexRange.Start.Value == dataIndexRange.End.Value) return;
    if (dataIndexRange.Start.Value + 1 == dataIndexRange.End.Value) return;
    double min = OHLCChartSource[dataIndexRange].Min(x => x.Low);
    double max = OHLCChartSource[dataIndexRange].Max(x => x.High);
    double diff = max - min;
    min -= diff / 18;
    max += diff / 18;
    plot.Axes.SetLimitsY(min, max);
  }
  public void UpdateClose(double value) {
    if (OHLCChartSource.Count == 0) return;
    var candle = OHLCChartSource[^1];
    candle.Close = value;
    candle.Low = Math.Min(value, candle.Low);
    candle.High = Math.Max(value, candle.High);
  }
  // public void AddCandle(double open, double high, double low, double close) {
  //   OHLCChartSource.Add(new(open, high, low, close));
  // }
  // public void AddCandle(OHLC ohlc) {
  //   OHLCChartSource.Add(ohlc);
  // }
}