using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Components;

public partial class PriceDisplay : UserControl {
  public PriceDisplay() {
    InitializeComponent();
    var context = new ViewModel.PriceDisplay();
    DataContext = context;
    if (Design.IsDesignMode) {
    }
  }
}