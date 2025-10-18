namespace trading_platform.ViewModel;

public interface IRefresh {
  void Refresh();
  Task RefreshAsync();
}