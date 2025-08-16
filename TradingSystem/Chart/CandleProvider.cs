namespace TradingSystem.Chart;

public abstract class CandleProvider<TOption> {
  public CandlestickChart Chart { get; protected set; }
  protected Thread? RealtimeFetchThread { get; set; } = null;

  public abstract Task FetchCandle(TOption option);
  public abstract bool BeginRealtimeFetch(TOption option);
  public abstract bool EndRealtimeFetch();
}