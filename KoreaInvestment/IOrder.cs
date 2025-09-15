namespace trading_platform.KoreaInvestment;

public interface IOrder {
  public OrderPosition Position { get; init; }
  public string Ticker { get; init; }
  public OrderMethod OrderDivision { get; init; }
  public decimal UnitPrice { get; init; }
  public ulong Quantity { get; init; }
  public decimal? StopLossLimit { get; init; }
}