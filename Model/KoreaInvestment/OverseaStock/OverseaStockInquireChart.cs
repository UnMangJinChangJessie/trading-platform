using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockInquireChartQueries {
  public required Exchange Exchange { get; init; }
  public required string Ticker { get; init; }
  public required DateOnly EndDate { get; init; }
  public required CandlePeriod CandlePeriod { get; init; }
  public required bool Adjusted { get; init; }
}
public class OverseaStockInquireChartResult : KisReturnMessage {
  [JsonPropertyName("output2")]
  public IEnumerable<OverseaStockChart>? Chart { get; init; }
}
public static partial class OverseaStock {
  public static async Task<(HttpStatusCode StatusCode, OverseaStockInquireChartResult? Result)> InquireStockChart(OverseaStockInquireChartQueries queries) {
    return await ApiClient.Request<object, OverseaStockInquireChartResult>(
      transId: "HHDFS76240000",
      method: HttpMethod.Get,
      relUri: "/uapi/overseas-price/v1/quotations/dailyprice",
      queries: new Dictionary<string, string>() {
        ["AUTH"] = "",
        ["EXCD"] = queries.Exchange.GetCode(),
        ["SYMB"] = queries.Ticker,
        ["GUBN"] = queries.CandlePeriod switch {
          CandlePeriod.Daily => "0",
          CandlePeriod.Weekly => "1",
          CandlePeriod.Monthly => "2",
          CandlePeriod.Yearly => "3",
          _ => "0"
        },
        ["BYMD"] = queries.EndDate.ToString("yyyyMMdd"),
        ["MODP"] = queries.Adjusted ? "1" : "0",
      },
      header: null,
      body: null
    );
  }
}