using System.Collections.Immutable;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts;

public static class ContinuouslyAutoscaleExtensions {
  public static void ContinuouslyAutoscaleAction(this CandlestickChartPlot plottable, RenderPack rp) {
    var candles = plottable.Data.GetOHLCs();
    if (candles.Count == 0) return;
    var range = rp.Plot.Axes.GetLimits();
    var beginIdx = candles.BinarySearch(range.HorizontalRange.Min, x => x.DateTime.ToOADate());
    if (beginIdx < 0) beginIdx = ~beginIdx;
    var endIdx = candles.BinarySearch(range.HorizontalRange.Max, x => x.DateTime.ToOADate());
    if (endIdx < 0) endIdx = ~endIdx;
    if (beginIdx == endIdx) return;
    var (min, max) = candles.Skip(beginIdx).Take(endIdx - beginIdx).Aggregate(
      (Min: candles[beginIdx].Low, Max: candles[beginIdx].High),
      (prev, x) => (Math.Min(prev.Min, x.Low), Math.Max(prev.Max, x.High))
    );
    // rp.Plot.Axes.SetLimitsY(min, max);
    var bottom = min * (1.0 + 0.05) - max * 0.05;
    var top = min * (-0.05) + max * 1.05;
    rp.Plot.Axes.SetLimitsY(bottom, top);
    rp.Plot.Axes.DefaultGrid.YAxis.Range.Set(bottom, top);
  }
  public static void ContinuouslyAutoscaleAction(this Indicator plottable, RenderPack rp) {
    var results = plottable.RenderingResults;
    if (results.Length == 0) return;
    var range = rp.Plot.Axes.GetLimits();
    var beginIdx = results.BinarySearch(range.HorizontalRange.Min, x => x.Date.ToOADate());
    if (beginIdx < 0) beginIdx = ~beginIdx;
    var endIdx = results.BinarySearch(range.HorizontalRange.Max, x => x.Date.ToOADate());
    if (endIdx < 0) endIdx = ~endIdx;
    if (beginIdx == endIdx) return;
    var (min, max) = results[beginIdx..endIdx].Where(x => double.IsFinite(x.Value)).Aggregate(
      (Min: results[beginIdx].Value, Max: results[beginIdx].Value),
      (prev, x) => {
        double min = prev.Min, max = prev.Max;
        bool isFinite = double.IsFinite(x.Value);
        if (!double.IsFinite(min) && isFinite) min = x.Value;
        if (!double.IsFinite(max) && isFinite) max = x.Value;
        return (Math.Min(min, x.Value), Math.Max(max, x.Value));
      }
    );
    var bottom = min * 1.05 - max * 0.05;
    var top = min * (-0.05) + max * 1.05;
    rp.Plot.Axes.SetLimitsY(bottom, top);
    rp.Plot.Axes.DefaultGrid.YAxis.Range.Set(bottom, top);
  }
}