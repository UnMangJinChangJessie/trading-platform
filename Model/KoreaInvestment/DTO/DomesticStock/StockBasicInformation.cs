namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public class StockBasicInformation {
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; set; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign;
  [JsonPropertyName("prdy_ctrt")]
  public required float ChangeRate { get; set; }
  [JsonPropertyName("stck_prdy_clpr")]
  public required decimal PreviousClose { get; set; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; set; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required decimal Amount { get; set; }
  [JsonPropertyName("hts_kor_isnm")]
  public required string Name { get; set; }
  [JsonPropertyName("stck_prpr")]
  public required decimal Close { get; set; }
  [JsonPropertyName("stck_shrn_iscd")]
  public required string Ticker { get; set; }
  [JsonPropertyName("prdy_vol")]
  public required decimal PreviousVolume { get; set; }
  [JsonPropertyName("stck_mxpr")]
  public required decimal UpperLimit { get; set; }
  [JsonPropertyName("stck_llam")]
  public required decimal LowerLimit { get; set; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; set; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; set; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; set; }
  [JsonPropertyName("stck_prdy_oprc")]
  public required decimal PreviousOpen { get; set; }
  [JsonPropertyName("stck_prdy_hgpr")]
  public required decimal PreviousHigh { get; set; }
  [JsonPropertyName("stck_prdy_lwpr")]
  public required decimal PreviousLow { get; set; }
  [JsonPropertyName("askp")]
  public required decimal AskingPrice { get; set; }
  [JsonPropertyName("bidp")]
  public required decimal BiddingPrice { get; set; }
  [JsonPropertyName("prdy_vrss_vol")]
  public required float VolumeChangeRate { get; set; }
  [JsonPropertyName("vol_tnrt")]
  public required float VolumeTurningRate { get; set; }
  [JsonPropertyName("stck_fcam")]
  public required decimal FaceValue { get; set; }
  [JsonPropertyName("lstn_stcn")]
  public required decimal ListedStocks { get; set; }
  [JsonPropertyName("cpfn")]
  public required decimal Foundation { get; set; }
  public decimal MarketCapitalization => ListedStocks * Close;
  [JsonPropertyName("per")]
  public required float PriceEarningsRate { get; set; }
  [JsonPropertyName("eps")]
  public required float EarningsPerShare { get; set; }
  [JsonPropertyName("pbr")]
  public required float PriceBookValueRatio{ get; set; }
  [JsonPropertyName("itewhol_loan_rmnd_ratem name")]
  public required float LoanRemainderRate { get; set; }
}