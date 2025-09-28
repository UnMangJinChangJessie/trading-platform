using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockCreditOrderBody : IAccount, IOrder, ICredit {
  [JsonPropertyName("CANO")]
  public required string AccountBase { get; init; }
  [JsonPropertyName("ACNT_PRDT_CD")]
  public required string AccountCode { get; init; }

  [JsonIgnore]
  public required OrderPosition Position { get; init; }
  [JsonPropertyName("PDNO")]
  public required string Ticker { get; init; }
  [JsonPropertyName("ORD_DVSN")]
  public required OrderMethod OrderDivision { get; init; }
  [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
  public required decimal UnitPrice { get; init; }
  [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
  public required ulong Quantity { get; init; }
  [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
  public required decimal? StopLossLimit { get; init; }
  
  [JsonPropertyName("CRDT_TYPE")]
  public required OrderCredit CreditType { get; init; }
  [JsonPropertyName("LOAN_DT"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly LoanDate { get; init; }
  
  [JsonPropertyName("EXCG_ID_DVSN_CD"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public DomesticOrderRoute? Exchange { get; init; }
}

public class StockCreditOrderResult : KisReturnMessage {
  [JsonPropertyName("output")]
  public OrderResult? Response { get; init; }
}

public static partial class DomesticStock {
  public static async Task<(HttpStatusCode StatusCode, StockCreditOrderResult? Result)> OrderStockCredit(StockCreditOrderBody body) {
    string transId = body.Position switch {
      OrderPosition.Buy => "TTTC0052U",
      OrderPosition.Sell => "TTTC0051U",
      _ => throw new ArgumentOutOfRangeException(nameof(body))
    };
    const string uri = "/uapi/domestic-stock/v1/trading/order-credit";
    return await ApiClient.Request<StockCreditOrderBody, StockCreditOrderResult>(transId, HttpMethod.Post, uri, null, null, body);
  }
}