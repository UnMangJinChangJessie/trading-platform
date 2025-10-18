using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class GetModifiableQueries : IAccount, IConsecutive {
    public const int ORDER = 0;
    public const int TICKER = 1;
    public const int ALL = 0;
    public const int SELL = 1;
    public const int BUY = 2;
    
    public required string AccountBase { get; set; }
    public required string AccountCode { get; set; }
    public required string FirstConsecutiveContext { get; set; } = "";
    public required string SecondConsecutiveContext { get; set; } = "";
    public required int OrderOrTicker { get; set; }
    public required int SellOrBuy { get; set; }
  }

  public class GetModifiableResult : KisReturnMessage, IReturnConsecutive {
    [JsonIgnore] public bool HasNextData { get; set; }
    [JsonPropertyName("ctx_area_fk100")] public string? FirstConsecutiveContext { get; set; }
    [JsonPropertyName("ctx_area_nk100")] public string? SecondConsecutiveContext { get; set; }

    [JsonPropertyName("output")] public IEnumerable<PendingOrder>? ModifiableList { get; set; }
  }
  public static Action<GetModifiableQueries, Action<string, bool, object?>?, object?> GetModifiableOrder = (queries, cb, args) =>
    ApiClient.PushRequest(
      "TTTC0084R",
      queries: new Dictionary<string, string>() {
        ["CANO"] = queries.AccountBase,
        ["ACNT_PRDT_CD"] = queries.AccountCode,
        ["CTX_AREA_FK100"] = queries.FirstConsecutiveContext,
        ["CTX_AREA_NK100"] = queries.SecondConsecutiveContext,
        ["INQR_DVSN_1"] = queries.OrderOrTicker.ToString(),
        ["INQR_DVSN_2"] = queries.SellOrBuy.ToString()
      },
      callback: cb,
      callbackParameters: args,
      next: queries.FirstConsecutiveContext != "" && queries.SecondConsecutiveContext != ""
    );
}