namespace trading_platform.KoreaInvestment;

using System.Text.Json.Serialization;

public interface IOrderResult {
  public string ExchangeCode { get; init; }
  public string OrderNumber { get; init; }
  public TimeOnly OrderTime { get; init; }
}

public class OrderResult {
  [JsonPropertyName("KRX_FWDG_ORD_ORGNO")]
  public required string ExchangeOrderNumber { get; init; }
  [JsonPropertyName("ODNO")]
  public required string OrderNumber { get; init; }
  [JsonPropertyName("ORD_TMD"), JsonConverter(typeof(TimeToStringConverter))]
  public required TimeOnly OrderTime { get; init; }
}