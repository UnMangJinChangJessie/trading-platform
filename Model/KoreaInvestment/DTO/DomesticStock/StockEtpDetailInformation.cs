using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public record StockEtpDetailInformation {
  [JsonPropertyName("stck_prpr")]
  public required decimal Close { get; init; }
  [JsonPropertyName("prdy_vrss_sign")]
  public required PriceChangeSign PriceChangeSign { get; init; }
  [JsonPropertyName("prdy_vrss")]
  public required decimal PriceChange { get; init; }
  [JsonPropertyName("prdy_ctrt")]
  public required float PriceChangeRate { get; init; }
  [JsonPropertyName("acml_vol")]
  public required decimal Volume { get; init; }
  [JsonPropertyName("prdy_vol")]
  public required decimal PreviousVolume { get; init; }
  [JsonPropertyName("stck_mxpr")]
  public required decimal UpperLimit { get; init; }
  [JsonPropertyName("stck_llam")]
  public required decimal LowerLimit { get; init; }
  [JsonPropertyName("stck_prdy_clpr")]
  public required decimal PreviousClose { get; init; }
  [JsonPropertyName("stck_oprc")]
  public required decimal Open { get; init; }
  [JsonPropertyName("prdy_clpr_vrss_oprc_rate")]
  public required float OpenChangeRate { get; init; }
  [JsonPropertyName("stck_hgpr")]
  public required decimal High { get; init; }
  [JsonPropertyName("prdy_clpr_vrss_hgpr_rate")]
  public required float HighChangeRate { get; init; }
  [JsonPropertyName("stck_lwpr")]
  public required decimal Low { get; init; }
  [JsonPropertyName("prdy_clpr_vrss_lwpr_rate")]
  public required float LowChangeRate { get; init; }
  [JsonPropertyName("prdy_last_nav")]
  public required decimal PreviousNav { get; init; }
  [JsonPropertyName("nav")]
  public required decimal Nav { get; init; }
  [JsonPropertyName("nav_prdy_vrss")]
  public required float NavChange { get; init; }
  [JsonPropertyName("nav_prdy_vrss_sign")]
  public required PriceChangeSign NavChangeSign { get; init; }
  [JsonPropertyName("nav_prdy_ctrt")]
  public required float NavChangeRate { get; init; }
  [JsonPropertyName("trc_errt")]
  public required float TrackErrorRate { get; init; }
  [JsonPropertyName("stck_sdpr")]
  public required decimal StandardPrice { get; init; }
  [JsonPropertyName("stck_sspr")]
  public required decimal SubstitutePrice { get; init; }
  [JsonPropertyName("nmix_ctrt")]
  public required string ToIndexRate { get; init; } // 지수 대비율이 뭔데?
  [JsonPropertyName("etf_crcl_stcn")]
  public required decimal ListedEtf { get; init; }
  [JsonPropertyName("etf_ntas_ttam")]
  public required decimal NetAsset { get; init; } // (억)
  [JsonPropertyName("etf_frcr_ntas_ttam")]
  public required decimal ForeignCurrencyNetAsset { get; init; }
  [JsonPropertyName("frgn_limt_rate")]
  public required float ForeignLimitRate { get; init; }
  [JsonPropertyName("frgn_oder_able_qty")]
  public required decimal ForeignMaxQuantity { get; init; }
  [JsonPropertyName("etf_cu_unit_scrt_cnt")]
  public required decimal CreationUnitSecuritiesQuantity { get; init; }
  [JsonPropertyName("etf_cnfg_issu_cnt")]
  public required ulong CreationUnitContentCount { get; init; }
  [JsonPropertyName("etf_dvdn_cycl")]
  public required string DividendPeriod { get; init; }
  [JsonPropertyName("crcd")]
  public required string Currency { get; init; }
  // [JsonPropertyName("etf_crcl_ntas_ttam")]
  public decimal ListedNetAsset => Close * ListedEtf; // (억)
  [JsonPropertyName("etf_frcr_crcl_ntas_ttam")]
  public required decimal ListedForeignNetAsset { get; init; }
  [JsonPropertyName("etf_frcr_last_ntas_wrth_val")]
  public required decimal TotalForeignNetAsset { get; init; }
  [JsonPropertyName("lp_oder_able_cls_code")]
  public required bool AllowLiquidityProvider { get; init; }
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
  [JsonPropertyName("bstp_kor_isnm")]
  public required string Name { get; init; }
  [JsonPropertyName("vi_cls_code")]
  public required bool Interrupted { get; init; }
  [JsonPropertyName("lstn_stcn")]
  public required decimal ListedStocks { get; init; }
  [JsonPropertyName("frgn_hldn_qty")]
  public required decimal ForeignHoldingQuantity { get; init; }
  [JsonPropertyName("frgn_hldn_qty_rate")]
  public required float ForeignHoldingRate { get; init; }
  [JsonPropertyName("etf_trc_ert_mltp")]
  public required float Leverage { get; init; }
  [JsonPropertyName("dprt")]
  public required float DisparateRate { get; init; }
  [JsonPropertyName("mbcr_name")]
  public required string FundManagingCompany { get; init; }
  [JsonPropertyName("stck_lstn_date")]
  public required DateOnly ListedDate { get; init; }
  [JsonPropertyName("mtrt_date")]
  public required DateOnly ExpirationDate { get; init; }
  [JsonPropertyName("shrg_type_code")]
  public required string DividendType { get; init; } // ?
  [JsonPropertyName("lp_hldn_rate")]
  public required float LiquidityProviderHoldingRate { get; init; }
  [JsonPropertyName("etf_trgt_nmix_bstp_code")]
  public required string IndexCode { get; init; }
  [JsonPropertyName("etf_div_name")]
  public required string Sector { get; init; }
  [JsonPropertyName("etf_rprs_bstp_kor_isnm")]
  public required string IndexName { get; init; }
  [JsonPropertyName("lp_hldn_vol")]
  public required decimal LiquidityProviderHoldingQuantity { get; init; }
}