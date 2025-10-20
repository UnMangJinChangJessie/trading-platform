using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace trading_platform.Model.KoreaInvestment;

public partial class ApiModel {
  public async ValueTask<bool> IssueToken() {
    // Base URI 설정
    RequestClient = new() {
      BaseAddress = new Uri(IsSimulation ? "https://openapivts.koreainvestment.com:29443" : "https://openapi.koreainvestment.com:9443"),
      Timeout = TimeSpan.FromSeconds(10),
    };
    RequestRateLimit = TimeSpan.FromMilliseconds(IsSimulation ? 500 : 50);
    var body = new {
      grant_type = "client_credentials",
      appkey = AppPublicKey,
      appsecret = AppSecretKey
    };
    HttpResponseMessage result;
    try {
      result = await RequestClient.PostAsJsonAsync("/oauth2/tokenP", body);
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return false;
    }
    if (!result.IsSuccessStatusCode) {
      return false;
    }
    var responseBody = await result.Content.ReadFromJsonAsync<JsonElement>();
    AccessToken = responseBody.GetProperty("access_token").GetString() ?? "";
    AccessTokenExpire = DateTime.Now + TimeSpan.FromSeconds(Convert.ToInt32(responseBody.GetProperty("expires_in").GetDouble()));
    if (PollingTask is not null) {
      await PollingTaskCancellationToken.CancelAsync();
      await PollingTask;
      PollingTaskCancellationToken.TryReset();
    }
    PollingTask = Task.Run(PollApiRequest, PollingTaskCancellationToken.Token)
      .ContinueWith(task => Console.WriteLine($"API polling task terminated: {task.Exception?.Message}"));
    return !string.IsNullOrEmpty(AccessToken);
  }

  public async ValueTask<bool> RevokeToken() {
    var body = new {
      appkey = AppPublicKey,
      appsecret = AppSecretKey,
      token = AccessToken
    };
    var result = await RequestClient.PostAsJsonAsync("/oauth2/revokeP", body);
    if (!result.IsSuccessStatusCode) {
      return false;
    }
    if (PollingTask != null) {
      await PollingTaskCancellationToken.CancelAsync();
      await PollingTask;
    }
    AccessTokenExpire = null;
    return true;
  }
  public async ValueTask<string?> IssueWebSocketToken() {
    var body = new {
      grant_type = "client_credentials",
      appkey = AppPublicKey,
      secretkey = AppSecretKey,
    };
    try {
      var result = await RequestClient.PostAsJsonAsync("/oauth2/Approval", body);
      var responseBody = await result.Content.ReadFromJsonAsync<JsonNode>();
      if (responseBody == null) {
        Debug.WriteLine("Failed to get response from the server: [{0}] {1}", args: [result.StatusCode, result.ReasonPhrase]);
        return null;
      }
      if (!result.IsSuccessStatusCode) {
        Debug.WriteLine(
          "Failed to issue a WebSocket token: [{0}] {1}",
          responseBody["error_code"]?.GetValue<string>(),
          responseBody["error_description"]?.GetValue<string>()
        );
        return null;
      }
      return responseBody["approval_key"]?.GetValue<string>();
    }
    catch (Exception ex) {
      ExceptionHandler.PrintExceptionMessage(ex);
      return null;
    }
  }
}