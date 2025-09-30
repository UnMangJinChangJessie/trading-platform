using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockBalance {
  [JsonPropertyName("pdno")]
  public required string Ticker { get; init; }
  [JsonPropertyName("pdrt_name")]
  public required string TickerName { get; init; }
  [JsonPropertyName("trad_dvsn_name")]
  public required string PositionName { get; init; }
  [JsonPropertyName("bfdy_buy_qty")]
  public required ulong YesterdayBuyQuantity { get; init; }
  [JsonPropertyName("bfdy_sll_qty")]
  public required ulong YesterdaySellQuantity { get; init; }
  [JsonPropertyName("thdt_sll_qty")]
  public required ulong TodaySellQuantity { get; init; }
  [JsonPropertyName("thdt_buy_qty")]
  public required ulong TodayBuyQuantity { get; init; }
  [JsonPropertyName("hldg_qty")]
  public required ulong Quantity { get; init; }
  [JsonPropertyName("ord_psbl_qty")]
  public required ulong OrderableQuantity { get; init; }
  [JsonPropertyName("pchs_avg_pric")]
  public required decimal AverageUnitPrice { get; init; }
  [JsonPropertyName("pchs_amt")]
  public required decimal PositionAmount { get; init; }
  [JsonPropertyName("prpr")]
  public required decimal CurrentPrice { get; init; }
  [JsonPropertyName("evlu_amt")]
  public required decimal EvaluationAmount { get; init; }
  [JsonPropertyName("evlu_pfls_amt")]
  public required decimal ProfitLoss { get; init; }
  [JsonPropertyName("evlu_pfls_rt")]
  public required float ProfitLossRate { get; init; }
  [JsonPropertyName("loandt"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly LoanDate { get; init; }
  [JsonPropertyName("loan_amt")]
  public required decimal LoanAmount { get; init; }
  [JsonPropertyName("stck_loan_unpr")]
  public required decimal StockLoanUnitPrice { get; init; }
  [JsonPropertyName("expd_dt"), JsonConverter(typeof(DateToStringConverter))]
  public required DateOnly ExpiresAt { get; init; }
  [JsonPropertyName("bdfy_cprs_icdc")]
  public required decimal ChangeAmount { get; init; }
  [JsonPropertyName("sbst_pric")]
  public required decimal SubstituteUnitPrice { get; init; }
}

public class AccountBalance {
  [JsonPropertyName("dnca_tot_amt")]
  public required decimal TotalAmount { get; init; }
  [JsonPropertyName("nxdy_excc_amt")]
  public required decimal TomorrowAmount { get; init; }
  [JsonPropertyName("prvs_rcdl_excc_amt")]
  public required decimal OvermorrowAmount { get; init; }
  [JsonPropertyName("cma_evlu_amt")]
  public required decimal CashManagementAccountAmount { get; init; }
  [JsonPropertyName("bfdy_buy_amt")]
  public required decimal PreviousBuyAmount { get; init; }
  [JsonPropertyName("thdt_buy_amt")]
  public required decimal TodayBuyAmount { get; init; }
  [JsonPropertyName("nxdy_auto_rdpt_amt")]
  public required decimal TomorrowPaybackAmount { get; init; }
  [JsonPropertyName("bfdy_sll_amt")]
  public required decimal YesterdaySellAmount { get; init; }
  [JsonPropertyName("thdy_sll_amt")]
  public required decimal TodaySellAmount { get; init; }
  [JsonPropertyName("d2_auto_rdpt_amt")]
  public required decimal OvermorrowPaybackAmount { get; init; }
  [JsonPropertyName("bfdy_tlex_amt")]
  public required decimal YesterdayFee { get; init; }
  [JsonPropertyName("thdt_tlex_amt")]
  public required decimal TodayFee { get; init; }
  [JsonPropertyName("tot_loan_amt")]
  public required decimal TotalLoanAmount { get; init; }
  [JsonPropertyName("scts_evlu_amt")]
  public required decimal SecuritiesEvaluationAmount { get; init; }
  [JsonPropertyName("tot_evlu_amt")]
  public required decimal TotalEvaluationAmount { get; init; }
  [JsonPropertyName("nass_amt")]
  public required decimal NetAssetAmount { get; init; }
  [JsonPropertyName("fncg_gld_auto_rdpt_yn")]
  public required bool LoanAutoRepay { get; init; }
  [JsonPropertyName("pchs_amt_smtl_amt")]
  public required decimal NetLongAmount { get; init; }
  [JsonPropertyName("evlu_amt_smtl_amt")]
  public required decimal NetEvaluationAmount { get; init; }
  [JsonPropertyName("evlu_pfls_smtl_amt")]
  public required decimal NetProfitLossAmount { get; init; }
  [JsonPropertyName("tot_stln_slng_chgs")]
  public required decimal NetLoanLiquidationAmount { get; init; }
  [JsonPropertyName("bfdy_tot_asst_evlu_amt")]
  public required decimal YesterdayNetAssetAmount { get; init; }
  [JsonPropertyName("asst_icdc_amt")]
  public required decimal AssetChangeAmount { get; init; }
}