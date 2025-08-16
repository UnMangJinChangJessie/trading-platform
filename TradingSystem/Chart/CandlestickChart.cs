namespace TradingSystem.Chart;

public class CandlestickChart
{
  public delegate void UpdateCandleEventHandler(object sender, Candle previousCandle, Candle updatedCandle);
  public delegate void NewCandleEventHandler(object sender, Candle newCandle);
  public List<Candle> Candles { get; private set; }
  public Dictionary<string, Indicator> Indicators { get; private set; }
  public int Count => Candles.Count;
  public event UpdateCandleEventHandler LastCandleUpdated;
  public event NewCandleEventHandler CandleAppended;
  public event NewCandleEventHandler CandlePrepended;
  public event NewCandleEventHandler CandlePoppedFront;
  public event NewCandleEventHandler CandlePoppedBack;

  public CandlestickChart()
  {
    Candles = new();
    Indicators = new();
    LastCandleUpdated = default!;
    CandleAppended = default!;
    CandlePrepended = default!;
    CandlePoppedFront = default!;
    CandlePoppedBack = default!;
  }
  public bool AddIndicator(string name, Indicator indicator)
  {
    if (Indicators.ContainsKey(name)) return false;
    indicator.BindChart(this);
    Indicators.Add(name, indicator);
    return true;
  }
  public bool RemoveIndicator(string name)
  {
    if (!Indicators.ContainsKey(name)) return false;
    Indicators.Remove(name);
    return true;
  }
  public void Append(Candle candle)
  {
    Candles.Add(candle);
    CandleAppended(this, candle);
  }
  public void Prepend(Candle candle)
  {
    Candles.Insert(0, candle);
    CandlePrepended(this, candle);
  }
  public void UpdateLast(Candle candle)
  {
    if (Candles.Count == 0) return;
    Candle prev = Candles[^1];
    Candles[^1] = candle;
    LastCandleUpdated(this, prev, candle);
  }
  public void PopFront()
  {
    Candle front = Candles[0];
    Candles.RemoveAt(0);
    CandlePoppedFront(this, front);
  }
  public void PopBack()
  {
    Candle back = Candles[0];
    Candles.RemoveAt(Candles.Count - 1);
    CandlePoppedBack(this, back);
  }
  public Candle this[Index i]
  {
    get => Candles[i];
  }
  public List<Candle> this[Range r]
  {
    get => Candles[r];
  }
}