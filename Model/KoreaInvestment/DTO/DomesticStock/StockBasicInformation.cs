namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public static partial class DomesticStock {
  public class BasicInformation {
    [JsonPropertyName("hts_kor_isnm")]
    public required string Name { get; set; }
    [JsonPropertyName("stck_shrn_iscd")]
    public required string Ticker { get; set; }
    [JsonPropertyName("prdy_vrss_sign")]
    public required PriceChangeSign PriceChangeSign;
    [JsonPropertyName("stck_mxpr")]
    public required decimal UpperLimit { get; set; }
    [JsonPropertyName("stck_llam")]
    public required decimal LowerLimit { get; set; }
    [JsonPropertyName("stck_oprc")]
    public required decimal CurrentOpen { get; set; }
    [JsonPropertyName("stck_hgpr")]
    public required decimal CurrentHigh { get; set; }
    [JsonPropertyName("stck_lwpr")]
    public required decimal CurrentLow { get; set; }
    [JsonPropertyName("stck_prpr")]
    public required decimal CurrentClose { get; set; }
    [JsonPropertyName("acml_vol")]
    public required decimal CurrentVolume { get; set; }
    [JsonPropertyName("acml_tr_pbmn")]
    public required decimal CurrentAmount { get; set; }
    [JsonPropertyName("stck_prdy_oprc")]
    public required decimal PreviousOpen { get; set; }
    [JsonPropertyName("stck_prdy_hgpr")]
    public required decimal PreviousHigh { get; set; }
    [JsonPropertyName("stck_prdy_lwpr")]
    public required decimal PreviousLow { get; set; }
    [JsonPropertyName("stck_prdy_clpr")]
    public required decimal PreviousClose { get; set; }
    [JsonPropertyName("prdy_vol")]
    public required decimal PreviousVolume { get; set; }
    [JsonPropertyName("vol_tnrt")]
    public required float VolumeTurningRate { get; set; }
    [JsonPropertyName("stck_fcam")]
    public required decimal FaceValue { get; set; }
    [JsonPropertyName("lstn_stcn")]
    public required decimal ListedStocks { get; set; }
    [JsonPropertyName("cpfn")]
    public required decimal Foundation { get; set; }
    public decimal MarketCapitalization => ListedStocks * CurrentClose;
    [JsonPropertyName("per")]
    public required float PriceEarningsRate { get; set; }
    [JsonPropertyName("eps")]
    public required float EarningsPerShare { get; set; }
    [JsonPropertyName("pbr")]
    public required float PriceBookValueRatio { get; set; }
  }
}