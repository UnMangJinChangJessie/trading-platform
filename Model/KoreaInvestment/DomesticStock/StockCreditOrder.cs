using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;


public static partial class DomesticStock {
  public class CreditOrderBody : IAccount {
    [JsonIgnore]
    public OrderPosition Position { get; set; }
    [JsonIgnore]
    public string TransactionId => Position switch {
      OrderPosition.Long => "TTTC0052U",
      OrderPosition.Short => "TTTC0051U",
      _ => throw new ArgumentOutOfRangeException(nameof(Position))
    };
    
    [JsonPropertyName("CANO")]
    public required string AccountBase { get; set; }
    [JsonPropertyName("ACNT_PRDT_CD")]
    public required string AccountCode { get; set; }

    [JsonPropertyName("PDNO")]
    public required string Ticker { get; set; }
    [JsonPropertyName("ORD_DVSN")]
    public required OrderMethod OrderDivision { get; set; }
    [JsonPropertyName("ORD_UNPR"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required decimal UnitPrice { get; set; }
    [JsonPropertyName("ORD_QTY"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required ulong Quantity { get; set; }
    [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)]
    public required decimal? StopLossLimit { get; set; }
    
    [JsonPropertyName("CRDT_TYPE")]
    public required OrderCredit CreditType { get; set; }
    [JsonPropertyName("LOAN_DT"), JsonConverter(typeof(DateToStringConverter))]
    public required DateOnly LoanDate { get; set; }
    
    [JsonPropertyName("EXCG_ID_DVSN_CD"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public DomesticOrderRoute? Exchange { get; set; }
  }

  public class CreditOrderResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public OrderInformation? Response { get; set; }
  }
  public static readonly Action<CreditOrderBody, Action<string, object?>?, object?> OrderCredit = (body, cb, args) =>
    ApiClient.PushRequest(body.TransactionId, callback: cb, callbackParameters: args, body: body);
}