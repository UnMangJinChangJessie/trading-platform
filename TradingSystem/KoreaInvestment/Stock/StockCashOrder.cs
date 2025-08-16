using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TradingSystem.KoreaInvestment;

public class StockCashOrderBody {
  [JsonIgnore] public required OrderType Position { get; set; }
  [JsonPropertyName("CANO")] public required string Account;
  [JsonPropertyName("ACNT_PRDT_CD")] public required string AccountCode;
  [JsonPropertyName("PDNO")] public required string Ticker { get; set; }
  [JsonPropertyName("SLL_TYPE"),
  JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public SellingType? SellType { get; set; }
  [JsonPropertyName("ORD_DVSN")] public required OrderDivision Division { get; set; }
  [JsonPropertyName("ORD_QTY"),
  JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required int Quantity { get; set; }
  [JsonPropertyName("ORD_UNPR"),
  JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required int Price { get; set; }
  [JsonPropertyName("CNDT_PRIC"),
  JsonNumberHandling(JsonNumberHandling.WriteAsString),
  JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public int? StopLossLimit { get; set; }
  [JsonPropertyName("EXCG_ID_DVSN_CD")] public DomesticExchangeId? Exchange { get; set; }
}
public struct StockCashOrderResult {
  [JsonPropertyName("rt_cd"), JsonConverter(typeof(YesNoToBooleanConverter))] public bool Success { get; set; }
  [JsonPropertyName("msg1")] public string Message { get; set; }
  [JsonPropertyName("msg_cd")] public string MessageCode { get; set; }
  public class OrderResult {
    [JsonPropertyName("KRX_FWDG_ORD_ORGNO")] public required string ExchangeCode { get; set; }
    [JsonPropertyName("ODNO")] public required string OrderNumber { get; set; }
    [JsonPropertyName("ORD_TMD"), JsonConverter(typeof(TimeToStringConverter))] public required TimeOnly OrderTime { get; set; }
  }
  [JsonPropertyName("output")] public OrderResult Response { get; set; }
}
public static partial class ApiClient {
  public static async Task<StockCashOrderResult> OrderStockCash(StockCashOrderBody body) {
    string tradeId = body.Position switch {
      OrderType.Buy => Simulation ? "VTTC0012U" : "TTTC0012U",
      OrderType.Sell => Simulation ? "VTTC0011U" : "TTTC0011U",
      _ => throw new ArgumentOutOfRangeException(nameof(body))
    };
    var result = await Request(tradeId, HttpMethod.Post, "/uapi/domestic-stock/v1/trading/order-cash", [], [], body);
    return await result.Content.ReadFromJsonAsync<StockCashOrderResult>();
  }
}