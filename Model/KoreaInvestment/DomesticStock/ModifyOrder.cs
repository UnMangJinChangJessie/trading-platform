using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class ModifyOrderBody : IAccount {
    [JsonIgnore]
    public string TransactionId => ApiClient.Simulation ? "VTTC0013U" : "TTTC0013U";
    [JsonPropertyName("CANO")]
    public required string AccountBase { get; set; }
    [JsonPropertyName("ACNT_PRDT_CD")]
    public required string AccountCode { get; set; }

    [JsonPropertyName("RVSE_CNCL_DVSN_CD")] public required Modification ModificationType { get; set; }
    [JsonPropertyName("KRX_FWDG_ORD_ORGNO")] public required string OrganizationNumber { get; set; }
    [JsonPropertyName("ORGN_ODNO")] public required string OrderNumber { get; set; }
    [JsonPropertyName("QTY_ALL_ORD_YN")] public required bool ModifyEntirely { get; set; }

    [JsonPropertyName("ORD_DVSN")] public required OrderMethod OrderDivision { get; set; }
    [JsonPropertyName("ORD_QTY")] public required decimal Quantity { get; set; }
    [JsonPropertyName("ORD_UNPR")] public required ulong UnitPrice { get; set; }
    [JsonPropertyName("CNDT_PRIC"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public decimal? StopLossLimit { get; set; }

    [JsonPropertyName("EXCG_ID_DVSN_CD")] public DomesticOrderRoute? Exchange { get; set; }
  }
  public class ModifyOrderResult : KisReturnMessage {
    [JsonPropertyName("output")]
    public OrderInformation? Response { get; set; }
  }
  public static readonly Action<ModifyOrderBody, Action<string, bool, object?>?, object?> ModifyOrder = (body, cb, args) => 
    ApiClient.PushRequest(body.TransactionId, callback: cb, callbackParameters: args, body: body);
}