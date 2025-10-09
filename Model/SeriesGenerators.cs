namespace trading_platform.Model;

public static partial class Generators {
  public static class Series {
    public static ChartOHLC[] GenerateBrownianOHLC(
      double start,
      double ratePercent,
      double stdDevPercent,
      TimeSpan interval,
      DateTime startDateTime,
      int count,
      int sampleSize = 30
    ) {
      var result = new ChartOHLC[count];
      var time = startDateTime;
      double price = start;
      var volumes = ScottPlot.Generate.RandomWalk(count, 200_000);
      for (int i = 0; i < count; i++) {
        double open = price;
        double high = price;
        double low = price;
        for (int j = 0; j < sampleSize; j++) {
          price *= 1 + ScottPlot.Generate.RandomNormalNumber(
            mean: Math.Pow(1.0 + ratePercent * 0.01, 1.0 / sampleSize) - 1.0,
            stdDev: Math.Pow(1.0 + stdDevPercent * 0.01, 1.0 / sampleSize) - 1.0
          );
          if (j == 0) open = price;
          high = Math.Max(high, price);
          low = Math.Min(low, price);
        }
        result[i] = new(
          Math.Round((decimal)open, 2),
          Math.Round((decimal)high, 2),
          Math.Round((decimal)low, 2),
          Math.Round((decimal)price, 2)
        ) {
          Volume = (int)volumes[i],
          Date = time
        };
        time += interval;
      }
      return result;
    }
  }
}