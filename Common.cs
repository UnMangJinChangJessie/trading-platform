namespace trading_platform;

public static class Common {
  public static string BuildQueryString(IEnumerable<(string Key, string Value)>? keyValues) {
    if (keyValues == null) return "";
    return string.Join("&", keyValues.Select(kv => kv.Key + "=" + kv.Value));
  }
  public static string BuildQueryString(IDictionary<string, string>? keyValues) {
    if (keyValues == null) return "";
    return "?" + string.Join("&", keyValues.Select(kv => $"{kv.Key}={kv.Value}"));
  }
}