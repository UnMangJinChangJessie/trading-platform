using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using ScottPlot;
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
      case IndicatorName.RelativeStrengthIndicator:
        newIndicator = new RelativeStrengthIndex(CastedDataContext, 14);
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
    // 수정할 수 있는 속성을 StackPanel 속에 있는 Grid에 나열한다.
    // reflection 사용
    var indicatorParameters = indicator.GetType()
      .GetProperties()
      .Select(x => (x, x.Name, Attribute: x.GetCustomAttribute<IndicatorParameterAttribute>()))
      .Where(x => x.Attribute is not null);
    ParameterModificationGrid.Children.Clear();
    ParameterModificationGrid.RowDefinitions.Clear();
    foreach (var (type, name, attr) in indicatorParameters) {
      if (Enumerable.Contains([typeof(int), typeof(long), typeof(double), typeof(float), typeof(decimal)], type.PropertyType)) {
        var input = new NumericUpDown() {
          Value = Convert.ToDecimal(type.GetValue(indicator)!),
        };
        if (Enumerable.Contains([typeof(int), typeof(long)], type.PropertyType)) {
          input.FormatString = "{0:F0}";
        }
        else if (type.PropertyType == typeof(float)) {
          input.FormatString = "{0:G7}";
        }
        else if (type.PropertyType == typeof(double)) {
          input.FormatString = "{0:G15}";
        }
        else if (type.PropertyType == typeof(decimal)) {
          input.FormatString = "{0}";
        }
        input.Bind(NumericUpDown.TextProperty, new Binding(name, BindingMode.TwoWay) { Source = indicator });
        AddModificationGridRow(attr!.ParameterName, input);
      }
      else if (type.PropertyType == typeof(LineStyle)) {
        var control = new Components.LineStyleEditor();
        control.Bind(DataContextProperty, new Binding(name, BindingMode.TwoWay) { Source = indicator });
        AddModificationGridRow(attr!.ParameterName, control);
      }
      else if (type.PropertyType == typeof(BarStyle)) {
        AddModificationGridRow(attr!.ParameterName, null);
        Control control;
        control = new Components.LineStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.PositiveBarIncreasingLine") { Source = indicator });
        AddModificationGridRow("양수 외곽선(상승)", control);
        control = new Components.LineStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.PositiveBarDecreasingLine") { Source = indicator });
        AddModificationGridRow("양수 외곽선(하락)", control);
        control = new Components.LineStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.NegativeBarIncreasingLine") { Source = indicator });
        AddModificationGridRow("음수 외곽선(상승)", control);
        control = new Components.LineStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.NegativeBarDecreasingLine") { Source = indicator });
        AddModificationGridRow("음수 외곽선(하락)", control);
        control = new Components.FillStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.PositiveBarIncreasingFill") { Source = indicator });
        AddModificationGridRow("양수 내부(상승)", control);
        control = new Components.FillStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.PositiveBarDecreasingFill") { Source = indicator });
        AddModificationGridRow("양수 내부(하락)", control);
        control = new Components.FillStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.NegativeBarIncreasingFill") { Source = indicator });
        AddModificationGridRow("음수 내부(상승)", control);
        control = new Components.FillStyleEditor();
        control.Bind(DataContextProperty, new Binding($"{name}.NegativeBarDecreasingFill") { Source = indicator });
        AddModificationGridRow("음수 내부(하락)", control);
      }
    }
  }
  private void AddModificationGridRow(string label, Control? control) {
    int rowIdx = ParameterModificationGrid.RowDefinitions.Count;
    ParameterModificationGrid.RowDefinitions.Add(new(GridLength.Auto));
    var labelBlock = new TextBlock() { Text = label };
    Grid.SetRow(labelBlock, rowIdx);
    Grid.SetColumn(labelBlock, 0);
    labelBlock.Margin = new Thickness(5);
    ParameterModificationGrid.Children.Add(labelBlock);
    if (control is null) return;
    Grid.SetRow(control, rowIdx);
    Grid.SetColumn(control, 1);
    control.Margin = new Thickness(5);
    ParameterModificationGrid.Children.Add(control);
  }
}