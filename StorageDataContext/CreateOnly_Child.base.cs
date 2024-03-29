//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into CreateOnly_Child.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a "CreateOnly" Child, i.e. the child's properties will not change. If it is added to a parent during its
    /// creation, it cannot be removed from the parent, because the Parent property of the child cannot be changed
    /// either.
    /// </summary>
  public partial class CreateOnly_Child: IStorageItemGeneric<CreateOnly_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for CreateOnly_Child. Gets set once CreateOnly_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem createOnly_Child, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release CreateOnly_Child key @{createOnly_Child.Key} #{createOnly_Child.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store CreateOnly_Child key @{key} #{createOnly_Child.GetHashCode()}");
        }
      }
#endif
      ((CreateOnly_Child)createOnly_Child).Key = key;
    }


    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// CreateOnlyParent will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public CreateOnly_Parent CreateOnlyParent { get; }


    /// <summary>
    /// CreateOnlyParentNullable will be readonly even it is not marked as such, because class is not updatable
    /// </summary>          
    public CreateOnly_ParentNullable? CreateOnlyParentNullable { get; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text", "CreateOnlyParent", "CreateOnlyParentNullable"};


    /// <summary>
    /// None existing CreateOnly_Child
    /// </summary>
    internal static CreateOnly_Child NoCreateOnly_Child = new CreateOnly_Child("NoText", CreateOnly_Parent.NoCreateOnly_Parent, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action</*old*/CreateOnly_Child, /*new*/CreateOnly_Child>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// CreateOnly_Child Constructor. If isStoring is true, adds CreateOnly_Child to DC.Data.CreateOnly_Children.
    /// </summary>
    public CreateOnly_Child(string text, CreateOnly_Parent createOnlyParent, CreateOnly_ParentNullable? createOnlyParentNullable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      CreateOnlyParent = createOnlyParent;
      CreateOnlyParentNullable = createOnlyParentNullable;
#if DEBUG
      DC.Trace?.Invoke($"new CreateOnly_Child: {ToTraceString()}");
#endif
      CreateOnlyParent.AddToCreateOnly_Children(this);
      if (CreateOnlyParentNullable!=null) {
        CreateOnlyParentNullable.AddToCreateOnly_Children(this);
      }
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(30,TransactionActivityEnum.New, Key, this));
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
    public CreateOnly_Child(CreateOnly_Child original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      CreateOnlyParent = original.CreateOnlyParent;
      CreateOnlyParentNullable = original.CreateOnlyParentNullable;
      onCloned(this);
    }
    partial void onCloned(CreateOnly_Child clone);


    /// <summary>
    /// Constructor for CreateOnly_Child read from CSV file
    /// </summary>
    private CreateOnly_Child(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      var createOnly_ParentKey = csvReader.ReadInt();
      if (DC.Data.CreateOnly_Parents.TryGetValue(createOnly_ParentKey, out var createOnlyParent)) {
          CreateOnlyParent = createOnlyParent;
      } else {
        throw new Exception($"Read CreateOnly_Child from CSV file: Cannot find CreateOnlyParent with key {createOnly_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var createOnlyParentNullableKey = csvReader.ReadIntNull();
      if (createOnlyParentNullableKey.HasValue) {
        if (DC.Data.CreateOnly_ParentNullables.TryGetValue(createOnlyParentNullableKey.Value, out var createOnlyParentNullable)) {
          CreateOnlyParentNullable = createOnlyParentNullable;
        } else {
          CreateOnlyParentNullable = CreateOnly_ParentNullable.NoCreateOnly_ParentNullable;
        }
      }
      if (CreateOnlyParent!=CreateOnly_Parent.NoCreateOnly_Parent) {
        CreateOnlyParent.AddToCreateOnly_Children(this);
      }
      if (createOnlyParentNullableKey.HasValue && CreateOnlyParentNullable!=CreateOnly_ParentNullable.NoCreateOnly_ParentNullable) {
        CreateOnlyParentNullable!.AddToCreateOnly_Children(this);
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New CreateOnly_Child read from CSV file
    /// </summary>
    internal static CreateOnly_Child Create(int key, CsvReader csvReader) {
      return new CreateOnly_Child(key, csvReader);
    }


    /// <summary>
    /// Verify that createOnly_Child.CreateOnlyParent exists.
    /// Verify that createOnly_Child.CreateOnlyParentNullable exists.
    /// </summary>
    internal static bool Verify(CreateOnly_Child createOnly_Child) {
      if (createOnly_Child.CreateOnlyParent==CreateOnly_Parent.NoCreateOnly_Parent) return false;
      if (createOnly_Child.CreateOnlyParentNullable==CreateOnly_ParentNullable.NoCreateOnly_ParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds CreateOnly_Child to DC.Data.CreateOnly_Children.<br/>
    /// Throws an Exception when CreateOnly_Child is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"CreateOnly_Child cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      if (CreateOnlyParent.Key<0) {
        throw new Exception($"Cannot store child CreateOnly_Child '{this}'.CreateOnlyParent to CreateOnly_Parent '{CreateOnlyParent}' because parent is not stored yet.");
      }
      if (CreateOnlyParentNullable?.Key<0) {
        throw new Exception($"Cannot store child CreateOnly_Child '{this}'.CreateOnlyParentNullable to CreateOnly_ParentNullable '{CreateOnlyParentNullable}' because parent is not stored yet.");
      }
      DC.Data.CreateOnly_Children.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored CreateOnly_Child #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write CreateOnly_Child to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write CreateOnly_Child to CSV file
    /// </summary>
    internal static void Write(CreateOnly_Child createOnly_Child, CsvWriter csvWriter) {
      createOnly_Child.onCsvWrite();
      csvWriter.Write(createOnly_Child.Text);
      if (createOnly_Child.CreateOnlyParent.Key<0) throw new Exception($"Cannot write createOnly_Child '{createOnly_Child}' to CSV File, because CreateOnlyParent is not stored in DC.Data.CreateOnly_Parents.");

      csvWriter.Write(createOnly_Child.CreateOnlyParent.Key.ToString());
      if (createOnly_Child.CreateOnlyParentNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (createOnly_Child.CreateOnlyParentNullable.Key<0) throw new Exception($"Cannot write createOnly_Child '{createOnly_Child}' to CSV File, because CreateOnlyParentNullable is not stored in DC.Data.CreateOnly_ParentNullables.");

        csvWriter.Write(createOnly_Child.CreateOnlyParentNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Releasing CreateOnly_Child from DC.Data.CreateOnly_Children is not supported.
    /// </summary>
    public void Release() {
      throw new NotSupportedException("StorageClass attribute AreInstancesDeletable is false.");
    }


    /// <summary>
    /// Removes CreateOnly_Child from parents as part of a transaction rollback of the new() statement.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var createOnly_Child = (CreateOnly_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new CreateOnly_Child(): {createOnly_Child.ToTraceString()}");
#endif
      if (createOnly_Child.CreateOnlyParent!=CreateOnly_Parent.NoCreateOnly_Parent) {
        createOnly_Child.CreateOnlyParent.RemoveFromCreateOnly_Children(createOnly_Child);
      }
      if (createOnly_Child.CreateOnlyParentNullable!=null && createOnly_Child.CreateOnlyParentNullable!=CreateOnly_ParentNullable.NoCreateOnly_ParentNullable) {
        createOnly_Child.CreateOnlyParentNullable.RemoveFromCreateOnly_Children(createOnly_Child);
      }
      createOnly_Child.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases CreateOnly_Child from DC.Data.CreateOnly_Children as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var createOnly_Child = (CreateOnly_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback CreateOnly_Child.Store(): {createOnly_Child.ToTraceString()}");
#endif
      createOnly_Child.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the CreateOnly_Child item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (CreateOnly_Child) oldStorageItem;
      var newItem = (CreateOnly_Child) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back CreateOnly_Child.Update(): {newItem.ToTraceString()}");
#endif
      if (newItem.Text!=oldItem.Text) {
        throw new Exception($"CreateOnly_Child.Update(): Property Text '{newItem.Text}' is " +
          $"readonly, Text '{oldItem.Text}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      if (newItem.CreateOnlyParent!=oldItem.CreateOnlyParent) {
        throw new Exception($"CreateOnly_Child.Update(): Property CreateOnlyParent '{newItem.CreateOnlyParent}' is " +
          $"readonly, CreateOnlyParent '{oldItem.CreateOnlyParent}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      if (newItem.CreateOnlyParentNullable!=oldItem.CreateOnlyParentNullable) {
        throw new Exception($"CreateOnly_Child.Update(): Property CreateOnlyParentNullable '{newItem.CreateOnlyParentNullable}' is " +
          $"readonly, CreateOnlyParentNullable '{oldItem.CreateOnlyParentNullable}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back CreateOnly_Child.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(CreateOnly_Child oldCreateOnly_Child);


    /// <summary>
    /// Adds CreateOnly_Child to DC.Data.CreateOnly_Children as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var createOnly_Child = (CreateOnly_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback CreateOnly_Child.Release(): {createOnly_Child.ToTraceString()}");
#endif
      createOnly_Child.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {Text}|" +
        $" CreateOnlyParent {CreateOnlyParent.GetKeyOrHash()}|" +
        $" CreateOnlyParentNullable {CreateOnlyParentNullable?.GetKeyOrHash()}";
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
        $" {Text}," +
        $" {CreateOnlyParent.ToShortString()}," +
        $" {CreateOnlyParentNullable?.ToShortString()}";
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
        $" CreateOnlyParent: {CreateOnlyParent.ToShortString()}," +
        $" CreateOnlyParentNullable: {CreateOnlyParentNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
