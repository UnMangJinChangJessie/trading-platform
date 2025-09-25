using Avalonia;
using Avalonia.Controls;
using ScottPlot;

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
    get => this.GetValue(ZoomSensitivityProperty);
    set => SetValue(ZoomSensitivityProperty, value);
  }
  public List<OHLC> CandlesticksSource { get; private set; } = [];

  private double BottomLimitRateStart { get; set; } = 0.0;
  private double BottomLimitRateEnd { get; set; } = 1.0;
  public CandlestickChart() {
    InitializeComponent();
    // 디버그 전용 파일 불러오기
    if (Design.IsDesignMode) {
      CandlesticksSource = Generate.RandomOHLCs(250 * 3, DateTime.Today);
    }
    var ohlc = PriceChart.Plot.Add.OHLC(CandlesticksSource);
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
    ohlc.RisingStyle.Color = Colors.LightPink;
    ohlc.FallingStyle.Color = Colors.LightBlue;
  }
  public void PriceChart_ContinuousAutoscale(RenderPack rp) {
    if (CandlesticksSource.Count == 0) return;
    var plot = rp.Plot;
    if (plot == null) return;
    var dataLimits = plot.Axes.GetDataLimits();
    var limits = plot.Axes.GetLimits();
    var (leftCoordinate, rightCoordinate) = (
      (limits.Left - dataLimits.Left) / dataLimits.XRange.Length * CandlesticksSource.Count,
      (limits.Right - dataLimits.Left) / dataLimits.XRange.Length * CandlesticksSource.Count
    );
    leftCoordinate = Math.Clamp(leftCoordinate, -10, CandlesticksSource.Count);
    rightCoordinate = Math.Clamp(rightCoordinate, leftCoordinate + 10, CandlesticksSource.Count + 10);
    plot.Axes.SetLimitsX(
      leftCoordinate / CandlesticksSource.Count * dataLimits.XRange.Length + dataLimits.Left,
      rightCoordinate / CandlesticksSource.Count * dataLimits.XRange.Length + dataLimits.Left
    );
    var dataIndexRange = new System.Range(
      (int)Math.Floor(Math.Clamp(leftCoordinate, 0, CandlesticksSource.Count - 1)),
      (int)Math.Ceiling(Math.Clamp(rightCoordinate, 0, CandlesticksSource.Count - 1)));
    if (dataIndexRange.Start.Value == dataIndexRange.End.Value) return;
    if (dataIndexRange.Start.Value + 1 == dataIndexRange.End.Value) return;
    double min = CandlesticksSource[dataIndexRange].Min(x => x.Low);
    double max = CandlesticksSource[dataIndexRange].Max(x => x.High);
    double diff = max - min;
    min -= diff / 18;
    max += diff / 18;
    plot.Axes.SetLimitsY(min, max);
  }
  public void UpdateClose(double value) {
    if (CandlesticksSource.Count == 0) return;
    var candle = CandlesticksSource[^1];
    candle.Close = value;
    candle.Low = Math.Min(value, candle.Low);
    candle.High = Math.Max(value, candle.High);
  }
  public void AddCandle(double open, double high, double low, double close) {
    CandlesticksSource.Add(new(open, high, low, close));
  }
  public void AddCandle(OHLC ohlc) {
    CandlesticksSource.Add(ohlc);
  }
}