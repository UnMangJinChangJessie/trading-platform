using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StockInquirePurchasableBody : IAccount {
  public required string AccountBase { get; init; }
  public required string AccountCode { get; init; }

  public required string Ticker { get; init; }
  public required decimal UnitPrice { get; init; }
  public required ulong Quantity { get; init; }
  public required OrderMethod OrderDivision { get; init; }
  public required bool IncludeCashManagementAccount { get; init; }
  public required bool IncludeForeignCash { get; init; }
}

public class StockInquirePurchasableResult : KisReturnMessage {
  [JsonPropertyName("output")] public StockPurchasable? Result { get; init; }
}

public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockInquirePurchasableResult? Result)> InquireStockPurchasable(StockInquirePurchasableBody body) =>
    await ApiClient.Request<StockInquirePurchasableBody, StockInquirePurchasableResult>(
      transId: ApiClient.Simulation ? "VTTTC8908R" : "TTTC8908R",
      method: HttpMethod.Get,
      relUri: "/uapi/domestic-stock/v1/trading/inquire-psbl-order",
      header: null,
      queries: new Dictionary<string, string>() {
        ["CANO"] = body.AccountBase,
        ["ACNT_PRDT_CD"] = body.AccountCode,
        ["PDNO"] = body.Ticker,
        ["ORD_UNPR"] = body.UnitPrice.ToString(),
        ["ORD_DVSN"] = body.OrderDivision.GetCode(),
        ["CMA_EVLU_AMT_ICLD_YN"] = body.IncludeCashManagementAccount ? "Y" : "N",
        ["OVRS_ICLD_YN"] = body.IncludeForeignCash ? "Y" : "N"
      },
      body: null
    );
}