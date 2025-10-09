using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public abstract class KisReturnMessage {
  [JsonPropertyName("rt_cd")]
  public required int ReturnCode { get; set; }
  [JsonPropertyName("msg_cd")]
  public required string ResponseMessageCode { get; set; }
  [JsonPropertyName("msg1")]
  public required string ResponseMessage { get; set; }
}