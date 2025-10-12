using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class DetailedInformation {
    [JsonPropertyName("stck_shrn_iscd")]
    public required string Ticker { get; set; }
    
    [JsonPropertyName("prdy_vrss_sign")]
    public required PriceChangeSign PriceChangeSign { get; set; }
    [JsonPropertyName("stck_oprc")]
    public required ulong CurrentOpen { get; set; }
    [JsonPropertyName("stck_hgpr")]
    public required ulong CurrentHigh { get; set; }
    [JsonPropertyName("stck_lwpr")]
    public required ulong CurrentLow { get; set; }
    [JsonPropertyName("stck_prpr")]
    public required ulong CurrentClose { get; set; }
    [JsonPropertyName("prdy_vrss")]
    public required long PriceChange { get; set; }
    [JsonPropertyName("acml_vol")]
    public required ulong CurrentVolume { get; set; }
    [JsonPropertyName("acml_tr_pbmn")]
    public required ulong CurrentAmount { get; set; }
    [JsonPropertyName("vol_tnrt")]
    public required float VolumeTurningRate { get; set; }
    
    [JsonPropertyName("stck_mxpr")]
    public required ulong UpperLimit { get; set; }
    [JsonPropertyName("stck_llam")]
    public required ulong LowerLimit { get; set; }
    [JsonPropertyName("stck_sdpr")]
    public required ulong StandardPrice { get; set; }
    [JsonPropertyName("stck_sspr")]
    public required ulong SubstitutePrice { get; set; }
    [JsonPropertyName("pvt_pont_val")]
    public required ulong PivotPoint { get; set; }
    [JsonPropertyName("pvt_scnd_dmrs_prc")]
    public required ulong PivotSecondResistance { get; set; }
    [JsonPropertyName("pvt_frst_dmrs_prc")]
    public required ulong PivotFirstResistance { get; set; }
    [JsonPropertyName("pvt_frst_dmsp_prc")]
    public required ulong PivotFirstSupport { get; set; }
    [JsonPropertyName("pvt_scnd_dmsp_prc")]
    public required ulong PivotSecondSupport { get; set; }
    [JsonPropertyName("dmrs_val")]
    public required ulong DeMarkResistance { get; set; }
    [JsonPropertyName("dmsp_val")]
    public required ulong DeMarkSupport { get; set; }
    [JsonPropertyName("per")]
    public required float PriceEarningsRate { get; set; }
    [JsonPropertyName("pbr")]
    public required float PriceBookValueRatio { get; set; }
    [JsonPropertyName("eps")]
    public required float EarningsPerShare { get; set; }
    [JsonPropertyName("bps")]
    public required float BookValuePerShare { get; set; }
    
    [JsonPropertyName("stck_dryy_hgpr")]
    public required ulong YearlyHigh { get; set; }
    [JsonPropertyName("dryy_hgpr_date")]
    public required DateOnly YearlyHighDate { get; set; }
    [JsonPropertyName("stck_dryy_lwpr")]
    public required ulong YearlyLow { get; set; }
    [JsonPropertyName("dryy_lwpr_date")]
    public required DateOnly YearlyLowDate { get; set; }

    [JsonPropertyName("bstp_kor_isnm")]
    public string? IndexName { get; set; }
    [JsonPropertyName("rprs_mrkt_kor_name")]
    public required string MarketName { get; set; }
    [JsonPropertyName("iscd_stat_cls_code")]
    public required TradingStatusType TradingStatus { get; set; }
    [JsonPropertyName("marg_rate")]
    public required float MarginRate { get; set; }
    [JsonPropertyName("new_hgpr_lwpr_cls_code")]
    public string? NewExtremeCode { get; set; }
    [JsonPropertyName("temp_stop_yn")]
    public required bool TemporaryCease { get; set; }
    [JsonPropertyName("oprc_rang_cont_yn")]
    public required bool OpenRangeContinued { get; set; }
    [JsonPropertyName("clpr_rang_cont_yn")]
    public required bool CloseRangeContinued { get; set; }
    [JsonPropertyName("crdt_able_yn")]
    public required bool AllowCredit { get; set; }
    [JsonPropertyName("elw_pblc_yn")]
    public required bool HasEquityLinkedWarrant { get; set; }
    [JsonPropertyName("hts_frgn_ehrt")]
    public required float ForeignExhaustRate { get; set; }
    [JsonPropertyName("frgn_ntby_qty")]
    public required long ForeignNetBuyVolume { get; set; }
    [JsonPropertyName("pgtr_ntby_qty")]
    public required long ProgramNetBuyVolume { get; set; }
    [JsonPropertyName("cpfn")]
    public required ulong CapitalFoundation { get; set; }
    [JsonPropertyName("rstc_wdth_prc")]
    public required ulong RestrictionWidth { get; set; }
    [JsonPropertyName("stck_fcam")]
    public required ulong FaceValue { get; set; }
    [JsonPropertyName("hts_deal_qty_unit_val")]
    public required ulong TradingUnit { get; set; }
    [JsonPropertyName("lstn_stcn")]
    public required ulong ListedStocks { get; set; }
    [JsonPropertyName("stac_month")]
    public byte? SettlementMonth { get; set; }
    [JsonPropertyName("ssts_yn")]
    public required bool AllowShortSelling { get; set; }
    [JsonPropertyName("fcam_cnnm")]
    public required string FaceValueCurrency { get; set; }
    [JsonPropertyName("cpfn_cnnm")]
    public required string CapitalCurrency { get; set; }
    [JsonPropertyName("frgn_hldn_qty")]
    public required ulong ForeignHoldingQuantity { get; set; }
    [JsonPropertyName("vi_cls_code")]
    public required bool Interrupted { get; set; }
    [JsonPropertyName("ovtm_vi_cls_code"), JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public bool? AfterMarketVolatilityInterrupt { get; set; }
    [JsonPropertyName("last_ssts_cntg_qty")]
    public required ulong ShortSellingQuantity { get; set; }
    [JsonPropertyName("invt_caful_yn")]
    public required bool MarketWarning { get; set; }
    [JsonPropertyName("mrkt_warn_cls_code")]
    public required MarketWarning MarketWarningCode { get; set; }
    [JsonPropertyName("short_over_yn")]
    public required bool ShortTermOverheat { get; set; }
    [JsonPropertyName("sltr_yn")]
    public required bool IsFinalizing { get; set; }
  }
}