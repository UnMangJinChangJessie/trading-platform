using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class ChartQueries {
    public required Exchange Exchange { get; set; }
    public required string Ticker { get; set; }
    public required DateOnly EndDate { get; set; }
    public required CandlePeriod CandlePeriod { get; set; }
    public required bool Adjusted { get; set; }
  }
  public class ChartResult : KisReturnMessage {
    [JsonPropertyName("output2")]
    public IEnumerable<ChartItem>? Chart { get; set; }
  }
  public readonly static Action<ChartQueries, Action<string>?> GetChart = (queries, callback) =>
    ApiClient.PushRequest(
      transId: "HHDFS76240000",
      callback: callback,
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
      }
    );
}