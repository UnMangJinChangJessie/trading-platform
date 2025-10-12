using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class CashOrderBody : IAccount {
    [JsonIgnore]
    public OrderPosition Position { get; set; }
    [JsonIgnore]
    public string TransactionId => Position switch {
      OrderPosition.Long => ApiClient.Simulation ? "VTTC0012U" : "TTTC0012U",
      OrderPosition.Short => ApiClient.Simulation ? "VTTC0011U" : "TTTC0011U",
      _ => throw new ArgumentOutOfRangeException(nameof(Position))
    };
    [JsonPropertyName("CANO")]
    public required string AccountBase { get; set; }
    [JsonPropertyName("ACNT_PRDT_CD")]
    public required string AccountCode { get; set; }
    [JsonPropertyName("PDNO")]
    public required string Ticker { get; set; }
    [JsonPropertyName("ORD_DVSN")]
    public OrderMethod Method { get; set; }
    [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public ulong UnitPrice { get; set; }
    [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public ulong Quantity { get; set; }
    [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public ulong? StopLossLimit { get; set; }
    [JsonPropertyName("SLL_TYPE"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public OrderSelling? SellType { get; set; }
    [JsonPropertyName("EXCG_ID_DVSN_CD")]
    public DomesticOrderRoute? Exchange { get; set; }
  }
  public class CashOrderResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public OrderInformation? Response { get; set; }
  }
  public static readonly Action<CashOrderBody, Action<string>?> OrderCash = (body, callback) =>
    ApiClient.PushRequest(body.TransactionId, callback: callback, body: body);
}