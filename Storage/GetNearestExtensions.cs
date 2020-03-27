using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;


namespace Storage {

  /// <summary>
  /// SortedList Extensions returning TValue of next higher TKey, if TKey does not exist in SortedList
  /// </summary>
  public static class GetNearestExtensions {


    /// <summary>
    /// Returns TValue for iReadOnlyDictionary[TKey]. If TKey is not in iReadOnlyDictionary, the next higher TKey is 
    /// searched and its TValue returned. If TKey is greater than any TKey in iReadOnlyDictionary, TValue for the 
    /// highest TKey in iReadOnlyDictionary gets returned. An Exception is thrown if iReadOnlyDictionary is not a 
    /// SortedList. If sortedList is empty and TValue a class, null gets returned. If TValue is a struct, an
    /// Exception gets thrown.
    /// </summary>
    public static TValue? GetEqualGreater<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> iReadOnlyDictionary, TKey key)
      where TKey : notnull where TValue : class
      {
        var sortedList = (SortedList<TKey, TValue>) iReadOnlyDictionary;
      return sortedList.GetEqualGreater(key);
    }
    

    /// <summary>
    /// Returns TValue for sortedList[TKey]. If TKey is not in SortedList, the next higher TKey is searched and
    /// its TValue returned. If TKey is greater than any TKey in SortedList, TValue for the highest TKey in SortedList
    /// gets returned. If sortedList is empty and TValue a class, null gets returned. If TValue is a struct, an
    /// Exception gets thrown.
    /// </summary>
    [return: MaybeNull]
    public static TValue GetEqualGreater<TKey, TValue>(this SortedList<TKey, TValue> sortedList, TKey key)
      where TKey: notnull 
    {
      var comparer = sortedList.Comparer;

      if (sortedList.Count==0) {
        if (default(TValue) is null) {
          return default;
        } else {
          throw new Exception("SortedList.GetEqualGreater() works only when SortedList has item(s).");
        }
      }
      var firstKey = sortedList.Keys[0];
      if (comparer.Compare(firstKey, key)==1) return sortedList[firstKey];// item is missing, key is too small

      var lastKey = sortedList.Keys[sortedList.Count-1];
      if (comparer.Compare(lastKey, key)==-1) return sortedList[lastKey];// item is missing, key is too big

      //search
      int min = 0;
      int max = sortedList.Count-1;
      while (min<=max) {
        int mid = (min + max) / 2;
        var midKey = sortedList.Keys[mid];
        int compareResult = comparer.Compare(midKey, key);

        if (compareResult==0) {
          return sortedList[midKey];
        } else if (compareResult==1) {
          max = mid - 1;
        } else {
          min = mid + 1;
        }
      }
      return sortedList[sortedList.Keys[min]];
    }
  }
}
