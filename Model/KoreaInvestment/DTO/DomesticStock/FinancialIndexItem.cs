using System.Text.Json.Serialization;
using ScottPlot.TickGenerators.TimeUnits;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialIndexItem {
    [JsonPropertyName("stac_yymm")]
    public required string StatementPoint { get; set; }
    [JsonPropertyName("grs")]
    public required float SalesChangeRate { get; set; }
    [JsonPropertyName("bsop_prfi_inrt")]
    public required float EarningChangeRate { get; set; } // 영업이익 증가율
    [JsonPropertyName("ntin_inrt")]
    public required float ProfitChangeRate { get; set; } // 순이익 증가율, 더 좋은 변수명 필요
    [JsonPropertyName("roe_val")]
    public required float ReturnOnEquity { get; set; }
    [JsonPropertyName("eps")]
    public required decimal EarningPerShare { get; set; }
    [JsonPropertyName("sps")]
    public required decimal SalesPerShare { get; set; }
    [JsonPropertyName("bps")]
    public required decimal BookValuePerShare { get; set; }
    [JsonPropertyName("rsrv_rate")]
    public required float RetentionRate { get; set; }
    [JsonPropertyName("lblt_rate")]
    public required float DebtRate { get; set; }

    public ViewModel.KoreaInvestment.KoreaStock.FinancialIndex ToViewModelObject() =>
      new() {
        StatementPoint = new DateOnly(year: int.Parse(StatementPoint[..4]), month: int.Parse(StatementPoint[4..]), day: 1),
        SalesChangeRate = SalesChangeRate / 100.0F,
        EarningChangeRate = EarningChangeRate / 100.0F,
        ProfitChangeRate = ProfitChangeRate / 100.0F,
        ReturnOnEquity = ReturnOnEquity / 100.0F,
        EarningPerShare = EarningPerShare,
        SalesPerShare = SalesPerShare,
        BookValuePerShare = BookValuePerShare,
        RetentionRate = RetentionRate / 100.0F,
        DebtRate = DebtRate / 100.0F
      };
  }
}