using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace trading_platform.Converters;

public class IndexAccessConverter : IMultiValueConverter {
  object? IMultiValueConverter.Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
    var iter = values.GetEnumerator();
    if (!iter.MoveNext()) return null;
    if (iter.Current is not IList collection) return null;
    if (!iter.MoveNext()) return null;
    if (iter.Current is not int idx) return null;
    return collection[idx];
  }
}