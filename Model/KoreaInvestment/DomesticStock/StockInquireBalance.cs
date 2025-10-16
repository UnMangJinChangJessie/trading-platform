using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;


public partial class DomesticStock {
  public class BalanceQueries : IAccount, IConsecutive {
    public const string PRICE_DEFAULT = "N";
    public const string PRICE_AFTER_MARKET = "Y";
    public const string PRICE_NEXTRADE = "X";

    public const string INQUIRY_LOAN_DATE = "01";
    public const string INQUIRY_TICKER = "02";

    public required string AccountBase { get; set; }
    public required string AccountCode { get; set; }
    public required string FirstConsecutiveContext { get; set; } = "";
    public required string SecondConsecutiveContext { get; set; } = "";

    public required string DisplayPrice { get; set; }
    public required string InquiryType { get; set; }
    public required bool IncludeFund { get; set; }
    public required bool IncludePreviousTrade { get; set; }
  }

  public class BalanceResult : KisReturnMessage, IReturnConsecutive {
    [JsonPropertyName("ctx_area_fk100")]
    public string? FirstConsecutiveContext { get; set; }
    [JsonPropertyName("ctx_area_nk100")]
    public string? SecondConsecutiveContext { get; set; }
    [JsonIgnore]
    public bool HasNextData { get; set; }
    [JsonPropertyName("output1")]
    public IEnumerable<BalanceItem>? HoldingStocks { get; set; }
    [JsonPropertyName("output2")]
    public IEnumerable<Balance>? AccountBalance { get; set; } // Is defined as array but pretty sure this is a singleton
  }
  public static Action<BalanceQueries, Action<string, object?>?, object?> GetBalance = (queries, cb, args) =>
    ApiClient.PushRequest(
      "TTTC8434R",
      queries: new Dictionary<string, string>() {
        ["CANO"] = queries.AccountBase,
        ["ACNT_PRDT_CD"] = queries.AccountCode,
        ["AFHR_FLPR_YN"] = queries.DisplayPrice,
        ["INQR_DVSN"] = queries.InquiryType,
        ["FUND_STTL_ICLD_YN"] = queries.IncludeFund ? "Y" : "N",
        ["PRCS_DVSN"] = queries.IncludePreviousTrade ? "00" : "01",
        ["CTX_AREA_FK100"] = queries.FirstConsecutiveContext,
        ["CTX_AREA_NK100"] = queries.SecondConsecutiveContext,
        ["OFL_YN"] = "",
        ["UNPR_DVSN"] = "01",
        ["FNCG_AMT_AUTO_RDPT_YN"] = "N",
      },
      callback: cb,
      callbackParameters: args,
      next: queries.FirstConsecutiveContext != "" && queries.SecondConsecutiveContext != ""
    );
}