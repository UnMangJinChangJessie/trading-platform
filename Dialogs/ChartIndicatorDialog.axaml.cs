using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
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
  public void Window_Loaded(object? sender, RoutedEventArgs args) {
    
  }
  private void AddIndicator_Click(object? sender, RoutedEventArgs args) {
    if (IndicatorTreeView.SelectedItem is TreeViewItem { DataContext: IndicatorName name }) {
      AddIndicator(name);
      // 수정할 수 있는 속성을 StackPanel 속에 있는 Grid에 나열한다.
      // reflection 사용
      
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
  private void RemoveIndicator(object? sender, RoutedEventArgs args) {
    if (IndicatorListBox.SelectedItem is not Indicator indicator) return;
    if (CastedDataContext == null) return;
    CastedDataContext.Indicators.Remove(indicator);
  }
  private void IndicatorListBox_SelectionChanged(object? sender, SelectionChangedEventArgs args) {
    if (IndicatorListBox.SelectedItem is not Indicator indicator) return;
    var indicatorParameters = indicator.GetType()
      .GetProperties()
      .Select(x => (x.PropertyType, x.Name, Attribute: x.GetCustomAttribute<IndicatorParameterAttribute>()))
      .Where(x => x.Attribute is not null);
    ParameterModificationGrid.Children.Clear();
    ParameterModificationGrid.RowDefinitions.Clear();
    int rowIdx = 0;
    foreach (var (type, name, attr) in indicatorParameters) {
      if (Enumerable.Contains([typeof(int), typeof(long), typeof(double), typeof(float)], type)) {
        ParameterModificationGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var label = new TextBlock {
          Text = attr!.ParameterName
        };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);

        var input = new NumericUpDown();
        input.Bind(NumericUpDown.TextProperty, new Binding(name, BindingMode.Default) { Source = indicator });
        Grid.SetRow(input, rowIdx);
        Grid.SetColumn(input, 1);
        ParameterModificationGrid.Children.Add(input);
        rowIdx++;
      }
      // 선 색 및 두께 설정 가능, ScottPlot.Color와 호환되기 위한 converter 구현 필요.
      else if (type == typeof(ScottPlot.LineStyle)) {
        var label = new TextBlock {
          Text = attr!.ParameterName
        };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);

      }
      rowIdx++;
    }
  }
}