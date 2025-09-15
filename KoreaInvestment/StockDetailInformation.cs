using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StockDetailInformation {
  [JsonPropertyName("iscd_stat_cls_code")]
  public required TradingStatusType TradingStatus { get; init; }
  [JsonPropertyName("marg_rate")]
  public required float MarginRate { get; init; }
  [JsonPropertyName("rprs_mrkt_kor_name")]
  public required string MarketName { get; init; }
  [JsonPropertyName("new_hgpr_lwpr_cls_code")]
  public required string NewExtremeCode { get; init; }
  [JsonPropertyName("bstp_kor_isnm")]
  public required string IndexName { get; init; }
  [JsonPropertyName("temp_stop_yn")]
  public required bool TemporaryCease { get; init; }
  [JsonPropertyName("oprc_rang_cont_yn")]
  public required bool OpenRangeContinued { get; init; }
  [JsonPropertyName("clpr_rang_cont_yn")]
  public required bool CloseRangeContinued { get; init; }
  [JsonPropertyName("crdt_able_yn")]
  public required bool AllowCredit { get; init; }
  // [JsonPropertyName("grmn_rate_cls_code")]
  // public required MarginRule MarginRateRule { get; init; } // No API specs found.
  [JsonPropertyName("elw_pblc_yn")]
  public required bool HasEquityLinkedWarrant { get; init; }
  [JsonPropertyName("stck_prpr")]
  public required decimal Close { get; init; }
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; init; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; init; }
  [JsonPropertyName("prdy_ctrt")]
  public required float ChangeRate { get; init; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required decimal Amount { get; init; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; init; }
  [JsonPropertyName("prdy_vrss_vol_rate")]
  public required float VolumeRatio { get; init; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; init; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; init; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; init; }
  [JsonPropertyName("stck_mxpr")]
  public required decimal UpperLimit { get; init; }
  [JsonPropertyName("stck_llam")]
  public required decimal LowerLimit { get; init; }
  [JsonPropertyName("stck_sdpr")]
  public required Type StandardPrice { get; init; }
  [JsonPropertyName("wghn_avrg_stck_prc")]
  public required decimal WeightedAveragePrice { get; init; }
  [JsonPropertyName("hts_frgn_ehrt")]
  public required float ForeignExhaustRate { get; init; }
  [JsonPropertyName("frgn_ntby_qty")]
  public required decimal ForeignNetBuyVolume { get; init; }
  [JsonPropertyName("pgtr_ntby_qty")]
  public required decimal ProgramNetBuyVolume { get; init; }
  [JsonPropertyName("pvt_scnd_dmrs_prc")]
  public required decimal PivotSecondResistance { get; init; }
  [JsonPropertyName("pvt_frst_dmrs_prc")]
  public required decimal PivotFirstResistance { get; init; }
  [JsonPropertyName("pvt_pont_val")]
  public required decimal PivotPoint { get; init; }
  [JsonPropertyName("pvt_frst_dmsp_prc")]
  public required decimal PivotFirstSupport { get; init; }
  [JsonPropertyName("pvt_scnd_dmsp_prc")]
  public required decimal PivotSecondSupport { get; init; }
  [JsonPropertyName("dmrs_val")]
  public required decimal DeMarkResistance { get; init; }
  [JsonPropertyName("dmsp_val")]
  public required decimal DeMarkSupport { get; init; }
  [JsonPropertyName("cpfn")]
  public required decimal CapitalFoundation { get; init; }
  [JsonPropertyName("rstc_wdth_prc")]
  public required decimal RestrictionWidth { get; init; }
  [JsonPropertyName("stck_fcam")]
  public required decimal FaceValue { get; init; }
  [JsonPropertyName("stck_sspr")]
  public required decimal SubstitutePrice { get; init; }
  [JsonPropertyName("aspr_unit")]
  public required decimal AskingPriceUnit { get; init; }
  [JsonPropertyName("hts_deal_qty_unit_val")]
  public required decimal TradingUnit { get; init; }
  [JsonPropertyName("lstn_stcn")]
  public required decimal ListedStocks { get; init; }
  public decimal MarketCapitalization => ListedStocks * Close;
  [JsonPropertyName("per")]
  public required float PriceEarningsRate { get; init; }
  [JsonPropertyName("pbr")]
  public required float PriceBookValueRatio { get; init; }
  [JsonPropertyName("stac_month")]
  public required byte SettlementMonth { get; init; }
  [JsonPropertyName("vol_tnrt")]
  public required float VolumeTurningRate { get; init; }
  [JsonPropertyName("eps")]
  public required float EarningsPerShare { get; init; }
  [JsonPropertyName("bps")]
  public required float BookValuePerShare { get; init; }
  [JsonPropertyName("d250_hgpr")]
  public required decimal High250Day { get; init; }
  [JsonPropertyName("d250_hgpr_date")]
  public required DateOnly High250DayDate { get; init; }
  [JsonPropertyName("d250_hgpr_vrss_prpr_rate")]
  public required float High250DayRate { get; init; }
  [JsonPropertyName("d250_lwpr")]
  public required decimal Low250Day { get; init; }
  [JsonPropertyName("d250_lwpr_date")]
  public required DateOnly Low250DayDate { get; init; }
  [JsonPropertyName("d250_lwpr_vrss_prpr_rate")]
  public required float Low250DayRate { get; init; }
  [JsonPropertyName("stck_dryy_hgpr")]
  public required decimal HighYear { get; init; }
  [JsonPropertyName("dryy_hgpr_vrss_prpr_rate")]
  public required float HighYearRate { get; init; }
  [JsonPropertyName("dryy_hgpr_date")]
  public required DateOnly HighYearDate { get; init; }
  [JsonPropertyName("stck_dryy_lwpr")]
  public required decimal LowYear { get; init; }
  [JsonPropertyName("dryy_lwpr_vrss_prpr_rate")]
  public required float LowYearRate { get; init; }
  [JsonPropertyName("dryy_lwpr_date")]
  public required DateOnly LowYearDate { get; init; }
  [JsonPropertyName("w52_hgpr")]
  public required decimal High52Week { get; init; }
  [JsonPropertyName("w52_hgpr_vrss_prpr_ctrt")]
  public required float High52WeekRate { get; init; }
  [JsonPropertyName("w52_hgpr_date")]
  public required DateOnly High52WeekDate { get; init; }
  [JsonPropertyName("w52_lwpr")]
  public required decimal Low52Week { get; init; }
  [JsonPropertyName("w52_lwpr_vrss_prpr_ctrt")]
  public required float Low52WeekRate { get; init; }
  [JsonPropertyName("w52_lwpr_date")]
  public required DateOnly Low52WeekDate { get; init; }
  [JsonPropertyName("whol_loan_rmnd_rate")]
  public required float LoanRemainderRate { get; init; }
  [JsonPropertyName("ssts_yn")]
  public required bool AllowShortSelling { get; init; }
  [JsonPropertyName("stck_shrn_iscd")]
  public required string Ticker { get; init; }
  [JsonPropertyName("fcam_cnnm")]
  public required string FaceValueCurrency { get; init; }
  [JsonPropertyName("cpfn_cnnm")]
  public required string CapitalCurrency { get; init; }
  // [JsonPropertyName("apprch_rate")]
  // public required float ApproachRate { get; init; } // What is this?
  [JsonPropertyName("frgn_hldn_qty")]
  public required decimal ForeignHoldingQuantity { get; init; }
  [JsonPropertyName("vi_cls_code")]
  public required bool Interrupted { get; init; }
  [JsonPropertyName("ovtm_vi_cls_code")]
  public required bool AfterMarketVolatilityInterrupt { get; init; }
  [JsonPropertyName("last_ssts_cntg_qty")]
  public required decimal ShortSellingQuantity { get; init; }
  [JsonPropertyName("invt_caful_yn")]
  public required bool MarketWarning { get; init; }
  [JsonPropertyName("mrkt_warn_cls_code")]
  public required MarketWarning MarketWarningCode { get; init; }
  [JsonPropertyName("short_over_yn")]
  public required bool ShortTermOverheat { get; init; }
  [JsonPropertyName("sltr_yn")]
  public required bool Cleaning { get; init; }
  [JsonPropertyName("mang_issu_cls_code")]
  public required bool InManagement { get; init; }
}