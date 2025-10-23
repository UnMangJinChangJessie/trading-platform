using System.Collections.Immutable;
using ScottPlot;
using ScottPlot.Plottables;
using trading_platform.Extensions;

namespace trading_platform.Model.Charts;

public static class ContinuouslyAutoscaleExtensions {
  public static void ContinuouslyAutoscaleAction(this ScottPlot.Plottables.CandlestickPlot plottable, RenderPack rp) {
    // var range = plottable.GetPriceRangeInView();
    // rp.Plot.Axes.SetLimitsY(range.Min, range.Max);
  }
  public static void ContinuouslyAutoscaleAction(this Indicator plottable, RenderPack rp) {
    ImmutableArray<IIndicatorResult> snapshot;
    lock (plottable.Results) {
      snapshot = [.. plottable.Results.Where(x => double.IsFinite(x.Value)).OrderBy(x => x.Date)];
    }
    if (snapshot.Length == 0) return;
    var range = rp.Plot.Grid.XAxis.Range;
    var beginIdx = snapshot.BinarySearch(range.Min, x => x.Date.ToOADate());
    if (beginIdx < 0) beginIdx = ~beginIdx;
    var endIdx = snapshot.BinarySearch(range.Max, x => x.Date.ToOADate());
    if (endIdx < 0) endIdx = ~endIdx;
    if (beginIdx == endIdx) return;
    var (min, max) = snapshot[beginIdx..endIdx].Aggregate(
      (Min: snapshot[beginIdx].Value, Max: snapshot[beginIdx].Value),
      (prev, x) => (Math.Min(prev.Min, x.Value), Math.Max(prev.Max, x.Value))
    );
    rp.Plot.Axes.SetLimitsY(min, max);
  }
}