/**************************************************************************************

Storage.StorageDictionary
=========================

Stores instances (=item) of classes inheriting IStorage in an array. An item can be accessed by its key. It behaves
like a dictionary, but is much faster.
The data is only stored in RAM and gets lost once the application ends. Use StorageDictionaryCSV for permanent data 
storage.

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;


namespace Storage {

  /// <summary>
  /// A fast collection of items which implement IStorage. Each item has a unique int key. Only items can be added which have 
  /// a key grater than any other existing key.
  /// Ideally, TContext is the parent and holds all StorageDictionaries of the application.
  /// </summary>
  public class StorageDictionary<TItem, TContext>: IEnumerable<TItem>, IDictionary<int, TItem>, IDisposable 
    where TItem : class, IStorage<TItem>
    where TContext: class
  {

    #region Properties
    //      ----------

    /// <summary>
    /// Indexer, returns item based on key
    /// </summary>
    public TItem this[int key] { 
      get {
        int arrayIndex = binarySearch(key);
        if (arrayIndex<0) throw new ArgumentOutOfRangeException();

        return items[arrayIndex]?? throw new ArgumentException($"There is no value for key '{key}'.");
      } 
      set => throw new NotSupportedException(); 
    }


    /// <summary>
    /// Returns all keys in an array.
    /// </summary>
    public ICollection<int> Keys {
      get {
        int[] keys = new int[count];
        if (count==0) return keys;

        var keyIndex = 0;
        for (int itemsIndex = FirstItemIndex; itemsIndex<=LastItemIndex; itemsIndex++) {
          var item = items[itemsIndex];
          if (item!=null) {
            keys[keyIndex++] = item.Key;
          }
        }
        return keys;
      }
    }


    /// <summary>
    /// Returns all items in an array
    /// </summary>
    public System.Collections.Generic.ICollection<TItem> Values {
      get {
        TItem[] values = new TItem[count];
        if (count==0) return values;

        var keyIndex = 0;
        for (int itemsIndex = FirstItemIndex; itemsIndex<=LastItemIndex; itemsIndex++) {
          var item = items[itemsIndex];
          if (item!=null) {
            values[keyIndex++] = item;
          }
        }
        return values;
      }
    }


    /// <summary>
    /// Number if items in StorageDictionary
    /// </summary>
    public int Count => count;


    /// <summary>
    /// Returns true if items are not updatable nor deletable
    /// </summary>
    public bool IsReadOnly {
      get { return !AreItemsUpdatable && !AreItemsDeletable; }
    }


    /// <summary>
    /// Gets the capacity of StorageDictionary. The capacity is the size of
    /// the internal array used to hold items, which can change over time.
    /// </summary>
    public int Capacity {
      get {
        return items.Length;
      }
    }


    ///// <summary>
    ///// Item with lowest Key in StorageDictionary
    ///// </summary>
    //public TItem? FirstItem { get; private set; }


    ///// <summary>
    ///// Item with highest value in StorageDictionary
    ///// </summary>
    //public TItem? LastItem { get; private set; }


    /// <summary>
    /// Are all Keys just incremented by 1 from the previous Key ? 
    /// </summary>
    public bool AreKeysContinous { get; private set; }
    internal void UpdateAreKeysContinous() {
      if (IsReadOnly || count==0) {
        AreKeysContinous = true;
      } else {
        AreKeysContinous = keys[LastItemIndex] - keys[FirstItemIndex] + 1 == count;
      }
    }


    /// <summary>
    /// Can content of an items be changed ? If yes, a change item gets written to the CVS file
    /// </summary>
    public bool AreItemsUpdatable { get; }


    /// <summary>
    /// The content of some items has changed and change items have been written to the CVS file. During
    /// Dispose() a new file is written containing only the latest version of the changed items.
    /// </summary>
    public bool AreItemsUpdated { get; private set; }


    /// <summary>
    /// Can stored items be removed ? If yes, a delete item gets written to the CVS file
    /// </summary>
    public bool AreItemsDeletable { get; }


    /// <summary>
    /// Some items have been deleted and delete items have been written to the CVS file. During
    /// Dispose() a new file is written containing only the undeleted items.
    /// </summary>
    public bool AreItemsDeleted { get; private set; }


    /// <summary>
    /// Parent holding all StorageDictionaries of the application
    /// </summary>
    public TContext? Context { get; }


    /// <summary>
    /// Index of first item, usually 0, not really used by inheritors
    /// </summary>
    protected int FirstItemIndex { get; private set; }


    /// <summary>
    /// Index of last item stored. Used by readonly StorageDictionaryCSV to create key for new item.
    /// </summary>
    protected int LastItemIndex { get; private set; }
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// An item was added to dictionary
    /// </summary>
    public event Action<TItem>? Added;

    /// <summary>
    /// A value of an item has changed in the dictionary
    /// </summary>
    public event Action<TItem>? Changed;

    /// <summary>
    /// An item was deleted from the dictionary
    /// </summary>
    public event Action<TItem>? Removed;

    #endregion


    #region Constructor
    //     ------------

    const uint defaultCapacity = 4;

    readonly Action<TItem, int> setKey;
    readonly Action<TItem>? disconnect;

    readonly object itemsLock = new object();
    TItem?[] items;
    static readonly TItem?[] emptyItems = new TItem[0];
    int[] keys; //keys don't get deleted when an item gets removed, because the key of the removed item is still needed for binary search
    static readonly int[] emptyKeys = new int[0];
    int count;
    int version;


    /// <summary>
    /// Constructs a readonly StorageDictionary with a given initial capacity. It is initially empty, but will have room for 
    /// the given number of items. When too many items get added, the capacity gets increased.
    /// </summary>
    public StorageDictionary(
      TContext? context,
      Action<TItem, int> setKey,
      int capacity = 0): this(context, setKey, null, false, false, capacity) {}


    /// <summary>
    /// Constructs StorageDictionary with a given initial capacity. It is initially empty, but will have room for the given 
    /// number of items. When too many items get added, the capacity gets increased.
    /// </summary>
    /// <param name="context">Should be parent holding all StorageDictionaries of the application</param>
    /// <param name="setKey">Called when an item gets added without a key (=-1)</param>
    /// <param name="disconnect">Called when an item gets removed (deleted). It might be necessary to disconnect also child
    /// items linked to this item and/or to remove item from parent(s)</param>
    /// <param name="areItemsUpdatable">Can the property of an item change ?</param>
    /// <param name="areItemsDeletable">Can an item be removed from StorageDictionary</param>
    /// <param name="capacity">How many items should StorageDictionary by able to hold initially ?</param>
    public StorageDictionary(
      TContext? context,
      Action<TItem, int> setKey,
      Action<TItem>? disconnect,
      bool areItemsUpdatable = false,
      bool areItemsDeletable = false,
      int capacity = 0) 
    {
      Context = context;
      this.setKey = setKey;
      this.disconnect = disconnect;
      if (capacity < 0) throw new ArgumentOutOfRangeException("Capacity must be equal or grater , but was '" + capacity + "'.");

      if (capacity==0) {
        items = emptyItems;
        keys = emptyKeys;
      } else { 
        items = new TItem[capacity];
        keys = new int[capacity];
      }

      AreItemsUpdatable = areItemsUpdatable;
      AreItemsUpdated = false;
      AreItemsDeletable = areItemsDeletable;
      AreItemsDeleted = false;
      initialiseItemsParamertes();
    }


    private void initialiseItemsParamertes() {
      FirstItemIndex = -1;
      LastItemIndex = -1;
      AreKeysContinous = true; // is true for empty collection
      count = 0;
    }
    #endregion


    #region IDictionary Interface
    //     ----------------------

    /// <summary>
    /// Adds the given item to the end of StorageDictionary. Better use Add(TItem item), this overload is provided for compatibility with
    /// IDictionary. If key!=value.key, an exception is thrown
    /// </summary>
    public void Add(int key, TItem value) {
      if (key!=value.Key) {
        throw new ArgumentException($"Key {key} must be the same as value.Key {value.Key}.");
      }
      Add(value);
    }


    /// <summary>
    /// Adds the given item to the end of StorageDictionary. Better use Add(TItem item), this overload is provided for compatibility with
    /// IDictionary. If item.Key!=item.value.key, an exception is thrown
    /// </summary>
    public void Add(KeyValuePair<int, TItem> item) {
      if (item.Key!=item.Value.Key) {
        throw new ArgumentException($"item.Key {item.Key} must be the same as item.Value.Key {item.Value.Key}.");
      }
      Add(item.Value);
    }


    /// <summary>
    /// Would clear the contents of StorageDictionary, but not supported to prevent re-entrance and concurrency problems. Create 
    /// new StorageDictionary instead.
    /// </summary>
    public void Clear() {
      throw new NotSupportedException();
    }


    /// <summary>
    /// Checks if item exists in StorageDirectionary and ensures that it is not marked as deleted. This method is provided for 
    /// compatibility with IDictionary. If item.Key!=item.value.key, an exception is thrown. Better use ContainsKey(int key) instead. 
    /// </summary>
    public bool Contains(KeyValuePair<int, TItem> item) {
      if (item.Key!=item.Value.Key) {
        throw new ArgumentException($"item.Key {item.Key} must be the same as item.Value.Key {item.Value.Key}.");
      }
      return binarySearch(item.Key)>=0;
    }


    /// <summary>
    /// Checks if item exists in StorageDirectionary and ensures that it is not marked as deleted. 
    /// </summary>
    public bool ContainsKey(int key) {
      return binarySearch(key)>=0;
    }


    /// <summary>
    /// Copies the elements of the ICollection to an Array, starting at a particular Array index. This method is provided for 
    /// compatibility with IDictionary.
    /// </summary>
    public void CopyTo(KeyValuePair<int, TItem>[] array, int index) {
      if (index < 0 || index > array.Length) {
        throw new ArgumentException($"Index {index} must be within array boundaries 0..{array.Length}.");
      }
      if (array.Length - index < count) {
        throw new ArgumentException($"Array with Length {array.Length} is too small to add {count} items at Index {index}.");
      }

      for (int itemIndex = FirstItemIndex; itemIndex<=LastItemIndex; itemIndex++) {
        var item = items[itemIndex];
        if (item!=null) {
          array[index++] = new KeyValuePair<int, TItem>(item.Key, item);
        }
      }
    }


    /// <summary>
    /// Returns an enumerator over all undeleted TItems in StorageDictionary
    /// </summary>
    public IEnumerator<TItem> GetEnumerator() {
      return new EnumeratorItems(this);
    }


    /// <summary>
    /// Returns an object enumerator over all undeleted TItems in StorageDictionary. Better use the strongly 
    /// typed IEnumerator<TItem> GetEnumerator(). 
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return new EnumeratorItems(this);
    }


    /// <summary>
    /// Returns an KeyValuePair enumerator over all undeleted TItems in StorageDictionary. This is only provided for
    /// to implement the IDictionary<int, TItem> interface. If possible use IEnumerator<TItem> GetEnumerator() instead. 
    /// </summary>
    IEnumerator<KeyValuePair<int, TItem>> IEnumerable<KeyValuePair<int, TItem>>.GetEnumerator() {
      return new EnumeratorItems(this);
    }


    /// <summary>
    /// Provided for compatibility with IDictionary. Use Remove(TItem item) instead;
    /// </summary>
    public bool Remove(int key) {
      if (!AreItemsDeletable) throw new NotSupportedException($"StorageDictionary for {typeof(TItem).Name} does not allow key '{key}' to be deleted.");

      int index;
      TItem? item;
      lock (itemsLock) {

        index = binarySearch(key);
        if (index<0) return false;

        item = items[index];
        if (item==null) return true; //item was already deleted

        disconnect!(item);
        items[index] = null;
        item.HasChanged -= item_HasChanged;
        version++;
        count--;
#if DEBUG
        if (count<0) throw new Exception(); //count should never become negative
#endif

        if (count<=0) {
          FirstItemIndex = -1;
          LastItemIndex = -1;
          AreKeysContinous = true;
        } else if (count==1) {
          if (index==FirstItemIndex) {
            FirstItemIndex = LastItemIndex;
#if DEBUG
          } else if (index!=LastItemIndex) {
            //we should never arrive here.
            throw new Exception();
#endif
          } else {
            LastItemIndex = FirstItemIndex;
          }
          AreKeysContinous = true;
        } else {
          if (FirstItemIndex==index) {
            do {
              FirstItemIndex++;
            } while (items[FirstItemIndex]==null);//since there are at least 2 items left, firstItemKey will always be smaller than lastItemkey
          } else if (LastItemIndex==index) {
            do {
              LastItemIndex--;
            } while (items[LastItemIndex]==null);//since there are at least 2 items left, lastItemkey will always be bigger than firstItemKey
          }
          UpdateAreKeysContinous();
        }
      }
      version++;
      AreItemsDeleted = true;
      OnItemRemoved(item);
      Removed?.Invoke(item);
      return true;
    }


    /// <summary>
    /// Called after item has been marked as deleted in StorageDictionary
    /// </summary>
    protected virtual void OnItemRemoved(TItem item) {
    }


    /// <summary>
    /// Provided for compatibility with IDictionary. Use Remove(TItem item) instead;
    /// </summary>
    public bool Remove(KeyValuePair<int, TItem> kvpItem) {
      if (kvpItem.Key!=kvpItem.Value.Key) {
        throw new ArgumentException($"item.Key {kvpItem.Key} must be the same as item.Value.Key {kvpItem.Value.Key}.");
      }
      if (kvpItem.Key<0) throw new Exception($"StorageDictionary can not remove item '{kvpItem}' with no key (-1).");

      return Remove(kvpItem.Key);
    }


    /// <summary>
    /// Removes item from this StorageDirectory and any children from their StorageDirectories. Removed event gets fired.
    /// If item was removed already, still true gets returned. No Removed event gets fired.
    /// </summary>
    public bool Remove(TItem item) {
      if (item.Key<0) throw new Exception($"StorageDictionary can not remove item '{item}' with no key (-1).");

      return Remove(item.Key);
    }


    /// <summary>
    /// If item with key is found, returns true and item in value, otherwise false and null in value.
    /// </summary>
    public bool TryGetValue(int key, [MaybeNullWhen(false)] out TItem value) {
      var index = binarySearch(key);
      if (index<0) {
        value = default!;
        return false;
      }
      value = items?[index]!;
      return true;
    }
    #endregion


    #region Disposable Interface
    //     ---------------------

    /// <summary>
    /// Executes disposal of StorageDictionary exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is StorageDictionary already disposed ?
    /// </summary>
    protected bool IsDisposed {
      get { return isDisposed==1; }
    }
    int isDisposed = 0;


    /// <summary>
    /// Inheritors should call Dispose(false) from their destructor
    /// </summary>
    protected void Dispose(bool disposing) {
      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
      if (wasDisposed==1) return; // already disposed

      OnDispose(disposing);
    }


    /// <summary>
    /// Inheritors should overwrite OnDispose() and put the disposal code in there. 
    /// </summary>
    /// <param name="disposing">is false if it is called from a destructor.</param>
    protected virtual void OnDispose(bool disposing) {
      //nothing to do for StorageDictionary, but inheritors might need to dispose a StreamWriter, db connection or something
    }
    #endregion


    #region Public Methods
    //     ---------------

    // From Microsoft imposes limits on maximum array length (defined as internal constant in Array class)
    const uint maxArrayLength = 0X7FEFFFFF;


    /// <summary>
    /// Adds the given item to the end of StorageDictionary and sets the item.Key. The Count is increased by one. If required, 
    /// the capacity of StorageDictionary is doubled before adding the new item.
    /// </summary>
    public void Add(TItem item) {
      if (item.Key>=0) throw new Exception($"Cannot add {typeof(TItem).Name} '{item}' to StorageDictionary, because it is already added (Key is 0 or bigger).");

      AddProtected(item);
    }


    protected void AddProtected(TItem item) {
      if (IsDisposed) throw new ObjectDisposedException("StorageDictionary");

      lock (itemsLock) {
        var lastItemKey = LastItemIndex==-1 ? -1 : items[LastItemIndex]!.Key;//throws exception if indexed item is null
        if (item.Key==StorageExtensions.NoKey) {
          setKey(item, ++lastItemKey);
        } else {
          if (item.Key<=lastItemKey) throw new Exception($"Cannot add {typeof(TItem).Name} '{item}' to StorageDictionary, because its key should be greater that lastItemKey {lastItemKey}.");
        }
        LastItemIndex++;
        if (count==0) {
          FirstItemIndex = LastItemIndex;
        }

        //ensure there is enough space
        if (LastItemIndex>=items.Length) {
          uint itemsLength = (uint)items.Length;
          uint newCapacityUInt = itemsLength == 0 ? defaultCapacity : itemsLength * 2;
          if (newCapacityUInt > maxArrayLength) newCapacityUInt = maxArrayLength;

          int newCapacity = (int)newCapacityUInt;

          var newItems = new TItem[newCapacity];
          var newKeys = new int[newCapacity];
          if (items!=emptyItems) {
            Array.Copy(items, 0, newItems, 0, LastItemIndex);
            Array.Copy(keys, 0, newKeys, 0, LastItemIndex);
          }
          items = newItems;
          keys = newKeys;
        }

        item.HasChanged += item_HasChanged;
        items[LastItemIndex] = item;
        keys[LastItemIndex] = item.Key;
        count++;
        version++;
      }
      OnItemAdded(item);
      Added?.Invoke(item);
    }


    /// <summary>
    /// Called when new item was added to StorageDictionary. 
    /// </summary>
    protected virtual void OnItemAdded(TItem item) {
    }


    private void item_HasChanged(TItem item) {
      if (!AreItemsUpdatable) throw new NotSupportedException($"StorageDictionary for {typeof(TItem).Name} does not allow item '{item}' to be updated.");

      AreItemsUpdated = true;
      version++;
      OnItemHasChanged(item);
      Changed?.Invoke(item);
    }


    /// <summary>
    /// Called when the content of an item has changed. There is no change (add, remove) in StorageDictionary itself. 
    /// </summary>
    protected virtual void OnItemHasChanged(TItem item) {
    }


    public override string ToString() {
      return
        $"Count: {Count};{(AreItemsUpdatable ? " Upd" : "")}{(AreItemsDeletable ? " Del" : "")}{(IsReadOnly ? " ReadOnly" : "")}" +
        $"{(AreKeysContinous ? " Cont" : "")}";
    }
    #endregion

    #region Private Methods
    //      ---------------

    int binarySearch(int key) {
      if (count==0) return -1;// StorageDictionary is empty

      var firstItemKey = items[FirstItemIndex]!.Key;//throws exception if firstItemIndex invalid or indexed item is null
      if (firstItemKey>key) return -1;// item is missing, key is too small

      var lastItemKey = items[LastItemIndex]!.Key;
      if (lastItemKey<key) return -1;// item is missing, key is too big

      if (AreKeysContinous) {
        return FirstItemIndex + key - firstItemKey;
      }

      return binarySearch(key, FirstItemIndex, LastItemIndex);
    }


    int binarySearch(int key, int min, int max) {
      while (min<=max) {
        int mid = (min + max) / 2;
        int compareResult = keys[mid].CompareTo(key);
        if (compareResult == 0) {
          if (items[mid]==null) {
            return -1;
          }
          return mid;
        } else if (compareResult == 1) {
          max = mid - 1;
        } else {
          min = mid + 1;
        }
      }
      return -1;
    }
    #endregion


    #region StorageDictionary Enumerator
    //      ----------------------------

    /// <summary>
    /// Item Enumerator for StorageDictionary
    /// </summary>
    [Serializable]
    public struct EnumeratorItems: IEnumerator<TItem>, IEnumerator<KeyValuePair<int, TItem>>, IEnumerator {
      public TItem Current {
        get { return current ?? throw new InvalidOperationException(); }
      }


      Object System.Collections.IEnumerator.Current {
        get { return current ?? throw new InvalidOperationException(); }
      }


      KeyValuePair<int, TItem> IEnumerator<KeyValuePair<int, TItem>>.Current {
        get {
          if (current==null) throw new InvalidOperationException();
          return new KeyValuePair<int, TItem>(current.Key, current);
        }
      }


      readonly StorageDictionary<TItem, TContext> storageDictionary;
      readonly int version;
      int index;
      readonly int maxIndex;
      TItem? current;


      /// <summary>
      /// Constructor
      /// </summary>
      internal EnumeratorItems(StorageDictionary<TItem, TContext> storageDictionary) {
        this.storageDictionary = storageDictionary;
        version = storageDictionary.version;
        if (storageDictionary.Count==0) {
          index = -1;
          maxIndex = -1;
        } else {
          index = storageDictionary.FirstItemIndex - 1;
          maxIndex = storageDictionary.LastItemIndex;
        }
        current = null;
      }


      /// <summary>
      /// Doesn't do anything
      /// </summary>
      public void Dispose() {
      }


      public bool MoveNext() {
        if (version!=storageDictionary.version) {
          throw new InvalidOperationException("StorageDictionary content has changed during enumeration.");
        }
        while (true) {
          index++;
          if (index>maxIndex) {
            break;
          }
          var item = storageDictionary.items[index];
          if (item!=null) {
            current = item;
            return true;
          }
        }

        //end reached
        index = maxIndex+1;
        current = null;
        return false;
      }


      void System.Collections.IEnumerator.Reset() {
        index = -1;
        current = default;
      }

      //bool IEnumerator.MoveNext() {
      //  throw new NotImplementedException();
      //}

      //void IDisposable.Dispose() {
      //  throw new NotImplementedException();
      //}
    }
    #endregion
  }
}
