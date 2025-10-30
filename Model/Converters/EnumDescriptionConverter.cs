using System.ComponentModel;
using System.Reflection;
using Avalonia;
using Avalonia.Data.Converters;

namespace trading_platform.Model;

public static partial class Converters {
  public readonly static FuncValueConverter<object, string> ObjectDescriptionConverter = new(
    x => {
      if (x == null || x.GetType() == typeof(UnsetValueType)) return "";
      if (x is Type t) {
        return t.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault()?.Description ?? x.ToString() ?? "No description or name";
      }
      else if (x is Enum e) {
        return e.GetType().GetField(e.ToString())?.GetCustomAttribute<DescriptionAttribute>()?.Description ?? x.ToString() ?? "No description or name";
      }
      else return "Cannot find description.";
    }
  );
}