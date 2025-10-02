using System.Numerics;
using Avalonia.Data.Converters;

namespace trading_platform.Model;

public static partial class Converters {
  /// <summary>
  /// Converts a decimal number to a Korean unit string representation (양, 자, 해, 경, 조, 억, 만).
  /// Uses the largest applicable unit and includes sign for negative values.
  /// </summary>
  public static FuncValueConverter<decimal, string> SimpleNumberKoreanConverter { get; } = new(val => {
    var absoluteValue = Math.Abs(val);
    var sign = val >= 0 ? "" : "-";
    // 양 (C# decimal의 상한. 그런데 쓸 일이 있을까?)
    var yang = (int)Math.Floor(absoluteValue / 1E+28M);
    // 자
    var ja = (int)Math.Floor(absoluteValue % 1E+28M / 1E+24M);
    if (yang != 0) {
      return sign + $"{yang}양" + (ja != 0 ? $" {ja}자" : "");
    }
    // 해
    var hae = (int)Math.Floor(absoluteValue % 1E+24M / 1E+20M);
    if (ja != 0) {
      return sign + $"{ja}자" + (hae != 0 ? $" {hae}해" : "");
    }
    // 경
    var gyeong = (int)Math.Floor(absoluteValue % 1E+20M / 1E+16M);
    if (hae != 0) {
      return sign + $"{hae}해" + (gyeong != 0 ? $" {gyeong}경" : "");
    }
    // 조
    var jo = (int)Math.Floor(absoluteValue % 1E+16M / 1E+12M);
    if (gyeong != 0) {
      return sign + $"{gyeong}경" + (jo != 0 ? $" {jo}조" : "");
    }
    // 억
    var eok = (int)Math.Floor(absoluteValue % 1E+12M / 1E+8M);
    if (jo != 0) {
      return sign + $"{jo}조" + (eok != 0 ? $" {eok}억" : "");
    }
    // 만
    var man = (int)Math.Floor(absoluteValue % 1E+8M / 1E+4M);
    if (eok != 0) {
      return sign + $"{eok}억" + (man != 0 ? $" {man}만" : "");
    }
    var ones = absoluteValue % 1E+4M;
    if (man != 0) {
      return sign + $"{man}만" + (ones != 0 ? $" {Math.Floor(ones)}" : "");
    }
    else return ones.ToString();
  });
}