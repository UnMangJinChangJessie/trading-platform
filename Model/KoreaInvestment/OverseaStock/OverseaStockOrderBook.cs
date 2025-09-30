using System.Net;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public class OverseaStockOrderBookQueries {
  public required Exchange ExchangeCode { get; init; }
  public required string Ticker { get; init; }
}
public class OverseaStockOrderBookResult : KisReturnMessage {
  [JsonPropertyName("output1")]
  public OverseaStockOrderBookInformation? Information { get; init; }
  [JsonPropertyName("output2")]
  public OverseaStockOrderBook? OrderBook { get; init; }
}

public static partial class OverseaStock {
  public static async ValueTask<(HttpStatusCode StatusCode, OverseaStockOrderBookResult? Result)> InquireOrderBook(OverseaStockOrderBookQueries queries) =>
    await ApiClient.Request<object, OverseaStockOrderBookResult>(
      "HHDFS76200100", HttpMethod.Get, "/uapi/overseas-price/v1/quotations/inquire-asking-price",
      header: null,
      queries: new Dictionary<string, string>() {
        ["AUTH"] = "",
        ["EXCD"] = queries.ExchangeCode.GetCode(),
        ["SYMB"] = queries.Ticker,
      },
      body: null
    );
}