using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockCashOrderBody : IAccount, IOrder {
  [JsonPropertyName("CANO")]
  public required string AccountBase { get; init; }
  [JsonPropertyName("ACNT_PRDT_CD")]
  public required string AccountCode { get; init; }
  [JsonIgnore]
  public OrderPosition Position { get; init; }
  [JsonPropertyName("PDNO")]
  public required string Ticker { get; init; }
  [JsonPropertyName("ORD_DVSN")]
  public OrderMethod OrderDivision { get; init; }
  [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
  public decimal UnitPrice { get; init; }
  [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
  public ulong Quantity { get; init; }
  [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public decimal? StopLossLimit { get; init; }
  [JsonPropertyName("SLL_TYPE"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public OrderSelling? SellType { get; init; }
  [JsonPropertyName("EXCG_ID_DVSN_CD")]
  public DomesticOrderRoute? Exchange { get; init; }
}
public class StockCashOrderResult : KisReturnMessage {
  [JsonPropertyName("output")] public OrderResult? Response { get; init; }
}
public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockCashOrderResult? Result)> OrderCash(StockCashOrderBody body) {
    string transId = body.Position switch {
      OrderPosition.Buy => ApiClient.Simulation ? "VTTC0012U" : "TTTC0012U",
      OrderPosition.Sell => ApiClient.Simulation ? "VTTC0011U" : "TTTC0011U",
      _ => throw new ArgumentOutOfRangeException(nameof(body))
    };
    const string uri = "/uapi/domestic-stock/v1/trading/order-cash";
    return await ApiClient.Request<StockCashOrderBody, StockCashOrderResult>(transId, HttpMethod.Post, uri, null, null, body);
  }
}