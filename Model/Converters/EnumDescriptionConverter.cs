using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Data.Converters;
using trading_platform.Model.Charts;

namespace trading_platform.Model;

public static partial class Converters {
  public readonly static FuncValueConverter<object, string> EnumDescriptionConverter = new(
    x => {
      if (x == null || x.GetType() == typeof(UnsetValueType)) return "";
      return x.GetType().GetField(x.ToString() ?? "")?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? x.ToString() ?? "";
    }
  );
}