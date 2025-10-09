namespace trading_platform.Model.KoreaInvestment;

public interface IConsecutive {
  public string FirstConsecutiveContext { get; set; }
  public string SecondConsecutiveContext { get; set; }
}

public interface IReturnConsecutive {
  public string? FirstConsecutiveContext { get; set; }
  public string? SecondConsecutiveContext { get; set; }
  public bool HasNextData { get; set; }
}