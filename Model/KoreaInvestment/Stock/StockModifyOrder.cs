using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public struct StockModifyOrderBody : IAccount {
  [JsonPropertyName("CANO")]
  public required string AccountBase { get; init; }
  [JsonPropertyName("ACNT_PRDT_CD")]
  public required string AccountCode { get; init; }

  [JsonPropertyName("RVSE_CNCL_DVSN_CD")] public required Modification ModificationType { get; init; }
  [JsonPropertyName("KRX_FWDG_ORD_ORGNO")] public required string OrganizationNumber { get; init; }
  [JsonPropertyName("ORGN_ODNO")] public required string OrderNumber { get; init; }
  [JsonPropertyName("QTY_ALL_ORD_YN")] public required bool ModifyEntirely { get; init; }

  [JsonPropertyName("ORD_DVSN")] public required OrderMethod OrderDivision { get; init; }
  [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required decimal Quantity { get; init; }
  [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required ulong UnitPrice { get; init; }
  [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public decimal? StopLossLimit { get; init; }

  [JsonPropertyName("EXCG_ID_DVSN_CD")] public DomesticOrderRoute? Exchange { get; init; }
}
public class StockModifyOrderResult : KisReturnMessage {
  [JsonPropertyName("output")]
  public OrderResult? Response { get; init; }
}

public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockModifyOrderResult? Result)> ModifyStockOrder(StockModifyOrderBody body) {
    string transId = ApiClient.Simulation ? "VTTC0013U" : "TTTC0013U";
    const string uri = "/uapi/domestic-stock/v1/trading/order-rvsecncl";
    return await ApiClient.Request<StockModifyOrderBody, StockModifyOrderResult>(transId, HttpMethod.Post, uri, null, null, body);
  }
}