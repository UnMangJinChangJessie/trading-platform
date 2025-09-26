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
  public string? NewExtremeCode { get; init; }
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
  public required ulong Close { get; init; }
  [JsonPropertyName("prdy_vrss")]
  public required long PriceChange { get; init; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; init; }
  [JsonPropertyName("prdy_ctrt")]
  public required float ChangeRate { get; init; }
  [JsonPropertyName("acml_tr_pbmn")]
  public required ulong Amount { get; init; }
  [JsonPropertyName("acml_vol")]
  public required ulong Volume { get; init; }
  [JsonPropertyName("prdy_vrss_vol_rate")]
  public required float VolumeRatio { get; init; }
  [JsonPropertyName("stck_oprc")]
  public required ulong Open { get; init; }
  [JsonPropertyName("stck_hgpr")]
  public required ulong High { get; init; }
  [JsonPropertyName("stck_lwpr")]
  public required ulong Low { get; init; }
  [JsonPropertyName("stck_mxpr")]
  public required ulong UpperLimit { get; init; }
  [JsonPropertyName("stck_llam")]
  public required ulong LowerLimit { get; init; }
  [JsonPropertyName("stck_sdpr")]
  public required ulong StandardPrice { get; init; }
  [JsonPropertyName("wghn_avrg_stck_prc")]
  public required decimal WeightedAveragePrice { get; init; }
  [JsonPropertyName("hts_frgn_ehrt")]
  public required float ForeignExhaustRate { get; init; }
  [JsonPropertyName("frgn_ntby_qty")]
  public required ulong ForeignNetBuyVolume { get; init; }
  [JsonPropertyName("pgtr_ntby_qty")]
  public required ulong ProgramNetBuyVolume { get; init; }
  [JsonPropertyName("pvt_scnd_dmrs_prc")]
  public required ulong PivotSecondResistance { get; init; }
  [JsonPropertyName("pvt_frst_dmrs_prc")]
  public required ulong PivotFirstResistance { get; init; }
  [JsonPropertyName("pvt_pont_val")]
  public required ulong PivotPoint { get; init; }
  [JsonPropertyName("pvt_frst_dmsp_prc")]
  public required ulong PivotFirstSupport { get; init; }
  [JsonPropertyName("pvt_scnd_dmsp_prc")]
  public required ulong PivotSecondSupport { get; init; }
  [JsonPropertyName("dmrs_val")]
  public required ulong DeMarkResistance { get; init; }
  [JsonPropertyName("dmsp_val")]
  public required ulong DeMarkSupport { get; init; }
  [JsonPropertyName("cpfn")]
  public required ulong CapitalFoundation { get; init; }
  [JsonPropertyName("rstc_wdth_prc")]
  public required ulong RestrictionWidth { get; init; }
  [JsonPropertyName("stck_fcam")]
  public required ulong FaceValue { get; init; }
  [JsonPropertyName("stck_sspr")]
  public required ulong SubstitutePrice { get; init; }
  [JsonPropertyName("aspr_unit")]
  public required ulong AskingPriceUnit { get; init; }
  [JsonPropertyName("hts_deal_qty_unit_val")]
  public required ulong TradingUnit { get; init; }
  [JsonPropertyName("lstn_stcn")]
  public required ulong ListedStocks { get; init; }
  public ulong MarketCapitalization => ListedStocks * Close;
  [JsonPropertyName("per")]
  public required float PriceEarningsRate { get; init; }
  [JsonPropertyName("pbr")]
  public required float PriceBookValueRatio { get; init; }
  [JsonPropertyName("stac_month")]
  public byte? SettlementMonth { get; init; }
  [JsonPropertyName("vol_tnrt")]
  public required float VolumeTurningRate { get; init; }
  [JsonPropertyName("eps")]
  public required float EarningsPerShare { get; init; }
  [JsonPropertyName("bps")]
  public required float BookValuePerShare { get; init; }
  [JsonPropertyName("d250_hgpr")]
  public required ulong High250Day { get; init; }
  [JsonPropertyName("d250_hgpr_date")]
  public required DateOnly High250DayDate { get; init; }
  [JsonPropertyName("d250_hgpr_vrss_prpr_rate")]
  public required float High250DayRate { get; init; }
  [JsonPropertyName("d250_lwpr")]
  public required ulong Low250Day { get; init; }
  [JsonPropertyName("d250_lwpr_date")]
  public required DateOnly Low250DayDate { get; init; }
  [JsonPropertyName("d250_lwpr_vrss_prpr_rate")]
  public required float Low250DayRate { get; init; }
  [JsonPropertyName("stck_dryy_hgpr")]
  public required ulong HighYear { get; init; }
  [JsonPropertyName("dryy_hgpr_vrss_prpr_rate")]
  public required float HighYearRate { get; init; }
  [JsonPropertyName("dryy_hgpr_date")]
  public required DateOnly HighYearDate { get; init; }
  [JsonPropertyName("stck_dryy_lwpr")]
  public required ulong LowYear { get; init; }
  [JsonPropertyName("dryy_lwpr_vrss_prpr_rate")]
  public required float LowYearRate { get; init; }
  [JsonPropertyName("dryy_lwpr_date")]
  public required DateOnly LowYearDate { get; init; }
  [JsonPropertyName("w52_hgpr")]
  public required ulong High52Week { get; init; }
  [JsonPropertyName("w52_hgpr_vrss_prpr_ctrt")]
  public required float High52WeekRate { get; init; }
  [JsonPropertyName("w52_hgpr_date")]
  public required DateOnly High52WeekDate { get; init; }
  [JsonPropertyName("w52_lwpr")]
  public required ulong Low52Week { get; init; }
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
  public required ulong ForeignHoldingQuantity { get; init; }
  [JsonPropertyName("vi_cls_code")]
  public required bool Interrupted { get; init; }
  [JsonPropertyName("ovtm_vi_cls_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
  public bool? AfterMarketVolatilityInterrupt { get; init; }
  [JsonPropertyName("last_ssts_cntg_qty")]
  public required ulong ShortSellingQuantity { get; init; }
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