using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class FinancialStatementsItem {
    [JsonPropertyName("stac_yymm")]
    public required string StatementPoint { get; set; }
    [JsonPropertyName("cras")]
    public required decimal CurrentAssets { get; set; }
    [JsonPropertyName("fxas")]
    public required decimal NonCurrentAssets { get; set; }
    [JsonPropertyName("flow_lblt")]
    public required decimal CurrentLiabilities { get; set; }
    [JsonPropertyName("fix_lblt")]
    public required decimal NonCurrentLiabilities { get; set; }
    [JsonPropertyName("cpfn")]
    public required decimal Capital { get; set; }
    [JsonPropertyName("total_cptl")]
    public required decimal TotalCapital { get; set; }

    public ViewModel.KoreaInvestment.KoreaStock.FinancialStatements ToViewModelObject() => new() {
      StatementPoint = new DateOnly(int.Parse(StatementPoint[..4]), int.Parse(StatementPoint[4..]), 1),
      // 단위는 억 = 100_000_000
      CurrentAssets = CurrentAssets * 1E+8M,
      NonCurrentAssets = NonCurrentAssets * 1E+8M,
      CurrentLiabilities = CurrentLiabilities * 1E+8M,
      NonCurrentLiabilities = NonCurrentLiabilities * 1E+8M,
      CapitalFunds = Capital * 1E+8M,
    };
  }
}