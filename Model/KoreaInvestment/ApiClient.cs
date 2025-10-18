using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace trading_platform.Model.KoreaInvestment;

public record RequestBlock(string transId, IDictionary<string, string>? queries = null, string? body = null, bool next = false) {
  public string TransactionId { get; set; } = transId;
  public string? BodyString { get; set; } = body;
  public IDictionary<string, string>? Queries { get; set; } = queries;
  public bool RequestNext { get; set; } = next;
  public Action<string, bool, object?>? Callback { get; set; }
  public object? CallbackParameters { get; set; }
}

public static partial class ApiClient {
  public static readonly TimeSpan REQUEST_RATE_LIMIT = TimeSpan.FromMilliseconds(50);
  public static DateTime LastRequestTime { get; private set; } = DateTime.Now;
  public static bool Personal { get; set; } = true;
  public static bool Simulation { get; private set; } = false;
  public static string AppPublicKey { get; set; } = "";
  public static string AppSecretKey { get; set; } = "";
  public static string DefaultAccountBase { get; set; } = "";
  public static string DefaultAccountCode { get; set; } = "";
  public static string BrokerageId { get; set; } = "";
  public static string AccountId { get; set; } = "";
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
  private readonly static JsonSerializerOptions JsonSerializerOption = new() {
    NumberHandling = JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString,
    AllowTrailingCommas = true,
    Converters = {
      new DateToStringConverter(),
      new TimeToStringConverter(),
      new StringToBooleanConverter(),
    },
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  };
  public static T? DeserializeJson<T>(string jsonString) where T : class {
    if (jsonString == null) return null;
    try {
      return JsonSerializer.Deserialize<T>(jsonString, JsonSerializerOption);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return null;
    }
  }
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
    while (true) {
      SpinWait.SpinUntil(() =>
        PollingTaskCancellationToken.IsCancellationRequested || // 취소 요청
        DateTime.Now >= AccessTokenExpire || //접근 토큰 만료
        (!PendingRequests.IsEmpty && LastRequestTime + REQUEST_RATE_LIMIT <= DateTime.Now) // 정보 수신 요청
      );
      LastRequestTime = DateTime.Now;
      if (PollingTaskCancellationToken.IsCancellationRequested) {
        break;
      }
      if (DateTime.Now >= AccessTokenExpire) {
        IssueToken().Wait();
        continue;
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
        string? nextDataHeader = response.Headers.GetValues("tr_cont").FirstOrDefault();
        bool hasNextData = nextDataHeader != null ? Enumerable.Contains(["F", "M"], nextDataHeader) : false;
        request.Callback?.Invoke(responseBody, hasNextData, request.CallbackParameters);
      }
      catch (Exception ex) {
        ExceptionHandler.PrintExceptionMessage(ex);
      }
    }
  }
  public static void PushRequest(
    string transId,
    Action<string, bool, object?>? callback = null,
    object? callbackParameters = null,
    IDictionary<string, string>? queries = null,
    object? body = null,
    bool next = false
  ) {
    PendingRequests.Enqueue(new(transId, queries, body == null ? null : JsonSerializer.Serialize(body, JsonSerializerOption), next) {
      Callback = callback,
      CallbackParameters = callbackParameters
    });
  }
}