using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockChart {
  [JsonPropertyName("xymd"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly Date { get; set; }
  [JsonPropertyName("clos")]
  public required decimal Close { get; set; }
  [JsonPropertyName("open")]
  public required decimal Open { get; set; }
  [JsonPropertyName("high")]
  public required decimal High { get; set; }
  [JsonPropertyName("low")]
  public required decimal Low { get; set; }
  [JsonPropertyName("tvol")]
  public required decimal Volume { get; set; }
  [JsonPropertyName("tamt")]
  public required decimal Amount { get; set; }
  [JsonPropertyName("sign")]
  public required PriceChangeSign PriceChangeSign { get; set; }
  [JsonPropertyName("diff")]
  public required decimal PriceChange { get; set; }
}