using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class StockPurchasable {
  [JsonPropertyName("ord_psbl_cash")]
  public required decimal MaximumCashAmount { get; set; }
  [JsonPropertyName("ord_psbl_sbst")]
  public required decimal MaximumSubstituteAmount { get; set; }
  [JsonPropertyName("ruse_psbl_amt")]
  public required decimal MaximumReusableAmount { get; set; }
  [JsonPropertyName("fund_rpch_chgs")]
  public required decimal MaximumFundRepayAmount { get; set; }
  [JsonPropertyName("psbl_qty_calc_unpr")]
  public required decimal UnitPrice { get; set; }
  [JsonPropertyName("nrcvb_buy_amt")]
  public required decimal MaximumFullMarginAmount { get; set; }
  [JsonPropertyName("nrcvb_buy_qty")]
  public required ulong MaximumFullMarginQuantity { get; set; }
  [JsonPropertyName("max_buy_amt")]
  public required decimal MaximumMarginAmount { get; set; }
  [JsonPropertyName("max_buy_qty")]
  public required ulong MaximumMarginQuantity { get; set; }
  [JsonPropertyName("cma_evlu_amt")]
  public required decimal CashManagementAccountAmount { get; set; }
  [JsonPropertyName("ovrs_ruse_amt_wcrc")]
  public required decimal ForeignReusableAmount { get; set; }
  [JsonPropertyName("ord_psbl_frcr_amt_wcrc")]
  public required decimal ForeignAmount { get; set; }
}