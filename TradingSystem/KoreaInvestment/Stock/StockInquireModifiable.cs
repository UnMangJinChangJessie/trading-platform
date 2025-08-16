using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradingSystem.KoreaInvestment;

public class StockInquireModifiableBody {
  public required bool FetchNext { get; set; }
  public required string Account { get; set; }
  public required string AccountCode { get; set; }
  public required string FirstContinuousContext { get; set; }
  public required string SecondContinuousContext { get; set; }
  public enum OrderFilter { Order = 0, Ticker = 1 }
  public required OrderFilter Filter1 { get; set; }
  public enum TypeFilter { All = 0, Buy = 1, Sell = 2 }
  public required TypeFilter Filter2 { get; set; }
}

public class StockInquireModifiableResult {
  [JsonIgnore] public bool HasNextData { get; set; } = false;
  [JsonPropertyName("rt_cd"), JsonConverter(typeof(OneZeroToBooleanConverter))] public required bool Success { get; set; }
  [JsonPropertyName("ctx_area_fk100")] public string? FirstContinuousContext { get; set; }
  [JsonPropertyName("ctx_area_nk100")] public string? SecondContinuousContext { get; set; }
  [JsonPropertyName("msg_cd")] public required string MessageCode { get; set; }
  [JsonPropertyName("msg1")] public required string Message { get; set; }
  public class Modifiable {
    [JsonPropertyName("ord_gno_brno")] public required string BrokerBranchCode { get; set; }
    [JsonPropertyName("odno")] public required string OrderNumber { get; set; }
    [JsonPropertyName("orgn_odno")] public required string OriginOrderNumber { get; set; }
    [JsonPropertyName("ord_dvsn_name")] public required string OrderName { get; set; }
    [JsonPropertyName("pdno")] public required string Ticker { get; set; }
    [JsonPropertyName("prdt_name")] public required string TickerName { get; set; }
    [JsonPropertyName("rvse_cncl_dvsn_name")] public required string ModificationName { get; set; }
    [JsonPropertyName("ord_qty"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public required long Quantity { get; set; }
    [JsonPropertyName("ord_unpr"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public required long UnitPrice { get; set; }
    [JsonPropertyName("ord_tmd"), JsonConverter(typeof(TimeToStringConverter))] public required TimeOnly OrderTime { get; set; }
    [JsonPropertyName("tot_ccld_qty"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public required long ConcludedQuantity { get; set; }
    [JsonPropertyName("tot_ccld_amt"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public required long ConcludedAmount { get; set; }
    [JsonPropertyName("psbl_qty"), JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)] public required long ModifiableQuantity { get; set; }
    [JsonPropertyName("sll_buy_dvsn_cd")] public required OrderType OrderType { get; set; }
    [JsonPropertyName("ord_dvsn_cd")] public required OrderDivision OrderDivision { get; set; }
    [JsonPropertyName("mgco_aptm_odno")] public required string ManagementOrderNumber { get; set; }
    [JsonPropertyName("excg_dvsn_cd")] public required Exchange Exchange { get; set; }
    [JsonPropertyName("excg_id_dvsn_cd")] public required DomesticExchangeId ExchangeCode { get; set; }
    [JsonPropertyName("excg_id_dvsn_name")] public required string ExchangeName { get; set; }
    [JsonPropertyName("stpm_cndt_pric"), JsonNumberHandling(JsonNumberHandling.WriteAsString)] public required long StopLossLimit { get; set; }
    [JsonPropertyName("stpm_efct_occr_yn"), JsonConverter(typeof(YesNoToBooleanConverter))] public required bool StopLossActivated { get; set; }
  }
  [JsonPropertyName("output")] public required IEnumerable<Modifiable> ModifiableList { get; set; }
}

public static partial class ApiClient {
  public static async Task<StockInquireModifiableResult> InquireStockModifiableOrder(StockInquireModifiableBody body) {
    const string tradeId = "TTTC0084R";
    var result = await Request(tradeId, HttpMethod.Get, "/uapi/domestic-stock/v1/trading/inquire-psbl-rvsecncl",
      headers: [("tr_cont", body.FetchNext ? "N" : "")],
      queries: [
        ("CANO", body.Account),
        ("ACNT_PRDT_CD", body.AccountCode),
        ("CTX_AREA_FK100", body.FetchNext ? body.FirstContinuousContext : ""),
        ("CTX_AREA_NK100", body.FetchNext ? body.SecondContinuousContext : ""),
        ("INQR_DVSN_1", ((int)body.Filter1).ToString()),
        ("INQR_DVSN_2", ((int)body.Filter2).ToString())
      ], null
    );
    bool hasNextData = Enumerable.Contains(['F', 'M'], result.Headers.GetValues("tr_cont").Single()[0]);
    var responseBody = await result.Content.ReadFromJsonAsync<StockInquireModifiableResult>();
    responseBody!.HasNextData = hasNextData;
    return responseBody;
  }
}