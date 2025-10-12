using Avalonia;
using Avalonia.Data.Converters;

namespace trading_platform.Model;

public static partial class Converters {
  /// <summary>
  /// {0} - {1} switch { > 0 => {2}, < 0 => {3}, 0 => {4} }
  /// </summary>
  /// {0}과 {1}은 IComparable를 구현해야 합니다.
  public static readonly FuncMultiValueConverter<object, object> ComparisonConverter = new(values => {
    var iter = values.GetEnumerator();
    if (!iter.MoveNext() || iter.Current is not IComparable first) return AvaloniaProperty.UnsetValue;
    if (!iter.MoveNext() || iter.Current is not IComparable second) return AvaloniaProperty.UnsetValue;
    if (!iter.MoveNext() || iter.Current is not object positiveResult) return AvaloniaProperty.UnsetValue;
    if (!iter.MoveNext() || iter.Current is not object negativeResult) return AvaloniaProperty.UnsetValue;
    if (!iter.MoveNext() || iter.Current is not object zeroResult) return AvaloniaProperty.UnsetValue;
    return first.CompareTo(second) switch { > 0 => positiveResult, < 0 => negativeResult, 0 => zeroResult };
  });
}