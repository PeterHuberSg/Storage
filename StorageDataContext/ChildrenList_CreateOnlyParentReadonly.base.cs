//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenList_CreateOnlyParentReadonly.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of none deletable parent using a List for its children. It can have deletable and none
    /// deletable children. The child must have a parent (the Parent property is not nullable). The relationship 
    /// cannot be updated, since child is readonly.
    /// </summary>
  public partial class ChildrenList_CreateOnlyParentReadonly: IStorageItemGeneric<ChildrenList_CreateOnlyParentReadonly> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenList_CreateOnlyParentReadonly. Gets set once ChildrenList_CreateOnlyParentReadonly gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenList_CreateOnlyParentReadonly, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenList_CreateOnlyParentReadonly key @{childrenList_CreateOnlyParentReadonly.Key} #{childrenList_CreateOnlyParentReadonly.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenList_CreateOnlyParentReadonly key @{key} #{childrenList_CreateOnlyParentReadonly.GetHashCode()}");
        }
      }
#endif
      ((ChildrenList_CreateOnlyParentReadonly)childrenList_CreateOnlyParentReadonly).Key = key;
    }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// These deletable children must have a parent
    /// </summary>
    public IReadOnlyList<ChildrenList_Child> ChildrenList_Children => childrenList_Children;
    readonly List<ChildrenList_Child> childrenList_Children;


    /// <summary>
    /// These none deletable children must have a none deletable parent
    /// </summary>
    public IReadOnlyList<ChildrenList_CreateOnlyChild> ChildrenList_CreateOnlyChildren => childrenList_CreateOnlyChildren;
    readonly List<ChildrenList_CreateOnlyChild> childrenList_CreateOnlyChildren;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text"};


    /// <summary>
    /// None existing ChildrenList_CreateOnlyParentReadonly
    /// </summary>
    internal static ChildrenList_CreateOnlyParentReadonly NoChildrenList_CreateOnlyParentReadonly = new ChildrenList_CreateOnlyParentReadonly("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action</*old*/ChildrenList_CreateOnlyParentReadonly, /*new*/ChildrenList_CreateOnlyParentReadonly>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenList_CreateOnlyParentReadonly Constructor. If isStoring is true, adds ChildrenList_CreateOnlyParentReadonly to DC.Data.ChildrenList_CreateOnlyParentReadonlys.
    /// </summary>
    public ChildrenList_CreateOnlyParentReadonly(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      childrenList_Children = new List<ChildrenList_Child>();
      childrenList_CreateOnlyChildren = new List<ChildrenList_CreateOnlyChild>();
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenList_CreateOnlyParentReadonly: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(17,TransactionActivityEnum.New, Key, this));
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
    public ChildrenList_CreateOnlyParentReadonly(ChildrenList_CreateOnlyParentReadonly original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(ChildrenList_CreateOnlyParentReadonly clone);


    /// <summary>
    /// Constructor for ChildrenList_CreateOnlyParentReadonly read from CSV file
    /// </summary>
    private ChildrenList_CreateOnlyParentReadonly(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      childrenList_Children = new List<ChildrenList_Child>();
      childrenList_CreateOnlyChildren = new List<ChildrenList_CreateOnlyChild>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenList_CreateOnlyParentReadonly read from CSV file
    /// </summary>
    internal static ChildrenList_CreateOnlyParentReadonly Create(int key, CsvReader csvReader) {
      return new ChildrenList_CreateOnlyParentReadonly(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenList_CreateOnlyParentReadonly to DC.Data.ChildrenList_CreateOnlyParentReadonlys.<br/>
    /// Throws an Exception when ChildrenList_CreateOnlyParentReadonly is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenList_CreateOnlyParentReadonly cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenList_CreateOnlyParentReadonlys.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenList_CreateOnlyParentReadonly #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenList_CreateOnlyParentReadonly to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write ChildrenList_CreateOnlyParentReadonly to CSV file
    /// </summary>
    internal static void Write(ChildrenList_CreateOnlyParentReadonly childrenList_CreateOnlyParentReadonly, CsvWriter csvWriter) {
      childrenList_CreateOnlyParentReadonly.onCsvWrite();
      csvWriter.Write(childrenList_CreateOnlyParentReadonly.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Add childrenList_Child to ChildrenList_Children.
    /// </summary>
    internal void AddToChildrenList_Children(ChildrenList_Child childrenList_Child) {
#if DEBUG
      if (childrenList_Child==ChildrenList_Child.NoChildrenList_Child) throw new Exception();
      if ((childrenList_Child.Key>=0)&&(Key<0)) throw new Exception();
      if (childrenList_Children.Contains(childrenList_Child)) throw new Exception();
#endif
      childrenList_Children.Add(childrenList_Child);
      onAddedToChildrenList_Children(childrenList_Child);
#if DEBUG
      DC.Trace?.Invoke($"Add ChildrenList_Child {childrenList_Child.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} ChildrenList_CreateOnlyParentReadonly.ChildrenList_Children");
#endif
    }
    partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes childrenList_Child from ChildrenList_CreateOnlyParentReadonly.
    /// </summary>
    internal void RemoveFromChildrenList_Children(ChildrenList_Child childrenList_Child) {
#if DEBUG
      if (!childrenList_Children.Remove(childrenList_Child)) throw new Exception();
#else
        childrenList_Children.Remove(childrenList_Child);
#endif
      onRemovedFromChildrenList_Children(childrenList_Child);
#if DEBUG
      DC.Trace?.Invoke($"Remove ChildrenList_Child {childrenList_Child.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} ChildrenList_CreateOnlyParentReadonly.ChildrenList_Children");
#endif
    }
    partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Add childrenList_CreateOnlyChild to ChildrenList_CreateOnlyChildren.
    /// </summary>
    internal void AddToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild) {
#if DEBUG
      if (childrenList_CreateOnlyChild==ChildrenList_CreateOnlyChild.NoChildrenList_CreateOnlyChild) throw new Exception();
      if ((childrenList_CreateOnlyChild.Key>=0)&&(Key<0)) throw new Exception();
      if (childrenList_CreateOnlyChildren.Contains(childrenList_CreateOnlyChild)) throw new Exception();
#endif
      childrenList_CreateOnlyChildren.Add(childrenList_CreateOnlyChild);
      onAddedToChildrenList_CreateOnlyChildren(childrenList_CreateOnlyChild);
#if DEBUG
      DC.Trace?.Invoke($"Add ChildrenList_CreateOnlyChild {childrenList_CreateOnlyChild.GetKeyOrHash()} to " +
        $"{this.GetKeyOrHash()} ChildrenList_CreateOnlyParentReadonly.ChildrenList_CreateOnlyChildren");
#endif
    }
    partial void onAddedToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild);


    /// <summary>
    /// Removes childrenList_CreateOnlyChild from ChildrenList_CreateOnlyParentReadonly.
    /// </summary>
    internal void RemoveFromChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild) {
#if DEBUG
      if (!childrenList_CreateOnlyChildren.Remove(childrenList_CreateOnlyChild)) throw new Exception();
#else
        childrenList_CreateOnlyChildren.Remove(childrenList_CreateOnlyChild);
#endif
      onRemovedFromChildrenList_CreateOnlyChildren(childrenList_CreateOnlyChild);
#if DEBUG
      DC.Trace?.Invoke($"Remove ChildrenList_CreateOnlyChild {childrenList_CreateOnlyChild.GetKeyOrHash()} from " +
        $"{this.GetKeyOrHash()} ChildrenList_CreateOnlyParentReadonly.ChildrenList_CreateOnlyChildren");
#endif
    }
    partial void onRemovedFromChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild);


    /// <summary>
    /// Releasing ChildrenList_CreateOnlyParentReadonly from DC.Data.ChildrenList_CreateOnlyParentReadonlys is not supported.
    /// </summary>
    public void Release() {
      throw new NotSupportedException("StorageClass attribute AreInstancesDeletable is false.");
    }


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenList_CreateOnlyParentReadonly = (ChildrenList_CreateOnlyParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenList_CreateOnlyParentReadonly(): {childrenList_CreateOnlyParentReadonly.ToTraceString()}");
#endif
      childrenList_CreateOnlyParentReadonly.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenList_CreateOnlyParentReadonly from DC.Data.ChildrenList_CreateOnlyParentReadonlys as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenList_CreateOnlyParentReadonly = (ChildrenList_CreateOnlyParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_CreateOnlyParentReadonly.Store(): {childrenList_CreateOnlyParentReadonly.ToTraceString()}");
#endif
      childrenList_CreateOnlyParentReadonly.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenList_CreateOnlyParentReadonly item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenList_CreateOnlyParentReadonly) oldStorageItem;
      var newItem = (ChildrenList_CreateOnlyParentReadonly) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenList_CreateOnlyParentReadonly.Update(): {newItem.ToTraceString()}");
#endif
      if (newItem.Text!=oldItem.Text) {
        throw new Exception($"ChildrenList_CreateOnlyParentReadonly.Update(): Property Text '{newItem.Text}' is " +
          $"readonly, Text '{oldItem.Text}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenList_CreateOnlyParentReadonly.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenList_CreateOnlyParentReadonly oldChildrenList_CreateOnlyParentReadonly);


    /// <summary>
    /// Adds ChildrenList_CreateOnlyParentReadonly to DC.Data.ChildrenList_CreateOnlyParentReadonlys as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenList_CreateOnlyParentReadonly = (ChildrenList_CreateOnlyParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_CreateOnlyParentReadonly.Release(): {childrenList_CreateOnlyParentReadonly.ToTraceString()}");
#endif
      childrenList_CreateOnlyParentReadonly.onRollbackItemRelease();
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
        $" ChildrenList_Children: {ChildrenList_Children.Count}," +
        $" ChildrenList_CreateOnlyChildren: {ChildrenList_CreateOnlyChildren.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region ChildrenList_CreateOnlyParentReadonlyRaw
  //      ----------------------------------------

  /// <summary>
  /// ChildrenList_CreateOnlyParentReadonlyRaw is used instead ChildrenList_CreateOnlyParentReadonly and DC.Data to read an instance from a CSV file with 
  /// ChildrenList_CreateOnlyParentReadonlyReader or write with ChildrenList_CreateOnlyParentReadonlyWriter.
  /// </summary>
  public class ChildrenList_CreateOnlyParentReadonlyRaw {

    /// <summary>
    /// Unique identifier for ChildrenList_CreateOnlyParentReadonlyRaw.
    /// </summary>
    public int Key { get; set; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; set; } ="";


    /// <summary>
    /// Default Constructor.
    /// </summary>
    public ChildrenList_CreateOnlyParentReadonlyRaw() {
    }


    /// <summary>
    /// Constructor, will replace links to parents with the parents' key.
    /// </summary>
    public ChildrenList_CreateOnlyParentReadonlyRaw(ChildrenList_CreateOnlyParentReadonly childrenList_CreateOnlyParentReadonly) {
      Key = childrenList_CreateOnlyParentReadonly.Key;
      Text = childrenList_CreateOnlyParentReadonly.Text;
    }


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      var returnString =
        $"Key: {Key}," +
        $" Text: {Text};";
      return returnString;
    }
  }
  #endregion


  #region ChildrenList_CreateOnlyParentReadonlyReader
  //      -------------------------------------------

  /// <summary>
  /// Reads from a CSV file containing ChildrenList_CreateOnlyParentReadonly instances. Note that the keys of linked objects will be returned 
  /// and not the linked objects themselves, since the data context will not be involved.
  /// </summary>
  public class ChildrenList_CreateOnlyParentReadonlyReader: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvReader csvReader;
    int nextKey = 0;


    /// <summary>
    /// Constructor, will read and verify the ChildrenList_CreateOnlyParentReadonly header line. You need to dispose ChildrenList_CreateOnlyParentReadonlyReader once
    /// you are done, except when you have read the whole file, then ChildrenList_CreateOnlyParentReadonlyReader.ReadLine() disposes automatically.
    /// </summary>
    public ChildrenList_CreateOnlyParentReadonlyReader(string fileNamePath, CsvConfig csvConfig) {
      this.csvConfig = csvConfig;
      csvReader = new CsvReader(fileNamePath, csvConfig, ChildrenList_CreateOnlyParentReadonly.EstimatedLineLength);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_CreateOnlyParentReadonly.Headers, csvConfig.Delimiter);
      var headerLine = csvReader.ReadLine();
      if (csvHeaderString!=headerLine) throw new Exception($"Error reading file {csvReader.FileName}{Environment.NewLine}'" +
        headerLine + "' should be '" + csvHeaderString + "'.");
    }


    /// <summary>
    /// Reads the details of one ChildrenList_CreateOnlyParentReadonly from the CSV file. Returns false when all lines are
    /// read and disposes the reader.
    /// </summary>
    public bool ReadLine([NotNullWhen(true)] out ChildrenList_CreateOnlyParentReadonlyRaw? childrenList_CreateOnlyParentReadonlyRaw){
      if (csvReader.IsEndOfFileReached()) {
        csvReader.Dispose();
        childrenList_CreateOnlyParentReadonlyRaw = null;
        return false;
      }
      childrenList_CreateOnlyParentReadonlyRaw = new ChildrenList_CreateOnlyParentReadonlyRaw();
      childrenList_CreateOnlyParentReadonlyRaw.Key = nextKey++;
      childrenList_CreateOnlyParentReadonlyRaw.Text = csvReader.ReadString();
      csvReader.ReadEndOfLine();
      return true;
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_CreateOnlyParentReadonlyReader exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_CreateOnlyParentReadonlyReader already exposed ?
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

      csvReader.Dispose();
    }
    #endregion
  }
  #endregion


  #region ChildrenList_CreateOnlyParentReadonlyWriter
  //      -------------------------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as ChildrenList_CreateOnlyParentReadonly. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class ChildrenList_CreateOnlyParentReadonlyWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int nextKey = 0;


    /// <summary>
    /// Constructor, will write the ChildrenList_CreateOnlyParentReadonly header line into the CSV file. Dispose ChildrenList_CreateOnlyParentReadonlyWriter once done.
    /// </summary>
    public ChildrenList_CreateOnlyParentReadonlyWriter(string fileNamePath, CsvConfig csvConfig){
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, ChildrenList_CreateOnlyParentReadonly.EstimatedLineLength, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_CreateOnlyParentReadonly.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one ChildrenList_CreateOnlyParentReadonlyRaw to the CSV file
    /// </summary>
    public void Write(ChildrenList_CreateOnlyParentReadonlyRaw childrenList_CreateOnlyParentReadonlyRaw){
      if (childrenList_CreateOnlyParentReadonlyRaw.Key!=nextKey) {
        throw new Exception($"ChildrenList_CreateOnlyParentReadonlyRaw's key {childrenList_CreateOnlyParentReadonlyRaw.Key} should be {nextKey}.");
      }
      nextKey++;
      csvWriter.StartNewLine();
      csvWriter.Write(childrenList_CreateOnlyParentReadonlyRaw.Text);
      csvWriter.WriteEndOfLine();
    }


    /// <summary>
    /// Writes the details of one ChildrenList_CreateOnlyParentReadonly to the CSV file
    /// </summary>
    public void Write(int key, string text) {
      if (key!=nextKey) {
        throw new Exception($"ChildrenList_CreateOnlyParentReadonly's key {key} should be {nextKey}.");
      }
      nextKey++;
      csvWriter.StartNewLine();
      csvWriter.Write(text);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_CreateOnlyParentReadonlyWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_CreateOnlyParentReadonlyWriter already exposed ?
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

      csvWriter.Dispose();
    }
    #endregion
  }
  #endregion
}
