using System.Collections.Immutable;
using CommunityToolkit.Mvvm.ComponentModel;
using ScottPlot;

namespace trading_platform.Model.Charts;

[AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
sealed class IndicatorParameterAttribute : Attribute
{
  public required string ParameterName;
}

public interface IIndicatorResult {
  public DateTime Date { get; set; }
  public TimeSpan Span { get; set; }
  public double Value { get; set; }
}

public abstract partial class Indicator : ObservableObject, IPlottable, IHasLegendText {
  // 마지막으로 결과를 갱신한 시간
  protected DateTime _lastResultUpdateTime = DateTime.UnixEpoch;
  // 결과물이 변경되었는지를 저장하는 속성
  protected bool _isResultChanged = true;
  // 결과를 갱신할 때가 되었는지 확인하는 속성으로 1초 당 최대 20번 갱신하도록 설계됨.
  protected bool ShouldUpdateResult => DateTime.UtcNow - _lastResultUpdateTime >= TimeSpan.FromMilliseconds(20) && _isResultChanged;
  // 결과를 렌더링 할 때는 이 배열을 이용하고, ShouldUpdateResult에 의해 지시될 때만 Results(나 IEnumerable 아래의 수열)에서부터 값을 갱신합니다.
  public ImmutableArray<IIndicatorResult> RenderingResults { get; protected set; }
  protected void ResetUpdateTime() {
    _lastResultUpdateTime = DateTime.UtcNow;
    _isResultChanged = false;
  }
  public virtual string LegendText { get; set; } = "Indicator";
  public bool IsVisible { get; set; } = true;
  public IAxes Axes { get; set; } = new Axes();
  public IEnumerable<LegendItem> LegendItems => LegendItem.None;
  public double PaddingRate { get; set; }
  public CandlestickChartData BaseChart { get; }
  public abstract IEnumerable<IIndicatorResult> Results { get; }
  public virtual bool IsPriceOverlay => false;
  public abstract void Render(RenderPack rp);
  public AxisLimits GetAxisLimits() {
    if (RenderingResults.Length == 0) return AxisLimits.Unset;
    else return new(
      left: RenderingResults[0].Date.ToOADate(),
      right: RenderingResults[^1].Date.ToOADate() + BaseChart.TimeSpan.TotalDays,
      bottom: RenderingResults.Min(x => x.Value), RenderingResults.Max(x => x.Value)
    );
  }
  public virtual void Reset(object? sender, CandlestickChartData.LoadedEventArgs args) {
    _isResultChanged = true;
  }
  public virtual void UpdateEnd(object? sender, CandlestickChartData.UpdatedEndEventArgs args) {
    _isResultChanged = true;
  }
  public virtual void Clear() {
    _isResultChanged = true;
  }
  public Indicator(CandlestickChartData chart) {
    BaseChart = chart;
    // BaseChart.Loaded += Reset;
    // BaseChart.UpdatedEnd += UpdateEnd;
  }
}