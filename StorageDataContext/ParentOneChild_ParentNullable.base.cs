//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ParentOneChild_ParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example for parent which can have at most 1 child and the parent property in the child is nullable.
    /// </summary>
  public partial class ParentOneChild_ParentNullable: IStorageItemGeneric<ParentOneChild_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ParentOneChild_ParentNullable. Gets set once ParentOneChild_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem parentOneChild_ParentNullable, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ParentOneChild_ParentNullable key @{parentOneChild_ParentNullable.Key} #{parentOneChild_ParentNullable.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ParentOneChild_ParentNullable key @{key} #{parentOneChild_ParentNullable.GetHashCode()}");
        }
      }
#endif
      ((ParentOneChild_ParentNullable)parentOneChild_ParentNullable).Key = key;
    }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Links to conditional child. Parent might or might not have a child, since the parent always gets
    /// created before the child.
    /// </summary>
    public ParentOneChild_Child? Child { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ParentOneChild_ParentNullable
    /// </summary>
    internal static ParentOneChild_ParentNullable NoParentOneChild_ParentNullable = new ParentOneChild_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ParentOneChild_ParentNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ParentOneChild_ParentNullable, /*new*/ParentOneChild_ParentNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ParentOneChild_ParentNullable Constructor. If isStoring is true, adds ParentOneChild_ParentNullable to DC.Data.ParentOneChild_ParentNullables.
    /// </summary>
    public ParentOneChild_ParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
#if DEBUG
      DC.Trace?.Invoke($"new ParentOneChild_ParentNullable: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(10,TransactionActivityEnum.New, Key, this));
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
    public ParentOneChild_ParentNullable(ParentOneChild_ParentNullable original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(ParentOneChild_ParentNullable clone);


    /// <summary>
    /// Constructor for ParentOneChild_ParentNullable read from CSV file
    /// </summary>
    private ParentOneChild_ParentNullable(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ParentOneChild_ParentNullable read from CSV file
    /// </summary>
    internal static ParentOneChild_ParentNullable Create(int key, CsvReader csvReader) {
      return new ParentOneChild_ParentNullable(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ParentOneChild_ParentNullable to DC.Data.ParentOneChild_ParentNullables.<br/>
    /// Throws an Exception when ParentOneChild_ParentNullable is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ParentOneChild_ParentNullable cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ParentOneChild_ParentNullables.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ParentOneChild_ParentNullable #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ParentOneChild_ParentNullable to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write ParentOneChild_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ParentOneChild_ParentNullable parentOneChild_ParentNullable, CsvWriter csvWriter) {
      parentOneChild_ParentNullable.onCsvWrite();
      csvWriter.Write(parentOneChild_ParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ParentOneChild_ParentNullable with the provided values
    /// </summary>
    public void Update(string text) {
      var clone = new ParentOneChild_ParentNullable(this);
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ParentOneChild_ParentNullable: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ParentOneChild_ParentNullables.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(10, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ParentOneChild_ParentNullable: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated(ParentOneChild_ParentNullable old);


    /// <summary>
    /// Updates this ParentOneChild_ParentNullable with values from CSV file
    /// </summary>
    internal static void Update(ParentOneChild_ParentNullable parentOneChild_ParentNullable, CsvReader csvReader){
      parentOneChild_ParentNullable.Text = csvReader.ReadString();
      parentOneChild_ParentNullable.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add parentOneChild_Child to Child.
    /// </summary>
    internal void AddToChild(ParentOneChild_Child parentOneChild_Child) {
#if DEBUG
      if (parentOneChild_Child==ParentOneChild_Child.NoParentOneChild_Child) throw new Exception();
      if ((parentOneChild_Child.Key>=0)&&(Key<0)) throw new Exception();
      if(Child==parentOneChild_Child) throw new Exception();
#endif
      if (Child!=null) {
        throw new Exception($"ParentOneChild_ParentNullable.AddToChild(): '{Child}' is already assigned to Child, it is not possible to assign now '{parentOneChild_Child}'.");
      }
      Child = parentOneChild_Child;
      onAddedToChild(parentOneChild_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add ParentOneChild_Child {parentOneChild_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} ParentOneChild_ParentNullable.Child");
#endif
    }
    partial void onAddedToChild(ParentOneChild_Child parentOneChild_Child);


    /// <summary>
    /// Removes parentOneChild_Child from ParentOneChild_ParentNullable.
    /// </summary>
    internal void RemoveFromChild(ParentOneChild_Child parentOneChild_Child) {
#if DEBUG
      if (Child!=parentOneChild_Child) {
        throw new Exception($"ParentOneChild_ParentNullable.RemoveFromChild(): Child does not link to parentOneChild_Child '{parentOneChild_Child}' but '{Child}'.");
      }
#endif
      Child = null;
      onRemovedFromChild(parentOneChild_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove ParentOneChild_Child {parentOneChild_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} ParentOneChild_ParentNullable.Child");
#endif
    }
    partial void onRemovedFromChild(ParentOneChild_Child parentOneChild_Child);


    /// <summary>
    /// Removes ParentOneChild_ParentNullable from DC.Data.ParentOneChild_ParentNullables.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ParentOneChild_ParentNullable.Release(): ParentOneChild_ParentNullable '{this}' is not stored in DC.Data, key is {Key}.");
      }
      if (Child?.Key>=0) {
        throw new Exception($"Cannot release ParentOneChild_ParentNullable '{this}' " + Environment.NewLine + 
          $"because '{Child}' in ParentOneChild_ParentNullable.Child is still stored.");
      }
      onReleased();
      DC.Data.ParentOneChild_ParentNullables.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ParentOneChild_ParentNullable @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var parentOneChild_ParentNullable = (ParentOneChild_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ParentOneChild_ParentNullable(): {parentOneChild_ParentNullable.ToTraceString()}");
#endif
      parentOneChild_ParentNullable.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ParentOneChild_ParentNullable from DC.Data.ParentOneChild_ParentNullables as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var parentOneChild_ParentNullable = (ParentOneChild_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ParentOneChild_ParentNullable.Store(): {parentOneChild_ParentNullable.ToTraceString()}");
#endif
      parentOneChild_ParentNullable.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ParentOneChild_ParentNullable item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ParentOneChild_ParentNullable) oldStorageItem;
      var newItem = (ParentOneChild_ParentNullable) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ParentOneChild_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ParentOneChild_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ParentOneChild_ParentNullable oldParentOneChild_ParentNullable);


    /// <summary>
    /// Adds ParentOneChild_ParentNullable to DC.Data.ParentOneChild_ParentNullables as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var parentOneChild_ParentNullable = (ParentOneChild_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ParentOneChild_ParentNullable.Release(): {parentOneChild_ParentNullable.ToTraceString()}");
#endif
      parentOneChild_ParentNullable.onRollbackItemRelease();
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
        $" Child: {Child?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
