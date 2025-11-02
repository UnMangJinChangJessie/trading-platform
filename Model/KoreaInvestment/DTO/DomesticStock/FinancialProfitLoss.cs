using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialProfitLossItem {
    [JsonPropertyName("stac_yymm")]
    public required string StatementPoint { get; set; }
    [JsonPropertyName("sale_account")]
    public required decimal Sales { get; set; }
    [JsonPropertyName("sale_cost")]
    public required decimal SalesCost { get; set; }
    [JsonPropertyName("sale_totl_prfi")]
    public required decimal SalesProfit { get; set; }
    [JsonPropertyName("bsop_prti")]
    public decimal Profit { get; set; }
    [JsonPropertyName("op_prfi")]
    public decimal OrdinaryProfit { get; set; }
    [JsonPropertyName("spec_prfi")]
    public decimal SpecialProfit { get; set; }
    [JsonPropertyName("spec_loss")]
    public decimal SpecialLoss { get; set; }
    [JsonPropertyName("thtr_ntin")]
    public decimal NetProfit { get; set; }

    public ViewModel.KoreaInvestment.KoreaStock.FinancialProfitLoss ToViewModelObject() => new() {
      StatementPoint = new DateOnly(year: int.Parse(StatementPoint[..4]), month: int.Parse(StatementPoint[4..]), day: 1),
      // 단위가 모두 억 = 100_000_000
      Sales = Sales * 1E+8M,
      SalesCost = SalesCost * 1E+8M,
      Profit = Profit * 1E+8M,
      OrdinaryProfit = OrdinaryProfit * 1E+8M,
      SpecialProfit = SpecialProfit * 1E+8M,
      SpecialLoss = SpecialLoss * 1E+8M,
      NetProfit = NetProfit * 1E+8M
    };
  }
}