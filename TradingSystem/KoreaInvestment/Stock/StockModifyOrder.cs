using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TradingSystem.KoreaInvestment;

public struct StockModifyOrderBody {
  [JsonPropertyName("CANO")] public required string Account { get; set; }
  [JsonPropertyName("ACNT_PRDT_CD")] public required string AccountCode { get; set; }
  [JsonPropertyName("KRX_FWDG_ORD_ORGNO")] public required string OrganizationNumber { get; set; }
  [JsonPropertyName("ORGN_ODNO")] public required string OrderNumber { get; set; }
  [JsonPropertyName("ORD_DVSN")] public required OrderDivision OrderDivision { get; set; }
  [JsonPropertyName("RVSE_CNCL_DVSN_CD")] public required Modification ModificationType { get; set; }
  [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long Quantity { get; set; }
  [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long UnitPrice { get; set; }
  [JsonPropertyName("QTY_ALL_ORD_YN"), JsonConverter(typeof(YesNoToBooleanConverter))] public required bool ModifyEntirely { get; set; }
  [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public long? StopLossLimit { get; set; }
  [JsonPropertyName("EXCG_ID_DVSN_CD")] public Exchange? Exchange { get; set; }
}
public struct StockModifyOrderResult {
  [JsonPropertyName("rt_cd"), JsonConverter(typeof(YesNoToBooleanConverter))] public required bool Success { get; set; }
  [JsonPropertyName("msg_cd")] public required string MessageCode { get; set; }
  [JsonPropertyName("msg1")] public required string Message { get; set; }
  public struct OrderResult {
    [JsonPropertyName("krx_fwdg_ord_orgno")] public required string OrganizationNumber { get; set; }
    [JsonPropertyName("odno")] public required string OrderNumber { get; set; }
    [JsonPropertyName("ord_tmd"), JsonConverter(typeof(TimeToStringConverter))] public required TimeOnly OrderTime { get; set; }
  }
}

public static partial class ApiClient {
  public static async Task<StockModifyOrderResult> ModifyStockOrder(StockModifyOrderBody body) {
    string tradeId = Simulation ? "VTTC0013U" : "TTTC0013U";
    if (!Enumerable.Contains([Exchange.KoreaExchange, Exchange.NexTrade, Exchange.DomesticUnified], body.Exchange)) {
      body.Exchange = Exchange.KoreaExchange;
    }
    var result = await Request(tradeId, HttpMethod.Post, "/uapi/domestic-stock/v1/trading/order-rvsecncl", [], [], body);
    return await result.Content.ReadFromJsonAsync<StockModifyOrderResult>();
  }
}