using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace TradingSystem.KoreaInvestment;

public class StockCreditOrderBody {
  [JsonIgnore] public required OrderType OrderType { get; set; }
  [JsonPropertyName("CANO")] public required string Account;
  [JsonPropertyName("ACNT_PRDT_CD")] public required string AccountCode;
  [JsonPropertyName("PDNO")] public required string Ticker { get; set; }
  [JsonPropertyName("CRDT_TYPE")] public required CreditType CreditType { get; set; }
  [JsonPropertyName("LOAN_DT"), JsonConverter(typeof(DateToStringConverter))] public required DateOnly LoanDate { get; set; }
  [JsonPropertyName("ORD_DVSN")] public required OrderDivision OrderDivision { get; set; }
  [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long Quantity { get; set; }
  [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long UnitPrice { get; set; }
  [JsonPropertyName("RSVN_ORD_YN"), JsonConverter(typeof(YesNoToBooleanConverter)), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public bool? Reservation { get; set; }
  [JsonPropertyName("EMGC_ORD_YN"), JsonConverter(typeof(YesNoToBooleanConverter)), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public bool? Emergency { get; set; }
  [JsonPropertyName("EXCG_ID_DVSN_CD"), JsonConverter(typeof(YesNoToBooleanConverter)), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public DomesticExchangeId? Exchange { get; set; }
  [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long StopLossLimit { get; set; }
}

public struct StockCreditOrderResult {
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
  public static async Task<StockCreditOrderResult> OrderStockCredit(StockCreditOrderBody body) {
    string tradeId = body.OrderType switch {
      OrderType.Buy => "TTTC0052U",
      OrderType.Sell => "TTTC0051U",
      _ => throw new ArgumentOutOfRangeException(nameof(body))
    };
    var result = await Request(tradeId, HttpMethod.Post, "/uapi/domestic-stock/v1/trading/order-credit", [], [], body);
    return await result.Content.ReadFromJsonAsync<StockCreditOrderResult>();
  }
}