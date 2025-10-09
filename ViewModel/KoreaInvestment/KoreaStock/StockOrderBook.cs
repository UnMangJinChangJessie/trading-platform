
using System.Text.Json;
using System.Text.Json.Nodes;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.VisualBasic;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.ViewModel.KoreaInvestment;

public partial class StockOrderBook : OrderBook {
  public StockOrderBook() {
    ApiClient.KisWebSocket.MessageReceived += (sender, args) => {
      if (args.TransactionId != "H0UNASP0" && args.TransactionId != "H0STASP0" && args.TransactionId != "H0NXASP0") return; // 통합 | KRX | NexTrade
      if (args.Message.Count == 0) return;
      if (args.Message[^1][0] != Ticker) return;
      if (args.TransactionId == "H0UNASP0" || args.TransactionId == "H0NXASP0") {
        for (int i = 0; i < 10; i++) {
          AskPrice[i].Value = decimal.Parse(args.Message[^1][3 + i]);
          BidPrice[i].Value = decimal.Parse(args.Message[^1][13 + i]);
          AskQuantity[i].Value = decimal.Parse(args.Message[^1][23 + i]);
          BidQuantity[i].Value = decimal.Parse(args.Message[^1][33 + i]);
        }
        decimal krxIntermediateQuantity = decimal.Parse(args.Message[^1][60]);
        bool krxIntermediateExists = args.Message[^1][61] != "0";
        bool krxIntermediateAsking = args.Message[^1][61] == "1";
        decimal nxtIntermediateQuantity = decimal.Parse(args.Message[^1][63]);
        bool nxtIntermediateExists = args.Message[^1][64] != "0";
        bool nxtIntermediateAsking = args.Message[^1][64] == "1";
        decimal intermediatePrice = decimal.Parse(args.Message[^1][59]);
        bool intermediateDoesNotExist = !nxtIntermediateExists && !krxIntermediateExists;
        if (intermediateDoesNotExist) {
          IntermediatePrice = null;
          IntermediateAskQuantity = null;
          IntermediateBidQuantity = null;
        }
        else {
          IntermediatePrice = intermediatePrice;
          decimal ask = (krxIntermediateAsking ? krxIntermediateQuantity : 0) + (nxtIntermediateAsking ? nxtIntermediateQuantity : 0);
          decimal bid = (krxIntermediateAsking ? 0 : krxIntermediateQuantity) + (nxtIntermediateAsking ? 0 : nxtIntermediateQuantity);
          IntermediateAskQuantity = ask == 0 ? null : ask;
          IntermediateBidQuantity = bid == 0 ? null : bid;
        }
        ConclusionTime = TimeOnly.ParseExact(args.Message[^1][1], "HHmmss");
        HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
        OnPropertyChanged(propertyName: null);
      }
      else {
        for (int i = 0; i < 10; i++) {
          AskPrice[i].Value = decimal.Parse(args.Message[^1][3 + i]);
          BidPrice[i].Value = decimal.Parse(args.Message[^1][13 + i]);
          AskQuantity[i].Value = decimal.Parse(args.Message[^1][23 + i]);
          BidQuantity[i].Value = decimal.Parse(args.Message[^1][33 + i]);
        }
        IntermediatePrice = null;
        IntermediateAskQuantity = null;
        IntermediateBidQuantity = null;
        ConclusionTime = TimeOnly.ParseExact(args.Message[^1][1], "HHmmss");
        HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
      }
    };
    if (Design.IsDesignMode) {
      for (int i = 0; i < 10; i++) {
        AskPrice[i].Value = 450.00M + i * 0.05M;
        BidPrice[i].Value = 450.00M - (i + 1) * 0.05M;
        AskQuantity[i].Value = Random.Shared.Next(1, 50 - 5 * i);
        BidQuantity[i].Value = Random.Shared.Next(1, 50 - 5 * i);
      }
      HighestQuantity = Math.Max(AskQuantity.Max(x => x.Value), BidQuantity.Max(x => x.Value));
    }
  }
  public void OnReceiveMessage(string jsonString) {
    StockInquireOrderBookResult json;
    try {
      json = JsonSerializer.Deserialize<StockInquireOrderBookResult>(jsonString, ApiClient.JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return;
    }
    if (json!.ReturnCode != 0) return;
    var result = json.Output!;
    AskPrice[0].Value = result.AskPrice_1;
    AskPrice[1].Value = result.AskPrice_2;
    AskPrice[2].Value = result.AskPrice_3;
    AskPrice[3].Value = result.AskPrice_4;
    AskPrice[4].Value = result.AskPrice_5;
    AskPrice[5].Value = result.AskPrice_6;
    AskPrice[6].Value = result.AskPrice_7;
    AskPrice[7].Value = result.AskPrice_8;
    AskPrice[8].Value = result.AskPrice_9;
    AskPrice[9].Value = result.AskPrice_10;
    BidPrice[0].Value = result.BidPrice_1;
    BidPrice[1].Value = result.BidPrice_2;
    BidPrice[2].Value = result.BidPrice_3;
    BidPrice[3].Value = result.BidPrice_4;
    BidPrice[4].Value = result.BidPrice_5;
    BidPrice[5].Value = result.BidPrice_6;
    BidPrice[6].Value = result.BidPrice_7;
    BidPrice[7].Value = result.BidPrice_8;
    BidPrice[8].Value = result.BidPrice_9;
    BidPrice[9].Value = result.BidPrice_10;
    AskQuantity[0].Value = result.AskQuantity_1;
    AskQuantity[1].Value = result.AskQuantity_2;
    AskQuantity[2].Value = result.AskQuantity_3;
    AskQuantity[3].Value = result.AskQuantity_4;
    AskQuantity[4].Value = result.AskQuantity_5;
    AskQuantity[5].Value = result.AskQuantity_6;
    AskQuantity[6].Value = result.AskQuantity_7;
    AskQuantity[7].Value = result.AskQuantity_8;
    AskQuantity[8].Value = result.AskQuantity_9;
    AskQuantity[9].Value = result.AskQuantity_10;
    BidQuantity[0].Value = result.BidQuantity_1;
    BidQuantity[1].Value = result.BidQuantity_2;
    BidQuantity[2].Value = result.BidQuantity_3;
    BidQuantity[3].Value = result.BidQuantity_4;
    BidQuantity[4].Value = result.BidQuantity_5;
    BidQuantity[5].Value = result.BidQuantity_6;
    BidQuantity[6].Value = result.BidQuantity_7;
    BidQuantity[7].Value = result.BidQuantity_8;
    BidQuantity[8].Value = result.BidQuantity_9;
    BidQuantity[9].Value = result.BidQuantity_10;
    HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
    ConclusionTime = result.Time;
  }
  public override async Task RefreshAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (KRXStock.SearchByTicker(ticker) is not KRXStockInformation info) return;
    DomesticStock.GetOrderBook(
      new() {
        Ticker = ticker,
        MarketClassification = info.Exchange
      },
      OnReceiveMessage
    );
  }
  public override async Task StartRefreshRealtimeAsync(IDictionary<string, object> args) {
    if (!args.TryGetValue("ticker", out var tickerObject) || tickerObject is not string ticker) return;
    if (KRXStock.SearchByTicker(ticker) is null) return;
    Ticker = ticker;
    await ApiClient.KisWebSocket.Subscribe("H0UNASP0", ticker);
    RealTimeRefresh = true;
  }
  public override async Task EndRefreshRealtimeAsync(IDictionary<string, object> args) {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNASP0", Ticker);
    RealTimeRefresh = false;
  }
}