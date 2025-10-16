using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;


public static partial class DomesticStock {
  public class OrderBookQueries {
    public required Exchange MarketClassification { get; set; }
    public required string Ticker { get; set; }
  }

  public class OrderBookResult : KisReturnMessage {
    [JsonPropertyName("output1")]
    public OrderBook? Output { get; set; }
    [JsonPropertyName("output2")]
    public OrderBookInformation? Information { get; set; }
  }
  public static readonly Action<OrderBookQueries, Action<string, object?>?, object?> GetOrderBook = (queries, cb, args) =>
    ApiClient.PushRequest(
      "FHKST01010200", 
      callback: cb,
      callbackParameters: args,
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.MarketClassification.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker
      }
    );
}