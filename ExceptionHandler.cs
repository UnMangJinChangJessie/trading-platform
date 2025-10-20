using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;

namespace trading_platform;

public static class ExceptionHandler {
  public const int IGNORABLE = 0;
  public const int WARNING = 10;
  public const int CRITICAL = 20;
  public const int UNKNOWN = -1;
  
  public static int GetCriticalLevel(Exception ex) {
    return ex switch {
      OperationCanceledException => IGNORABLE,
      TimeoutException => IGNORABLE,
      ThreadAbortException => IGNORABLE,
      IOException => IGNORABLE,
      SocketException => IGNORABLE,
      JsonException => WARNING,
      FormatException => WARNING,
      InvalidCastException => WARNING,
      KeyNotFoundException => WARNING,
      UnauthorizedAccessException => WARNING,
      HttpRequestException => WARNING,
      WebException => WARNING,
      InvalidOperationException => WARNING,
      ArgumentNullException => CRITICAL,
      ArgumentOutOfRangeException => CRITICAL,
      ArgumentException => CRITICAL,
      DivideByZeroException => CRITICAL,
      NullReferenceException => CRITICAL,
      IndexOutOfRangeException => CRITICAL,
      StackOverflowException => CRITICAL,
      OutOfMemoryException => CRITICAL,
      AccessViolationException => CRITICAL,
      TypeInitializationException => CRITICAL,
      MissingMethodException => CRITICAL,
      BadImageFormatException => CRITICAL,
      _ => UNKNOWN
    }; 
  }
  
  public static void PrintExceptionMessage(Exception ex, bool rethrow = false) {
    var frame = new StackFrame(1, true); // 바로 위에서 exception이 났을 테니
    string filename = frame.GetFileName() ?? "<unknown>";
    int lineNumber = frame.GetFileLineNumber();
    int level = GetCriticalLevel(ex);
    Debugger.Log(level, ex.GetType().ToString(), $"[{ex.GetType()}, {filename}:{lineNumber}] {ex.Source}: {ex.Message}\n");
    // 추가 정보
    // Debugger.Log(level, ex.GetType().ToString(), $"Additional Information:\n");
    if (rethrow) throw ex;
  }
}