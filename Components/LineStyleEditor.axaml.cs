using Avalonia.Controls;
using Avalonia.Interactivity;
using ScottPlot;

namespace trading_platform.Components;

public partial class LineStyleEditor : UserControl {
  public LineStyleEditor() {
    InitializeComponent();
    PatternComboBox.ItemsSource = new LinePattern[] {
      LinePattern.Solid, LinePattern.Dotted, LinePattern.DenselyDashed, LinePattern.Dashed
    };
  }
  public void UserControl_Initialized(object? sender, EventArgs args) {
  }
}