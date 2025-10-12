using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class BalanceItem {
    [JsonPropertyName("ovrs_pdno")]
    public required string Ticker { get; set; }
    [JsonPropertyName("ovrs_item_name")]
    public required string Name { get; set; }
    [JsonPropertyName("frcr_evlu_pfls_amt")]
    public required decimal ProfitLoss { get; set; }
    [JsonPropertyName("ovrs_cblc_qty")]
    public required ulong Quantity { get; set; }
    [JsonPropertyName("frcr_pchs_amt1")]
    public required decimal EntryAmount { get; set; }
    [JsonPropertyName("ovrs_stck_evlu_amt")]
    public required decimal CurrentEvaluation { get; set; }
    [JsonPropertyName("now_pric2")]
    public required decimal CurrentPrice { get; set; }
  }
  public class Balance {
    [JsonPropertyName("frcr_pchs_amt1")]
    public required decimal TotalEntryAmount { get; set; }
    [JsonPropertyName("ovrs_rlzt_pfls_amt")]
    public required decimal RealizedProfitLoss { get; set; }
    [JsonPropertyName("ovrs_tot_pfls")]
    public required decimal TotalProfitLoss { get; set; }
    [JsonPropertyName("tot_evlu_pfls_amt")]
    public required decimal TotalProfitLossKrw { get; set; }
  }
}