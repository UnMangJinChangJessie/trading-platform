using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model.KoreaInvestment;

public partial class Account : ObservableObject {
  [JsonPropertyName("account_base")]
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [JsonPropertyName("account_code")]
  [ObservableProperty]
  public partial string AccountCode { get; set; }
}