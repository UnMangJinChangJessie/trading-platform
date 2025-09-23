using System.Diagnostics;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class KoreaStock : UserControl {
  private KisWebSocket? PriceWebSocket { get; set; } = null;
  private KisWebSocket? BiddingWebSocket { get; set; } = null;
  private KisWebSocket? ConclusionWebSocket { get; set; } = null;
  public KoreaStock() {
    InitializeComponent();
    // API 토큰이 발급되었다면(즉, 만료시각 전이라면) 실시간 체결/가격/호가 정보 WebSocket 개방.
    // 디자인 모드에서는 1초마다 주가가 바뀌도록 시뮬레이션 (how?)
    if (Design.IsDesignMode) {
      // Invoke StockChartAndOrder.OHLCChart.UpdateClose with a random price for every one second.
      var timer = new System.Timers.Timer() {
        Interval = 1000.0,
        AutoReset = true,
      };
      timer.Elapsed += (s, e) => UpdateClose((decimal)(
        StockChartAndOrder.OHLCChart.CandlesticksSource[^1].Close + Random.Shared.NextDouble()
      ));
      timer.Start();
    }
    else if (string.IsNullOrEmpty(ApiClient.WebSocketAccessToken)) {
      // 실시간 체결가
      // TODO: make 'AccountId' property to ApiClient then open conclusion websocket here.
    }
  }
  public void UpdateClose(decimal close) {
    StockChartAndOrder.OHLCChart.UpdateClose((double)close);
    if (StockChartAndOrder.PriceDisplay.DataContext is ViewModel.PriceDisplay context) context.CurrentPrice = close;
  }
}