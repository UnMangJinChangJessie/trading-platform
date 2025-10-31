using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Skia;

namespace trading_platform.Model;

public static partial class Converters {
  public class ColorConverterClass : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
      return value switch {
        ScottPlot.Color scottColor => Avalonia.Media.Color.FromUInt32(scottColor.ARGB),
        SkiaSharp.SKColor skColor => Avalonia.Media.Color.FromArgb(skColor.Alpha, skColor.Red, skColor.Green, skColor.Blue),
        System.Drawing.Color systemColor => Avalonia.Media.Color.FromUInt32((uint)systemColor.ToArgb()),
        Avalonia.Media.Color avaColor => avaColor,
        _ => AvaloniaProperty.UnsetValue
      };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
      return value switch {
        ScottPlot.Color => value,
        SkiaSharp.SKColor skColor => ScottPlot.Color.FromSKColor(skColor),
        System.Drawing.Color systemColor => ScottPlot.Color.FromColor(systemColor),
        Avalonia.Media.Color avaColor => ScottPlot.Color.FromSKColor(avaColor.ToSKColor()),
        _ => AvaloniaProperty.UnsetValue
      };
    }
  }
  public readonly static ColorConverterClass ColorConverter = new();
}