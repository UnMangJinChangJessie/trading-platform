using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockOrderBookInformation {
  [JsonPropertyName("last")]
  public required decimal CurrentClose { get; init; }
  [JsonPropertyName("base")]
  public required decimal PreviousClose { get; init; }
  [JsonPropertyName("dymd")]
  public required DateOnly CurrentDate { get; init; }
  [JsonPropertyName("dhms")]
  public required TimeOnly CurrentTime { get; init; }
}