using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class StockInquireEtpPriceQueries {
    public required string Ticker { get; set; }
  }
  public class StockInquireEtpPriceResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public EtpInformation? Information { get; set; }
  }
  public static readonly Action<StockInquireEtpPriceQueries, Action<string>?> GetEtpPrice = (queries, cb) => 
    ApiClient.PushRequest(
      transId: "FHPST02400000",
      callback: cb,
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = "J",
        ["FID_INPUT_ISCD"] = queries.Ticker,
      }
    );
}