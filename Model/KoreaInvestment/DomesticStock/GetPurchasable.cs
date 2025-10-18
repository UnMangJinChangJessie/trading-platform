using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class PurchasableQueries : IAccount {
    public string TransactionId => ApiClient.Simulation ? "VTTTC8908R" : "TTTC8908R";
    public required string AccountBase { get; set; }
    public required string AccountCode { get; set; }

    public required string Ticker { get; set; }
    public required decimal UnitPrice { get; set; }
    public required ulong Quantity { get; set; }
    public required OrderMethod OrderDivision { get; set; }
    public required bool IncludeCashManagementAccount { get; set; }
    public required bool IncludeForeignCash { get; set; }
  }

  public class PurchasableResult : KisReturnMessage {
    [JsonPropertyName("output")] public Purchasable? Result { get; set; }
  }
  public static readonly Action<PurchasableQueries, Action<string, bool, object?>?, object?> GetPurchasable = (queries, cb, args) =>
    ApiClient.PushRequest(
      queries.TransactionId,
      callback: cb,
      callbackParameters: args,
      queries: new Dictionary<string, string>() {
        ["CANO"] = queries.AccountBase,
        ["ACNT_PRDT_CD"] = queries.AccountCode,
        ["PDNO"] = queries.Ticker,
        ["ORD_UNPR"] = queries.UnitPrice.ToString(),
        ["ORD_DVSN"] = queries.OrderDivision.GetCode(),
        ["CMA_EVLU_AMT_ICLD_YN"] = queries.IncludeCashManagementAccount ? "Y" : "N",
        ["OVRS_ICLD_YN"] = queries.IncludeForeignCash ? "Y" : "N"
      }
    );
}