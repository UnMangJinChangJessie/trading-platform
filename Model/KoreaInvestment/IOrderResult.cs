namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public interface IOrderResult {
  public string ExchangeCode { get; set; }
  public string OrderNumber { get; set; }
  public TimeOnly OrderTime { get; set; }
}

public class OrderInformation {
  [JsonPropertyName("KRX_FWDG_ORD_ORGNO")]
  public required string ExchangeOrderNumber { get; set; }
  [JsonPropertyName("ODNO")]
  public required string OrderNumber { get; set; }
  [JsonPropertyName("ORD_TMD"), JsonConverter(typeof(TimeToStringConverter))]
  public required TimeOnly OrderTime { get; set; }
}