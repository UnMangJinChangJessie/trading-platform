using System.ComponentModel;
using System.Globalization;

namespace trading_platform.Styles.Catppuccin;

public class ThemeVariantTypeConverter : TypeConverter {
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) {
    return sourceType == typeof(string);
  }
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value) {
    return value switch {
      "Latte" => ColourPalette.Latte,
      "Frappe" => ColourPalette.Frappe,
      "Macchiato" => ColourPalette.Macchiato,
      "Mocha" => ColourPalette.Mocha,
      _ => throw new ArgumentException($"'{value}' is not recognized theme variant, available setter parameter lists are: 'Latte', 'Frappe', 'Macchiato' and 'Latte'.")
    };
  }
}

[TypeConverter(typeof(ThemeVariantTypeConverter))]
public class ColourPalette {
  public static readonly Avalonia.Styling.ThemeVariant Latte = new(nameof(Latte), null);
  public static readonly Avalonia.Styling.ThemeVariant Frappe = new(nameof(Frappe), null);
  public static readonly Avalonia.Styling.ThemeVariant Macchiato = new(nameof(Macchiato), null);
  public static readonly Avalonia.Styling.ThemeVariant Mocha = new(nameof(Mocha), null);
}

public class Theme : Avalonia.Styling.Styles, Avalonia.Controls.IResourceNode {
  public Theme(IServiceProvider sp) {
    Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(sp, this);
      // ?? throw new InvalidOperationException("Catppuccin theme was initialized with missing ColorPaletteResourcesCollection.");
  }
  bool Avalonia.Controls.IResourceNode.TryGetResource(object key, Avalonia.Styling.ThemeVariant? theme, out object? value) {
    if (theme == null) {
      value = null;
      return false;
    }
    if (key.GetType() != typeof(string)) {
      value = null;
      return false;
    }
    if (!Resources.ThemeDictionaries.TryGetValue(theme, out var provider)) {
      value = null;
      return false;
    }
    if (!provider.TryGetResource(key, theme, out value)) {
      return base.TryGetResource(key, theme, out value);
    }
    return true;
  }
}