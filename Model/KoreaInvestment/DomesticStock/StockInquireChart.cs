using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class ChartQueries {
    public required Exchange Exchange { get; set; }
    public required string Ticker { get; set; }
    public required DateOnly From { get; set; }
    public required DateOnly To { get; set; }
    public required CandlePeriod CandlePeriod { get; set; }
    public required bool Adjusted { get; set; }
  }
  public class ChartResult : KisReturnMessage {
    [JsonPropertyName("output1")]
    public BasicInformation? Information { get; set; }
    [JsonPropertyName("output2")]
    public IEnumerable<ChartItem>? Chart { get; set; }
  }
  public static readonly Action<ChartQueries, Action<string, object?>?, object?> GetChart = (queries, cb, args) => 
    ApiClient.PushRequest(
      transId: "FHKST03010100",
      callback: cb,
      callbackParameters: args,
      queries: new Dictionary<string, string>() {
        ["FID_COND_MRKT_DIV_CODE"] = queries.Exchange.GetCode(),
        ["FID_INPUT_ISCD"] = queries.Ticker,
        ["FID_INPUT_DATE_1"] = queries.From.ToString("yyyyMMdd"),
        ["FID_INPUT_DATE_2"] = queries.To.ToString("yyyyMMdd"),
        ["FID_PERIOD_DIV_CODE"] = queries.CandlePeriod.GetCode(),
        ["FID_ORG_ADJ_PRC"] = queries.Adjusted ? "0" : "1",
      }
    );
}