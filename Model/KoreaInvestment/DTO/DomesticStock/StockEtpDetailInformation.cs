using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public record StockEtpDetailInformation {
  [JsonPropertyName("stck_prpr")]
  public required decimal Close { get; set; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; set; }
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; set; }
  [JsonPropertyName("prdy_ctrt")]
  public required float PriceChangeRate { get; set; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; set; }
  [JsonPropertyName("prdy_vol")]
  public required decimal PreviousVolume { get; set; }
  [JsonPropertyName("stck_mxpr")]
  public required decimal UpperLimit { get; set; }
  [JsonPropertyName("stck_llam")]
  public required decimal LowerLimit { get; set; }
  [JsonPropertyName("stck_prdy_clpr")]
  public required decimal PreviousClose { get; set; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; set; }
  [JsonPropertyName("prdy_clpr_vrss_oprc_rate")]
  public required float OpenChangeRate { get; set; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; set; }
  [JsonPropertyName("prdy_clpr_vrss_hgpr_rate")]
  public required float HighChangeRate { get; set; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; set; }
  [JsonPropertyName("prdy_clpr_vrss_lwpr_rate")]
  public required float LowChangeRate { get; set; }
  [JsonPropertyName("prdy_last_nav")]
  public required decimal PreviousNav { get; set; }
  [JsonPropertyName("nav")]
  public required decimal Nav { get; set; }
  [JsonPropertyName("nav_prdy_vrss")]
  public required float NavChange { get; set; }
  [JsonPropertyName("nav_prdy_vrss_sign")]
  public required PriceChangeSign NavChangeSign { get; set; }
  [JsonPropertyName("nav_prdy_ctrt")]
  public required float NavChangeRate { get; set; }
  [JsonPropertyName("trc_errt")]
  public required float TrackErrorRate { get; set; }
  [JsonPropertyName("stck_sdpr")]
  public required decimal StandardPrice { get; set; }
  [JsonPropertyName("stck_sspr")]
  public required decimal SubstitutePrice { get; set; }
  [JsonPropertyName("nmix_ctrt")]
  public required string ToIndexRate { get; set; } // 지수 대비율이 뭔데?
  [JsonPropertyName("etf_crcl_stcn")]
  public required decimal ListedEtf { get; set; }
  [JsonPropertyName("etf_ntas_ttam")]
  public required decimal NetAsset { get; set; } // (억)
  [JsonPropertyName("etf_frcr_ntas_ttam")]
  public required decimal ForeignCurrencyNetAsset { get; set; }
  [JsonPropertyName("frgn_limt_rate")]
  public required float ForeignLimitRate { get; set; }
  [JsonPropertyName("frgn_oder_able_qty")]
  public required decimal ForeignMaxQuantity { get; set; }
  [JsonPropertyName("etf_cu_unit_scrt_cnt")]
  public required decimal CreationUnitSecuritiesQuantity { get; set; }
  [JsonPropertyName("etf_cnfg_issu_cnt")]
  public required ulong CreationUnitContentCount { get; set; }
  [JsonPropertyName("etf_dvdn_cycl")]
  public required string DividendPeriod { get; set; }
  [JsonPropertyName("crcd")]
  public required string Currency { get; set; }
  // [JsonPropertyName("etf_crcl_ntas_ttam")]
  public decimal ListedNetAsset => Close * ListedEtf; // (억)
  [JsonPropertyName("etf_frcr_crcl_ntas_ttam")]
  public required decimal ListedForeignNetAsset { get; set; }
  [JsonPropertyName("etf_frcr_last_ntas_wrth_val")]
  public required decimal TotalForeignNetAsset { get; set; }
  [JsonPropertyName("lp_oder_able_cls_code")]
  public required bool AllowLiquidityProvider { get; set; }
  [JsonPropertyName("stck_dryy_hgpr")]
  public required decimal HighYear { get; set; }
  [JsonPropertyName("dryy_hgpr_vrss_prpr_rate")]
  public required float HighYearRate { get; set; }
  [JsonPropertyName("dryy_hgpr_date")]
  public required DateOnly HighYearDate { get; set; }
  [JsonPropertyName("stck_dryy_lwpr")]
  public required decimal LowYear { get; set; }
  [JsonPropertyName("dryy_lwpr_vrss_prpr_rate")]
  public required float LowYearRate { get; set; }
  [JsonPropertyName("dryy_lwpr_date")]
  public required DateOnly LowYearDate { get; set; }
  [JsonPropertyName("bstp_kor_isnm")]
  public required string Name { get; set; }
  [JsonPropertyName("vi_cls_code")]
  public required bool Interrupted { get; set; }
  [JsonPropertyName("lstn_stcn")]
  public required decimal ListedStocks { get; set; }
  [JsonPropertyName("frgn_hldn_qty")]
  public required decimal ForeignHoldingQuantity { get; set; }
  [JsonPropertyName("frgn_hldn_qty_rate")]
  public required float ForeignHoldingRate { get; set; }
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
  [JsonPropertyName("etf_rprs_bstp_kor_isnm")]
  public required string IndexName { get; set; }
  [JsonPropertyName("lp_hldn_vol")]
  public required decimal LiquidityProviderHoldingQuantity { get; set; }
}