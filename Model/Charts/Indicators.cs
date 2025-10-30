using System.ComponentModel;

namespace trading_platform.Model.Charts;

public enum IndicatorName {
  [Description("평균실질변동폭(ATR)")]
  AverageTrueRange,
  [Description("Bollinger Bands")]
  BollingerBands,
  [Description("지수이동평균")]
  ExponentialMovingAverage,
  [Description("이동평균수렴발산지표(MACD)")]
  MovingAverageConvergenceDivergence,
  [Description("상대강도지표(RSI)")]
  RelativeStrengthIndicator,
  [Description("단순이동평균")]
  SimpleMovingAverage,
  [Description("표본표준편차")]
  StandardDeviation,
  [Description("Stochastics")]
  StochasticsOscillator,
}