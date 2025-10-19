using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace trading_platform.Model.KoreaInvestment;

public partial class ApiModel {
  public record RequestBlock(string transId, IDictionary<string, string>? queries = null, string? body = null, bool next = false) {
    public string TransactionId { get; set; } = transId;
    public string? BodyString { get; set; } = body;
    public IDictionary<string, string>? Queries { get; set; } = queries;
    public bool RequestNext { get; set; } = next;
    public Action<string, bool, object?>? Callback { get; set; }
    public object? CallbackParameters { get; set; }
  }
  public static readonly TimeSpan REQUEST_RATE_LIMIT = TimeSpan.FromMilliseconds(50);
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
}

public partial class ApiModel : ObservableObject {
  [JsonPropertyName("public_key")]
  [ObservableProperty]
  public partial string? AppPublicKey { get; set; } = null;
  [JsonPropertyName("secret_key")]
  [ObservableProperty]
  public partial string? AppSecretKey { get; set; } = null;
  [JsonPropertyName("simulation")]
  [ObservableProperty]
  public partial bool IsSimulation { get; set; } = false;
  [JsonPropertyName("personal")]
  [ObservableProperty]
  public partial bool IsPersonal { get; set; } = true;
  [JsonPropertyName("account")]
  [ObservableProperty]
  public partial Account Account { get; set; } = new();
  [JsonPropertyName("hts_id")]
  [ObservableProperty]
  public partial string BrokerageId { get; set; } = "";
  [JsonPropertyName("access_token")]
  [ObservableProperty]
  public partial string? AccessToken { get; private set; } = null;
  [JsonPropertyName("token_expire")]
  [ObservableProperty]
  public partial DateTime? AccessTokenExpire { get; private set; } = null;
  [JsonIgnore]
  public DateTime LastRequestTime { get; private set; } = DateTime.Now;
  [JsonIgnore]
  public ConcurrentQueue<RequestBlock> PendingRequests { get; private set; } = new();

  [JsonIgnore]
  private static Task? PollingTask;
  [JsonIgnore]
  private readonly static CancellationTokenSource PollingTaskCancellationToken = new();
  [JsonIgnore]
  private readonly HttpClient RequestClient = new() {
    Timeout = TimeSpan.FromSeconds(10)
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
  public string GetBaseAddressString() => $"https://openapi.koreainvestment.com:{(IsSimulation ? 29443 : 9443)}";

  public async Task PollApiRequest() {
    while (true) {
      SpinWait.SpinUntil(() =>
        PollingTaskCancellationToken.IsCancellationRequested || // 취소 요청
        DateTime.Now >= AccessTokenExpire || //접근 토큰 만료
        (!PendingRequests.IsEmpty && LastRequestTime + REQUEST_RATE_LIMIT <= DateTime.Now) // 정보 수신 요청 존재
      );
      if (PollingTaskCancellationToken.IsCancellationRequested) {
        break;
      }
      if (DateTime.Now >= AccessTokenExpire) {
        await IssueToken();
        continue;
      }

      LastRequestTime = DateTime.Now;
      if (!PendingRequests.TryDequeue(out var request)) continue; // SpinUntil과 위의 예외 처리로 인해 일어나지는 않는 코드
      try {
        var relUri = TransactionIdTable.GetRelativeUri(request.TransactionId);
        var method = TransactionIdTable.GetHttpMethod(request.TransactionId);
        HttpRequestMessage message = new(method, relUri + Common.BuildQueryString(request.Queries));
        message.Headers.Add("appkey", AppPublicKey);
        message.Headers.Add("appsecret", AppSecretKey);
        message.Headers.Add("authorization", $"Bearer {AccessToken}");
        message.Headers.Add("tr_id", request.TransactionId);
        if (request.RequestNext) message.Headers.Add("tr_cont", "N");
        message.Headers.Add("custtype", IsPersonal ? "P" : "B");
        // 그 외에는 사실 넣을 헤더가 없음.
        if (request.BodyString != null) message.Content = new StringContent(request.BodyString, Encoding.UTF8, "application/json");
        var response = await RequestClient.SendAsync(message);
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
  public void PushRequest(
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