using System.Collections.Immutable;
using ScottPlot;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts;

public static class ContinuouslyAutoscaleExtensions {
  public static void ContinuouslyAutoscaleAction(this CandlestickChartPlot plottable, RenderPack rp) {
    var candles = plottable.MainPlot.Data;
    if (candles.Count == 0) return;
    var range = rp.Plot.Grid.XAxis.Range;
    var beginIdx = candles.BinarySearch(range.Min, x => x.Date.ToOADate());
    if (beginIdx < 0) beginIdx = ~beginIdx;
    var endIdx = plottable.RenderingResults.BinarySearch(range.Max, x => x.Date.ToOADate());
    if (endIdx < 0) endIdx = ~endIdx;
    if (beginIdx == endIdx) return;
    var (min, max) = plottable.RenderingResults[beginIdx..endIdx].Aggregate(
      (Min: plottable.RenderingResults[beginIdx].Value, Max: plottable.RenderingResults[beginIdx].Value),
      (prev, x) => (Math.Min(prev.Min, x.Value), Math.Max(prev.Max, x.Value))
    );
    rp.Plot.Axes.SetLimitsY(min, max);
  }
  public static void ContinuouslyAutoscaleAction(this Indicator plottable, RenderPack rp) {
    if (plottable.RenderingResults.Length == 0) return;
    var range = rp.Plot.Grid.XAxis.Range;
    var beginIdx = plottable.RenderingResults.BinarySearch(range.Min, x => x.Date.ToOADate());
    if (beginIdx < 0) beginIdx = ~beginIdx;
    var endIdx = plottable.RenderingResults.BinarySearch(range.Max, x => x.Date.ToOADate());
    if (endIdx < 0) endIdx = ~endIdx;
    if (beginIdx == endIdx) return;
    var (min, max) = plottable.RenderingResults[beginIdx..endIdx].Aggregate(
      (Min: plottable.RenderingResults[beginIdx].Value, Max: plottable.RenderingResults[beginIdx].Value),
      (prev, x) => (Math.Min(prev.Min, x.Value), Math.Max(prev.Max, x.Value))
    );
    rp.Plot.Axes.SetLimitsY(min, max);
  }
}