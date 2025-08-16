using System.Net.Http.Json;
using System.Text.Json;

namespace TradingSystem.KoreaInvestment;

public static partial class ApiClient {
  public static async Task<bool> IssueToken() {
    var body = new {
      grant_type = "client_credentials",
      appkey = AppPublicKey,
      appsecret = AppSecretKey
    };
    var result = await RequestClient.PostAsJsonAsync("/oauth2/tokenP", body);
    if (!result.IsSuccessStatusCode) {
      return false;
    }
    var responseBody = await result.Content.ReadFromJsonAsync<JsonElement>();
    AccessToken = responseBody.GetProperty("access_token").GetString() ?? "";
    AccessTokenExpire = DateTime.Now + TimeSpan.FromSeconds(Convert.ToInt32(responseBody.GetProperty("expires_in").GetString()));
    return string.IsNullOrEmpty(AccessToken);
  }
  public static async Task<bool> RevokeToken() {
    var body = new {
      appkey = AppPublicKey,
      appsecret = AppSecretKey,
      token = AccessToken
    };
    // var result = await Request(HttpMethod.Post, "/oauth/revokeP", [], [], body);
    var result = await RequestClient.PostAsJsonAsync("/oauth2/revokeP", body);
    if (!result.IsSuccessStatusCode) {
      return false;
    }
    return true;
  }
  public static async Task<bool> IssueWebSocketToken(string publicKey, string secretKey) {
    var body = new {
      grant_type = "client_credentials",
      appkey = publicKey,
      secretkey = secretKey,
    };
    var result = await RequestClient.PostAsJsonAsync("/oauth2/Approval", body);
    if (!result.IsSuccessStatusCode) return false;
    var responseBody = await result.Content.ReadFromJsonAsync<JsonElement>();
    WebSocketAccessToken = responseBody.GetProperty("approval_key").GetString() ?? "";
    return string.IsNullOrEmpty(WebSocketAccessToken);
  }
}