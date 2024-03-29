//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into DemoParent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Demo parent
    /// </summary>
    /// 
  public partial class DemoParent: IStorageItemGeneric<DemoParent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for DemoParent. Gets set once DemoParent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem demoParent, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release DemoParent key @{demoParent.Key} #{demoParent.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store DemoParent key @{key} #{demoParent.GetHashCode()}");
        }
      }
#endif
      ((DemoParent)demoParent).Key = key;
    }


    public string DemoParentData { get; private set; }


    public IReadOnlyList<DemoChild> DemoChildren => demoChildren;
    readonly List<DemoChild> demoChildren;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "DemoParentData"};


    /// <summary>
    /// None existing DemoParent
    /// </summary>
    internal static DemoParent NoDemoParent = new DemoParent("NoDemoParentData", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of DemoParent has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/DemoParent, /*new*/DemoParent>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// DemoParent Constructor. If isStoring is true, adds DemoParent to DC.Data.DemoParents.
    /// </summary>
    public DemoParent(string demoParentData, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      DemoParentData = demoParentData;
      demoChildren = new List<DemoChild>();
#if DEBUG
      DC.Trace?.Invoke($"new DemoParent: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(34,TransactionActivityEnum.New, Key, this));
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
    public DemoParent(DemoParent original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      DemoParentData = original.DemoParentData;
      onCloned(this);
    }
    partial void onCloned(DemoParent clone);


    /// <summary>
    /// Constructor for DemoParent read from CSV file
    /// </summary>
    private DemoParent(int key, CsvReader csvReader){
      Key = key;
      DemoParentData = csvReader.ReadString();
      demoChildren = new List<DemoChild>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New DemoParent read from CSV file
    /// </summary>
    internal static DemoParent Create(int key, CsvReader csvReader) {
      return new DemoParent(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds DemoParent to DC.Data.DemoParents.<br/>
    /// Throws an Exception when DemoParent is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"DemoParent cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.DemoParents.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored DemoParent #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write DemoParent to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write DemoParent to CSV file
    /// </summary>
    internal static void Write(DemoParent demoParent, CsvWriter csvWriter) {
      demoParent.onCsvWrite();
      csvWriter.Write(demoParent.DemoParentData);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates DemoParent with the provided values
    /// </summary>
    public void Update(string demoParentData) {
      var clone = new DemoParent(this);
      var isCancelled = false;
      onUpdating(demoParentData, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating DemoParent: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (DemoParentData!=demoParentData) {
        DemoParentData = demoParentData;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.DemoParents.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(34, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated DemoParent: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string demoParentData, ref bool isCancelled);
    partial void onUpdated(DemoParent old);


    /// <summary>
    /// Updates this DemoParent with values from CSV file
    /// </summary>
    internal static void Update(DemoParent demoParent, CsvReader csvReader){
      demoParent.DemoParentData = csvReader.ReadString();
      demoParent.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add demoChild to DemoChildren.
    /// </summary>
    internal void AddToDemoChildren(DemoChild demoChild) {
#if DEBUG
      if (demoChild==DemoChild.NoDemoChild) throw new Exception();
      if ((demoChild.Key>=0)&&(Key<0)) throw new Exception();
      if (demoChildren.Contains(demoChild)) throw new Exception();
#endif
      demoChildren.Add(demoChild);
      onAddedToDemoChildren(demoChild);
#if DEBUG
      DC.Trace?.Invoke($"Add DemoChild {demoChild.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} DemoParent.DemoChildren");
#endif
    }
    partial void onAddedToDemoChildren(DemoChild demoChild);


    /// <summary>
    /// Removes demoChild from DemoParent.
    /// </summary>
    internal void RemoveFromDemoChildren(DemoChild demoChild) {
#if DEBUG
      if (!demoChildren.Remove(demoChild)) throw new Exception();
#else
        demoChildren.Remove(demoChild);
#endif
      onRemovedFromDemoChildren(demoChild);
#if DEBUG
      DC.Trace?.Invoke($"Remove DemoChild {demoChild.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} DemoParent.DemoChildren");
#endif
    }
    partial void onRemovedFromDemoChildren(DemoChild demoChild);


    /// <summary>
    /// Removes DemoParent from DC.Data.DemoParents.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"DemoParent.Release(): DemoParent '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var demoChild in DemoChildren) {
        if (demoChild?.Key>=0) {
          throw new Exception($"Cannot release DemoParent '{this}' " + Environment.NewLine + 
            $"because '{demoChild}' in DemoParent.DemoChildren is still stored.");
        }
      }
      onReleased();
      DC.Data.DemoParents.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released DemoParent @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var demoParent = (DemoParent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new DemoParent(): {demoParent.ToTraceString()}");
#endif
      demoParent.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases DemoParent from DC.Data.DemoParents as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var demoParent = (DemoParent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback DemoParent.Store(): {demoParent.ToTraceString()}");
#endif
      demoParent.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the DemoParent item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (DemoParent) oldStorageItem;
      var newItem = (DemoParent) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back DemoParent.Update(): {newItem.ToTraceString()}");
#endif
      newItem.DemoParentData = oldItem.DemoParentData;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back DemoParent.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(DemoParent oldDemoParent);


    /// <summary>
    /// Adds DemoParent to DC.Data.DemoParents as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var demoParent = (DemoParent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback DemoParent.Release(): {demoParent.ToTraceString()}");
#endif
      demoParent.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {DemoParentData}";
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
        $" {DemoParentData}";
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
        $" DemoParentData: {DemoParentData}," +
        $" DemoChildren: {DemoChildren.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
