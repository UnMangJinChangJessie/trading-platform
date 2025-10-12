using System.Net;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class OverseaStock {
  public class OrderBody : IAccount {
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public OrderPosition Position { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    public string TransactionId => (ApiClient.Simulation ? "V" : "") + (Exchange switch {
      Exchange.Nasdaq or Exchange.NewYorkStockExchange or Exchange.NyseAmerican =>
        Position switch { OrderPosition.Long => "TTTT1002U", OrderPosition.Short => "TTTT1006U", _ => "" },
      Exchange.Tokyo =>
        Position switch { OrderPosition.Long => "TTTS0308U", OrderPosition.Short => "TTTS0307U", _ => "" },
      Exchange.Shanghai =>
        Position switch { OrderPosition.Long => "TTTS0202U", OrderPosition.Short => "TTTS1005U", _ => "" },
      Exchange.HongKong =>
        Position switch { OrderPosition.Long => "TTTS1002U", OrderPosition.Short => "TTTS1001U", _ => "" },
      Exchange.Shenzhen =>
        Position switch { OrderPosition.Long => "TTTS0305U", OrderPosition.Short => "TTTS0304U", _ => "" },
      Exchange.Hanoi or Exchange.HoChiMinh =>
        Position switch { OrderPosition.Long => "TTTS0311U", OrderPosition.Short => "TTTS0310U", _ => "" },
      _ => ""
    });
    [JsonIgnore]
    public Exchange Exchange { get; set; }
    [JsonPropertyName("CANO")]
    public required string AccountBase { get; set; }
    [JsonPropertyName("ACNT_PRDT_CD")]
    public required string AccountCode { get; set; }
    [JsonPropertyName("OVRS_EXCG_CD")]
    public string StringExchange => Exchange switch {
      Exchange.Nasdaq => "NASD",
      Exchange.NewYorkStockExchange => "NYSE",
      Exchange.NyseAmerican => "AMEX",
      Exchange.HongKong => "SEHK",
      Exchange.Shanghai => "SHAA",
      Exchange.Shenzhen => "SZAA",
      Exchange.Hanoi => "HASE",
      Exchange.HoChiMinh => "VNSE",
      Exchange.Tokyo => "TKSE",
      _ => ""
    };
    [JsonPropertyName("SLL_TYPE")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? SellType => Position switch {
      OrderPosition.Short => "00",
      _ => null
    };
    [JsonPropertyName("PDNO")]
    public required string Ticker { get; set; }
    [JsonPropertyName("OVRS_ORD_UNPR")]
    public required decimal UnitPrice { get; set; }
    [JsonPropertyName("ORD_QTY")]
    public required ulong Quantity { get; set; }
    [JsonPropertyName("ORD_DVSN")]
    public required OrderMethod Method { get; set; }
    [JsonPropertyName("START_TIME")]
    public TimeOnly? GradualStart { get; set; } = null;
    [JsonPropertyName("END_TIME")]
    public TimeOnly? GradualEnd { get; set; } = null;
    [JsonPropertyName("ORD_SVR_DVSN_CD")]
    public string OrderServerDivisionCode => "0";
  }
  public class OrderResult : KisReturnMessage {
    // 해외주식인데 한국 주식 주문 결과와 데이터 형식이 같음.
    [JsonPropertyName("output")]
    public OrderResult? Result { get; set; }
  }
  public static readonly Action<OrderBody, Action<string>?> Order = (body, callback) =>
    ApiClient.PushRequest(body.TransactionId, callback: callback, body: body);
}