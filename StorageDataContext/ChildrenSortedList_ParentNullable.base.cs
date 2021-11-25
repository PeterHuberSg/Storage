//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenSortedList_ParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a parent child relationship using a SortedList where the child's parent property is nullable.
    /// </summary>
  public partial class ChildrenSortedList_ParentNullable: IStorageItemGeneric<ChildrenSortedList_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenSortedList_ParentNullable. Gets set once ChildrenSortedList_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenSortedList_ParentNullable, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenSortedList_ParentNullable key @{childrenSortedList_ParentNullable.Key} #{childrenSortedList_ParentNullable.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenSortedList_ParentNullable key @{key} #{childrenSortedList_ParentNullable.GetHashCode()}");
        }
      }
#endif
      ((ChildrenSortedList_ParentNullable)childrenSortedList_ParentNullable).Key = key;
    }


    /// <summary>
    /// This text is readonly. Readonly only matters when [StorageClass(areInstancesUpdatable: true)]
    /// </summary>
    public string TextReadOnly { get; }


    /// <summary>
    /// This text can be updated
    /// </summary>
    public string TextUpdateable { get; private set; }


    /// <summary>
    /// SortedList used instead of List. Comment is required and indicates which property of the SortedListChild to 
    /// use as key
    /// </summary>
    public IReadOnlyDictionary<DateTime, ChildrenSortedList_Child> ChildrenSortedList_Children => childrenSortedList_Children;
    readonly SortedList<DateTime, ChildrenSortedList_Child> childrenSortedList_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "TextReadOnly", "TextUpdateable"};


    /// <summary>
    /// None existing ChildrenSortedList_ParentNullable
    /// </summary>
    internal static ChildrenSortedList_ParentNullable NoChildrenSortedList_ParentNullable = new ChildrenSortedList_ParentNullable("NoTextReadOnly", "NoTextUpdateable", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenSortedList_ParentNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenSortedList_ParentNullable, /*new*/ChildrenSortedList_ParentNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenSortedList_ParentNullable Constructor. If isStoring is true, adds ChildrenSortedList_ParentNullable to DC.Data.ChildrenSortedList_ParentNullables.
    /// </summary>
    public ChildrenSortedList_ParentNullable(string textReadOnly, string textUpdateable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      TextReadOnly = textReadOnly;
      TextUpdateable = textUpdateable;
      childrenSortedList_Children = new SortedList<DateTime, ChildrenSortedList_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenSortedList_ParentNullable: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(26,TransactionActivityEnum.New, Key, this));
      }

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Cloning constructor. It will copy all data from original except any collection (children).
    /// </summary>
    #pragma warning disable CS8618 // Children collections are uninitialized.
    public ChildrenSortedList_ParentNullable(ChildrenSortedList_ParentNullable original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      TextReadOnly = original.TextReadOnly;
      TextUpdateable = original.TextUpdateable;
      onCloned(this);
    }
    partial void onCloned(ChildrenSortedList_ParentNullable clone);


    /// <summary>
    /// Constructor for ChildrenSortedList_ParentNullable read from CSV file
    /// </summary>
    private ChildrenSortedList_ParentNullable(int key, CsvReader csvReader){
      Key = key;
      TextReadOnly = csvReader.ReadString();
      TextUpdateable = csvReader.ReadString();
      childrenSortedList_Children = new SortedList<DateTime, ChildrenSortedList_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenSortedList_ParentNullable read from CSV file
    /// </summary>
    internal static ChildrenSortedList_ParentNullable Create(int key, CsvReader csvReader) {
      return new ChildrenSortedList_ParentNullable(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenSortedList_ParentNullable to DC.Data.ChildrenSortedList_ParentNullables.<br/>
    /// Throws an Exception when ChildrenSortedList_ParentNullable is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenSortedList_ParentNullable cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenSortedList_ParentNullables.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenSortedList_ParentNullable #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenSortedList_ParentNullable to CSV file
    /// </summary>
    public const int EstimatedLineLength = 300;


    /// <summary>
    /// Write ChildrenSortedList_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ChildrenSortedList_ParentNullable childrenSortedList_ParentNullable, CsvWriter csvWriter) {
      childrenSortedList_ParentNullable.onCsvWrite();
      csvWriter.Write(childrenSortedList_ParentNullable.TextReadOnly);
      csvWriter.Write(childrenSortedList_ParentNullable.TextUpdateable);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenSortedList_ParentNullable with the provided values
    /// </summary>
    public void Update(string textUpdateable) {
      var clone = new ChildrenSortedList_ParentNullable(this);
      var isCancelled = false;
      onUpdating(textUpdateable, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ChildrenSortedList_ParentNullable: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (TextUpdateable!=textUpdateable) {
        TextUpdateable = textUpdateable;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenSortedList_ParentNullables.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(26, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ChildrenSortedList_ParentNullable: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string textUpdateable, ref bool isCancelled);
    partial void onUpdated(ChildrenSortedList_ParentNullable old);


    /// <summary>
    /// Updates this ChildrenSortedList_ParentNullable with values from CSV file
    /// </summary>
    internal static void Update(ChildrenSortedList_ParentNullable childrenSortedList_ParentNullable, CsvReader csvReader){
      var textReadOnly = csvReader.ReadString();
      if (childrenSortedList_ParentNullable.TextReadOnly!=textReadOnly) {
        throw new Exception($"ChildrenSortedList_ParentNullable.Update(): Property TextReadOnly '{childrenSortedList_ParentNullable.TextReadOnly}' is " +
          $"readonly, textReadOnly '{textReadOnly}' read from the CSV file should be the same." + Environment.NewLine + 
          childrenSortedList_ParentNullable.ToString());
      }
      childrenSortedList_ParentNullable.TextUpdateable = csvReader.ReadString();
      childrenSortedList_ParentNullable.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add childrenSortedList_Child to ChildrenSortedList_Children.
    /// </summary>
    internal void AddToChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child) {
#if DEBUG
      if (childrenSortedList_Child==ChildrenSortedList_Child.NoChildrenSortedList_Child) throw new Exception();
      if ((childrenSortedList_Child.Key>=0)&&(Key<0)) throw new Exception();
      if (childrenSortedList_Children.ContainsKey(childrenSortedList_Child.DateKey)) throw new Exception();
#endif
      childrenSortedList_Children.Add(childrenSortedList_Child.DateKey, childrenSortedList_Child);
      onAddedToChildrenSortedList_Children(childrenSortedList_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add ChildrenSortedList_Child {childrenSortedList_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} ChildrenSortedList_ParentNullable.ChildrenSortedList_Children");
#endif
    }
    partial void onAddedToChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child);


    /// <summary>
    /// Removes childrenSortedList_Child from ChildrenSortedList_ParentNullable.
    /// </summary>
    internal void RemoveFromChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child) {
#if DEBUG
      if (!childrenSortedList_Children.Remove(childrenSortedList_Child.DateKey)) throw new Exception();
#else
        childrenSortedList_Children.Remove(childrenSortedList_Child.DateKey);
#endif
      onRemovedFromChildrenSortedList_Children(childrenSortedList_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove ChildrenSortedList_Child {childrenSortedList_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} ChildrenSortedList_ParentNullable.ChildrenSortedList_Children");
#endif
    }
    partial void onRemovedFromChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child);


    /// <summary>
    /// Removes ChildrenSortedList_ParentNullable from DC.Data.ChildrenSortedList_ParentNullables.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ChildrenSortedList_ParentNullable.Release(): ChildrenSortedList_ParentNullable '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var childrenSortedList_Child in ChildrenSortedList_Children.Values) {
        if (childrenSortedList_Child?.Key>=0) {
          throw new Exception($"Cannot release ChildrenSortedList_ParentNullable '{this}' " + Environment.NewLine + 
            $"because '{childrenSortedList_Child}' in ChildrenSortedList_ParentNullable.ChildrenSortedList_Children is still stored.");
        }
      }
      onReleased();
      DC.Data.ChildrenSortedList_ParentNullables.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ChildrenSortedList_ParentNullable @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenSortedList_ParentNullable = (ChildrenSortedList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenSortedList_ParentNullable(): {childrenSortedList_ParentNullable.ToTraceString()}");
#endif
      childrenSortedList_ParentNullable.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenSortedList_ParentNullable from DC.Data.ChildrenSortedList_ParentNullables as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenSortedList_ParentNullable = (ChildrenSortedList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenSortedList_ParentNullable.Store(): {childrenSortedList_ParentNullable.ToTraceString()}");
#endif
      childrenSortedList_ParentNullable.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenSortedList_ParentNullable item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenSortedList_ParentNullable) oldStorageItem;
      var newItem = (ChildrenSortedList_ParentNullable) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenSortedList_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
      if (newItem.TextReadOnly!=oldItem.TextReadOnly) {
        throw new Exception($"ChildrenSortedList_ParentNullable.Update(): Property TextReadOnly '{newItem.TextReadOnly}' is " +
          $"readonly, TextReadOnly '{oldItem.TextReadOnly}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      newItem.TextUpdateable = oldItem.TextUpdateable;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenSortedList_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenSortedList_ParentNullable oldChildrenSortedList_ParentNullable);


    /// <summary>
    /// Adds ChildrenSortedList_ParentNullable to DC.Data.ChildrenSortedList_ParentNullables as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenSortedList_ParentNullable = (ChildrenSortedList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenSortedList_ParentNullable.Release(): {childrenSortedList_ParentNullable.ToTraceString()}");
#endif
      childrenSortedList_ParentNullable.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {TextReadOnly}|" +
        $" {TextUpdateable}";
      onToTraceString(ref returnString);
      return returnString;
    }
    partial void onToTraceString(ref string returnString);


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {TextReadOnly}," +
        $" {TextUpdateable}";
      onToShortString(ref returnString);
      return returnString;
    }
    partial void onToShortString(ref string returnString);


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      var returnString =
        $"Key: {Key.ToKeyString()}," +
        $" TextReadOnly: {TextReadOnly}," +
        $" TextUpdateable: {TextUpdateable}," +
        $" ChildrenSortedList_Children: {ChildrenSortedList_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
