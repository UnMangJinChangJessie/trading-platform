namespace trading_platform;

public static class Common {
  public static string BuildQueryString(IEnumerable<(string Key, string Value)> keyValues) {
    return string.Join("&", keyValues.Select(kv => kv.Key + "=" + kv.Value));
  }
  public static string BuildQueryString(IDictionary<string, string> keyValues) {
    return string.Join("&", keyValues.Select(kv => $"{kv.Key}={kv.Value}"));
  }
}