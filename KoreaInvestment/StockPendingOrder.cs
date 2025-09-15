namespace trading_platform.KoreaInvestment;

using System.Text.Json.Serialization;

public class StockPendingOrder : IOrderResult, IOrder {
  [JsonPropertyName("KRX_FWDG_ORD_ORGNO")]
  public required string ExchangeCode { get; init; }
  [JsonPropertyName("ODNO")]
  public required string OrderNumber { get; init; }
  [JsonPropertyName("ORD_TMD"), JsonConverter(typeof(TimeToStringConverter))]
  public required TimeOnly OrderTime { get; init; }

  [JsonIgnore]
  public required OrderPosition Position { get; init; }
  [JsonPropertyName("odno")]
  public required string Ticker { get; init; }
  [JsonPropertyName("ord_dvsn_cd")]
  public required OrderMethod OrderDivision { get; init; }
  [JsonPropertyName("ord_unpr")]
  public required decimal UnitPrice { get; init; }
  [JsonPropertyName("ord_qty")]
  public required ulong Quantity { get; init; }
  [JsonPropertyName("stpm_cndt_pric")]
  public decimal? StopLossLimit { get; init; }

  [JsonPropertyName("prdt_name")]
  public required string TickerName { get; init; }
  [JsonPropertyName("rvse_cncl_dvsn_name")]
  public required string ModificationType { get; init; }

  [JsonPropertyName("tot_ccld_qty")]
  public required long ConcludedQuantity { get; init; }
  [JsonPropertyName("tot_ccld_amt")]
  public required long ConcludedAmount { get; init; }
  [JsonPropertyName("psbl_qty")]
  public required long ModifiableQuantity { get; init; }

  [JsonPropertyName("excg_dvsn_cd")] public required Exchange Exchange { get; init; }
  [JsonPropertyName("excg_id_dvsn_name")] public required string ExchangeName { get; init; }
  [JsonPropertyName("mgco_aptm_odno")] public required string ManagementOrderNumber { get; init; }
  [JsonPropertyName("stpm_efct_occr_yn")] public required bool StopLossActivated { get; init; }
}