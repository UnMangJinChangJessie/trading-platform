using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockOrderBook {
  [JsonPropertyName("pbid1")]
  public required decimal FirstBidPrice { get; init; }
  [JsonPropertyName("pask1")]
  public required decimal FirstAskPrice { get; init; }
  [JsonPropertyName("vbid1")]
  public required decimal FirstBidQuantity { get; init; }
  [JsonPropertyName("vask1")]
  public required decimal FirstAskQuantity { get; init; }
}