using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockInquireChartQueries {
  public required Exchange Exchange { get; init; }
  public required string Ticker { get; init; }
  public required DateOnly From { get; init; }
  public required DateOnly To { get; init; }
  public required CandlePeriod CandlePeriod { get; init; }
  public required bool Adjusted { get; init; }
}
public class StockInquireChartResult : KisReturnMessage {
  [JsonPropertyName("output1")]
  public StockBasicInformation? Information { get; init; }
  [JsonPropertyName("output2")]
  public IEnumerable<StockChart>? Chart { get; init; }
}
public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquireChartResult? Result)> InquireStockChart(StockInquireChartQueries queries) {
    return await ApiClient.Request<object, StockInquireChartResult>(
      transId: "FHKST03010100",
      method: HttpMethod.Get,
      relUri: "/uapi/domestic-stock/v1/quotations/inquire-daily-itemchartprice",
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.Exchange.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker,
        ["FID_INPUT_DATE_1"] = queries.From.ToString("yyyyMMdd"),
        ["FID_INPUT_DATE_2"] = queries.To.ToString("yyyyMMdd"),
        ["FID_PERIOD_DIV_CODE"] = queries.CandlePeriod.GetCode(),
        ["FID_ORG_ADJ_PRC"] = queries.Adjusted ? "0" : "1",
      },
      header: null,
      body: null
    );
  }
}