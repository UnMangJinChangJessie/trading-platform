using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockChart {
  [JsonPropertyName("xymd"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly Date { get; init; }
  [JsonPropertyName("clos")]
  public required decimal Close { get; init; }
  [JsonPropertyName("open")]
  public required decimal Open { get; init; }
  [JsonPropertyName("high")]
  public required decimal High { get; init; }
  [JsonPropertyName("low")]
  public required decimal Low { get; init; }
  [JsonPropertyName("tvol")]
  public required decimal Volume { get; init; }
  [JsonPropertyName("tamt")]
  public required decimal Amount { get; init; }
  [JsonPropertyName("sign")]
  public required PriceChangeSign PriceChangeSign { get; init; }
  [JsonPropertyName("diff")]
  public required decimal PriceChange { get; init; }
}