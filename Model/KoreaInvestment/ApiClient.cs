using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public record RequestBlock(string transId, IDictionary<string, string>? queries = null, string? body = null, bool next = false) {
  public string TransactionId { get; set; } = transId;
  public string? BodyString { get; set; } = body;
  public IDictionary<string, string>? Queries { get; set; } = queries;
  public bool RequestNext { get; set; } = next;
  public Action<string>? Callback { get; set; }
}

public static partial class ApiClient {
  public static readonly TimeSpan REQUEST_RATE_LIMIT = TimeSpan.FromMilliseconds(50);
  public static DateTime LastRequestTime { get; private set; } = DateTime.Now;
  public static bool Personal { get; set; } = true;
  public static bool Simulation { get; private set; } = false;
  public static string AppPublicKey { get; set; } = "";
  public static string AppSecretKey { get; set; } = "";
  public static string AccessToken { get; set; } = default!;
  public static DateTime AccessTokenExpire { get; private set; } = DateTime.UnixEpoch;
  public static ConcurrentQueue<RequestBlock> PendingRequests = new();
  private readonly static HttpClient RequestClient = new() {
    BaseAddress = BuildApiBaseAddress(),
    Timeout = TimeSpan.FromSeconds(10)
  };
  private static Task? PollingTask;
  private readonly static CancellationTokenSource PollingTaskCancellationToken = new();
  private static CancellationTokenSource CancellationSource = new();
  public readonly static JsonSerializerOptions JsonSerializerOption = new() {
    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
    AllowTrailingCommas = true,
    Converters = {
      new DateToStringConverter(),
      new TimeToStringConverter(),
      new StringToBooleanConverter(),
    },
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };
  public static bool ToggleSimulation() {
    Simulation = !Simulation;
    RequestClient.BaseAddress = BuildApiBaseAddress();
    return Simulation;
  }
  public static Uri BuildApiBaseAddress() {
    UriBuilder builder = new($"https://openapi.koreainvestment.com") {
      Port = Simulation ? 29443 : 9443
    };
    return builder.Uri;
  }
  public static string GetFirstThreeTokenChar() {
    return AccessToken[..Math.Min(3, AccessToken.Length)] + "...";
  }
  public static void PollApiRequest() {
    while (!PollingTaskCancellationToken.IsCancellationRequested) {
      SpinWait.SpinUntil(() => !PendingRequests.IsEmpty && LastRequestTime + REQUEST_RATE_LIMIT <= DateTime.Now);
      LastRequestTime = DateTime.Now;
      if (DateTime.Now >= AccessTokenExpire) {
        IssueToken().Wait();
      }
      if (!PendingRequests.TryDequeue(out var request)) continue; // SpinUntil로 인해 일어나지는 않는 코드
      try {
        var relUri = TransactionIdTable.GetRelativeUri(request.TransactionId);
        var method = TransactionIdTable.GetHttpMethod(request.TransactionId);
        HttpRequestMessage message = new(method, relUri + Common.BuildQueryString(request.Queries));
        message.Headers.Add("appkey", AppPublicKey);
        message.Headers.Add("appsecret", AppSecretKey);
        message.Headers.Add("authorization", $"Bearer {AccessToken}");
        message.Headers.Add("tr_id", request.TransactionId);
        if (request.RequestNext) message.Headers.Add("tr_cont", "N");
        message.Headers.Add("custtype", Personal ? "P" : "B");
        // 그 외에는 사실 넣을 헤더가 없음.
        if (request.BodyString != null) message.Content = new StringContent(request.BodyString, Encoding.UTF8, "application/json");
        var response = RequestClient.Send(message);
        string responseBody;
        using (var reader = new StreamReader(response.Content.ReadAsStream())) {
          responseBody = reader.ReadToEnd();
        }
        request.Callback?.Invoke(responseBody);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
  }
  public static void PushRequest(
    string transId,
    Action<string>? callback = null,
    IDictionary<string, string>? queries = null,
    object? body = null,
    bool next = false
  ) {
    PendingRequests.Enqueue(new(transId, queries, body == null ? null : JsonSerializer.Serialize(body, JsonSerializerOption), next) { Callback = callback } );
  }
}