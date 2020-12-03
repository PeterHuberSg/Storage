//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into CreateOnlyParentChangeableChild_Parent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a "CreateOnly" Parent, i.e. this parent's properties will not change and this parent will never get
    /// deleted, but it is still possible to add and remove children.
    /// </summary>
  public partial class CreateOnlyParentChangeableChild_Parent: IStorageItemGeneric<CreateOnlyParentChangeableChild_Parent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for CreateOnlyParentChangeableChild_Parent. Gets set once CreateOnlyParentChangeableChild_Parent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem createOnlyParentChangeableChild_Parent, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release CreateOnlyParentChangeableChild_Parent key @{createOnlyParentChangeableChild_Parent.Key} #{createOnlyParentChangeableChild_Parent.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store CreateOnlyParentChangeableChild_Parent key @{key} #{createOnlyParentChangeableChild_Parent.GetHashCode()}");
        }
      }
#endif
      ((CreateOnlyParentChangeableChild_Parent)createOnlyParentChangeableChild_Parent).Key = key;
    }


    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// List of children
    /// </summary>
    public IReadOnlyList<CreateOnlyParentChangeableChild_Child> CreateOnlyParentChangeableChild_Children => createOnlyParentChangeableChild_Children;
    readonly List<CreateOnlyParentChangeableChild_Child> createOnlyParentChangeableChild_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text"};


    /// <summary>
    /// None existing CreateOnlyParentChangeableChild_Parent
    /// </summary>
    internal static CreateOnlyParentChangeableChild_Parent NoCreateOnlyParentChangeableChild_Parent = new CreateOnlyParentChangeableChild_Parent("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action</*old*/CreateOnlyParentChangeableChild_Parent, /*new*/CreateOnlyParentChangeableChild_Parent>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// CreateOnlyParentChangeableChild_Parent Constructor. If isStoring is true, adds CreateOnlyParentChangeableChild_Parent to DC.Data.CreateOnlyParentChangeableChild_Parents.
    /// </summary>
    public CreateOnlyParentChangeableChild_Parent(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      createOnlyParentChangeableChild_Children = new List<CreateOnlyParentChangeableChild_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new CreateOnlyParentChangeableChild_Parent: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data.IsTransaction) {
        DC.Data.AddTransaction(new TransactionItem(31,TransactionActivityEnum.New, Key, this));
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
    public CreateOnlyParentChangeableChild_Parent(CreateOnlyParentChangeableChild_Parent original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(CreateOnlyParentChangeableChild_Parent clone);


    /// <summary>
    /// Constructor for CreateOnlyParentChangeableChild_Parent read from CSV file
    /// </summary>
    private CreateOnlyParentChangeableChild_Parent(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      createOnlyParentChangeableChild_Children = new List<CreateOnlyParentChangeableChild_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New CreateOnlyParentChangeableChild_Parent read from CSV file
    /// </summary>
    internal static CreateOnlyParentChangeableChild_Parent Create(int key, CsvReader csvReader) {
      return new CreateOnlyParentChangeableChild_Parent(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds CreateOnlyParentChangeableChild_Parent to DC.Data.CreateOnlyParentChangeableChild_Parents.<br/>
    /// Throws an Exception when CreateOnlyParentChangeableChild_Parent is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"CreateOnlyParentChangeableChild_Parent cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.CreateOnlyParentChangeableChild_Parents.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored CreateOnlyParentChangeableChild_Parent #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write CreateOnlyParentChangeableChild_Parent to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write CreateOnlyParentChangeableChild_Parent to CSV file
    /// </summary>
    internal static void Write(CreateOnlyParentChangeableChild_Parent createOnlyParentChangeableChild_Parent, CsvWriter csvWriter) {
      createOnlyParentChangeableChild_Parent.onCsvWrite();
      csvWriter.Write(createOnlyParentChangeableChild_Parent.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Add createOnlyParentChangeableChild_Child to CreateOnlyParentChangeableChild_Children.
    /// </summary>
    internal void AddToCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child) {
#if DEBUG
      if (createOnlyParentChangeableChild_Child==CreateOnlyParentChangeableChild_Child.NoCreateOnlyParentChangeableChild_Child) throw new Exception();
      if ((createOnlyParentChangeableChild_Child.Key>=0)&&(Key<0)) throw new Exception();
      if (createOnlyParentChangeableChild_Children.Contains(createOnlyParentChangeableChild_Child)) throw new Exception();
#endif
      createOnlyParentChangeableChild_Children.Add(createOnlyParentChangeableChild_Child);
      onAddedToCreateOnlyParentChangeableChild_Children(createOnlyParentChangeableChild_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add CreateOnlyParentChangeableChild_Child {createOnlyParentChangeableChild_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} CreateOnlyParentChangeableChild_Parent.CreateOnlyParentChangeableChild_Children");
#endif
    }
    partial void onAddedToCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child);


    /// <summary>
    /// Removes createOnlyParentChangeableChild_Child from CreateOnlyParentChangeableChild_Parent.
    /// </summary>
    internal void RemoveFromCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child) {
#if DEBUG
      if (!createOnlyParentChangeableChild_Children.Remove(createOnlyParentChangeableChild_Child)) throw new Exception();
#else
        createOnlyParentChangeableChild_Children.Remove(createOnlyParentChangeableChild_Child);
#endif
      onRemovedFromCreateOnlyParentChangeableChild_Children(createOnlyParentChangeableChild_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove CreateOnlyParentChangeableChild_Child {createOnlyParentChangeableChild_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} CreateOnlyParentChangeableChild_Parent.CreateOnlyParentChangeableChild_Children");
#endif
    }
    partial void onRemovedFromCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child);


    /// <summary>
    /// Releasing CreateOnlyParentChangeableChild_Parent from DC.Data.CreateOnlyParentChangeableChild_Parents is not supported.
    /// </summary>
    public void Release() {
      throw new NotSupportedException("StorageClass attribute AreInstancesDeletable is false.");
    }


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var createOnlyParentChangeableChild_Parent = (CreateOnlyParentChangeableChild_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new CreateOnlyParentChangeableChild_Parent(): {createOnlyParentChangeableChild_Parent.ToTraceString()}");
#endif
      createOnlyParentChangeableChild_Parent.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases CreateOnlyParentChangeableChild_Parent from DC.Data.CreateOnlyParentChangeableChild_Parents as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var createOnlyParentChangeableChild_Parent = (CreateOnlyParentChangeableChild_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback CreateOnlyParentChangeableChild_Parent.Store(): {createOnlyParentChangeableChild_Parent.ToTraceString()}");
#endif
      createOnlyParentChangeableChild_Parent.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the CreateOnlyParentChangeableChild_Parent item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (CreateOnlyParentChangeableChild_Parent) oldStorageItem;
      var newItem = (CreateOnlyParentChangeableChild_Parent) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back CreateOnlyParentChangeableChild_Parent.Update(): {newItem.ToTraceString()}");
#endif
      if (newItem.Text!=oldItem.Text) {
        throw new Exception($"CreateOnlyParentChangeableChild_Parent.Update(): Property Text '{newItem.Text}' is " +
          $"readonly, Text '{oldItem.Text}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back CreateOnlyParentChangeableChild_Parent.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(CreateOnlyParentChangeableChild_Parent oldCreateOnlyParentChangeableChild_Parent);


    /// <summary>
    /// Adds CreateOnlyParentChangeableChild_Parent to DC.Data.CreateOnlyParentChangeableChild_Parents as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var createOnlyParentChangeableChild_Parent = (CreateOnlyParentChangeableChild_Parent) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback CreateOnlyParentChangeableChild_Parent.Release(): {createOnlyParentChangeableChild_Parent.ToTraceString()}");
#endif
      createOnlyParentChangeableChild_Parent.onRollbackItemRelease();
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
        $" CreateOnlyParentChangeableChild_Children: {CreateOnlyParentChangeableChild_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
