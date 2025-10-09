using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockInquireOrderBookQueries {
  public required Exchange MarketClassification { get; set; }
  public required string Ticker { get; set; }
}

public class StockInquireOrderBookResult : KisReturnMessage {
  [JsonPropertyName("output1")]
  public StockOrderBook? Output { get; set; }
}

public static partial class DomesticStock {
  public static readonly Action<StockInquireOrderBookQueries, Action<string>?> GetOrderBook = (queries, cb) =>
    ApiClient.PushRequest(
      "FHKST01010200", 
      callback: cb,
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.MarketClassification.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker
      }
    );
}