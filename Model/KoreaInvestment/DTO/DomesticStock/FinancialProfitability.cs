using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialProfitabilityItem {
    [JsonPropertyName("stac_yymm")]
    public required string StatementPoint { get; set; }
    [JsonPropertyName("cptl_ntin_rate")]
    public required float ProfitCapitalRate { get; set; }
    [JsonPropertyName("self_cptl_ntin_inrt")]
    public required float ProfitOwnCapitalRate { get; set; }
    [JsonPropertyName("sale_ntin_rate")]
    public required float ProfitSalesRate { get; set; }
    [JsonPropertyName("sale_totl_rate")]
    public required float NetProfitSalesRate { get; set; }

    public ViewModel.KoreaInvestment.KoreaStock.FinancialProfitability ToViewModelObject() => new() {
      StatementPoint = new DateOnly(year: int.Parse(StatementPoint[..4]), month: int.Parse(StatementPoint[4..]), day: 1),
      ProfitCapitalRate = ProfitCapitalRate / 100.0F,
      ProfitOwnCapitalRate = ProfitOwnCapitalRate / 100.0F,
      ProfitSalesRate = ProfitSalesRate / 100.0F,
      NetProfitSalesRate = NetProfitSalesRate / 100.0F
    };
  }
}