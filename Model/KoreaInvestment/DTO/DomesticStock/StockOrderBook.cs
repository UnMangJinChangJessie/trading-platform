using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public static partial class DomesticStock {
  public class OrderBook {
    [JsonPropertyName("aspr_acpt_hour")]
    public required TimeOnly Time { get; set; }
    [JsonPropertyName("askp1")]
    public required ulong AskPrice_1 { get; set; }
    [JsonPropertyName("askp2")]
    public required ulong AskPrice_2 { get; set; }
    [JsonPropertyName("askp3")]
    public required ulong AskPrice_3 { get; set; }
    [JsonPropertyName("askp4")]
    public required ulong AskPrice_4 { get; set; }
    [JsonPropertyName("askp5")]
    public required ulong AskPrice_5 { get; set; }
    [JsonPropertyName("askp6")]
    public required ulong AskPrice_6 { get; set; }
    [JsonPropertyName("askp7")]
    public required ulong AskPrice_7 { get; set; }
    [JsonPropertyName("askp8")]
    public required ulong AskPrice_8 { get; set; }
    [JsonPropertyName("askp9")]
    public required ulong AskPrice_9 { get; set; }
    [JsonPropertyName("askp10")]
    public required ulong AskPrice_10 { get; set; }
    [JsonPropertyName("bidp1")]
    public required ulong BidPrice_1 { get; set; }
    [JsonPropertyName("bidp2")]
    public required ulong BidPrice_2 { get; set; }
    [JsonPropertyName("bidp3")]
    public required ulong BidPrice_3 { get; set; }
    [JsonPropertyName("bidp4")]
    public required ulong BidPrice_4 { get; set; }
    [JsonPropertyName("bidp5")]
    public required ulong BidPrice_5 { get; set; }
    [JsonPropertyName("bidp6")]
    public required ulong BidPrice_6 { get; set; }
    [JsonPropertyName("bidp7")]
    public required ulong BidPrice_7 { get; set; }
    [JsonPropertyName("bidp8")]
    public required ulong BidPrice_8 { get; set; }
    [JsonPropertyName("bidp9")]
    public required ulong BidPrice_9 { get; set; }
    [JsonPropertyName("bidp10")]
    public required ulong BidPrice_10 { get; set; }
    [JsonPropertyName("askp_rsqn1")]
    public required ulong AskQuantity_1 { get; set; }
    [JsonPropertyName("askp_rsqn2")]
    public required ulong AskQuantity_2 { get; set; }
    [JsonPropertyName("askp_rsqn3")]
    public required ulong AskQuantity_3 { get; set; }
    [JsonPropertyName("askp_rsqn4")]
    public required ulong AskQuantity_4 { get; set; }
    [JsonPropertyName("askp_rsqn5")]
    public required ulong AskQuantity_5 { get; set; }
    [JsonPropertyName("askp_rsqn6")]
    public required ulong AskQuantity_6 { get; set; }
    [JsonPropertyName("askp_rsqn7")]
    public required ulong AskQuantity_7 { get; set; }
    [JsonPropertyName("askp_rsqn8")]
    public required ulong AskQuantity_8 { get; set; }
    [JsonPropertyName("askp_rsqn9")]
    public required ulong AskQuantity_9 { get; set; }
    [JsonPropertyName("askp_rsqn10")]
    public required ulong AskQuantity_10 { get; set; }
    [JsonPropertyName("bidp_rsqn1")]
    public required ulong BidQuantity_1 { get; set; }
    [JsonPropertyName("bidp_rsqn2")]
    public required ulong BidQuantity_2 { get; set; }
    [JsonPropertyName("bidp_rsqn3")]
    public required ulong BidQuantity_3 { get; set; }
    [JsonPropertyName("bidp_rsqn4")]
    public required ulong BidQuantity_4 { get; set; }
    [JsonPropertyName("bidp_rsqn5")]
    public required ulong BidQuantity_5 { get; set; }
    [JsonPropertyName("bidp_rsqn6")]
    public required ulong BidQuantity_6 { get; set; }
    [JsonPropertyName("bidp_rsqn7")]
    public required ulong BidQuantity_7 { get; set; }
    [JsonPropertyName("bidp_rsqn8")]
    public required ulong BidQuantity_8 { get; set; }
    [JsonPropertyName("bidp_rsqn9")]
    public required ulong BidQuantity_9 { get; set; }
    [JsonPropertyName("bidp_rsqn10")]
    public required ulong BidQuantity_10 { get; set; }
  }
}