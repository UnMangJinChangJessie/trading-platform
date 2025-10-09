using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockOrderBookQueries {
  public required Exchange ExchangeCode { get; set; }
  public required string Ticker { get; set; }
}
public class OverseaStockOrderBookResult : KisReturnMessage {
  [JsonPropertyName("output1")]
  public OverseaStockOrderBookInformation? Information { get; set; }
  [JsonPropertyName("output2")]
  public OverseaStockOrderBook? OrderBook { get; set; }
}

public static partial class OverseaStock {
  public readonly static Action<OverseaStockOrderBookQueries, Action<string>?> GetOrderBook = (queries, callback) =>
    ApiClient.PushRequest(
      "HHDFS76200100",
      callback: callback,
      queries: new Dictionary<string, string>() {
        ["AUTH"] = "",
        ["EXCD"] = queries.ExchangeCode.GetCode(),
        ["SYMB"] = queries.Ticker,
      }
    );
}