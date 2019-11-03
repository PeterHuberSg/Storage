//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;


//namespace Storage {
//  public class DictionaryMonitored<TKey, TValue>: IDictionary<TKey, TValue> {

//    #region Properties
//    //      ----------

//    public TValue this[TKey key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//    public ICollection<TKey> Keys => dictionary.Keys;

//    public ICollection<TValue> Values => dictionary.Values;

//    public int Count => dictionary.Count;

//    public bool IsReadOnly => false;

//    public IEqualityComparer<TKey> Comparer => dictionary.Comparer;
//    #endregion


//    #region Events
//    //      ------

//    /// <summary>
//    /// A value was added or overwritten in the dictionary
//    /// </summary>
//    public event Action<TValue> Changed;

//    /// <summary>
//    /// A value was deleted from the dictionary
//    /// </summary>
//    public event Action<TValue> Deleted;

//    #endregion


//    #region Constructor
//    //      -----------
//    Dictionary<TKey, TValue> dictionary;


//    public DictionaryMonitored() : this(0, null) { }


//    public DictionaryMonitored(int capacity) : this(capacity, null) { }


//    public DictionaryMonitored(IEqualityComparer<TKey> comparer) : this(0, comparer) { }


//    public DictionaryMonitored(int capacity, IEqualityComparer<TKey> comparer) {
//      dictionary = new Dictionary<TKey, TValue>(capacity, comparer);
//    }


//    public DictionaryMonitored(IDictionary<TKey, TValue> dictionary) : this(dictionary, null) { }


//    public DictionaryMonitored(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) {
//      this.dictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
//    }
//    #endregion


//    #region Methods
//    //      -------

//    public void Add(TKey key, TValue value) {
//      dictionary.Add(key, value);
//      Changed?.Invoke(value);
//    }

//    public void Add(KeyValuePair<TKey, TValue> item) {
//      dictionary.Add(item.Key, item.Value);
//      Changed?.Invoke(item.Value);
//    }

//    public void Clear() {
//      throw new NotImplementedException();
//    }

//    public bool Contains(KeyValuePair<TKey, TValue> item) {
//      return dictionary.ContainsKey(item.Key);
//    }

//    public bool ContainsKey(TKey key) {
//      return dictionary.ContainsKey(key);
//    }

//    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
//      throw new NotImplementedException();
//    }

//    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
//      return dictionary.GetEnumerator();
//    }

//    public bool Remove(TKey key) {
//      TValue value = dictionary[key];
//      var result = dictionary.Remove(key);
//      Deleted?.Invoke(value);
//      return result;
//    }

//    public bool Remove(KeyValuePair<TKey, TValue> item) {
//      return Remove(item.Key);
//    }


//    public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value) {
//      return dictionary.TryGetValue(key, out value);
//    }


//    IEnumerator IEnumerable.GetEnumerator() {
//      return dictionary.GetEnumerator();
//    }
//    #endregion
//  }
//}
