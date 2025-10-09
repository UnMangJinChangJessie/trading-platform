namespace trading_platform.Model.KoreaInvestment;

public interface IOrder {
  public OrderPosition Position { get; set; }
  public string Ticker { get; set; }
  public OrderMethod OrderDivision { get; set; }
  public decimal UnitPrice { get; set; }
  public ulong Quantity { get; set; }
  public decimal? StopLossLimit { get; set; }
}