namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public static partial class DomesticStock {
  public class PendingOrder {
    [JsonPropertyName("ord_gno_brno")]
    public required string BranchId { get; set; }
    [JsonPropertyName("odno")]
    public required string OrderId { get; set; }
    [JsonPropertyName("orgn_odno")]
    public required string InitialOrderId { get; set; }
    [JsonPropertyName("pdno")]
    public required string Ticker { get; set; }
    [JsonPropertyName("prdt_name")]
    public required string Name { get; set; }
    [JsonPropertyName("ord_qty")]
    public required ulong OrderedQuantity { get; set; }
    [JsonPropertyName("tot_ccld_qty")]
    public required ulong ConcludedQuantity { get; set; }
    [JsonPropertyName("tot_ccld_amt")]
    public required ulong ConcludedAmount { get; set; }
    [JsonPropertyName("psbl_qty")]
    public required ulong ModifiableQuantity { get; set; }
    [JsonPropertyName("ord_unpr")]
    public required ulong UnitPrice { get; set; }
    [JsonPropertyName("ord_tmd")]
    public required TimeOnly OrderedTime { get; set; }
    [JsonPropertyName("sll_buy_dvsn_cd")]
    public required OrderPosition Position { get; set; }
    [JsonPropertyName("ord_dvsn_cd")]
    public required OrderMethod Method { get; set; }
    [JsonPropertyName("stpm_cndt_pric")]
    public required ulong StopLossPrice { get; set; }
  }
}