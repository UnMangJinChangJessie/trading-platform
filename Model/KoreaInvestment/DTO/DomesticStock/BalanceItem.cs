using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class BalanceItem {
    [JsonPropertyName("pdno")]
    public required string Ticker { get; set; }
    [JsonPropertyName("prdt_name")]
    public required string Name { get; set; }
    [JsonPropertyName("trad_dvsn_name")]
    public required string MarginType { get; set; }
    [JsonPropertyName("bfdy_buy_qty")]
    public required ulong YesterdayBuyQuantity { get; set; }
    [JsonPropertyName("bfdy_sll_qty")]
    public required ulong YesterdaySellQuantity { get; set; }
    [JsonPropertyName("thdt_sll_qty")]
    public required ulong TodaySellQuantity { get; set; }
    [JsonPropertyName("thdt_buyqty")]
    public required ulong TodayBuyQuantity { get; set; }
    [JsonPropertyName("hldg_qty")]
    public required ulong Quantity { get; set; }
    [JsonPropertyName("ord_psbl_qty")]
    public required ulong OrderableQuantity { get; set; }
    [JsonPropertyName("pchs_avg_pric")]
    public required decimal AverageUnitPrice { get; set; }
    [JsonPropertyName("pchs_amt")]
    public required decimal EntryAmount { get; set; }
    [JsonPropertyName("prpr")]
    public required decimal CurrentPrice { get; set; }
    [JsonPropertyName("evlu_amt")]
    public required decimal CurrentEvaluation { get; set; }
    [JsonPropertyName("evlu_pfls_amt")]
    public required decimal ProfitLoss { get; set; }
    [JsonPropertyName("evlu_pfls_rt")]
    public required float ProfitLossRate { get; set; }
    [JsonPropertyName("loan_dt")]
    public required DateOnly LoanDate { get; set; }
    [JsonPropertyName("loan_amt")]
    public required decimal LoanAmount { get; set; }
    [JsonPropertyName("stck_loan_unpr")]
    public required decimal StockLoanUnitPrice { get; set; }
    [JsonPropertyName("expd_dt")]
    public required DateOnly ExpiresAt { get; set; }
    [JsonPropertyName("bfdy_cprs_icdc")]
    public required decimal ChangeAmount { get; set; }
    [JsonPropertyName("sbst_pric")]
    public required decimal SubstituteUnitPrice { get; set; }
  }

  public class Balance {
    [JsonPropertyName("dnca_tot_amt")]
    public required decimal TotalAmount { get; set; }
    [JsonPropertyName("nxdy_excc_amt")]
    public required decimal TomorrowAmount { get; set; }
    [JsonPropertyName("prvs_rcdl_excc_amt")]
    public required decimal OvermorrowAmount { get; set; }
    [JsonPropertyName("cma_evlu_amt")]
    public required decimal CashManagementAccountAmount { get; set; }
    [JsonPropertyName("bfdy_buy_amt")]
    public required decimal PreviousBuyAmount { get; set; }
    [JsonPropertyName("thdt_buy_amt")]
    public required decimal TodayBuyAmount { get; set; }
    [JsonPropertyName("nxdy_auto_rdpt_amt")]
    public required decimal TomorrowPaybackAmount { get; set; }
    [JsonPropertyName("bfdy_sll_amt")]
    public required decimal YesterdaySellAmount { get; set; }
    [JsonPropertyName("thdt_sll_amt")]
    public required decimal TodaySellAmount { get; set; }
    [JsonPropertyName("d2_auto_rdpt_amt")]
    public required decimal OvermorrowPaybackAmount { get; set; }
    [JsonPropertyName("bfdy_tlex_amt")]
    public required decimal YesterdayFee { get; set; }
    [JsonPropertyName("thdt_tlex_amt")]
    public required decimal TodayFee { get; set; }
    [JsonPropertyName("tot_loan_amt")]
    public required decimal TotalLoanAmount { get; set; }
    [JsonPropertyName("scts_evlu_amt")]
    public required decimal SecuritiesEvaluationAmount { get; set; }
    [JsonPropertyName("tot_evlu_amt")]
    public required decimal TotalEvaluationAmount { get; set; }
    [JsonPropertyName("nass_amt")]
    public required decimal NetAssetAmount { get; set; }
    [JsonPropertyName("fncg_gld_auto_rdpt_yn")]
    public required bool LoanAutoRepay { get; set; }
    [JsonPropertyName("pchs_amt_smtl_amt")]
    public required decimal NetLongAmount { get; set; }
    [JsonPropertyName("evlu_amt_smtl_amt")]
    public required decimal NetEvaluationAmount { get; set; }
    [JsonPropertyName("evlu_pfls_smtl_amt")]
    public required decimal NetProfitLossAmount { get; set; }
    [JsonPropertyName("tot_stln_slng_chgs")]
    public required decimal NetLoanLiquidationAmount { get; set; }
    [JsonPropertyName("bfdy_tot_asst_evlu_amt")]
    public required decimal YesterdayNetAssetAmount { get; set; }
    [JsonPropertyName("asst_icdc_amt")]
    public required decimal AssetChangeAmount { get; set; }
  }
}