
using System.Text.Json;
using System.Text.Json.Serialization;
using TradingSystem.KoreaInvestment;

namespace TradingSystem.Chart;

public sealed class KoreaInvestmentCandleProvider : CandleProvider<KoreaInvestmentCandleProviderOptions> {
  public KoreaInvestmentCandleProvider(Chart.CandlestickChart targetChart) {
    Chart = targetChart;
  }
  private async Task<IEnumerable<Candle>> FetchStockCandles(KoreaInvestmentCandleProviderOptions option) {
    UriBuilder builder = new();
    builder.Host = "";
    builder.Query = Common.BuildQueryString([
      ("FID_COND_MRKT_DIV_CODE", option.Exchange.GetCode()),
      ("FID_INPUT_ISCD", option.Ticker),
      ("FID_INPUT_DATE_1", option.FetchPeriod.Item1.ToString("yyyyMMdd")),
      ("FID_INPUT_DATE_2", option.FetchPeriod.Item2.ToString("yyyyMMdd")),
      ("FID_PERIOD_DIV_CODE", option.Candle.GetCode()),
      ("FID_ORG_ADJ_PRC", option.Adjusted ? "0" : "1"),
    ]);
  }
  public override async Task FetchCandle(KoreaInvestmentCandleProviderOptions option) {
    IEnumerable<Candle> candles;
    if (option.Market == Market.STOCK) candles = await FetchStockCandles(option);
    
  }
}