//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into NotMatchingChildrenListName_Parent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example where the parent's List for it's children is not the plural of the child type type. 
    /// </summary>
  public partial class NotMatchingChildrenListName_Parent: IStorageItemGeneric<NotMatchingChildrenListName_Parent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for NotMatchingChildrenListName_Parent. Gets set once NotMatchingChildrenListName_Parent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem notMatchingChildrenListName_Parent, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release NotMatchingChildrenListName_Parent key @{notMatchingChildrenListName_Parent.Key} #{notMatchingChildrenListName_Parent.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store NotMatchingChildrenListName_Parent key @{key} #{notMatchingChildrenListName_Parent.GetHashCode()}");
        }
      }
#endif
      ((NotMatchingChildrenListName_Parent)notMatchingChildrenListName_Parent).Key = key;
    }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Deletable children which must have a parent
    /// </summary>
    public IReadOnlyList<NotMatchingChildrenListName_Child> Children => children;
    readonly List<NotMatchingChildrenListName_Child> children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing NotMatchingChildrenListName_Parent
    /// </summary>
    internal static NotMatchingChildrenListName_Parent NoNotMatchingChildrenListName_Parent = new NotMatchingChildrenListName_Parent("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of NotMatchingChildrenListName_Parent has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/NotMatchingChildrenListName_Parent, /*new*/NotMatchingChildrenListName_Parent>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// NotMatchingChildrenListName_Parent Constructor. If isStoring is true, adds NotMatchingChildrenListName_Parent to DC.Data.NotMatchingChildrenListName_Parents.
    /// </summary>
    public NotMatchingChildrenListName_Parent(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      children = new List<NotMatchingChildrenListName_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new NotMatchingChildrenListName_Parent: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(36,TransactionActivityEnum.New, Key, this));
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
    public NotMatchingChildrenListName_Parent(NotMatchingChildrenListName_Parent original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(NotMatchingChildrenListName_Parent clone);


    /// <summary>
    /// Constructor for NotMatchingChildrenListName_Parent read from CSV file
    /// </summary>
    private NotMatchingChildrenListName_Parent(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      children = new List<NotMatchingChildrenListName_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New NotMatchingChildrenListName_Parent read from CSV file
    /// </summary>
    internal static NotMatchingChildrenListName_Parent Create(int key, CsvReader csvReader) {
      return new NotMatchingChildrenListName_Parent(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds NotMatchingChildrenListName_Parent to DC.Data.NotMatchingChildrenListName_Parents.<br/>
    /// Throws an Exception when NotMatchingChildrenListName_Parent is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"NotMatchingChildrenListName_Parent cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.NotMatchingChildrenListName_Parents.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored NotMatchingChildrenListName_Parent #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write NotMatchingChildrenListName_Parent to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write NotMatchingChildrenListName_Parent to CSV file
    /// </summary>
    internal static void Write(NotMatchingChildrenListName_Parent notMatchingChildrenListName_Parent, CsvWriter csvWriter) {
      notMatchingChildrenListName_Parent.onCsvWrite();
      csvWriter.Write(notMatchingChildrenListName_Parent.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates NotMatchingChildrenListName_Parent with the provided values
    /// </summary>
    public void Update(string text) {
      var clone = new NotMatchingChildrenListName_Parent(this);
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating NotMatchingChildrenListName_Parent: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.NotMatchingChildrenListName_Parents.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(36, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated NotMatchingChildrenListName_Parent: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated(NotMatchingChildrenListName_Parent old);


    /// <summary>
    /// Updates this NotMatchingChildrenListName_Parent with values from CSV file
    /// </summary>
    internal static void Update(NotMatchingChildrenListName_Parent notMatchingChildrenListName_Parent, CsvReader csvReader){
      notMatchingChildrenListName_Parent.Text = csvReader.ReadString();
      notMatchingChildrenListName_Parent.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add notMatchingChildrenListName_Child to Children.
    /// </summary>
    internal void AddToChildren(NotMatchingChildrenListName_Child notMatchingChildrenListName_Child) {
#if DEBUG
      if (notMatchingChildrenListName_Child==NotMatchingChildrenListName_Child.NoNotMatchingChildrenListName_Child) throw new Exception();
      if ((notMatchingChildrenListName_Child.Key>=0)&&(Key<0)) throw new Exception();
      if (children.Contains(notMatchingChildrenListName_Child)) throw new Exception();
#endif
      children.Add(notMatchingChildrenListName_Child);
      onAddedToChildren(notMatchingChildrenListName_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add NotMatchingChildrenListName_Child {notMatchingChildrenListName_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} NotMatchingChildrenListName_Parent.Children");
#endif
    }
    partial void onAddedToChildren(NotMatchingChildrenListName_Child notMatchingChildrenListName_Child);


    /// <summary>
    /// Removes notMatchingChildrenListName_Child from NotMatchingChildrenListName_Parent.
    /// </summary>
    internal void RemoveFromChildren(NotMatchingChildrenListName_Child notMatchingChildrenListName_Child) {
#if DEBUG
      if (!children.Remove(notMatchingChildrenListName_Child)) throw new Exception();
#else
        children.Remove(notMatchingChildrenListName_Child);
#endif
      onRemovedFromChildren(notMatchingChildrenListName_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove NotMatchingChildrenListName_Child {notMatchingChildrenListName_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} NotMatchingChildrenListName_Parent.Children");
#endif
    }
    partial void onRemovedFromChildren(NotMatchingChildrenListName_Child notMatchingChildrenListName_Child);


    /// <summary>
    /// Removes NotMatchingChildrenListName_Parent from DC.Data.NotMatchingChildrenListName_Parents.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"NotMatchingChildrenListName_Parent.Release(): NotMatchingChildrenListName_Parent '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var notMatchingChildrenListName_Child in Children) {
        if (notMatchingChildrenListName_Child?.Key>=0) {
          throw new Exception($"Cannot release NotMatchingChildrenListName_Parent '{this}' " + Environment.NewLine + 
            $"because '{notMatchingChildrenListName_Child}' in NotMatchingChildrenListName_Parent.Children is still stored.");
        }
      }
      onReleased();
      DC.Data.NotMatchingChildrenListName_Parents.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released NotMatchingChildrenListName_Parent @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var notMatchingChildrenListName_Parent = (NotMatchingChildrenListName_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new NotMatchingChildrenListName_Parent(): {notMatchingChildrenListName_Parent.ToTraceString()}");
#endif
      notMatchingChildrenListName_Parent.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases NotMatchingChildrenListName_Parent from DC.Data.NotMatchingChildrenListName_Parents as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var notMatchingChildrenListName_Parent = (NotMatchingChildrenListName_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback NotMatchingChildrenListName_Parent.Store(): {notMatchingChildrenListName_Parent.ToTraceString()}");
#endif
      notMatchingChildrenListName_Parent.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the NotMatchingChildrenListName_Parent item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (NotMatchingChildrenListName_Parent) oldStorageItem;
      var newItem = (NotMatchingChildrenListName_Parent) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back NotMatchingChildrenListName_Parent.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back NotMatchingChildrenListName_Parent.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(NotMatchingChildrenListName_Parent oldNotMatchingChildrenListName_Parent);


    /// <summary>
    /// Adds NotMatchingChildrenListName_Parent to DC.Data.NotMatchingChildrenListName_Parents as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var notMatchingChildrenListName_Parent = (NotMatchingChildrenListName_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback NotMatchingChildrenListName_Parent.Release(): {notMatchingChildrenListName_Parent.ToTraceString()}");
#endif
      notMatchingChildrenListName_Parent.onRollbackItemRelease();
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
        $" Children: {Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
