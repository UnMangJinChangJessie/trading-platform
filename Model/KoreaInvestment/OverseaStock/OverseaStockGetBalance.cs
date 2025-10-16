using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public struct BalanceQueries(string accountBase, string accountCode, Exchange exchange) : IConsecutive {
    public required string AccountBase { get; set; } = accountBase;
    public required string AccountCode { get; set; } = accountCode;
    public required Exchange ExchangeFilter { get; set; } = exchange;
    public string ExchangeCode => ExchangeFilter switch {
      Exchange.UnitedStates => "NASD",
      Exchange.Nasdaq => "NAS",
      Exchange.NewYorkStockExchange => "NYSE",
      Exchange.NyseAmerican => "AMEX",
      Exchange.HongKong => "SEHK",
      Exchange.Shanghai => "SHAA",
      Exchange.Shenzhen => "SZAA",
      Exchange.Tokyo => "TKSE",
      Exchange.Hanoi => "HASE",
      Exchange.HoChiMinh => "VNSE",
      _ => ""
    };
    public string CurrencyCode => ExchangeFilter switch {
      Exchange.UnitedStates or Exchange.Nasdaq or Exchange.NyseAmerican or Exchange.NewYorkStockExchange => "USD",
      Exchange.HongKong => "HKD",
      Exchange.Shenzhen or Exchange.Shanghai => "CNY",
      Exchange.Tokyo => "JPY",
      Exchange.Hanoi | Exchange.HoChiMinh => "VND",
      _ => "",
    };
    public string FirstConsecutiveContext { get; set; } = "";
    public string SecondConsecutiveContext { get; set; } = "";
  }
  public class BalanceResult : KisReturnMessage, IReturnConsecutive {
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public bool HasNextData { get; set; }
    [JsonPropertyName("ctx_area_fk200")]
    public string? FirstConsecutiveContext { get; set; }
    [JsonPropertyName("ctx_area_nk200")]
    public string? SecondConsecutiveContext { get; set; }
    
    [JsonPropertyName("output1")]
    public IEnumerable<BalanceItem>? HoldingStocks { get; set; }
    [JsonPropertyName("output2")]
    public Balance? AccountBalance { get; set; }
  }
  public readonly static Action<BalanceQueries, Action<string, object?>?, object?> GetBalance = (queries, cb, args) =>
    ApiClient.PushRequest(
      transId: "TTTS3012R",
      queries: new Dictionary<string, string>() {
        ["CANO"] = queries.AccountBase,
        ["ACNT_PRDT_CD"] = queries.AccountCode,
        ["OVRS_EXCG_CD"] = queries.ExchangeCode,
        ["TR_CRCY_CD"] = queries.CurrencyCode,
        ["CTX_AREA_FK200"] = queries.FirstConsecutiveContext,
        ["CTX_AREA_NK200"] = queries.SecondConsecutiveContext
      },
      callback: cb,
      callbackParameters: args
    );
}