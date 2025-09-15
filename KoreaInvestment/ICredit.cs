namespace trading_platform.KoreaInvestment;

public interface ICredit {
  public OrderCredit CreditType { get; init; }
  public DateOnly LoanDate { get; init; }
}