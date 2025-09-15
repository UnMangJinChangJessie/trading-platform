using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public abstract class KisReturnMessage {
  [JsonPropertyName("rt_cd")]
  public required int ReturnCode { get; init; }
  [JsonPropertyName("msg_cd")]
  public required string ResponseMessageCode { get; init; }
  [JsonPropertyName("msg1")]
  public required string ResponseMessage { get; init; }
}