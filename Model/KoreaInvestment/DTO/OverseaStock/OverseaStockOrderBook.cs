using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class OrderBook {
    [JsonPropertyName("pbid1")]
    public required decimal FirstBidPrice { get; set; }
    [JsonPropertyName("pask1")]
    public required decimal FirstAskPrice { get; set; }
    [JsonPropertyName("vbid1")]
    public required decimal FirstBidQuantity { get; set; }
    [JsonPropertyName("vask1")]
    public required decimal FirstAskQuantity { get; set; }
  }
}