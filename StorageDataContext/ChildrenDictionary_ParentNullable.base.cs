//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenDictionary_ParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a parent child relationship using a Dictionary where the child's parent property is nullable.
    /// </summary>
  public partial class ChildrenDictionary_ParentNullable: IStorageItemGeneric<ChildrenDictionary_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenDictionary_ParentNullable. Gets set once ChildrenDictionary_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenDictionary_ParentNullable, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenDictionary_ParentNullable key @{childrenDictionary_ParentNullable.Key} #{childrenDictionary_ParentNullable.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenDictionary_ParentNullable key @{key} #{childrenDictionary_ParentNullable.GetHashCode()}");
        }
      }
#endif
      ((ChildrenDictionary_ParentNullable)childrenDictionary_ParentNullable).Key = key;
    }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    /// use as key
    /// </summary>
    public IReadOnlyDictionary<DateTime, ChildrenDictionary_Child> ChildrenDictionary_Children => childrenDictionary_Children;
    readonly Dictionary<DateTime, ChildrenDictionary_Child> childrenDictionary_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ChildrenDictionary_ParentNullable
    /// </summary>
    internal static ChildrenDictionary_ParentNullable NoChildrenDictionary_ParentNullable = new ChildrenDictionary_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenDictionary_ParentNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenDictionary_ParentNullable, /*new*/ChildrenDictionary_ParentNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenDictionary_ParentNullable Constructor. If isStoring is true, adds ChildrenDictionary_ParentNullable to DC.Data.ChildrenDictionary_ParentNullables.
    /// </summary>
    public ChildrenDictionary_ParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      childrenDictionary_Children = new Dictionary<DateTime, ChildrenDictionary_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenDictionary_ParentNullable: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(23,TransactionActivityEnum.New, Key, this));
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
    public ChildrenDictionary_ParentNullable(ChildrenDictionary_ParentNullable original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(ChildrenDictionary_ParentNullable clone);


    /// <summary>
    /// Constructor for ChildrenDictionary_ParentNullable read from CSV file
    /// </summary>
    private ChildrenDictionary_ParentNullable(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      childrenDictionary_Children = new Dictionary<DateTime, ChildrenDictionary_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenDictionary_ParentNullable read from CSV file
    /// </summary>
    internal static ChildrenDictionary_ParentNullable Create(int key, CsvReader csvReader) {
      return new ChildrenDictionary_ParentNullable(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenDictionary_ParentNullable to DC.Data.ChildrenDictionary_ParentNullables.<br/>
    /// Throws an Exception when ChildrenDictionary_ParentNullable is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenDictionary_ParentNullable cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenDictionary_ParentNullables.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenDictionary_ParentNullable #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenDictionary_ParentNullable to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write ChildrenDictionary_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ChildrenDictionary_ParentNullable childrenDictionary_ParentNullable, CsvWriter csvWriter) {
      childrenDictionary_ParentNullable.onCsvWrite();
      csvWriter.Write(childrenDictionary_ParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenDictionary_ParentNullable with the provided values
    /// </summary>
    public void Update(string text) {
      var clone = new ChildrenDictionary_ParentNullable(this);
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ChildrenDictionary_ParentNullable: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenDictionary_ParentNullables.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(23, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ChildrenDictionary_ParentNullable: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated(ChildrenDictionary_ParentNullable old);


    /// <summary>
    /// Updates this ChildrenDictionary_ParentNullable with values from CSV file
    /// </summary>
    internal static void Update(ChildrenDictionary_ParentNullable childrenDictionary_ParentNullable, CsvReader csvReader){
      childrenDictionary_ParentNullable.Text = csvReader.ReadString();
      childrenDictionary_ParentNullable.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add childrenDictionary_Child to ChildrenDictionary_Children.
    /// </summary>
    internal void AddToChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child) {
#if DEBUG
      if (childrenDictionary_Child==ChildrenDictionary_Child.NoChildrenDictionary_Child) throw new Exception();
      if ((childrenDictionary_Child.Key>=0)&&(Key<0)) throw new Exception();
      if (childrenDictionary_Children.ContainsKey(childrenDictionary_Child.DateKey)) throw new Exception();
#endif
      childrenDictionary_Children.Add(childrenDictionary_Child.DateKey, childrenDictionary_Child);
      onAddedToChildrenDictionary_Children(childrenDictionary_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add ChildrenDictionary_Child {childrenDictionary_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} ChildrenDictionary_ParentNullable.ChildrenDictionary_Children");
#endif
    }
    partial void onAddedToChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child);


    /// <summary>
    /// Removes childrenDictionary_Child from ChildrenDictionary_ParentNullable.
    /// </summary>
    internal void RemoveFromChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child) {
#if DEBUG
      if (!childrenDictionary_Children.Remove(childrenDictionary_Child.DateKey)) throw new Exception();
#else
        childrenDictionary_Children.Remove(childrenDictionary_Child.DateKey);
#endif
      onRemovedFromChildrenDictionary_Children(childrenDictionary_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove ChildrenDictionary_Child {childrenDictionary_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} ChildrenDictionary_ParentNullable.ChildrenDictionary_Children");
#endif
    }
    partial void onRemovedFromChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child);


    /// <summary>
    /// Removes ChildrenDictionary_ParentNullable from DC.Data.ChildrenDictionary_ParentNullables.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ChildrenDictionary_ParentNullable.Release(): ChildrenDictionary_ParentNullable '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var childrenDictionary_Child in ChildrenDictionary_Children.Values) {
        if (childrenDictionary_Child?.Key>=0) {
          throw new Exception($"Cannot release ChildrenDictionary_ParentNullable '{this}' " + Environment.NewLine + 
            $"because '{childrenDictionary_Child}' in ChildrenDictionary_ParentNullable.ChildrenDictionary_Children is still stored.");
        }
      }
      onReleased();
      DC.Data.ChildrenDictionary_ParentNullables.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ChildrenDictionary_ParentNullable @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenDictionary_ParentNullable = (ChildrenDictionary_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenDictionary_ParentNullable(): {childrenDictionary_ParentNullable.ToTraceString()}");
#endif
      childrenDictionary_ParentNullable.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenDictionary_ParentNullable from DC.Data.ChildrenDictionary_ParentNullables as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenDictionary_ParentNullable = (ChildrenDictionary_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenDictionary_ParentNullable.Store(): {childrenDictionary_ParentNullable.ToTraceString()}");
#endif
      childrenDictionary_ParentNullable.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenDictionary_ParentNullable item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenDictionary_ParentNullable) oldStorageItem;
      var newItem = (ChildrenDictionary_ParentNullable) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenDictionary_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenDictionary_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenDictionary_ParentNullable oldChildrenDictionary_ParentNullable);


    /// <summary>
    /// Adds ChildrenDictionary_ParentNullable to DC.Data.ChildrenDictionary_ParentNullables as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenDictionary_ParentNullable = (ChildrenDictionary_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenDictionary_ParentNullable.Release(): {childrenDictionary_ParentNullable.ToTraceString()}");
#endif
      childrenDictionary_ParentNullable.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {Text}";
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
        $" {Text}";
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
        $" Text: {Text}," +
        $" ChildrenDictionary_Children: {ChildrenDictionary_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
