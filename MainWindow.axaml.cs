using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace trading_platform;

public partial class MainWindow : Window {
  private enum MiddlePanelType {
    Api, KoreaStock, OverseaStock, KoreaFutures, OverseaFutures, KoreaListedBond
  }
  private readonly static Dictionary<string, MiddlePanelType> SCREEN_SELECTION_DICT = new() {
    ["API 토큰 발급"] = MiddlePanelType.Api,
    ["한국 주식"] = MiddlePanelType.KoreaStock,
    ["해외 주식"] = MiddlePanelType.OverseaStock,
    ["한국 선물옵션"] = MiddlePanelType.KoreaFutures,
    ["해외 선물옵션"] = MiddlePanelType.OverseaFutures,
    ["한국 장내채권"] = MiddlePanelType.KoreaListedBond,
  };
  public MainWindow() {
    InitializeComponent();
    ScreenIds.ItemsSource = SCREEN_SELECTION_DICT.Keys;
    MiddleView.Content = new View.Api();
  }
  private void ScreenIds_SelectionChanged(object? sender, RoutedEventArgs args) {
    if (sender == null) return;
    var item = ((ListBox)sender).SelectedItem;
    if (item == null) return;
    Debug.WriteLine(SCREEN_SELECTION_DICT[(string)item]);
    try {
      SwitchMiddlePanel(SCREEN_SELECTION_DICT[(string)item]);
    }
    catch (NotImplementedException) {
      Debug.WriteLine("Not implemented");
    }
  }
  private void SwitchMiddlePanel(MiddlePanelType screen) {
    MiddleView.Content = screen switch {
      MiddlePanelType.KoreaStock => new View.KoreaStock(),
      MiddlePanelType.OverseaStock => new View.OverseaStock(),
      MiddlePanelType.Api => new View.Api(),
      _ => throw new NotImplementedException()
    };
  }
}