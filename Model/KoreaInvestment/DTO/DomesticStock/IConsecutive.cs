namespace trading_platform.Model.KoreaInvestment;

public interface IConsecutive {
  public string FirstConsecutiveContext { get; init; }
  public string SecondConsecutiveContext { get; init; }
}

public interface IReturnConsecutive {
  public string? FirstConsecutiveContext { get; init; }
  public string? SecondConsecutiveContext { get; init; }
  public bool HasNextData { get; set; }
}