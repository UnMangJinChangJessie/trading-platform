using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;

namespace trading_platform.Components;

public partial class PriceDisplay : UserControl {
  public ViewModel.PriceDisplay PriceDisplayViewModel { get; set; } = new();
  public PriceDisplay() {
    InitializeComponent();
  }
}