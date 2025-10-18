using CommunityToolkit.Mvvm.ComponentModel;
using System.Diagnostics;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using BalanceBase = ViewModel.Balance;

public partial class Balance(Account account) : BalanceBase {
  [ObservableProperty]
  public partial Account Account { get; set; } = account;

  public void OnReceivedBalance(string jsonString, bool hasNextData, object? args) {
    if (ApiClient.DeserializeJson<BalanceResult>(jsonString) is not BalanceResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedBalance)}] {result.ResponseMessage}");
      return;
    }
    lock (HoldingItems) {
      foreach (var item in result.HoldingStocks!) {
        HoldingItems.Add(new Item() {
          CurrentEvaluation = item.CurrentEvaluation,
          EntryAmount = item.EntryAmount,
          Label = new() { Ticker = item.Ticker, Name = item.Name },
          Quantity = item.Quantity,
        });
      }
    }
    if (hasNextData) {
      GetBalance(
        new BalanceQueries() {
          AccountBase = Account.AccountBase,
          AccountCode = Account.AccountCode,
          DisplayPrice = BalanceQueries.PRICE_DEFAULT,
          InquiryType = BalanceQueries.INQUIRY_TICKER,
          IncludePreviousTrade = true,
          IncludeFund = true,
          FirstConsecutiveContext = result.FirstConsecutiveContext!,
          SecondConsecutiveContext = result.SecondConsecutiveContext!
        },
        OnReceivedBalance,
        null
      );
    }
    else {
      lock (HoldingItems) {
        FreeFunds = result.AccountBalance!.Single().OvermorrowAmount;
        TotalEntryAmount = HoldingItems.Sum(x => x.EntryAmount);
        TotalEvaluation = HoldingItems.Sum(x => x.CurrentEvaluation);
      }
    }
  }
  public override void Refresh() {
    lock (HoldingItems) {
      HoldingItems.Clear();
    }
    GetBalance(
      new BalanceQueries() {
        AccountBase = Account.AccountBase,
        AccountCode = Account.AccountCode,
        DisplayPrice = BalanceQueries.PRICE_DEFAULT,
        InquiryType = BalanceQueries.INQUIRY_TICKER,
        IncludePreviousTrade = true,
        IncludeFund = true,
        FirstConsecutiveContext = "",
        SecondConsecutiveContext = ""
      },
      OnReceivedBalance,
      null
    );
  }
  public override Task RefreshAsync() {
    Refresh();
    return Task.CompletedTask;
  }
}