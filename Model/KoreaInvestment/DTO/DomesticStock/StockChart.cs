using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class ChartItem {
    [JsonPropertyName("stck_bsop_date")]
    public required DateOnly Date { get; set; }
    [JsonPropertyName("prdy_vrss_sign")]
    public required PriceChangeSign PriceChangeSign { get; set; }
    [JsonPropertyName("stck_oprc")]
    public required ulong Open { get; set; }
    [JsonPropertyName("stck_hgpr")]
    public required ulong High { get; set; }
    [JsonPropertyName("stck_lwpr")]
    public required ulong Low { get; set; }
    [JsonPropertyName("stck_clpr")]
    public required ulong Close { get; set; }
    [JsonPropertyName("prdy_vrss")]
    public required long PriceChange { get; set; }
    [JsonPropertyName("acml_vol")]
    public required ulong Volume { get; set; }
    [JsonPropertyName("acml_tr_pbmn")]
    public required ulong Amount { get; set; }
  }
}