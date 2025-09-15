using System.Net;
using System.Net.Http.Json;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json;

namespace trading_platform.KoreaInvestment;

public static partial class ApiClient {
  public static readonly TimeSpan REQUEST_RATE_LIMIT = TimeSpan.FromMilliseconds(50);
  public static DateTime LastRequestTime { get; private set; } = DateTime.Now;
  public static bool Personal { get; set; } = true;
  public static bool Simulation { get; private set; } = false;
  public static string AppPublicKey { get; set; } = "";
  public static string AppSecretKey { get; set; } = "";
  private static string AccessToken { get; set; } = default!;
  private static string WebSocketAccessToken { get; set; } = default!;
  public static DateTime AccessTokenExpire { get; private set; } = DateTime.Now;
  private readonly static HttpClient RequestClient = new() {
    BaseAddress = BuildApiBaseAddress(),
    Timeout = TimeSpan.FromSeconds(5)
  };
  private readonly static JsonSerializerOptions JsonSerializerOption = new() {
    NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString,
    AllowTrailingCommas = true,
    Converters = {
      new DateToStringConverter(),
      new TimeToStringConverter(),
      new StringToBooleanConverter(),
    }
  };
  public static bool ToggleSimulation() {
    Simulation = !Simulation;
    RequestClient.BaseAddress = BuildApiBaseAddress();
    return Simulation;
  }
  public static Uri BuildApiBaseAddress() {
    UriBuilder builder = new($"https://openapi.koreainvestment.com");
    builder.Port = Simulation ? 29443 : 9443;
    return builder.Uri;
  }
  public static string GetFirstThreeTokenChar() {
    return AccessToken[..Math.Min(3, AccessToken.Length)] + "...";
  }
  public static async Task<(HttpStatusCode StatusCode, TResult? Result)> Request<TBody, TResult>(
    string transId, HttpMethod method, string relUri,
    IDictionary<string, string>? header,
    IDictionary<string, string>? queries,
    TBody? body
  ) where TResult : KisReturnMessage {
    if (DateTime.Now >= AccessTokenExpire) {
      await IssueToken();
    }
    TimeSpan restSpan = DateTime.Now - LastRequestTime;
    if (restSpan < REQUEST_RATE_LIMIT) {
      await Task.Delay(restSpan);
    }
    var builder = new UriBuilder {
      Host = relUri,
      Query = queries != null ? Common.BuildQueryString(queries) : ""
    };
    HttpRequestMessage message = new(method, builder.Uri);
    message.Headers.Add("appkey", AppPublicKey);
    message.Headers.Add("appsecret", AppSecretKey);
    message.Headers.Add("authorization", $"Bearer {AccessToken}");
    message.Headers.Add("tr_id", transId);
    message.Headers.Add("custtype", Personal ? "P" : "B");
    if (header != null) foreach (var (key, value) in header) {
        message.Headers.Add(key, value);
      }
    if (body != null) message.Content = JsonContent.Create(body);
    var response = await RequestClient.SendAsync(message);
    var bodyJson = await response.Content.ReadFromJsonAsync<TResult>(JsonSerializerOption);
    return (response.StatusCode, bodyJson);
  }
  public static async Task<(HttpStatusCode StatusCode, TResult? Result)> RequestConsecutive<TBody, TResult>(
    string tradeId, HttpMethod method, string relUri,
    IDictionary<string, string>? header,
    IDictionary<string, string>? queries,
    TBody? body
  ) where TBody : IConsecutive where TResult : KisReturnMessage, IReturnConsecutive {
    if (DateTime.Now >= AccessTokenExpire) {
      await IssueToken();
    }
    TimeSpan restSpan = DateTime.Now - LastRequestTime;
    if (restSpan < REQUEST_RATE_LIMIT) {
      await Task.Delay(restSpan);
    }
    var builder = new UriBuilder {
      Host = relUri,
      Query = queries != null ? Common.BuildQueryString(queries) : ""
    };
    HttpRequestMessage message = new(method, builder.Uri);
    message.Headers.Add("appkey", AppPublicKey);
    message.Headers.Add("appsecret", AppSecretKey);
    message.Headers.Add("authorization", $"Bearer {AccessToken}");
    message.Headers.Add("tr_id", tradeId);
    message.Headers.Add("custtype", Personal ? "P" : "B");
    if (header != null) foreach (var (key, value) in header) {
      message.Headers.Add(key, value);
    }
    if (body != null) message.Content = JsonContent.Create(body);
    var response = await RequestClient.SendAsync(message);
    var bodyJson = await response.Content.ReadFromJsonAsync<TResult>();
    if (bodyJson != null) {
      bodyJson.HasNextData = Enumerable.Contains(["F", "M"], response.Headers.GetValues("tr_cont").Single());
    }
    return (response.StatusCode, bodyJson);
  }
}