using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class StockInquirePriceQueries {
    public required Exchange Exchange { get; set; }
    public required string Ticker { get; set; }
  }
  public class StockInquirePriceResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public DetailedInformation? Information { get; set; }
  }
  public static readonly Action<StockInquirePriceQueries, Action<string, object?>?, object?> InquireStockPrice = (queries, cb, args) => 
    ApiClient.PushRequest(
      transId: "FHKST01010100",
      callback: cb, 
      callbackParameters: args,
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.Exchange.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker,
      }
    );
}