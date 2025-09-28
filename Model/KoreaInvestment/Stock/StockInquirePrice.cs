using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockInquirePriceQueries {
  public required Exchange Exchange { get; init; }
  public required string Ticker { get; init; }
}
public class StockInquirePriceResult : KisReturnMessage {
  [JsonPropertyName("output")]
  public StockDetailInformation? Information { get; init; }
}
public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquirePriceResult? Result)> InquireStockPrice(StockInquirePriceQueries queries) {
    return await ApiClient.Request<object, StockInquirePriceResult>(
      transId: "FHKST01010100",
      method: HttpMethod.Get,
      relUri: "/uapi/domestic-stock/v1/quotations/inquire-daily-itemchartprice",
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.Exchange.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker,
      },
      header: null,
      body: null
    );
  }
}