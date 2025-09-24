namespace trading_platform.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

public class NumericComparisonConverter : IMultiValueConverter {
  object? IMultiValueConverter.Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
    var iterator = values.GetEnumerator();
    if (!iterator.MoveNext()) return null;
    object? firstComparison = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? secondComparison = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? objectMore = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? objectEqual = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? objectLess = iterator.Current;
    if (firstComparison is not IComparable firstComparisonAsComparable) return null;
    int comparison = firstComparisonAsComparable.CompareTo(secondComparison);
    return comparison switch {
      > 0 => objectMore,
      < 0 => objectLess,
      0 => objectEqual
    };
  }
}