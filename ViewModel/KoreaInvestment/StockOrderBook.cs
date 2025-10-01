
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
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
  public override async ValueTask<bool> RequestRefreshAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is null) return false;
    var (status, result) = await DomesticStock.InquireOrderBook(new() {
      MarketClassification = Exchange.DomesticUnified,
      Ticker = ticker
    });
    if (status != System.Net.HttpStatusCode.OK || result == null) return false;
    AskPrice[0].Value = result.Output?.AskPrice_1 ?? 0;
    AskPrice[1].Value = result.Output?.AskPrice_2 ?? 0;
    AskPrice[2].Value = result.Output?.AskPrice_3 ?? 0;
    AskPrice[3].Value = result.Output?.AskPrice_4 ?? 0;
    AskPrice[4].Value = result.Output?.AskPrice_5 ?? 0;
    AskPrice[5].Value = result.Output?.AskPrice_6 ?? 0;
    AskPrice[6].Value = result.Output?.AskPrice_7 ?? 0;
    AskPrice[7].Value = result.Output?.AskPrice_8 ?? 0;
    AskPrice[8].Value = result.Output?.AskPrice_9 ?? 0;
    AskPrice[9].Value = result.Output?.AskPrice_10 ?? 0;
    BidPrice[0].Value = result.Output?.BidPrice_1 ?? 0;
    BidPrice[1].Value = result.Output?.BidPrice_2 ?? 0;
    BidPrice[2].Value = result.Output?.BidPrice_3 ?? 0;
    BidPrice[3].Value = result.Output?.BidPrice_4 ?? 0;
    BidPrice[4].Value = result.Output?.BidPrice_5 ?? 0;
    BidPrice[5].Value = result.Output?.BidPrice_6 ?? 0;
    BidPrice[6].Value = result.Output?.BidPrice_7 ?? 0;
    BidPrice[7].Value = result.Output?.BidPrice_8 ?? 0;
    BidPrice[8].Value = result.Output?.BidPrice_9 ?? 0;
    BidPrice[9].Value = result.Output?.BidPrice_10 ?? 0;
    AskQuantity[0].Value = result.Output?.AskQuantity_1 ?? 0;
    AskQuantity[1].Value = result.Output?.AskQuantity_2 ?? 0;
    AskQuantity[2].Value = result.Output?.AskQuantity_3 ?? 0;
    AskQuantity[3].Value = result.Output?.AskQuantity_4 ?? 0;
    AskQuantity[4].Value = result.Output?.AskQuantity_5 ?? 0;
    AskQuantity[5].Value = result.Output?.AskQuantity_6 ?? 0;
    AskQuantity[6].Value = result.Output?.AskQuantity_7 ?? 0;
    AskQuantity[7].Value = result.Output?.AskQuantity_8 ?? 0;
    AskQuantity[8].Value = result.Output?.AskQuantity_9 ?? 0;
    AskQuantity[9].Value = result.Output?.AskQuantity_10 ?? 0;
    BidQuantity[0].Value = result.Output?.BidQuantity_1 ?? 0;
    BidQuantity[1].Value = result.Output?.BidQuantity_2 ?? 0;
    BidQuantity[2].Value = result.Output?.BidQuantity_3 ?? 0;
    BidQuantity[3].Value = result.Output?.BidQuantity_4 ?? 0;
    BidQuantity[4].Value = result.Output?.BidQuantity_5 ?? 0;
    BidQuantity[5].Value = result.Output?.BidQuantity_6 ?? 0;
    BidQuantity[6].Value = result.Output?.BidQuantity_7 ?? 0;
    BidQuantity[7].Value = result.Output?.BidQuantity_8 ?? 0;
    BidQuantity[8].Value = result.Output?.BidQuantity_9 ?? 0;
    BidQuantity[9].Value = result.Output?.BidQuantity_10 ?? 0;
    HighestQuantity = Math.Max(BidQuantity.Max(x => x.Value), AskQuantity.Max(x => x.Value));
    ConclusionTime = result.Output?.Time ?? TimeOnly.MinValue;
    OnPropertyChanged(propertyName: null);
    return true;
  }
  public override async ValueTask<bool> RequestRefreshRealTimeAsync(string ticker) {
    if (KRXStock.SearchByTicker(ticker) is null) return false;
    Ticker = ticker;
    await ApiClient.KisWebSocket.Subscribe("H0UNASP0", ticker);
    RealTimeRefresh = true;
    return RealTimeRefresh;
  }
  public override async Task EndRefreshRealTimeAsync(string ticker) {
    await ApiClient.KisWebSocket.Unsubscribe("H0UNASP0", ticker);
    RealTimeRefresh = false;
  }
}