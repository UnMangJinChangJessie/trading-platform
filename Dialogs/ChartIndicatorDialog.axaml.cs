using System.Collections.ObjectModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using trading_platform.Model.Charts;
using trading_platform.Model.Charts.Indicators;

namespace trading_platform.Dialogs;

public partial class ChartIndicatorDialog : Window {
  private CandlestickChartData? CastedDataContext => DataContext as CandlestickChartData;
  public ChartIndicatorDialog() {
    InitializeComponent();
  }
  private void AddIndicator_Click(object? sender, RoutedEventArgs args) {
    if (IndicatorTreeView.SelectedItem is TreeViewItem { DataContext: IndicatorName name }) {
      AddIndicator(name);
    }
  }
  private void IndicatorTreeView_DoubleTapped(object? sender, TappedEventArgs args) {
    // 지표 추가 버튼 누른 것과 같은 동작
    AddIndicator_Click(sender, args); 
  }
  private void AddIndicator(IndicatorName name) {
    if (CastedDataContext == null) return;
    Indicator? newIndicator = null;
    switch (name) {
      case IndicatorName.SimpleMovingAverage:
        newIndicator = new SimpleMovingAverage(CastedDataContext, 20);
        break;
      case IndicatorName.ExponentialMovingAverage:
        newIndicator = new ExponentialMovingAverage(CastedDataContext, 20);
        break;
      case IndicatorName.AverageTrueRange:
        newIndicator = new AverageTrueRange(CastedDataContext, 20);
        break;
      case IndicatorName.MovingAverageConvergenceDivergence:
        newIndicator = new MovingAverageConvergenceDivergence(CastedDataContext, 12, 26);
        break;
    }
    if (newIndicator != null) CastedDataContext.AddIndicator(newIndicator);
  }
}