using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace trading_platform.KoreaInvestment;

public enum OrderPosition {
  [JsonStringEnumMemberName("01")]
  Sell, // also short
  [JsonStringEnumMemberName("02")]
  Buy, // also long
}
public enum OrderSelling {
  [JsonStringEnumMemberName("01")]
  Ordinary, // 개인이면 이것만 사용하게 될 것
  [JsonStringEnumMemberName("02")]
  Involuntary,
  [JsonStringEnumMemberName("05")]
  Loaned,
}
public enum OrderCredit {
  [JsonStringEnumMemberName("21")] BrokerageLong,
  [JsonStringEnumMemberName("23")] LiquidityLong,
  [JsonStringEnumMemberName("25")] BrokerageLongReturn,
  [JsonStringEnumMemberName("26")] LiquidityLongReturn,
  [JsonStringEnumMemberName("24")] BrokerageShort,
  [JsonStringEnumMemberName("22")] LiquidityShort,
  [JsonStringEnumMemberName("28")] BrokerageShortReturn,
  [JsonStringEnumMemberName("27")] LiquidityShortReturn,
}
public enum OrderMethod {
  [JsonStringEnumMemberName("00")]
  [Display(Name = "지정가")]
  Limit,
  [JsonStringEnumMemberName("01")]
  [Display(Name = "시장가")]
  Market,
  [JsonStringEnumMemberName("02")]
  [Display(Name = "조건부지정가")]
  ConditionalLimit,
  [JsonStringEnumMemberName("03")]
  [Display(Name = "최유리지정가")]
  BestOffer,
  [JsonStringEnumMemberName("04")]
  [Display(Name = "최우선지정가")]
  TopPriority,
  [JsonStringEnumMemberName("05")]
  [Display(Name = "장전전일종가매매")]
  PreMarket,
  [JsonStringEnumMemberName("06")]
  [Display(Name = "장후종가매매")]
  PostMarket,
  [JsonStringEnumMemberName("07")]
  [Display(Name = "장후단일가매매")]
  AfterMarket,
  [JsonStringEnumMemberName("08")]
  [Display(Name = "자기주식")]
  TreasuryStock,
  [JsonStringEnumMemberName("09")]
  [Display(Name = "자기주식스톡옵션")]
  TreasuryStockOptions,
  [JsonStringEnumMemberName("10")]
  [Display(Name = "자기주식신탁")]
  TreasuryStockTrust,
  [JsonStringEnumMemberName("11")]
  [Display(Name = "지정가(Immediate or Cancel)")]
  IocLimit,
  [JsonStringEnumMemberName("12")]
  [Display(Name = "지정가(Fill or Kill)")]
  FokLimit,
  [JsonStringEnumMemberName("13")]
  [Display(Name = "시장가(Immediate or Cancel)")]
  IocMarket,
  [JsonStringEnumMemberName("14")]
  [Display(Name = "시장가(Fill or Kill)")]
  FokMarket,
  [JsonStringEnumMemberName("15")]
  [Display(Name = "최유리지정가(Immediate or Cancel)")]
  IocBestOffer,
  [JsonStringEnumMemberName("16")]
  [Display(Name = "최유리지정가(Fill or Kill)")]
  FokBestOffer,
  [JsonStringEnumMemberName("21")]
  [Display(Name = "중간가")]
  Intermediate,
  [JsonStringEnumMemberName("22")]
  [Display(Name = "손실제한지정가")]
  StopLossLimit,
  [JsonStringEnumMemberName("23")]
  [Display(Name = "중간가(Immediate or Cancel)")]
  IocIntermediate,
  [JsonStringEnumMemberName("24")]
  [Display(Name = "중간가(Fill or Kill)")]
  FokIntermediate,
  [JsonStringEnumMemberName("51")] MidMarketHuge,
  [JsonStringEnumMemberName("52")] MidMarketBasket,
  [JsonStringEnumMemberName("62")] PreMarketHuge,
  [JsonStringEnumMemberName("63")] PreMarketBasket,
  [JsonStringEnumMemberName("67")] PreMarketTreasuryStockTrust,
  [JsonStringEnumMemberName("69")] PreMarketTreasuryStock,
  [JsonStringEnumMemberName("72")] AfterMarketHuge,
  [JsonStringEnumMemberName("77")] AfterMarketTreasuryStockTrust,
  [JsonStringEnumMemberName("79")] AfterMarketHugeTreasuryStock,
  [JsonStringEnumMemberName("80")] Basket,
}
public enum DomesticOrderRoute {
  [JsonStringEnumMemberName("KRX")] KoreaExchange,
  [JsonStringEnumMemberName("NXT")] NexTrade,
  [JsonStringEnumMemberName("SOR")] SmartOrderRouting,
}
public enum CandlePeriod {
  [JsonStringEnumMemberName("D")] Daily,
  [JsonStringEnumMemberName("W")] Weekly,
  [JsonStringEnumMemberName("M")] Monthly,
  [JsonStringEnumMemberName("Y")] Yearly,
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
public enum PriceChangeSign {
  [JsonStringEnumMemberName("1")] UpperLimit,
  [JsonStringEnumMemberName("2")] Positive,
  [JsonStringEnumMemberName("3")] Tie,
  [JsonStringEnumMemberName("4")] LowerLimit,
  [JsonStringEnumMemberName("5")] Negative,
}
public enum LockType {
  [JsonStringEnumMemberName("01")] Claim,
  [JsonStringEnumMemberName("02")] Dividend,
  [JsonStringEnumMemberName("03")] Liquidation,
  [JsonStringEnumMemberName("04")] ClaimAndDividend,
  [JsonStringEnumMemberName("05")] IntermediateDividend,
  [JsonStringEnumMemberName("06")] ClaimedIntermediateDividend,
  [JsonStringEnumMemberName("07")] ClaimedPeriodicalDividend,
}
public enum TradingStatusType {
  [JsonStringEnumMemberName("51")] Managed,
  [JsonStringEnumMemberName("52")] Risky,
  [JsonStringEnumMemberName("53")] Warning,
  [JsonStringEnumMemberName("54")] Caution,
  [JsonStringEnumMemberName("55")] AllowCredit,
  [JsonStringEnumMemberName("57")] FullMargin, // 증거금 100%
  [JsonStringEnumMemberName("58")] Ceased,
  [JsonStringEnumMemberName("59")] ShortTermOverheat
}
public enum MarketWarning {
  [JsonStringEnumMemberName("00")] None,
  [JsonStringEnumMemberName("01")] Caution,
  [JsonStringEnumMemberName("02")] Warning,
  [JsonStringEnumMemberName("03")] Risky,
}

public static class KoreaInvestmentExtensions {
  public static string GetCode(this OrderMethod type) => type switch {
    OrderMethod.Limit => "00",
    OrderMethod.Market => "01",
    OrderMethod.ConditionalLimit => "02",
    OrderMethod.BestOffer => "03",
    OrderMethod.TopPriority => "04",
    OrderMethod.PreMarket => "05",
    OrderMethod.PostMarket => "06",
    OrderMethod.AfterMarket => "07",
    OrderMethod.TreasuryStock => "08",
    OrderMethod.TreasuryStockOptions => "09",
    OrderMethod.TreasuryStockTrust => "10",
    OrderMethod.IocLimit => "11",
    OrderMethod.FokLimit => "12",
    OrderMethod.IocMarket => "13",
    OrderMethod.FokMarket => "14",
    OrderMethod.IocBestOffer => "15",
    OrderMethod.FokBestOffer => "16",
    OrderMethod.Intermediate => "21",
    OrderMethod.StopLossLimit => "22",
    OrderMethod.IocIntermediate => "23",
    OrderMethod.FokIntermediate => "24",
    OrderMethod.MidMarketHuge => "51",
    OrderMethod.MidMarketBasket => "52",
    OrderMethod.PreMarketHuge => "62",
    OrderMethod.PreMarketBasket => "63",
    OrderMethod.PreMarketTreasuryStockTrust => "67",
    OrderMethod.PreMarketTreasuryStock => "69",
    OrderMethod.AfterMarketHuge => "72",
    OrderMethod.AfterMarketTreasuryStockTrust => "77",
    OrderMethod.AfterMarketHugeTreasuryStock => "79",
    OrderMethod.Basket => "80",
    _ => throw new ArgumentOutOfRangeException(nameof(type))
  };
  public static bool IsPriceMarket(this OrderMethod method) => Enumerable.Contains([
    OrderMethod.Market, OrderMethod.IocMarket, OrderMethod.FokMarket,
    OrderMethod.BestOffer, OrderMethod.IocBestOffer, OrderMethod.FokBestOffer,
    OrderMethod.TopPriority,
    OrderMethod.Intermediate, OrderMethod.IocIntermediate, OrderMethod.FokIntermediate,
  ], method);
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
  public static string GetCode(this CandlePeriod type) => type switch {
    CandlePeriod.Daily => "D",
    CandlePeriod.Weekly => "W",
    CandlePeriod.Monthly => "M",
    CandlePeriod.Yearly => "Y",
    _ => throw new ArgumentOutOfRangeException(nameof(type))
  };
}