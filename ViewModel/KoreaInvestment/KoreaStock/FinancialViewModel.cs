using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;

namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

public partial class FinancialViewModel([MaybeNull] KisClients api, MarketItemLabel label) : ObservableObject, IRefresh {
  // 더 변경할 API가 아래에 없음
  public KisClients Api { get; set; } = api;
  [ObservableProperty]
  public partial MarketItemLabel Label { get; set; } = label;
  public ObservableCollection<FinancialStatements> Statements { get; private set; } = [];
  public ObservableCollection<FinancialProfitLoss> ProfitLosses { get; private set; } = [];
  public ObservableCollection<FinancialIndex> Indices { get; private set; } = [];
  public ObservableCollection<FinancialProfitability> Profitability { get; private set; } = [];

  public void Refresh() => RefreshAsync().Wait();
  public async Task RefreshAsync() {
    await Task.Run(() => {
      if (Api == null) return;
      lock (Statements) Statements.Clear();
      lock (ProfitLosses) ProfitLosses.Clear();
      lock (Indices) Indices.Clear();
      lock (Profitability) Profitability.Clear();
      GetFinancialStatements(Api.ApiClient, new() {
        Period = FinancialIndexQueries.QUARTERLY,
        Ticker = Label.Ticker,
      }, OnReceiveStatements, null);
      GetFinancialProfitLoss(Api.ApiClient, new() {
        Period = FinancialIndexQueries.QUARTERLY,
        Ticker = Label.Ticker,
      }, OnReceiveProfitLosses, null);
      GetFinancialIndex(Api.ApiClient, new() {
        Period = FinancialIndexQueries.QUARTERLY,
        Ticker = Label.Ticker,
      }, OnReceiveIndices, null);
      GetFinancialProfitability(Api.ApiClient, new() {
        Period = FinancialIndexQueries.QUARTERLY,
        Ticker = Label.Ticker,
      }, OnReceiveProfitability, null);
    });
  }

  private void OnReceiveStatements(string jsonString, bool hasNext, object? args) {
    if (ApiModel.DeserializeJson<FinancialStatementsResult>(jsonString) is not FinancialStatementsResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceiveStatements)}] {result.ResponseMessage}");
      return;
    }
    lock (Statements) foreach (var statement in result.Output!) {
      Statements.Add(statement.ToViewModelObject());
    }
  }
  private void OnReceiveIndices(string jsonString, bool hasNext, object? args) {
    if (ApiModel.DeserializeJson<FinancialIndexResult>(jsonString) is not FinancialIndexResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceiveIndices)}] {result.ResponseMessage}");
      return;
    }
    lock (Indices) foreach (var index in result.Output!) {
      Indices.Add(index.ToViewModelObject());
    }
  }
  private void OnReceiveProfitLosses(string jsonString, bool hasNext, object? args) {
    if (ApiModel.DeserializeJson<FinancialProfitLossResult>(jsonString) is not FinancialProfitLossResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceiveProfitLosses)}] {result.ResponseMessage}");
      return;
    }
    lock (ProfitLosses) foreach (var pl in result.Output!) {
      ProfitLosses.Add(pl.ToViewModelObject());
    }
  }
  private void OnReceiveProfitability(string jsonString, bool hasNext, object? args) {
    if (ApiModel.DeserializeJson<FinancialProfitabilityResult>(jsonString) is not FinancialProfitabilityResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceiveProfitability)}] {result.ResponseMessage}");
      return;
    }
    lock (Profitability) foreach (var pl in result.Output!) {
      Profitability.Add(pl.ToViewModelObject());
    }
  }
  
}