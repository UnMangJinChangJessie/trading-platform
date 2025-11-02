using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class EtpInformation {
    [JsonPropertyName("prdy_vrss_sign")]
    public required PriceChangeSign PriceChangeSign { get; set; }
    [JsonPropertyName("acml_vol")]
    public required ulong Volume { get; set; }
    [JsonPropertyName("prdy_vol")]
    public required ulong PreviousVolume { get; set; }
    [JsonPropertyName("stck_mxpr")]
    public required ulong UpperLimit { get; set; }
    [JsonPropertyName("stck_llam")]
    public required ulong LowerLimit { get; set; }
    [JsonPropertyName("stck_prdy_clpr")]
    public required ulong PreviousClose { get; set; }
    [JsonPropertyName("stck_oprc")]
    public required ulong CurrentOpen { get; set; }
    [JsonPropertyName("stck_hgpr")]
    public required ulong CurrentHigh { get; set; }
    [JsonPropertyName("stck_lwpr")]
    public required ulong CurrentLow { get; set; }
    [JsonPropertyName("stck_prpr")]
    public required ulong CurrentClose { get; set; }
    [JsonPropertyName("nav_prdy_vrss_sign")]
    public required PriceChangeSign NavChangeSign { get; set; }
    [JsonPropertyName("prdy_last_nav")]
    public required decimal PreviousNav { get; set; }
    [JsonPropertyName("nav")]
    public required decimal CurrentNav { get; set; }
    [JsonPropertyName("stck_sdpr")]
    public required decimal StandardPrice { get; set; }
    [JsonPropertyName("stck_sspr")]
    public required decimal SubstitutePrice { get; set; }
    [JsonPropertyName("nmix_ctrt")]
    public required string ToIndexRate { get; set; } // 지수 대비율이 뭔데?
    [JsonPropertyName("etf_crcl_stcn")]
    public required ulong ListedEtf { get; set; }
    [JsonPropertyName("etf_ntas_ttam")]
    public required ulong NetAsset { get; set; } // (억)
    [JsonPropertyName("etf_frcr_ntas_ttam")]
    public required ulong ForeignCurrencyNetAsset { get; set; }
    [JsonPropertyName("frgn_limt_rate")]
    public required float ForeignLimitRate { get; set; }
    [JsonPropertyName("frgn_oder_able_qty")]
    public required ulong ForeignMaxQuantity { get; set; }
    [JsonPropertyName("etf_cu_unit_scrt_cnt")]
    public required ulong CreationUnitSecuritiesQuantity { get; set; }
    [JsonPropertyName("etf_cnfg_issu_cnt")]
    public required ulong CreationUnitContentCount { get; set; }
    [JsonPropertyName("etf_dvdn_cycl")]
    public required string DividendPeriod { get; set; }
    [JsonPropertyName("crcd")]
    public required string Currency { get; set; }
    public decimal ListedNetAsset => CurrentClose * ListedEtf; // (억)
    [JsonPropertyName("etf_frcr_crcl_ntas_ttam")]
    public required decimal ListedForeignNetAsset { get; set; }
    [JsonPropertyName("etf_frcr_last_ntas_wrth_val")]
    public required decimal TotalForeignNetAsset { get; set; }
    [JsonPropertyName("lp_oder_able_cls_code")]
    public required bool AllowLiquidityProvider { get; set; }
    [JsonPropertyName("stck_dryy_hgpr")]
    public required decimal YearlyHigh { get; set; }
    [JsonPropertyName("dryy_hgpr_date")]
    public required DateOnly YearlyHighDate { get; set; }
    [JsonPropertyName("stck_dryy_lwpr")]
    public required decimal YearlyLow { get; set; }
    [JsonPropertyName("dryy_lwpr_date")]
    public required DateOnly YearlyLowDate { get; set; }
    [JsonPropertyName("bstp_kor_isnm")]
    public required string IndexName { get; set; }
    [JsonPropertyName("vi_cls_code")]
    public required bool Interrupted { get; set; }
    [JsonPropertyName("lstn_stcn")]
    public required ulong ListedStocks { get; set; }
    [JsonPropertyName("frgn_hldn_qty")]
    public required ulong ForeignHoldingQuantity { get; set; }
    [JsonPropertyName("etf_trc_ert_mltp")]
    public required float Leverage { get; set; }
    [JsonPropertyName("dprt")]
    public required float DisparateRate { get; set; }
    [JsonPropertyName("mbcr_name")]
    public required string FundManagingCompany { get; set; }
    [JsonPropertyName("stck_lstn_date")]
    public required DateOnly ListedDate { get; set; }
    [JsonPropertyName("mtrt_date")]
    public required DateOnly ExpirationDate { get; set; }
    [JsonPropertyName("shrg_type_code")]
    public required string DividendType { get; set; } // ?
    [JsonPropertyName("lp_hldn_rate")]
    public required float LiquidityProviderHoldingRate { get; set; }
    [JsonPropertyName("etf_trgt_nmix_bstp_code")]
    public required string IndexCode { get; set; }
    [JsonPropertyName("etf_div_name")]
    public required string Sector { get; set; }
    [JsonPropertyName("lp_hldn_vol")]
    public required decimal LiquidityProviderHoldingQuantity { get; set; }
  }
}