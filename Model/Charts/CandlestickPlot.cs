using ScottPlot;
using ScottPlot.DataSources;

namespace trading_platform.Model.Charts;

public class ChartOHLCSource : OHLCSourceBase, IOHLCSource {
  public CandlestickChartData BaseData {
    get => field;
    set {
      if (field != value) {
        if (field != null) {
          field.Loaded -= ResetCandles;
          field.UpdatedEnd -= UpdateEndCandle;
        }
        value.Loaded += ResetCandles;
        value.UpdatedEnd += UpdateEndCandle;
        field = value;
      }
    }
  }
  public List<OHLC> Candles { get; private set; }
  public override int Count => Candles.Count;
  public ChartOHLCSource(CandlestickChartData data) {
    BaseData = data;
    lock (data.Candles) {
      Candles = [.. data.Candles.Select(x => x.ScottPlotCandle)];
    }
  }
  public override IReadOnlyList<OHLC> GetOHLCs() => Candles;
  public void ResetCandles(object? sender, CandlestickChartData.LoadedEventArgs args) {
    lock (Candles) {
      Candles = [.. args.WholeCandles.Select(x => x.ScottPlotCandle)];
    }
  }
  public void UpdateEndCandle(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    lock (Candles) {
      if (args.IsAppending || Count == 0) Candles.Add(args.Candle.ScottPlotCandle);
      else Candles[^1] = args.Candle.ScottPlotCandle;
    }
  }
  ~ChartOHLCSource() {
    if (BaseData is not null) {
      BaseData.Loaded -= ResetCandles;
      BaseData.UpdatedEnd -= UpdateEndCandle;
    }
  }
}

public class CandlestickChartPlot : Plot {
  public class CandlestickPlot : ScottPlot.Plottables.CandlestickPlot {
    public ChartOHLCSource? CastedDataSource => Data as ChartOHLCSource;
    public CandlestickPlot(CandlestickChartData data) : base(new ChartOHLCSource(data)) {}
    public override void Render(RenderPack rp) {
      base.Render(rp);
    }
  }
  public CandlestickPlot MainPlot { get; private set; }
  public ScottPlot.Plottables.HorizontalLine PriceHorizontalLine { get; private set; }
  private CandlestickChartData BaseChart;

  public CandlestickChartPlot(CandlestickChartData data) {
    BaseChart = data;
    BaseChart.Loaded += OnLoaded;
    BaseChart.UpdatedEnd += OnUpdatedEnd;
    MainPlot = new(data);
    PriceHorizontalLine = Add.HorizontalLine((double)(data[0]?.Close ?? 0), width: 1, color: Colors.Gray, pattern: LinePattern.DenselyDashed);
    Add.Plottable(MainPlot);
    Add.Plottable(PriceHorizontalLine);
    MainPlot.Axes.XAxis = Axes.Bottom;
    MainPlot.Axes.YAxis = Axes.Right;
    PriceHorizontalLine.Axes.XAxis = Axes.Bottom;
    PriceHorizontalLine.Axes.YAxis = Axes.Right;
    PriceHorizontalLine.LabelAlignment = Alignment.MiddleLeft;
    PriceHorizontalLine.LabelFontColor = Colors.White;
    PriceHorizontalLine.LabelOppositeAxis = true;
    PriceHorizontalLine.LineWidth = 1;
    PriceHorizontalLine.Text = data[0]?.Close.ToString() ?? "";
  }
  private void OnLoaded(object? sender, CandlestickChartData.LoadedEventArgs args) {
    if (args.WholeCandles.Count == 0) return;
    PriceHorizontalLine.Y = (double)args.WholeCandles[^1].Close;
    PriceHorizontalLine.Text = args.WholeCandles[^1].Close.ToString();
    PlotControl?.Refresh();
  }
  private void OnUpdatedEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    PriceHorizontalLine.Y = (double)args.Candle.Close;
    PriceHorizontalLine.Text = args.Candle.Close.ToString();
    PlotControl?.Refresh();
  }
  ~CandlestickChartPlot() {
    BaseChart.Loaded -= OnLoaded;
    BaseChart.UpdatedEnd -= OnUpdatedEnd;
  }
}