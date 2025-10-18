using System.Collections;
using System.Diagnostics;
using System.Text.Json;

namespace trading_platform.Model.KoreaInvestment;

public static class ExceptionHandler {
  public static void PrintExceptionMessage(Exception ex, bool rethrow = false) {
    var frame = new StackFrame(1, true); // 바로 위에서 exception이 났을 테니
    string filename = frame.GetFileName() ?? "<unknown>";
    int lineNumber = frame.GetFileLineNumber();
    Debug.WriteLine($"[{ex.GetType()}, {filename}:{lineNumber}] {ex.Source}: {ex.Message}");
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