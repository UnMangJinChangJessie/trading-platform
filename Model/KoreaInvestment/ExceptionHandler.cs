using System.Diagnostics;
using System.Text.Json;

namespace trading_platform.Model.KoreaInvestment;

public static class ExceptionHandler {
  public static void PrintExceptionMessage(Exception ex, bool rethrow = false) {
    Debug.WriteLine($"[{ex.GetType()}] {ex.Source}: {ex.Message}");
    Debug.WriteLine($"{ex.StackTrace}");
    if (rethrow) throw ex;
  }
}