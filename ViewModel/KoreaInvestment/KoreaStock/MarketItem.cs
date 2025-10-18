namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using MarketItemBase = ViewModel.MarketItem;

public partial class MarketItem : MarketItemBase {
  private string? WebSocketTicker { get; set; }
  [ObservableProperty]
  public partial StockMetric Metric { get; set; }
  public async void OnReceivedChart(string jsonString, bool hasNextData, object? args) {
    if (ApiClient.DeserializeJson<ChartResult>(jsonString) is not ChartResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedChart)}] {result.ResponseMessage}");
      return;
    }
    if (!result.Chart!.Any()) return;
    // 캔들은 일자 기준 내림차순으로 정렬되어 주어짐
    lock (ItemChart) {
      ItemChart.ExtendBegin(result.Chart!.Select(x => new Model.ChartOHLC(x.Open, x.High, x.Low, x.Close) {
        Volume = x.Volume,
        Amount = x.Amount,
        Date = x.Date.ToDateTime(TimeOnly.MinValue)
      }), assumeSorted: true);
    }
    if ((bool)args!) {
      ItemOHLC.CurrentOpen = result.Information!.CurrentOpen;
      ItemOHLC.CurrentHigh = result.Information!.CurrentHigh;
      ItemOHLC.CurrentLow = result.Information!.CurrentLow;
      ItemOHLC.CurrentClose = result.Information!.CurrentClose;
      ItemOHLC.CurrentVolume = result.Information!.CurrentVolume;
      ItemOHLC.CurrentAmount = result.Information!.CurrentAmount;
      ItemOHLC.PreviousClose = result.Information!.PreviousClose;
      // 기본 재무지표 수신
      // PER, PBR 등 주가에 의존하는 값을 계산하기 위해 부득이 하게 차트를 전부 불러온 뒤에 요청함.
      GetFinancialIndex(new FinancialIndexQueries() {
        Period = FinancialIndexQueries.QUARTERLY,
        Ticker = ItemLabel.Ticker
      }, OnReceivedFinancialInformation, null);
      // 실시간 체결 데이터 요청
      if (WebSocketTicker != null) await ApiClient.KisWebSocket.Unsubscribe("H0UNCNT0", WebSocketTicker);
      WebSocketTicker = ItemLabel.Ticker;
      await ApiClient.KisWebSocket.Subscribe("H0UNCNT0", ItemLabel.Ticker, OnReceivedRealtimeConclusion);
    }
  }
  public void OnReceivedRealtimeConclusion(object? sender, ApiClient.KisWebSocket.MessageReceivedEventArgs args) {
    if (args.Tokens.Length == 0) return;
    var lastToken = args.Tokens[^1];
    var open = ulong.Parse(lastToken[7]);
    var high = ulong.Parse(lastToken[8]);
    var low = ulong.Parse(lastToken[9]);
    var close = ulong.Parse(lastToken[2]);
    var change = long.Parse(lastToken[4]);
    var previous = (long)close - change;
    var volume = ulong.Parse(lastToken[13]);
    var amount = ulong.Parse(lastToken[14]);
    var date = DateOnly.ParseExact(lastToken[33], "yyyyMMdd");
    var time = TimeOnly.ParseExact(lastToken[1], "hhmmss");
    var dateTime = new DateTime(date, time);
    lock (ItemOHLC) {
      ItemOHLC.CurrentOpen = open;
      ItemOHLC.CurrentHigh = high;
      ItemOHLC.CurrentLow = low;
      ItemOHLC.CurrentClose = close;
      ItemOHLC.CurrentVolume = volume;
      ItemOHLC.CurrentAmount = amount;
      ItemOHLC.PreviousClose = previous;
      ItemOHLC.CurrentDateTime = dateTime;
    }
    lock (ItemChart) {
      ItemChart.UpdateEnd(new(open, high, low, close) { Date = dateTime, Volume = volume, Amount = amount });
    }
  }
  public void OnReceivedFinancialInformation(string jsonString, bool hasNextData, object? args) {
    if (ApiClient.DeserializeJson<FinancialIndexResult>(jsonString) is not FinancialIndexResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedChart)}] {result.ResponseMessage}");
      return;
    }
    var output = result.Output!;
    decimal currentClose;
    lock (ItemOHLC) {
      currentClose = ItemOHLC.CurrentClose;
    }
    lock (Metric) {
      Metric.BookValuePerShare = output.BookValuePerShare;
      Metric.DebtRate = output.DebtRate;
      Metric.EarningPerShare = output.EarningPerShare;
      Metric.PriceBookValueRate = output.BookValuePerShare == 0 ? 0 : (float)currentClose / (float)output.BookValuePerShare;
      Metric.PriceEarningRate = output.EarningChangeRate == 0 ? 0 : (float)currentClose / (float)output.EarningPerShare;
      Metric.ReturnOnEquity = output.ReturnOnEquity;
    }
  }
  public override void Refresh() {
    lock (ItemChart) {
      ItemChart.Clear();
    }
    // 차트 갱신
    var from = ItemChart.ChartDateBegin?.DateTime.Date?? DateTime.Today.AddDays(-280);
    var to = ItemChart.ChartDateEnd?.DateTime.Date ?? DateTime.Today;
    do {
      var newTo = to.AddDays(-140);
      var currentFrom = to.AddDays(1);
      currentFrom = from < currentFrom ? currentFrom : from;
      // from부터 to까지 주어진 기간을 140일 단위로 나누어 수신함.
      GetChart(new ChartQueries() {
        Ticker = ItemLabel.Ticker,
        Exchange = Exchange.DomesticUnified,
        CandlePeriod = ItemChart.Span.ToKisCandlePeriod(),
        From = DateOnly.FromDateTime(currentFrom),
        To = DateOnly.FromDateTime(to),
        Adjusted = true,
      }, OnReceivedChart, from > newTo);
      to = newTo;
    }
    while (from <= to);
    // 호가 갱신
    ItemOrderBook.Refresh();
  }
  public override Task RefreshAsync() {
    Refresh();
    return Task.CompletedTask;
  }
}