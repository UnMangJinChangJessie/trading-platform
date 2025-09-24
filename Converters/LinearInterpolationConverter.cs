using System.Globalization;
using System.Numerics;
using Avalonia.Data.Converters;

namespace trading_platform.Converters;

public class LinearInterpolationConverter : IMultiValueConverter {
  object? IMultiValueConverter.Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
    var iter = values.GetEnumerator();
    if (!iter.MoveNext()) return null;
    if (iter.Current is not double start) return null;
    if (!iter.MoveNext()) return null;
    if (iter.Current is not double finish) return null;
    if (!iter.MoveNext()) return null;
    if (iter.Current is not double alpha) return null;
    return (1.0 - alpha) * start + alpha * finish;
  }
}