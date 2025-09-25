using Avalonia.Controls;
using trading_platform.KoreaInvestment;

namespace trading_platform.View;

public partial class KoreaStock : UserControl {
  private KisWebSocket? PriceWebSocket { get; set; } = null;
  private KisWebSocket? BiddingWebSocket { get; set; } = null;
  private KisWebSocket? ConclusionWebSocket { get; set; } = null;
  private ViewModel.MarketData? CastedDataContext => DataContext as ViewModel.MarketData;
  public KoreaStock() {
    InitializeComponent();
    // API 토큰이 발급되었다면(즉, 만료시각 전이라면) 실시간 체결/가격/호가 정보 WebSocket 개방.
    if (string.IsNullOrEmpty(ApiClient.WebSocketAccessToken)) {
      // 실시간 체결가
      // TODO: make 'AccountId' property to ApiClient then open conclusion websocket here.
    }
  }
  public void UpdateClose(decimal close) {
    CastedDataContext!.PriceChart[^1].Close = close;
  }
}