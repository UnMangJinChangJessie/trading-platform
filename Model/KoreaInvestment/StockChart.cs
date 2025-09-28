using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockChart {
  [JsonPropertyName("stck_bsop_date"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly N { get; init; }
  [JsonPropertyName("stck_clpr")]
  public required decimal Close { get; init; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; init; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; init; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; init; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; init; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required decimal Amount { get; init; }
  [JsonPropertyName("flng_cls_code")]
  public required LockType LockDivision { get; init; }
  [JsonPropertyName("prtt_rate")]
  public required float PartitionRate { get; init; }
  [JsonPropertyName("mod_yn")]
  public required bool Modified { get; init; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; init; }
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; init; }
  // [JsonPropertyName("revl_issu_reas")]
  // public required T ReevaluationReason { get; init; }
}