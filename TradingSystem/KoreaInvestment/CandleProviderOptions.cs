using TradingSystem.KoreaInvestment;

public struct KoreaInvestmentCandleProviderOptions {
  public required Market Market { get; set; }
  public required Exchange Exchange { get; set; } //  for domestic stocks, otherwise the exchange code 
  public required string Ticker { get; set; }
  public required (DateOnly, DateOnly) FetchPeriod { get; set; } // required on non-realtime chart queries
  public required CandleType Candle { get; set; }
  public required bool Adjusted { get; set; }
}