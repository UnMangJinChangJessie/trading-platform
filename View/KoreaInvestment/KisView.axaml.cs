using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace trading_platform.View.KoreaInvestment;

public partial class KisView : UserControl {
  private ViewModel.KoreaInvestment.KisViewModel? CastedDataContext => DataContext as ViewModel.KoreaInvestment.KisViewModel;
  public KisView() {
    InitializeComponent();
  }
  public async void FileLoad_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    var topLevel = TopLevel.GetTopLevel(this)!;
    var jsonFiles = await topLevel.StorageProvider.OpenFilePickerAsync(new() {
      Title = "API 구성 파일 불러오기",
      AllowMultiple = false,
      FileTypeFilter = [new FilePickerFileType("JSON file") { MimeTypes = ["application/json", "text/json"], Patterns = ["*.json"] }],
    });
    if (jsonFiles == null || jsonFiles.Count == 0) return;
    var jsonFile = jsonFiles.Single();
    await CastedDataContext.AppendClient(jsonFile);
  }
  public void Delete_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    CastedDataContext.RemoveSelected();
  }
  public async void Activate_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.ActivateSelected();
  }
  public async void Deactivate_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    await CastedDataContext.DeactivateSelected();
  }
}