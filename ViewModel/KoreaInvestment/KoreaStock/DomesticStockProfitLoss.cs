using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class DomesticStockProfitLoss : ProfitLoss, IAccount {
  [ObservableProperty]
  public partial string AccountBase { get; set; }
  [ObservableProperty]
  public partial string AccountCode { get; set; }
  public DomesticStockProfitLoss() {
    AccountBase = "";
    AccountCode = "";
    if (Design.IsDesignMode) {
      ProfitLosses = [
        new() {
          Ticker = "005930", Name = "삼성전자",
          EntryAmount = 10_000_000,
          Quantity = 200,
          AveragePrice = 50_000,
          CurrentEvaluation = 13_000_000,
          CurrentProfitLoss = 3_000_000,
          CurrentProfitLossRate = 0.3F,
        },
        new() {
          Ticker = "035720", Name = "카카오",
          EntryAmount = 13_750_000,
          Quantity = 100,
          AveragePrice = 137_500,
          CurrentEvaluation = 6_000_000,
          CurrentProfitLoss = -7750000,
          CurrentProfitLossRate = -0.5636364F,
        },
      ];
      TotalEntryAmount = 23_750_000;
      TotalEvaluation = 19_000_000;
      TotalProfitLoss = -4750000;
      TotalProfitLossRate = -0.2F;
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
          EntryAmount = pl.PositionAmount,
          Quantity = pl.Quantity,
          CurrentEvaluation = pl.EvaluationAmount,
        });
        ProfitLosses[^1].ChangeDependentProperties();
      }
      if (!string.IsNullOrWhiteSpace(result.FirstConsecutiveContext) && !string.IsNullOrWhiteSpace(result.SecondConsecutiveContext)) {
        GetBalance(new() {
          AccountBase = AccountBase,
          AccountCode = AccountCode,
          FirstConsecutiveContext = result.FirstConsecutiveContext,
          SecondConsecutiveContext = result.SecondConsecutiveContext,
          IncludeFund = false,
          IncludePreviousTrade = true,
          DisplayPrice = BalanceQueries.PRICE_DEFAULT,
          InquiryType = BalanceQueries.INQUIRY_TICKER
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
      FirstConsecutiveContext = "",
      SecondConsecutiveContext = "",
      DisplayPrice = BalanceQueries.PRICE_DEFAULT,
      IncludeFund = false,
      IncludePreviousTrade = true,
      InquiryType = BalanceQueries.INQUIRY_TICKER
    }, OnMessageReceived);
  }
  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> dict) {
    return;
  }
  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> dict) {
    return;
  }
}