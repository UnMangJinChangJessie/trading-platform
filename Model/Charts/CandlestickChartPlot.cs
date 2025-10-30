using System.Collections.Concurrent;
using System.Collections.Immutable;
using ScottPlot;
using ScottPlot.DataSources;
using ScottPlot.Rendering;

namespace trading_platform.Model.Charts;


public class CandlestickChartPlot : Plot {
  private class ChartOHLCSource : OHLCSourceBase, IOHLCSource {
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
    private ConcurrentQueue<EventArgs> _updateQueue;
    public List<OHLC> Candles { get; private set; }
    public void RefreshIfNeeded() {
      if (_updateQueue.IsEmpty) return;
      lock (Candles) {
        while (!_updateQueue.IsEmpty) {
          _updateQueue.TryDequeue(out var result);
          if (result is CandlestickChartData.LoadedEventArgs loaded) {
            Candles = [.. loaded.WholeCandles.Select(x => x.ScottPlotCandle)];
          }
          else if (result is CandlestickChartData.UpdatedEndEventArgs updated) {
            if (updated.IsAppending) Candles.Add(updated.Candle.ScottPlotCandle);
            else Candles[^1] = updated.Candle.ScottPlotCandle;
          }
        }
      }
    }
    public override int Count => Candles.Count;
    public ChartOHLCSource(CandlestickChartData data) {
      BaseData = data;
      _updateQueue = new();
      lock (data.Candles) {
        Candles = [.. data.Candles.Select(x => x.ScottPlotCandle)];
      }
    }
    public override IReadOnlyList<OHLC> GetOHLCs() => Candles.AsReadOnly();
    public void ResetCandles(object? sender, CandlestickChartData.LoadedEventArgs args) {
      _updateQueue.Enqueue(args);
    }
    public void UpdateEndCandle(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
      _updateQueue.Enqueue(args);
    }
    ~ChartOHLCSource() {
      if (BaseData is not null) {
        BaseData.Loaded -= ResetCandles;
        BaseData.UpdatedEnd -= UpdateEndCandle;
      }
    }
  }
  private class CandlestickPlot : ScottPlot.Plottables.CandlestickPlot {
    public ChartOHLCSource? CastedDataSource => Data as ChartOHLCSource;
    public CandlestickPlot(CandlestickChartData data) : base(new ChartOHLCSource(data)) {}
  }
  private CandlestickPlot MainPlot { get; set; }
  private ScottPlot.Plottables.HorizontalLine PriceHorizontalLine { get; set; }
  private CandlestickChartData BaseChart;

  public FillStyle RisingFillStyle => MainPlot.RisingFillStyle;
  public LineStyle RisingLineStyle => MainPlot.RisingLineStyle;
  public FillStyle FallingFillStyle => MainPlot.FallingFillStyle;
  public LineStyle FallingLineStyle => MainPlot.FallingLineStyle;
  public IOHLCSource Data => MainPlot.Data;

  public CandlestickChartPlot(CandlestickChartData data) {
    Axes.Left.RemoveTickGenerator();
    Axes.Right.TickGenerator = new ScottPlot.TickGenerators.NumericAutomatic();
    Grid.XAxis = Axes.Bottom;
    Grid.YAxis = Axes.Right;
    Axes.DefaultGrid = Grid;
    BaseChart = data;
    BaseChart.Loaded += OnLoaded;
    BaseChart.UpdatedEnd += OnUpdatedEnd;
    MainPlot = new(data);
    // MainPlot.Axes.XAxis = Axes.Bottom;
    // MainPlot.Axes.YAxis = Axes.Right;
    PriceHorizontalLine = Add.HorizontalLine((double)(data[0]?.Close ?? 0), width: 1, color: Colors.Gray, pattern: LinePattern.DenselyDashed);
    // PriceHorizontalLine.Axes.XAxis = Axes.Bottom;
    // PriceHorizontalLine.Axes.YAxis = Axes.Right;
    Add.Plottable(MainPlot);
    Add.Plottable(PriceHorizontalLine);
    PriceHorizontalLine.LabelAlignment = Alignment.MiddleLeft;
    PriceHorizontalLine.LabelFontColor = Colors.White;
    PriceHorizontalLine.LabelOppositeAxis = true;
    PriceHorizontalLine.LabelRotation = 0;
    PriceHorizontalLine.LineWidth = 1;
    PriceHorizontalLine.Text = data[0]?.Close.ToString() ?? "";
    RenderManager.RenderStarting += OnRenderStart;
  }
  private void OnRenderStart(object? sender, RenderPack args) {
    var source = MainPlot.CastedDataSource;
    if (source == null) return;
    source.RefreshIfNeeded();
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