using System.Collections.ObjectModel;

namespace trading_platform.ViewModel;

public static class Extensions {
  public static int BinarySearch<T>(this Collection<T> collection, T? item) where T : IComparable<T> {
    if (collection.Count == 0) return ~0;
    int low = 0, high = collection.Count;
    while (low != high) {
      int mid = low + (high - low) / 2;
      int comparison = collection[mid].CompareTo(item);
      if (comparison == 0) return mid;
      if (comparison < 0) low = mid + 1;
      else high = mid;
    }
    return ~low;
  }
  public static int BinarySearch<T>(this IList<T> collection, T? item) where T : IComparable<T> {
    if (collection.Count == 0) return ~0;
    int low = 0, high = collection.Count;
    while (low != high) {
      int mid = low + (high - low) / 2;
      int comparison = collection[mid].CompareTo(item);
      if (comparison == 0) return mid;
      if (comparison < 0) low = mid + 1;
      else high = mid;
    }
    return ~low;
  }
  public static int BinarySearch<TItem, TCompare>(this Collection<TItem> collection, TCompare? item, Func<TItem, TCompare> selector) 
  where TCompare : IComparable<TCompare> {
    if (collection.Count == 0) return ~0;
    int low = 0, high = collection.Count;
    while (low != high) {
      int mid = low + (high - low) / 2;
      int comparison = selector(collection[mid]).CompareTo(item);
      if (comparison == 0) return mid;
      if (comparison < 0) low = mid + 1;
      else high = mid;
    }
    return ~low;
  }
  public static int BinarySearch<TItem, TCompare>(this IList<TItem> collection, TCompare? item, Func<TItem, TCompare> selector) 
  where TCompare : IComparable<TCompare> {
    if (collection.Count == 0) return ~0;
    int low = 0, high = collection.Count;
    while (low != high) {
      int mid = low + (high - low) / 2;
      int comparison = selector(collection[mid]).CompareTo(item);
      if (comparison == 0) return mid;
      if (comparison < 0) low = mid + 1;
      else high = mid;
    }
    return ~low;
  }
}