using System.ComponentModel;

namespace trading_platform.ViewModel;

public class Reactive<T> : INotifyPropertyChanged {
  private T _value = default!;
  public event PropertyChangedEventHandler PropertyChanged;
  public T Value {
    get => _value;
    set {
      if (!EqualityComparer<T>.Default.Equals(_value, value)) {
        _value = value;
        PropertyChanged?.Invoke(this, new(nameof(Value)));
      }
    }
  }
  public Reactive() {
    PropertyChanged = default!;
  }
  public Reactive(T value) {
    PropertyChanged = default!;
    _value = value;
  }
}