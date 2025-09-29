using System.Collections;
using System.Diagnostics;
using System.Text.Json;

namespace trading_platform.Model.KoreaInvestment;

public static class ExceptionHandler {
  public static void PrintExceptionMessage(Exception ex, bool rethrow = false) {
    Debug.WriteLine($"[{ex.GetType()}] {ex.Source}: {ex.Message}");
    Debug.WriteLine($"{ex.StackTrace}");
    // 추가 정보
    Debug.WriteLine($"Additional Information:");
    int i = 1;
    foreach (DictionaryEntry kv in ex.Data) {
      Debug.WriteLine($"[{i:02}] Key: {kv.Key}");
      Debug.WriteLine($"       Value: {kv.Value}");
    }
    if (rethrow) throw ex;
  }
}