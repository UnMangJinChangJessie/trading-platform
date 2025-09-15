using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public class StockPurchasable {
  [JsonPropertyName("ord_psbl_cash")]
  public required decimal MaximumCashAmount { get; init; }
  [JsonPropertyName("ord_psbl_sbst")]
  public required decimal MaximumSubstituteAmount { get; init; }
  [JsonPropertyName("ruse_psbl_amt")]
  public required decimal MaximumReusableAmount { get; init; }
  [JsonPropertyName("fund_rpch_chgs")]
  public required decimal MaximumFundRepayAmount { get; init; }
  [JsonPropertyName("psbl_qty_calc_unpr")]
  public required decimal UnitPrice { get; init; }
  [JsonPropertyName("nrcvb_buy_amt")]
  public required decimal MaximumFullMarginAmount { get; init; }
  [JsonPropertyName("nrcvb_buy_qty")]
  public required ulong MaximumFullMarginQuantity { get; init; }
  [JsonPropertyName("max_buy_amt")]
  public required decimal MaximumMarginAmount { get; init; }
  [JsonPropertyName("max_buy_qty")]
  public required ulong MaximumMarginQuantity { get; init; }
  [JsonPropertyName("cma_evlu_amt")]
  public required decimal CashManagementAccountAmount { get; init; }
  [JsonPropertyName("ovrs_ruse_amt_wcrc")]
  public required decimal ForeignReusableAmount { get; init; }
  [JsonPropertyName("ord_psbl_frcr_amt_wcrc")]
  public required decimal ForeignAmount { get; init; }
}