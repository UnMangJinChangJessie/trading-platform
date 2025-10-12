using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class OrderBookInformation {
    [JsonPropertyName("stck_prpr")]
    public required ulong CurrentClose { get; set; }
    [JsonPropertyName("stck_oprc")]
    public required ulong CurrentOpen { get; set; }
    [JsonPropertyName("stck_hgpr")]
    public required ulong CurrentHigh { get; set; }
    [JsonPropertyName("stck_lwpr")]
    public required ulong CurrentLow { get; set; }
    [JsonPropertyName("stck_sdpr")]
    public required ulong PreviousClose { get; set; }
  }
}