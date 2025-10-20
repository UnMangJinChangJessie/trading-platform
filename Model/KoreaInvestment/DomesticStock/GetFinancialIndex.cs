using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialIndexQueries {
    public const int YEARLY = 0;
    public const int QUARTERLY = 1;
    public required int Period { get; set; }
    public required string Ticker { get; set; }
  }
  public class FinancialIndexResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public IEnumerable<FinancialIndex>? Output { get; set; }
  }
  public readonly static Action<ApiModel, FinancialIndexQueries, Action<string, bool, object?>, object?> GetFinancialIndex = (api, queries, cb, args) =>
    api.PushRequest(
      transId: "FHKST66430300",
      queries: new Dictionary<string, string>() {
        ["FID_DIV_CLS_CODE"] = queries.Period.ToString(),
        ["fid_cond_mrkt_div_code"] = "J",
        ["fid_input_iscd"] = queries.Ticker
      },
      callback: cb, callbackParameters: args
    );
}