using Avalonia.Controls;

namespace trading_platform.View;

public interface IDataContextIsMarketData { 
  public object? DataContext { get; set; }
}