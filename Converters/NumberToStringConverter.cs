using System.Globalization;
using Avalonia.Data.Converters;

namespace trading_platform.Converters;

public class NumberToStringConverter : IValueConverter {
  private readonly static Type[] NUMERIC_TYPES = [
    typeof(sbyte), typeof(byte),
    typeof(short), typeof(ushort),
    typeof(int), typeof(uint),
    typeof(long), typeof(ulong),
    typeof(float), typeof(double), typeof(decimal)
  ];
  object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value == null || !NUMERIC_TYPES.Contains(value.GetType())) return null;
    if (targetType != typeof(string)) return null;
    if (parameter != null && parameter.GetType() != typeof(string)) return null;
    string? format = (string?)parameter;
    return string.Format($"{{0:{format}}}", value);
  }
  object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is not string valueString) return null;
    if (!NUMERIC_TYPES.Contains(targetType)) return null;
    Type valueType = value.GetType();
    if (valueType == typeof(sbyte)) return sbyte.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(byte)) return byte.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(short)) return short.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(ushort)) return ushort.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(int)) return int.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(uint)) return uint.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(long)) return long.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(ulong)) return ulong.Parse(valueString, NumberStyles.Integer | NumberStyles.AllowThousands);
    else if (valueType == typeof(float)) return float.Parse(valueString, NumberStyles.Float | NumberStyles.AllowThousands);
    else if (valueType == typeof(double)) return double.Parse(valueString, NumberStyles.Float | NumberStyles.AllowThousands);
    else return decimal.Parse(valueString, NumberStyles.Float | NumberStyles.AllowThousands);
  }
}