using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
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
    [JsonPropertyName("hts_id")]
    public required string BrokerageId { get; set; }
  }
  [ObservableProperty]
  public partial bool Selected { get; set; }
  [ObservableProperty]
  public partial string Name { get; set; }
  [ObservableProperty]
  public partial ApiModel ApiClient { get; private set; } = new();
  [ObservableProperty]
  public partial WebSocketModel WebSocketClient { get; private set; } = new();

  public async ValueTask<bool> LoadFromJsonFileAsync(Stream stream) {
    try
    {
      if (await JsonSerializer.DeserializeAsync<ApiClientInformation>(stream) is not ApiClientInformation information) return false;
      ApiClient.AppPublicKey = information.AppPublicKey;
      ApiClient.AppSecretKey = information.AppSecretKey;
      ApiClient.IsPersonal = information.IsPersonal;
      ApiClient.IsSimulation = information.IsSimulation;
      ApiClient.Account.AccountBase = information.AccountBase;
      ApiClient.Account.AccountCode = information.AccountCode;
      ApiClient.BrokerageId = information.BrokerageId;
      return true;
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return false;
    }
  }
  public async ValueTask<bool> ActivateApi() {
    if (!await ApiClient.IssueToken()) return false;
    if (!await WebSocketClient.Connect(ApiClient)) return false;
    return true;
  }
  public async ValueTask<bool> DeactivateApi() {
    return await ApiClient.RevokeToken();
  }
}

public partial class KisViewModel : ObservableObject {
  public ObservableCollection<KisClients> AvailableClients { get; set; } = [];
  [MaybeNull]
  public KisClients KoreaStockMarketClient {
    get => field;
    set {
      if (field != value) {
        field = value;
        KoreaStockMarketViewModel.Api = value;
        OnPropertyChanged(nameof(KoreaStockMarketClient));
      }
    } 
  } = null;
  [MaybeNull]
  public KisClients OverseaStockMarketClient {
    get => field;
    set {
      if (field != value) {
        field = value;
        OnPropertyChanged(nameof(OverseaStockMarketClient));
      }
    }
  } = null;
  [MaybeNull]
  public KisClients KoreaFuturesMarketClient {
    get => field;
    set {
      if (field != value) {
        field = value;
        OnPropertyChanged(nameof(KoreaFuturesMarketClient));
      }
    }
  } = null;
  [MaybeNull]
  public KisClients OverseaFuturesMarketClient {
    get => field;
    set {
      if (field != value) {
        field = value;
        OnPropertyChanged(nameof(OverseaFuturesMarketClient));
      }
    }
  } = null;
  [MaybeNull]
  public KisClients KoreaBondMarketClient {
    get => field;
    set {
      if (field != value) {
        field = value;
        OnPropertyChanged(nameof(KoreaBondMarketClient));
      }
    }
  } = null;
  [ObservableProperty]
  public partial KoreaStock.KoreaStockMarket KoreaStockMarketViewModel { get; set; }
  public KisViewModel() {
    #pragma warning disable CS8604 // Possible null reference argument.
    KoreaStockMarketViewModel = new(KoreaStockMarketClient);
    #pragma warning restore CS8604 // Possible null reference argument.
  }
  public async Task AppendClient(IStorageFile file) {
    Stream? stream = null;
    try {
      var client = new KisClients();
      stream = await file.OpenReadAsync();
      var result = await client.LoadFromJsonFileAsync(stream);
      if (result) AvailableClients.Add(client);
      client.Name = $"API {AvailableClients.Count}";
    }
    finally {
      stream?.Close();
    }
  }
  public void RemoveSelected() {
    var items = AvailableClients.Where(x => x.Selected).ToList();
    foreach (var item in items) AvailableClients.Remove(item);
  }
  public async Task ActivateSelected() {
    foreach (var item in AvailableClients) {
      if (item.Selected) await item.ActivateApi();
    }
  }
  public async Task DeactivateSelected() {
    foreach (var item in AvailableClients) {
      if (item.Selected) await item.DeactivateApi();
    }
  }
}