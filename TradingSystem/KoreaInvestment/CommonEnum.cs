using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradingSystem.KoreaInvestment;

public enum OrderType {
  [JsonStringEnumMemberName("01")] Sell,
  [JsonStringEnumMemberName("02")] Buy,
}
public enum SellingType {
  [JsonStringEnumMemberName("01")] Ordinary,
  [JsonStringEnumMemberName("02")] Voluntary,
  [JsonStringEnumMemberName("05")] Loaned,
}
public enum CreditType {
  [JsonStringEnumMemberName("21")] BrokerageLong,
  [JsonStringEnumMemberName("23")] LiquidityLong,
  [JsonStringEnumMemberName("25")] BrokerageLongReturn,
  [JsonStringEnumMemberName("26")] LiquidityLongReturn,
  [JsonStringEnumMemberName("24")] BrokerageShort,
  [JsonStringEnumMemberName("22")] LiquidityShort,
  [JsonStringEnumMemberName("28")] BrokerageShortReturn,
  [JsonStringEnumMemberName("27")] LiquidityShortReturn,
}
public enum OrderDivision {
  [JsonStringEnumMemberName("00")] Limit,
  [JsonStringEnumMemberName("01")] Market,
  [JsonStringEnumMemberName("02")] ConditionalLimit,
  [JsonStringEnumMemberName("03")] BestOffer,
  [JsonStringEnumMemberName("04")] TopPriority,
  [JsonStringEnumMemberName("05")] PreMarket,
  [JsonStringEnumMemberName("06")] PostMarket,
  [JsonStringEnumMemberName("07")] AfterMarket,
  [JsonStringEnumMemberName("11")] IocLimit,
  [JsonStringEnumMemberName("12")] FokLimit,
  [JsonStringEnumMemberName("13")] IocMarket,
  [JsonStringEnumMemberName("14")] FokMarket,
  [JsonStringEnumMemberName("15")] IocBestOffer,
  [JsonStringEnumMemberName("16")] FokBestOffer,
  [JsonStringEnumMemberName("21")] Intermediate,
  [JsonStringEnumMemberName("22")] StopLossLimit, 
  [JsonStringEnumMemberName("23")] IocIntermediate,
  [JsonStringEnumMemberName("24")] FokIntermediate,
}
public enum Market {
  STOCK,
  OVERSEA_STOCK,
  INDEX,
  STOCK_FUTURES,
  INDEX_FUTURES,
  COMMODITY_FUTURES,
  NIGHT_FUTURES,
  STOCK_OPTIONS,
  INDEX_OPTIONS,
  NIGHT_OPTIONS,
  OVERSEA_FUTURES,
  EXCHANGE_TRADED_FUND,
}

public enum DomesticExchangeId {
  [JsonStringEnumMemberName("KRX")] KoreaExchange,
  [JsonStringEnumMemberName("NXT")] NexTrade,
  [JsonStringEnumMemberName("SOR")] SmartOrderRouting,
}

public enum CandleType {
  [JsonStringEnumMemberName("D")] DAILY,
  [JsonStringEnumMemberName("W")] WEEKLY,
  [JsonStringEnumMemberName("M")] MONTHLY,
  [JsonStringEnumMemberName("Y")] YEARLY,
}

public enum Exchange {
  [JsonStringEnumMemberName("J")] KoreaExchange,
  [JsonStringEnumMemberName("NX")] NexTrade,
  [JsonStringEnumMemberName("UN")] DomesticUnified,
  [JsonStringEnumMemberName("NAS")] Nasdaq,
  [JsonStringEnumMemberName("NYS")] NewYorkStockExchange,
  [JsonStringEnumMemberName("AMS")] NyseAmerican,
  [JsonStringEnumMemberName("HKS")] HongKong,
  [JsonStringEnumMemberName("SHS")] Shanghai,
  [JsonStringEnumMemberName("SZS")] Shenzhen,
  [JsonStringEnumMemberName("TSE")] Tokyo,
  [JsonStringEnumMemberName("HNX")] Hanoi,
  [JsonStringEnumMemberName("HSX")] HoChiMinh,
}
public enum Modification {
  [JsonStringEnumMemberName("01")] Modify,
  [JsonStringEnumMemberName("02")] Cancel
}

public static class KoreaInvestmentExtensions {
  public static string GetCode(this OrderDivision type) => type switch {
    OrderDivision.Limit => "00",
    OrderDivision.Market => "01",
    OrderDivision.ConditionalLimit => "02",
    OrderDivision.BestOffer => "03",
    OrderDivision.TopPriority => "04",
    OrderDivision.PreMarket => "05",
    OrderDivision.PostMarket => "06",
    OrderDivision.AfterMarket => "07",
    OrderDivision.IocLimit => "11",
    OrderDivision.FokLimit => "12",
    OrderDivision.IocMarket => "13",
    OrderDivision.FokMarket => "14",
    OrderDivision.IocBestOffer => "15",
    OrderDivision.FokBestOffer => "16",
    OrderDivision.Intermediate => "21",
    OrderDivision.StopLossLimit => "22",
    OrderDivision.IocIntermediate => "23",
    OrderDivision.FokIntermediate => "24",
    _ => throw new ArgumentOutOfRangeException(nameof(type))
  };
  public static string GetCode(this Exchange exchange) => exchange switch {
    Exchange.KoreaExchange => "J",
    Exchange.NexTrade => "NX",
    Exchange.DomesticUnified => "UN",
    Exchange.Nasdaq => "NAS",
    Exchange.NewYorkStockExchange => "NYS",
    Exchange.NyseAmerican => "AMS",
    Exchange.HongKong => "HKS",
    Exchange.Shanghai => "SHS",
    Exchange.Shenzhen => "SZS",
    Exchange.Tokyo => "TSE",
    Exchange.Hanoi => "HNX",
    Exchange.HoChiMinh => "HSX",
    _ => throw new ArgumentOutOfRangeException(nameof(exchange))
  };
  public static string GetCode(this CandleType type) => type switch {
    CandleType.DAILY => "D",
    CandleType.WEEKLY => "W",
    CandleType.MONTHLY => "M",
    CandleType.YEARLY => "Y",
    _ => throw new ArgumentOutOfRangeException(nameof(type))
  };
}