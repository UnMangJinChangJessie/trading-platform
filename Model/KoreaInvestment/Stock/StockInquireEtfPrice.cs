using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockInquireEtpPriceQueries {
  public required string Ticker { get; init; }
}
public class StockInquireEtpPriceResult : KisReturnMessage {
  [JsonPropertyName("output")]
  public StockEtpDetailInformation? Information { get; init; }
}
public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquireEtpPriceResult? Result)> InquireStockEtpPrice(StockInquireChartQueries queries) {
    return await ApiClient.Request<object, StockInquireEtpPriceResult>(
      transId: "FHPST02400000",
      method: HttpMethod.Get,
      relUri: "/uapi/domestic-stock/v1/quotations/inquire-daily-itemchartprice",
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = "J",
        ["FID_INPUT_ISCD"] = queries.Ticker,
      },
      header: null,
      body: null
    );
  }
}