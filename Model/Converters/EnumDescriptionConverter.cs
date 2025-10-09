using System.ComponentModel;
using System.Reflection;
using Avalonia.Data.Converters;
using trading_platform.Model.Charts;

namespace trading_platform.Model;

public static partial class Converters {
  public readonly static FuncValueConverter<CandlestickChartData.CandlePeriod, string> EnumDescriptionConverter = new(
    x => x.GetType().GetField(x.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? x.ToString()
  );
}