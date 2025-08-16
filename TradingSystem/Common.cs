using System.Globalization;
using System.Numerics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TradingSystem;

public static class Common {
  public static string BuildQueryString(IEnumerable<(string Key, string Value)> keyValues) {
    return string.Join("&", keyValues.Select(kv => kv.Key + "=" + kv.Value));
  }
}