using Avalonia.Data.Converters;
using System.ComponentModel;
using System.Reflection;

namespace trading_platform.Model;

public static partial class Converters {
  public readonly static FuncValueConverter<KoreaInvestment.OrderMethod, string> OrderMethodConverter =
    new(x => x.GetType().GetField(x.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? x.ToString());
}