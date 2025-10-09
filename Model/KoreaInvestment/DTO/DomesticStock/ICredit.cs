namespace trading_platform.Model.KoreaInvestment;

public interface ICredit {
  public OrderCredit CreditType { get; set; }
  public DateOnly LoanDate { get; set; }
}