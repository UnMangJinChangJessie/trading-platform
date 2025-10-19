using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class KisClients : ObservableObject {
  internal class ApiClientInformation {
    [JsonPropertyName("public_key")]
    public required string AppPublicKey { get; set; }
    [JsonPropertyName("secret_key")]
    public required string AppSecretKey { get; set; }
    [JsonPropertyName("account_base")]
    public required string AccountBase { get; set; }
    [JsonPropertyName("account_code")]
    public required string AccountCode { get; set; }
    [JsonPropertyName("personal")]
    public required bool IsPersonal { get; set; }
    [JsonPropertyName("simulation")]
    public required bool IsSimulation { get; set; }
  }
  [ObservableProperty]
  public partial bool Selected { get; set; }
  [ObservableProperty]
  public partial string Name { get; set; }
  [ObservableProperty]
  public partial ApiModel ApiClient { get; private set; } = new();
  [ObservableProperty]
  public partial WebSocketModel WebSocketClient { get; private set; } = new();

  public async Task LoadFromJsonFileAsync(string path) {
    try
    {
      if (await JsonSerializer.DeserializeAsync<ApiClientInformation>(File.OpenRead(path)) is not ApiClientInformation information) return;
      ApiClient.AppPublicKey = information.AppPublicKey;
      ApiClient.AppPublicKey = information.AppSecretKey;
      ApiClient.IsPersonal = information.IsPersonal;
      ApiClient.IsSimulation = information.IsSimulation;
      ApiClient.Account.AccountBase = information.AccountBase;
      ApiClient.Account.AccountCode = information.AccountCode;
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
  }
  public async ValueTask<bool> ActivateApi() {
    if (!await ApiClient.IssueToken()) return false;
    if (!await WebSocketClient.Connect(ApiClient)) return false;
    return true;
  }
}

public partial class KisViewModel : ObservableObject {
  public ObservableCollection<KisClients> AvailableClients { get; set; } = [];
  [MaybeNull]
  [ObservableProperty]
  public partial KisClients KoreaStockMarketClient { get; set; } = null;
  [ObservableProperty]
  public partial KoreaStock.Market KoreaStockMarketViewModel { get; set; }
  public KisViewModel() {
    KoreaStockMarketViewModel = new(KoreaStockMarketClient);
  }
  public void AppendClient() {
    AvailableClients.Add(new());
  }
  public void RemoveClient(KisClients obj) {
    AvailableClients.Remove(obj);
  }
}