using System.Net.WebSockets;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using trading_platform.KoreaInvestment;
using static trading_platform.Model.StockMarketInformation;

namespace trading_platform.View;

public partial class KoreaStockChart : UserControl {
  private KisWebSocket? PriceWebSocket { get; set; } = null;
  private KisWebSocket? OrderBookWebSocket { get; set; } = null;
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  private byte[] WebSocketBuffer { get; set; } = new byte[8192]; // 8 KiB면 충분하겠지?
  private DispatcherTimer? Timer { get; set; }
  public KoreaStockChart() {
    InitializeComponent();
    if (CastedDataContext != null) CastedDataContext.Currency = "원";
  }
  private KRXStockInformation? SearchByTicker(string ticker) {
    return KRXStock.Data.Where(x => x.Ticker == ticker).FirstOrDefault();
  }
  private async Task OpenWebSockets(string ticker) {
    PriceWebSocket = await ApiClient.OpenWebSocket("/tryitout/H0UNCNT0", "H0UNCNT0", ticker);
    OrderBookWebSocket = await ApiClient.OpenWebSocket("/tryitout/H0UNASP0", "H0UNASP0", ticker);
    Timer = new(TimeSpan.FromMilliseconds(20), DispatcherPriority.Background, PollWebSockets);
    Timer.Start();
  }
  private async Task CloseWebSockets() {
    if (Timer != null && Timer.IsEnabled) Timer.Stop();
    await ApiClient.CloseWebSocket(PriceWebSocket);
    await ApiClient.CloseWebSocket(OrderBookWebSocket);
  }
  private async Task FetchCurrentPrice(string ticker) {
    if (CastedDataContext == null) return;
    var (status, result) = await DomesticStock.InquireStockPrice(new() {
      Ticker = ticker,
      Exchange = Exchange.DomesticUnified
    });
    if (result == null) return;
    if (status != System.Net.HttpStatusCode.OK || result.Information == null) {
      return;
    }
    CastedDataContext.CurrentOpen = result.Information.Open;
    CastedDataContext.CurrentHigh = result.Information.High;
    CastedDataContext.CurrentLow = result.Information.Low;
    CastedDataContext.CurrentClose = result.Information.Close;
    CastedDataContext.CurrentVolume = result.Information.Volume;
    CastedDataContext.CurrentAmount = result.Information.Amount;
    CastedDataContext.EarningPerShare = result.Information.EarningsPerShare;
    CastedDataContext.PriceBookValueRatio = result.Information.PriceBookValueRatio;
    CastedDataContext.PriceEarningsRatio = result.Information.PriceEarningsRate;
  }
  private async Task FetchOrderPrice(string ticker) {
    if (CastedDataContext == null) return;
    var (status, result) = await DomesticStock.InquireOrderBook(new() {
      MarketClassification = Exchange.DomesticUnified,
      Ticker = ticker
    });
    if (result is null) return;
    if (status != System.Net.HttpStatusCode.OK || result.Output == null) {
      return;
    }
    {
      CastedDataContext.CurrentOrderBook.SellingPrices[0] = result.Output.BidPrice_1;
      CastedDataContext.CurrentOrderBook.SellingPrices[1] = result.Output.BidPrice_2;
      CastedDataContext.CurrentOrderBook.SellingPrices[2] = result.Output.BidPrice_3;
      CastedDataContext.CurrentOrderBook.SellingPrices[3] = result.Output.BidPrice_4;
      CastedDataContext.CurrentOrderBook.SellingPrices[4] = result.Output.BidPrice_5;
      CastedDataContext.CurrentOrderBook.SellingPrices[5] = result.Output.BidPrice_6;
      CastedDataContext.CurrentOrderBook.SellingPrices[6] = result.Output.BidPrice_7;
      CastedDataContext.CurrentOrderBook.SellingPrices[7] = result.Output.BidPrice_8;
      CastedDataContext.CurrentOrderBook.SellingPrices[8] = result.Output.BidPrice_9;
      CastedDataContext.CurrentOrderBook.SellingPrices[9] = result.Output.BidPrice_10;
      CastedDataContext.CurrentOrderBook.BuyingPrices[0] = result.Output.AskPrice_1;
      CastedDataContext.CurrentOrderBook.BuyingPrices[1] = result.Output.AskPrice_2;
      CastedDataContext.CurrentOrderBook.BuyingPrices[2] = result.Output.AskPrice_3;
      CastedDataContext.CurrentOrderBook.BuyingPrices[3] = result.Output.AskPrice_4;
      CastedDataContext.CurrentOrderBook.BuyingPrices[4] = result.Output.AskPrice_5;
      CastedDataContext.CurrentOrderBook.BuyingPrices[5] = result.Output.AskPrice_6;
      CastedDataContext.CurrentOrderBook.BuyingPrices[6] = result.Output.AskPrice_7;
      CastedDataContext.CurrentOrderBook.BuyingPrices[7] = result.Output.AskPrice_8;
      CastedDataContext.CurrentOrderBook.BuyingPrices[8] = result.Output.AskPrice_9;
      CastedDataContext.CurrentOrderBook.BuyingPrices[9] = result.Output.AskPrice_10;
      CastedDataContext.CurrentOrderBook.SellingQuantities[0] = result.Output.BidQuantity_1;
      CastedDataContext.CurrentOrderBook.SellingQuantities[1] = result.Output.BidQuantity_2;
      CastedDataContext.CurrentOrderBook.SellingQuantities[2] = result.Output.BidQuantity_3;
      CastedDataContext.CurrentOrderBook.SellingQuantities[3] = result.Output.BidQuantity_4;
      CastedDataContext.CurrentOrderBook.SellingQuantities[4] = result.Output.BidQuantity_5;
      CastedDataContext.CurrentOrderBook.SellingQuantities[5] = result.Output.BidQuantity_6;
      CastedDataContext.CurrentOrderBook.SellingQuantities[6] = result.Output.BidQuantity_7;
      CastedDataContext.CurrentOrderBook.SellingQuantities[7] = result.Output.BidQuantity_8;
      CastedDataContext.CurrentOrderBook.SellingQuantities[8] = result.Output.BidQuantity_9;
      CastedDataContext.CurrentOrderBook.SellingQuantities[9] = result.Output.BidQuantity_10;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[0] = result.Output.AskQuantity_1;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[1] = result.Output.AskQuantity_2;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[2] = result.Output.AskQuantity_3;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[3] = result.Output.AskQuantity_4;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[4] = result.Output.AskQuantity_5;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[5] = result.Output.AskQuantity_6;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[6] = result.Output.AskQuantity_7;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[7] = result.Output.AskQuantity_8;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[8] = result.Output.AskQuantity_9;
      CastedDataContext.CurrentOrderBook.BuyingQuantities[9] = result.Output.AskQuantity_10;
    }
  }
  private async void PollWebSockets(object? sender, EventArgs args) {
    if (CastedDataContext == null) return;
    WebSocketReceiveResult result;
    string data;
    List<string[]> tokens;
    if (PriceWebSocket != null) {
      data = "";
      do {
        result = await PriceWebSocket.Client.ReceiveAsync(WebSocketBuffer, CancellationToken.None);
        data += KisWebSocket.SplitWebSocketMessage(Encoding.UTF8.GetString(WebSocketBuffer[..result.Count]));
      }
      while (!result.EndOfMessage);
      tokens = KisWebSocket.SplitWebSocketMessage(data).Message;
      foreach (var line in tokens) {
        CastedDataContext.CurrentClose = decimal.Parse(line[2]);
        CastedDataContext.CurrentOrderBook.LastConclusion = CastedDataContext.CurrentClose;
        CastedDataContext.CurrentOrderBook.PreviousClose = CastedDataContext.CurrentClose - decimal.Parse(line[4]);
        CastedDataContext.CurrentOpen = decimal.Parse(line[8]);
        CastedDataContext.CurrentHigh = decimal.Parse(line[9]);
        CastedDataContext.CurrentLow = decimal.Parse(line[10]);
        CastedDataContext.CurrentVolume = decimal.Parse(line[14]);
        CastedDataContext.CurrentOrderBook.Volume = CastedDataContext.CurrentVolume;
        CastedDataContext.CurrentAmount = decimal.Parse(line[15]);
      }
    }
    if (OrderBookWebSocket != null) {
      data = "";
      do {
        result = await OrderBookWebSocket.Client.ReceiveAsync(WebSocketBuffer, CancellationToken.None);
        data += KisWebSocket.SplitWebSocketMessage(Encoding.UTF8.GetString(WebSocketBuffer[..result.Count]));
      }
      while (!result.EndOfMessage);
      tokens = KisWebSocket.SplitWebSocketMessage(data).Message;
      foreach (var line in tokens) {
        for (int i = 0; i < 10; i++) {
          CastedDataContext.CurrentOrderBook.SellingPrices[i] = decimal.Parse(line[3 + i]);
          CastedDataContext.CurrentOrderBook.BuyingPrices[i] = decimal.Parse(line[13 + i]);
          CastedDataContext.CurrentOrderBook.SellingQuantities[i] = decimal.Parse(line[23 + i]);
          CastedDataContext.CurrentOrderBook.BuyingQuantities[i] = decimal.Parse(line[33 + i]);
        }
      }
    }
  }
  public async void UserControl_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    if (CastedDataContext == null) return;
    var ticker = CastedDataContext.Ticker.Trim();
    var searchResult = SearchByTicker(ticker);
    if (searchResult == null) return;
    CastedDataContext.Name = searchResult.Name;
    await FetchCurrentPrice(searchResult.Ticker);
    await FetchOrderPrice(searchResult.Ticker);
    await CloseWebSockets();
    await OpenWebSockets(searchResult.Ticker);
  }
  public async void UserControl_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs args) {
    await CloseWebSockets();
  }
  public async void TickerInquireButton_Click(object? sender, RoutedEventArgs args) {
    if (CastedDataContext == null) return;
    var ticker = CastedDataContext.Ticker.Trim();
    var searchResult = SearchByTicker(ticker);
    if (searchResult == null) return;
    CastedDataContext.Name = searchResult.Name;
    await FetchCurrentPrice(searchResult.Ticker);
    await FetchOrderPrice(searchResult.Ticker);
    await CloseWebSockets();
    await OpenWebSockets(searchResult.Ticker);
  }
}