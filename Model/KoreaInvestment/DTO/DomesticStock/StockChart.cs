using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockChart {
  [JsonPropertyName("stck_bsop_date"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly Date { get; set; }
  [JsonPropertyName("stck_clpr")]
  public required decimal Close { get; set; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; set; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; set; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; set; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; set; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required decimal Amount { get; set; }
  [JsonPropertyName("flng_cls_code")]
  public required LockType LockDivision { get; set; }
  [JsonPropertyName("prtt_rate")]
  public required float PartitionRate { get; set; }
  [JsonPropertyName("mod_yn")]
  public required bool Modified { get; set; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; set; }
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; set; }
  // [JsonPropertyName("revl_issu_reas")]
  // public required T ReevaluationReason { get; set; }
}