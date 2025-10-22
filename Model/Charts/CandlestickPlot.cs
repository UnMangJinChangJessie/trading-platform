using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    lock (data) {
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

public class CandlestickPlot : ScottPlot.Plottables.CandlestickPlot {
  public ChartOHLCSource? CastedDataSource => Data as ChartOHLCSource;
  public CandlestickPlot(CandlestickChartData data) : base(new ChartOHLCSource(data)) {
  }
}