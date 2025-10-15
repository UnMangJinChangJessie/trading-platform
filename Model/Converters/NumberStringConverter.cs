using Avalonia.Data.Converters;

namespace trading_platform.Model;

public static partial class Converters {
  internal static readonly (decimal Unit, string UnitName)[] KOREAN_NUMBER_UNITS = [
    (1E+28M, "양"),
    (1E+24M, "자"),
    (1E+20M, "해"),
    (1E+16M, "경"),
    (1E+12M, "조"),
    (1E+08M, "억"),
    (1E+04M, "만"),
    (1E+00M, "")
  ];
  /// <summary>
  /// Converts a decimal number to a Korean unit string representation (양, 자, 해, 경, 조, 억, 만).
  /// Uses the largest applicable unit and includes sign for negative values.
  /// </summary>
  public static FuncValueConverter<decimal, string> SimpleNumberKoreanConverter { get; } = new(val => {
    var absoluteValue = Math.Abs(val);
    var sign = val >= 0 ? "" : "-";
    int first = 0, second = (int)decimal.Floor(val / KOREAN_NUMBER_UNITS[0].Unit);
    val -= KOREAN_NUMBER_UNITS[0].Unit * second;
    for (int i = 1; i < KOREAN_NUMBER_UNITS.Length; i++) {
      first = second;
      second = (int)decimal.Floor(val / KOREAN_NUMBER_UNITS[i].Unit);
      val -= second * KOREAN_NUMBER_UNITS[i].Unit;
      if (first != 0) {
        if (second == 0) return sign + $"{first}" + KOREAN_NUMBER_UNITS[i - 1].UnitName;
        else return sign + $"{first}" + KOREAN_NUMBER_UNITS[i - 1].UnitName + $" {second}" + KOREAN_NUMBER_UNITS[i].UnitName;
      }
    }
    if (first == 0) return sign + $"{second}";
    else return sign + $"{first}" + KOREAN_NUMBER_UNITS[^2].UnitName + $"{second}" + KOREAN_NUMBER_UNITS[^1];
  });
}