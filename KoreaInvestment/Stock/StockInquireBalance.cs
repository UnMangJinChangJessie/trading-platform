using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StockInquireBalanceQueries : IAccount, IConsecutive {
  public const string PRICE_DEFAULT = "N";
  public const string PRICE_AFTER_MARKET = "Y";
  public const string PRICE_NEXTRADE = "X";

  public const string INQUIRY_LOAN_DATE = "01";
  public const string INQUIRY_TICKER = "02";

  public required string AccountBase { get; init; }
  public required string AccountCode { get; init; }
  public required string FirstConsecutiveContext { get; init; } = "";
  public required string SecondConsecutiveContext { get; init; } = "";

  public required string DisplayPrice { get; init; }
  public required string InquiryType { get; init; }
  public required bool IncludeFund { get; init; }
  public required bool IncludeYesterdayTrade { get; init; }
}

public class StockInquireBalanceResult : KisReturnMessage, IReturnConsecutive {
  [JsonPropertyName("ctx_area_fk100")] public string? FirstConsecutiveContext { get; init; }
  [JsonPropertyName("ctx_area_nk100")] public string? SecondConsecutiveContext { get; init; }
  [JsonIgnore] public bool HasNextData { get; set; }
  [JsonPropertyName("output1")] public IEnumerable<StockBalance>? HoldingStocks { get; init; }
  // Is defined as array but pretty sure this is a singleton
  [JsonPropertyName("output2")] public IEnumerable<AccountBalance>? AccountBalance { get; init; }
}

public partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquireBalanceResult? Result)> InquireStockBalance(StockInquireBalanceQueries body) {
    string transId = ApiClient.Simulation ? "VTTTC8434R" : "TTTC8434R";
    const string uri = "/uapi/domestic-stock/v1/trading/inquire-psbl-rvsecncl";
    return await ApiClient.RequestConsecutive<StockInquireBalanceQueries, StockInquireBalanceResult>(
      transId, HttpMethod.Get, uri,
      header: new Dictionary<string, string>() {
        ["tr_cont"] = body.FirstConsecutiveContext != "" ? "N" : "",
      },
      queries: new Dictionary<string, string>() {
        ["CANO"] = body.AccountBase,
        ["ACNT_PRDT_CD"] = body.AccountCode,
        ["AFHR_FLPR_YN"] = body.DisplayPrice,
        ["INQR_DVSN"] = body.InquiryType,
        ["FUND_STTL_ICLD_YN"] = body.IncludeFund ? "Y" : "N",
        ["PRCS_DVSN"] = body.IncludeYesterdayTrade ? "00" : "01",
        ["CTX_AREA_FK100"] = body.FirstConsecutiveContext,
        ["CTX_AREA_NK100"] = body.SecondConsecutiveContext,
        ["OFL_YN"] = "",
        ["UNPR_DVSN"] = "01",
        ["FNCG_AMT_AUTO_RDPT_YN"] = "N",
      },
      null
    );
  }
}