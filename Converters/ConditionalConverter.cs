namespace trading_platform.Converters;

using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

public class ConditionalConverter : IMultiValueConverter {
  object? IMultiValueConverter.Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) {
    var iterator = values.GetEnumerator();
    if (!iterator.MoveNext()) return null;
    object? condition = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? objectTrue = iterator.Current;
    if (!iterator.MoveNext()) return null;
    object? objectFalse = iterator.Current;
    if (condition?.GetType() == typeof(bool)) return (bool)condition ? objectTrue : objectFalse;
    else if (condition?.GetType() == typeof(string)) return string.IsNullOrEmpty((string)condition) ? objectTrue : objectFalse;
    else if (Enumerable.Contains([typeof(short), typeof(int), typeof(long), typeof(ushort), typeof(uint), typeof(ulong)], condition?.GetType())) {
      return ((IComparable)condition!).CompareTo(0) != 0 ? objectTrue : objectFalse;
    }
    else {
      return condition != null ? objectTrue : objectFalse;
    }
  }
}

