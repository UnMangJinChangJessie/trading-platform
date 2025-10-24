namespace trading_platform.ViewModel.KoreaInvestment.KoreaStock;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using trading_platform.Model.KoreaInvestment;
using static trading_platform.Model.KoreaInvestment.DomesticStock;
using MarketItemBase = ViewModel.MarketItem;

public partial class MarketItem : MarketItemBase {
  private (string Id, string Key)? WebSocketKeys { get; set; }
  public KisClients Api {
    get => field;
    set {
      if (field != value) {
        (ItemOrderBook as OrderBook)?.Api = value;
        field = value;
      }
    }
  }
  [ObservableProperty]
  public partial Exchange InquiringMarket { get; set; } = Exchange.None;
  private string TransactionId => InquiringMarket switch {
    Exchange.KoreaExchange => "H0STCNT0",
    Exchange.NexTrade => "H0NXCNT0",
    _ => "H0UNCNT0"
  };
  [ObservableProperty]
  public partial StockMetric Metric { get; set; }
  [ObservableProperty]
  public override partial ViewModel.OrderBook ItemOrderBook { get; protected set; }

  public MarketItem([MaybeNull] KisClients api) {
    Api = api;
    Metric = new();
    ItemOrderBook = new OrderBook(api, ItemLabel);
  }

  public async void OnReceivedChart(string jsonString, bool hasNextData, object? args) {
    if (ApiModel.DeserializeJson<ChartResult>(jsonString) is not ChartResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedChart)}] {result.ResponseMessage}");
      return;
    }
    // 캔들은 일자 기준 내림차순으로 정렬되어 주어짐
    lock (ItemChart) {
      ItemChart.ExtendBegin(result.Chart!.Select(x => new Model.ChartOHLC(x.Open, x.High, x.Low, x.Close) {
        Volume = x.Volume,
        Amount = x.Amount,
        Date = x.Date.ToDateTime(TimeOnly.MinValue)
      }), assumeSorted: true);
    }
    var (inquireFrom, inquireTo) = (ValueTuple<DateOnly, DateOnly>)args!;
    if (!result.Chart!.Any() || inquireFrom == result.Chart!.LastOrDefault()?.Date) {
      lock (ItemChart) {
        ItemChart.NotifyLoadComplete();
      }
      ItemOHLC.CurrentOpen = result.Information!.CurrentOpen;
      ItemOHLC.CurrentHigh = result.Information!.CurrentHigh;
      ItemOHLC.CurrentLow = result.Information!.CurrentLow;
      ItemOHLC.CurrentClose = result.Information!.CurrentClose;
      ItemOHLC.CurrentVolume = result.Information!.CurrentVolume;
      ItemOHLC.CurrentAmount = result.Information!.CurrentAmount;
      ItemOHLC.PreviousClose = result.Information!.PreviousClose;
      // 기본 재무지표 수신
      // PER, PBR 등 주가에 의존하는 값을 계산하기 위해 부득이 하게 차트를 전부 불러온 뒤에 요청함.
      GetFinancialIndex(
        Api!.ApiClient,
        new FinancialIndexQueries() {
          Period = FinancialIndexQueries.YEARLY,
          Ticker = ItemLabel.Ticker
        }, OnReceivedFinancialInformation, null
      );
      // 실시간 체결 데이터 요청
      WebSocketKeys = (TransactionId, ItemLabel.Ticker);
      await Api!.WebSocketClient.Subscribe(WebSocketKeys.Value.Id, WebSocketKeys.Value.Key, OnReceivedRealtimeConclusion);
    }
    else {
      inquireTo = result.Chart!.Last().Date.AddDays(-1);
      GetChart(
        Api.ApiClient,
        new ChartQueries() {
          Ticker = ItemLabel.Ticker,
          Exchange = Exchange.DomesticUnified,
          CandlePeriod = ItemChart.Span.ToKisCandlePeriod(),
          From = inquireFrom,
          To = inquireTo,
          Adjusted = true,
        },
        OnReceivedChart,
        (From: inquireFrom, To: inquireTo)
      );
    }
  }
  public void OnReceivedRealtimeConclusion(object? sender, WebSocketModel.MessageReceivedEventArgs args) {
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
    var time = TimeOnly.ParseExact(lastToken[1], "HHmmss");
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
    if (ApiModel.DeserializeJson<FinancialIndexResult>(jsonString) is not FinancialIndexResult result) return;
    if (result.ReturnCode != 0) {
      Debug.WriteLine($"[{result.ResponseMessageCode}, {nameof(OnReceivedChart)}] {result.ResponseMessage}");
      return;
    }
    var output = result.Output!.FirstOrDefault()!;
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
  private void SendChartRefreshRequests() {
    DateOnly from = DateOnly.FromDateTime(ItemChart.ChartDateBegin?.DateTime ?? DateTime.Today.AddDays(-280));
    DateOnly to = DateOnly.FromDateTime(ItemChart.ChartDateEnd?.DateTime.Date ?? DateTime.Today);
    GetChart(
      Api.ApiClient,
      new ChartQueries() {
        Ticker = ItemLabel.Ticker,
        Exchange = Exchange.DomesticUnified,
        CandlePeriod = ItemChart.Span.ToKisCandlePeriod(),
        From = from,
        To = to,
        Adjusted = true,
      },
      OnReceivedChart,
      (From: from, To: to)
    );
  }
  public override void Refresh() {
    if (Api == null) return;
    if (WebSocketKeys != null) Api!.WebSocketClient.Unsubscribe(WebSocketKeys.Value.Id, WebSocketKeys.Value.Key).Wait();
    lock (ItemChart) {
      ItemChart.Clear();
    }
    SendChartRefreshRequests();
    // 호가 갱신
    ItemOrderBook.Refresh();
  }
  public override async Task RefreshAsync() {
    if (Api == null) return;
    if (WebSocketKeys != null) await Api!.WebSocketClient.Unsubscribe(WebSocketKeys.Value.Id, WebSocketKeys.Value.Key);
    lock (ItemChart) {
      ItemChart.Clear();
    }
    SendChartRefreshRequests();
    // 호가 갱신
    ItemOrderBook.Refresh();
  }
  ~MarketItem() {
    if (WebSocketKeys != null) Api.WebSocketClient.Unsubscribe(WebSocketKeys.Value.Id, WebSocketKeys.Value.Key).Wait();
  }
}