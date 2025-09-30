namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public class StockBasicInformation {
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; init; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign;
  [JsonPropertyName("prdy_ctrt")]
  public required float ChangeRate { get; init; }
  [JsonPropertyName("stck_prdy_clpr")]
  public required decimal PreviousClose { get; init; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; init; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required decimal Amount { get; init; }
  [JsonPropertyName("hts_kor_isnm")]
  public required string Name { get; init; }
  [JsonPropertyName("stck_prpr")]
  public required decimal Close { get; init; }
  [JsonPropertyName("stck_shrn_iscd")]
  public required string Ticker { get; init; }
  [JsonPropertyName("prdy_vol")]
  public required decimal PreviousVolume { get; init; }
  [JsonPropertyName("stck_mxpr")]
  public required decimal UpperLimit { get; init; }
  [JsonPropertyName("stck_llam")]
  public required decimal LowerLimit { get; init; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; init; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; init; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; init; }
  [JsonPropertyName("stck_prdy_oprc")]
  public required decimal PreviousOpen { get; init; }
  [JsonPropertyName("stck_prdy_hgpr")]
  public required decimal PreviousHigh { get; init; }
  [JsonPropertyName("stck_prdy_lwpr")]
  public required decimal PreviousLow { get; init; }
  [JsonPropertyName("askp")]
  public required decimal AskingPrice { get; init; }
  [JsonPropertyName("bidp")]
  public required decimal BiddingPrice { get; init; }
  [JsonPropertyName("prdy_vrss_vol")]
  public required float VolumeChangeRate { get; init; }
  [JsonPropertyName("vol_tnrt")]
  public required float VolumeTurningRate { get; init; }
  [JsonPropertyName("stck_fcam")]
  public required decimal FaceValue { get; init; }
  [JsonPropertyName("lstn_stcn")]
  public required decimal ListedStocks { get; init; }
  [JsonPropertyName("cpfn")]
  public required decimal Foundation { get; init; }
  public decimal MarketCapitalization => ListedStocks * Close;
  [JsonPropertyName("per")]
  public required float PriceEarningsRate { get; init; }
  [JsonPropertyName("eps")]
  public required float EarningsPerShare { get; init; }
  [JsonPropertyName("pbr")]
  public required float PriceBookValueRatio{ get; init; }
  [JsonPropertyName("itewhol_loan_rmnd_ratem name")]
  public required float LoanRemainderRate { get; init; }
}