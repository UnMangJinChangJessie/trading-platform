namespace trading_platform.Model;

public static partial class Generators {
  public static class Series {
    public static OHLC[] GenerateBrownianOHLC(
      double start,
      double rate,
      double standardDeviation,
      TimeSpan interval,
      DateTime startDateTime,
      int count,
      int sampleSize = 30
    ) {
      var rates = ScottPlot.Generate.RandomNormal(sampleSize * count, rate, standardDeviation);
      var result = new OHLC[count];
      var time = startDateTime;
      double price = start;
      for (int i = 0; i < count; i++) {
        double open = price;
        double high = price;
        double low = price;
        for (int j = 0; j < sampleSize; j++) {
          price *= 1 + rates[sampleSize * i + j];
          if (j == 0) open = price;
          high = Math.Max(high, price);
          low = Math.Min(low, price);
        }
        result[i] = new() { Date = time, Open = (int)open, High = (int)high, Low = (int)low, Close = (int)price };
      }
      return result;
    }
  }
}