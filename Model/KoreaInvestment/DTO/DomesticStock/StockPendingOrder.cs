namespace trading_platform.Model.KoreaInvestment;

using System.Text.Json.Serialization;

public static partial class DomesticStock {
  public class PendingOrder {
    [JsonPropertyName("KRX_FWDG_ORD_ORGNO")]
    public required string OrganizationNumber { get; set; }
    [JsonPropertyName("ODNO")]
    public required string OrderNumber { get; set; }
    [JsonPropertyName("ORD_TMD"), JsonConverter(typeof(TimeToStringConverter))]
    public required TimeOnly OrderTime { get; set; }

    [JsonIgnore]
    public OrderPosition Position { get; set; }
    [JsonPropertyName("odno")]
    public required string Ticker { get; set; }
    [JsonPropertyName("ord_dvsn_cd")]
    public required OrderMethod OrderDivision { get; set; }
    [JsonPropertyName("ord_unpr")]
    public required ulong UnitPrice { get; set; }
    [JsonPropertyName("ord_qty")]
    public required ulong Quantity { get; set; }
    [JsonPropertyName("stpm_cndt_pric")]
    public decimal? StopLossLimit { get; set; }

    [JsonPropertyName("prdt_name")]
    public required string TickerName { get; set; }
    [JsonPropertyName("rvse_cncl_dvsn_name")]
    public required string ModificationType { get; set; }

    [JsonPropertyName("tot_ccld_qty")]
    public required ulong ConcludedQuantity { get; set; }
    [JsonPropertyName("tot_ccld_amt")]
    public required ulong ConcludedAmount { get; set; }
    [JsonPropertyName("psbl_qty")]
    public required ulong ModifiableQuantity { get; set; }

    [JsonPropertyName("excg_dvsn_cd")] public required Exchange Exchange { get; set; }
    [JsonPropertyName("excg_id_dvsn_name")] public required string ExchangeName { get; set; }
    [JsonPropertyName("mgco_aptm_odno")] public required string ManagementOrderNumber { get; set; }
    [JsonPropertyName("stpm_efct_occr_yn")] public required bool StopLossActivated { get; set; }
  }
}