using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StockInquireModifiableQueries : IAccount, IConsecutive {
  public const int ORDER = 0;
  public const int TICKER = 1;
  public const int ALL = 0;
  public const int SELL = 1;
  public const int BUY = 2;
  
  public required string AccountBase { get; init; }
  public required string AccountCode { get; init; }
  public required string FirstConsecutiveContext { get; init; } = "";
  public required string SecondConsecutiveContext { get; init; } = "";
  public required int OrderOrTicker { get; init; }
  public required int SellOrBuy { get; init; }
}

public class StockInquireModifiableResult : KisReturnMessage, IReturnConsecutive {
  [JsonIgnore] public bool HasNextData { get; set; }
  [JsonPropertyName("ctx_area_fk100")] public string? FirstConsecutiveContext { get; init; }
  [JsonPropertyName("ctx_area_nk100")] public string? SecondConsecutiveContext { get; init; }

  [JsonPropertyName("output")] public IEnumerable<StockPendingOrder>? ModifiableList { get; init; }
}

public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquireModifiableResult? Result)> InquireStockModifiableOrder(StockInquireModifiableQueries body) {
    const string transId = "TTTC0084R";
    const string uri = "/uapi/domestic-stock/v1/trading/inquire-psbl-rvsecncl";
    return await ApiClient.RequestConsecutive<StockInquireModifiableQueries, StockInquireModifiableResult>(
      transId, HttpMethod.Get, uri,
      header: new Dictionary<string, string>() {
        ["tr_cont"] = body.FirstConsecutiveContext != "" ? "N" : "",
      },
      queries: new Dictionary<string, string>() {
        ["CANO"] = body.AccountBase,
        ["ACNT_PRDT_CD"] = body.AccountCode,
        ["CTX_AREA_FK100"] = body.FirstConsecutiveContext,
        ["CTX_AREA_NK100"] = body.SecondConsecutiveContext,
        ["INQR_DVSN_1"] = body.OrderOrTicker.ToString(),
        ["INQR_DVSN_2"] = body.SellOrBuy.ToString()
      },
      null
    );
  }
}