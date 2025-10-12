using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class OrderBookInformation {
    [JsonPropertyName("last")]
    public required decimal CurrentClose { get; set; }
    [JsonPropertyName("base")]
    public required decimal PreviousClose { get; set; }
    [JsonPropertyName("dymd")]
    public required DateOnly CurrentDate { get; set; }
    [JsonPropertyName("dhms")]
    public required TimeOnly CurrentTime { get; set; }
  }
}