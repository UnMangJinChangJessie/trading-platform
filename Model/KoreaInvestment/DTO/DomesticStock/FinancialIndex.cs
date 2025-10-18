using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialIndex {
    [JsonPropertyName("grs")]
    public required float SalesChangeRate { get; set; }
    [JsonPropertyName("bsop_prfi_inrt")]
    public required float EarningChangeRate { get; set; }
    [JsonPropertyName("roe_val")]
    public required float ReturnOnEquity { get; set; }
    [JsonPropertyName("eps")]
    public required decimal EarningPerShare { get; set; }
    [JsonPropertyName("sps")]
    public required decimal SalesPerShare { get; set; }
    [JsonPropertyName("bps")]
    public required decimal BookValuePerShare { get; set; }
    [JsonPropertyName("lblt_rate")]
    public required float DebtRate { get; set; }
  }
}