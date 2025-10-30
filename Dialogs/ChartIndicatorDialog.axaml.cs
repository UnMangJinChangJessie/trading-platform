using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml.Templates;
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
    int rowIdx = 0;
    foreach (var (type, name, attr) in indicatorParameters) {
      if (Enumerable.Contains([typeof(int), typeof(long), typeof(double), typeof(float)], type.PropertyType)) {
        ParameterModificationGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        var label = new TextBlock {
          Text = attr!.ParameterName
        };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);

        var input = new NumericUpDown() {
          Value = Convert.ToDecimal(type.GetValue(indicator)!),
        };
        if (Enumerable.Contains([typeof(int), typeof(long)], type.PropertyType)) {
          input.FormatString = "{0:F0}";
        }
        input.Bind(NumericUpDown.TextProperty, new Binding(name, BindingMode.TwoWay) { Source = indicator });
        input.InvalidateVisual();
        Grid.SetRow(input, rowIdx);
        Grid.SetColumn(input, 1);
        ParameterModificationGrid.Children.Add(input);
      }
      // 선 색 및 두께, 모양 설정 가능, ScottPlot.Color와 호환되기 위한 converter 구현 필요.
      else if (type.PropertyType == typeof(LineStyle)) {
        ParameterModificationGrid.RowDefinitions.Add(new(GridLength.Auto));
        var label = new TextBlock {
          Text = attr!.ParameterName
        };
        label.FontWeight = Avalonia.Media.FontWeight.DemiBold;
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);
        rowIdx++;

        // 색상 설정 행
        ParameterModificationGrid.RowDefinitions.Add(new(GridLength.Auto));
        label = new TextBlock { Text = "색상" };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);
        var lineStyle = (type.GetValue(indicator) as ScottPlot.LineStyle)!;
        var colorInput = new ColorPicker() {
          Color = new Avalonia.Media.Color(lineStyle.Color.A, lineStyle.Color.R, lineStyle.Color.G, lineStyle.Color.B),
          Palette = new MaterialColorPalette(),
        };
        colorInput.Bind(ColorPicker.ColorProperty, new Binding($"{name}.{nameof(lineStyle.Color)}", BindingMode.TwoWay) {
          Source = indicator,
          Converter = new Model.Converters.ScottPlotAvaloniaColorConverter()
        });
        Grid.SetRow(colorInput, rowIdx);
        Grid.SetColumn(colorInput, 1);
        // colorInput.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center;
        ParameterModificationGrid.Children.Add(colorInput);
        rowIdx++;

        // 너비 설정 행
        ParameterModificationGrid.RowDefinitions.Add(new(GridLength.Auto));
        label = new TextBlock { Text = "선 너비" };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);
        var thicknessInput = new NumericUpDown() {
          Value = (decimal)lineStyle.Width,
          Minimum = 0,
          Increment = 0.1M,
          InnerRightContent = new TextBlock() { Text = "px" }
        };
        thicknessInput.Bind(NumericUpDown.ValueProperty, new Binding($"{name}.{nameof(lineStyle.Width)}", BindingMode.TwoWay) {
          Source = indicator,
        });
        Grid.SetRow(thicknessInput, rowIdx);
        Grid.SetColumn(thicknessInput, 1);
        ParameterModificationGrid.Children.Add(thicknessInput);
        rowIdx++;

        // 선 모양 설정 행
        ParameterModificationGrid.RowDefinitions.Add(new(GridLength.Auto));
        label = new TextBlock { Text = "선 모양" };
        Grid.SetRow(label, rowIdx);
        Grid.SetColumn(label, 0);
        ParameterModificationGrid.Children.Add(label);
        var lineStyleInput = new ComboBox() {
          ItemsSource = new LinePattern[] {
            LinePattern.Dashed,
            LinePattern.DenselyDashed,
            LinePattern.Dotted,
            LinePattern.Solid,
          },
          ItemTemplate = new FuncDataTemplate(
            typeof(LinePattern),
              (item, scope) => {
                return new TextBlock() { Text = ((LinePattern)item).Name };
              }
            )
        };
        lineStyleInput.Bind(SelectingItemsControl.SelectedItemProperty, new Binding($"{name}.{nameof(lineStyle.Pattern)}") {
          Source = indicator
        });
        Grid.SetRow(lineStyleInput, rowIdx);
        Grid.SetColumn(lineStyleInput, 1);
        ParameterModificationGrid.Children.Add(lineStyleInput);
      }
      rowIdx++;
    }
  }
}