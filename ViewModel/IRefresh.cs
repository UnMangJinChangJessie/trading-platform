namespace trading_platform.ViewModel;

public interface IRefresh {
  public Task RefreshAsync(IDictionary<string, object> args);
}

public interface IRefreshRealtime {
  public Task StartRefreshRealtimeAsync(IDictionary<string, object> args);
  public Task EndRefreshRealtimeAsync(IDictionary<string, object> args);
}