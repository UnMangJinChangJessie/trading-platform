using System.Collections.ObjectModel;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.OverseaStock;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class OverseaStockProfitLoss : ProfitLoss, IAccount {
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [ObservableProperty]
  public partial string AccountCode { get; set; }
  [ObservableProperty]
  public partial Exchange Exchange { get; set; }
  public OverseaStockProfitLoss() {
    AccountBase = "";
    AccountCode = "";
    if (Design.IsDesignMode) {
      ProfitLosses = [
        new() {
        Ticker = "QQQ", Name = "Invesco QQQ Trust Series 1",
          EntryAmount = 9919.2M,
          Quantity = 20,
          AveragePrice = 495.96M,
          CurrentEvaluation = 11789.8M,
          CurrentProfitLoss = 1870.6M,
          CurrentProfitLossRate = 0.1885838F,
        },
        new() {
          Ticker = "TSLA", Name = "Tesla Inc.",
          EntryAmount = 3150,
          Quantity = 7,
          AveragePrice = 450,
          CurrentEvaluation = 2897.3M,
          CurrentProfitLoss = -253,
          CurrentProfitLossRate = -0.08031746F,
        },
      ];
      TotalEntryAmount = 13069.2M;
      TotalEvaluation = 14687.1M;
      TotalProfitLoss = 1617.6M;
      TotalProfitLossRate = 0.1237719F;
    }
  }
  protected void OnMessageReceived(string jsonString) {
    BalanceResult result;
    try {
      result = JsonSerializer.Deserialize<BalanceResult>(jsonString, ApiClient.JsonSerializerOption)!;
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (result.ReturnCode != 0) return;
    Dispatcher.UIThread.Post(() => {
      foreach (var pl in result.HoldingStocks!) {
        ProfitLosses.Add(new() {
          Ticker = pl.Ticker,
          Name = pl.Name,
          EntryAmount = pl.EntryAmount,
          Quantity = pl.Quantity,
          CurrentEvaluation = pl.CurrentEvaluation,
        });
        ProfitLosses[^1].ChangeDependentProperties();
      }
      if (!string.IsNullOrWhiteSpace(result.FirstConsecutiveContext) && !string.IsNullOrWhiteSpace(result.SecondConsecutiveContext)) {
        GetBalance(new() {
          AccountBase = AccountBase,
          AccountCode = AccountCode,
          ExchangeFilter = Exchange,
          FirstConsecutiveContext = result.FirstConsecutiveContext,
          SecondConsecutiveContext = result.SecondConsecutiveContext,
        }, OnMessageReceived);
      }
      else {
        ChangeDependentProperties();
      }
    });
  }
  public override async Task RefreshAsync(IDictionary<string, object> dict) {
    ProfitLosses.Clear();
    GetBalance(new() {
      AccountBase = AccountBase,
      AccountCode = AccountCode,
      ExchangeFilter = Exchange,
      FirstConsecutiveContext = "",
      SecondConsecutiveContext = "",
    }, OnMessageReceived);
  }
  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> dict) {
    return;
  }
  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> dict) {
    return;
  }
}