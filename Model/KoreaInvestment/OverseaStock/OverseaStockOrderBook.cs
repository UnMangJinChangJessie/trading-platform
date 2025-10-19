using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class OrderBookQueries {
    public required Exchange ExchangeCode { get; set; }
    public required string Ticker { get; set; }
  }
  public class OrderBookResult : KisReturnMessage {
    [JsonPropertyName("output1")]
    public OrderBookInformation? Information { get; set; }
    [JsonPropertyName("output2")]
    public OrderBook? OrderBook { get; set; }
  }
  public readonly static Action<ApiModel, OrderBookQueries, Action<string, bool, object?>?, object?> GetOrderBook = (api, queries, callback, args) =>
    api.PushRequest(
      "HHDFS76200100",
      callback: callback,
      callbackParameters: args,
      queries: new Dictionary<string, string>() {
        ["AUTH"] = "",
        ["EXCD"] = queries.ExchangeCode.GetCode(),
        ["SYMB"] = queries.Ticker,
      }
    );
}