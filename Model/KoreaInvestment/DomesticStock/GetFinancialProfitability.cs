using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialProfitabilityQueries {
    public const int YEARLY = 0;
    public const int QUARTERLY = 1;
    public required int Period { get; set; }
    public required string Ticker { get; set; }
  }
  public class FinancialProfitabilityResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public IEnumerable<FinancialProfitabilityItem>? Output { get; set; }
  }
  public readonly static Action<ApiModel, FinancialProfitabilityQueries, Action<string, bool, object?>, object?> GetFinancialProfitability = (api, queries, cb, args) =>
    api.PushRequest(
      transId: "FHKST66430400",
      queries: new Dictionary<string, string>() {
        ["FID_DIV_CLS_CODE"] = queries.Period.ToString(),
        ["fid_cond_mrkt_div_code"] = "J",
        ["fid_input_iscd"] = queries.Ticker
      },
      callback: cb, callbackParameters: args
    );
}