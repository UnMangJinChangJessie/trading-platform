namespace trading_platform.ViewModel;

public interface IRefresh {
  private readonly static Dictionary<string, object> NullArguments = [];
  public Task RefreshAsync(IDictionary<string, object> args);
}