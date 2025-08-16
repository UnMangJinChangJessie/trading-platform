using System.Net.Http.Json;

namespace TradingSystem.KoreaInvestment;

public static partial class ApiClient {
  public static readonly TimeSpan REQUEST_RATE_LIMIT = TimeSpan.FromMilliseconds(50);
  public static DateTime LastRequestTime { get; private set; } = DateTime.Now;
  public static bool Personal { get; set; } = true;
  public static bool Simulation { get; set; } = false;
  public static string AppPublicKey { get; set; } = "";
  public static string AppSecretKey { get; set; } = "";
  private static string AccessToken { get; set; } = default!;
  private static string WebSocketAccessToken { get; set; } = default!;
  private static DateTime AccessTokenExpire { get; set; } = DateTime.Now;
  private static HttpClient RequestClient = new() {
    BaseAddress = new Uri($"https://openapi.koreainvestment.com:{(Simulation ? 9443 : 29443)}"),
    Timeout = TimeSpan.FromSeconds(5)
  };
  public static async Task<HttpResponseMessage> Request(
    string tradeId, 
    HttpMethod method, string relUri,
    IEnumerable<(string Key, string Value)> headers,
    IEnumerable<(string Key, string Value)> queries,
    object? body
  ) {
    if (DateTime.Now >= AccessTokenExpire) {
      await IssueToken();
    }
    TimeSpan restSpan = DateTime.Now - LastRequestTime;
    if (restSpan < REQUEST_RATE_LIMIT) {
      await Task.Delay(restSpan);
    }
    UriBuilder builder = new();
    builder.Host = relUri;
    builder.Query = Common.BuildQueryString(queries);
    HttpRequestMessage message = new(method, builder.Uri);
    message.Headers.Add("appkey", AppPublicKey);
    message.Headers.Add("appsecret", AppSecretKey);
    message.Headers.Add("authorization", $"Bearer {AccessToken}");
    message.Headers.Add("tr_id", tradeId);
    message.Headers.Add("custtype", Personal ? "P" : "B");
    foreach (var (key, value) in headers) {
      message.Headers.Add(key, value);
    }
    if (body != null) message.Content = JsonContent.Create(body);
    return await RequestClient.SendAsync(message);
  }
}