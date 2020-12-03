//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenList_ParentReadonly.cs.
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
    /// Example of deletable parent using a List for its children. It can have only deletable children. The child must have a 
    /// parent (the child.Parent property is not nullable). The relationship cannot be updated, since child is readonly.
    /// </summary>
  public partial class ChildrenList_ParentReadonly: IStorageItemGeneric<ChildrenList_ParentReadonly> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenList_ParentReadonly. Gets set once ChildrenList_ParentReadonly gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenList_ParentReadonly, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenList_ParentReadonly key @{childrenList_ParentReadonly.Key} #{childrenList_ParentReadonly.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenList_ParentReadonly key @{key} #{childrenList_ParentReadonly.GetHashCode()}");
        }
      }
#endif
      ((ChildrenList_ParentReadonly)childrenList_ParentReadonly).Key = key;
    }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Deletable children which must have a parent
    /// </summary>
    public IReadOnlyList<ChildrenList_Child> ChildrenList_Children => childrenList_Children;
    readonly List<ChildrenList_Child> childrenList_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ChildrenList_ParentReadonly
    /// </summary>
    internal static ChildrenList_ParentReadonly NoChildrenList_ParentReadonly = new ChildrenList_ParentReadonly("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenList_ParentReadonly has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenList_ParentReadonly, /*new*/ChildrenList_ParentReadonly>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenList_ParentReadonly Constructor. If isStoring is true, adds ChildrenList_ParentReadonly to DC.Data.ChildrenList_ParentReadonlys.
    /// </summary>
    public ChildrenList_ParentReadonly(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      childrenList_Children = new List<ChildrenList_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenList_ParentReadonly: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data.IsTransaction) {
        DC.Data.AddTransaction(new TransactionItem(13,TransactionActivityEnum.New, Key, this));
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
    public ChildrenList_ParentReadonly(ChildrenList_ParentReadonly original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(ChildrenList_ParentReadonly clone);


    /// <summary>
    /// Constructor for ChildrenList_ParentReadonly read from CSV file
    /// </summary>
    private ChildrenList_ParentReadonly(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      childrenList_Children = new List<ChildrenList_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenList_ParentReadonly read from CSV file
    /// </summary>
    internal static ChildrenList_ParentReadonly Create(int key, CsvReader csvReader) {
      return new ChildrenList_ParentReadonly(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenList_ParentReadonly to DC.Data.ChildrenList_ParentReadonlys.<br/>
    /// Throws an Exception when ChildrenList_ParentReadonly is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenList_ParentReadonly cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenList_ParentReadonlys.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenList_ParentReadonly #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenList_ParentReadonly to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write ChildrenList_ParentReadonly to CSV file
    /// </summary>
    internal static void Write(ChildrenList_ParentReadonly childrenList_ParentReadonly, CsvWriter csvWriter) {
      childrenList_ParentReadonly.onCsvWrite();
      csvWriter.Write(childrenList_ParentReadonly.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenList_ParentReadonly with the provided values
    /// </summary>
    public void Update(string text) {
      var clone = new ChildrenList_ParentReadonly(this);
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ChildrenList_ParentReadonly: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenList_ParentReadonlys.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(13, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ChildrenList_ParentReadonly: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated(ChildrenList_ParentReadonly old);


    /// <summary>
    /// Updates this ChildrenList_ParentReadonly with values from CSV file
    /// </summary>
    internal static void Update(ChildrenList_ParentReadonly childrenList_ParentReadonly, CsvReader csvReader){
      childrenList_ParentReadonly.Text = csvReader.ReadString();
      childrenList_ParentReadonly.onCsvUpdate();
    }
    partial void onCsvUpdate();


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
        $"{this.GetKeyOrHash()} ChildrenList_ParentReadonly.ChildrenList_Children");
#endif
    }
    partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes childrenList_Child from ChildrenList_ParentReadonly.
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
        $"{this.GetKeyOrHash()} ChildrenList_ParentReadonly.ChildrenList_Children");
#endif
    }
    partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes ChildrenList_ParentReadonly from DC.Data.ChildrenList_ParentReadonlys.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ChildrenList_ParentReadonly.Release(): ChildrenList_ParentReadonly '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var childrenList_Child in ChildrenList_Children) {
        if (childrenList_Child?.Key>=0) {
          throw new Exception($"Cannot release ChildrenList_ParentReadonly '{this}' " + Environment.NewLine + 
            $"because '{childrenList_Child}' in ChildrenList_ParentReadonly.ChildrenList_Children is still stored.");
        }
      }
      onReleased();
      DC.Data.ChildrenList_ParentReadonlys.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ChildrenList_ParentReadonly @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenList_ParentReadonly = (ChildrenList_ParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenList_ParentReadonly(): {childrenList_ParentReadonly.ToTraceString()}");
#endif
      childrenList_ParentReadonly.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenList_ParentReadonly from DC.Data.ChildrenList_ParentReadonlys as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenList_ParentReadonly = (ChildrenList_ParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_ParentReadonly.Store(): {childrenList_ParentReadonly.ToTraceString()}");
#endif
      childrenList_ParentReadonly.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenList_ParentReadonly item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenList_ParentReadonly) oldStorageItem;
      var newItem = (ChildrenList_ParentReadonly) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenList_ParentReadonly.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenList_ParentReadonly.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenList_ParentReadonly oldChildrenList_ParentReadonly);


    /// <summary>
    /// Adds ChildrenList_ParentReadonly to DC.Data.ChildrenList_ParentReadonlys as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenList_ParentReadonly = (ChildrenList_ParentReadonly) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_ParentReadonly.Release(): {childrenList_ParentReadonly.ToTraceString()}");
#endif
      childrenList_ParentReadonly.onRollbackItemRelease();
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
        $" ChildrenList_Children: {ChildrenList_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region ChildrenList_ParentReadonlyRaw
  //      ------------------------------

  /// <summary>
  /// ChildrenList_ParentReadonlyRaw is used instead ChildrenList_ParentReadonly and DC.Data to read an instance from a CSV file with 
  /// ChildrenList_ParentReadonlyReader or write with ChildrenList_ParentReadonlyWriter.
  /// </summary>
  public class ChildrenList_ParentReadonlyRaw {

    /// <summary>
    /// Unique identifier for ChildrenList_ParentReadonlyRaw.
    /// </summary>
    public int Key { get; set; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; set; } ="";


    /// <summary>
    /// How was ChildrenList_ParentReadonlyRaw marked in CSV file (read, update, delete) ? If not read from CSV file, RawState is new.
    /// </summary>
    public RawStateEnum RawState { get; set; }


    /// <summary>
    /// Default Constructor.
    /// </summary>
    public ChildrenList_ParentReadonlyRaw() {
    }


    /// <summary>
    /// Constructor, will replace links to parents with the parents' key.
    /// </summary>
    public ChildrenList_ParentReadonlyRaw(ChildrenList_ParentReadonly childrenList_ParentReadonly) {
      Key = childrenList_ParentReadonly.Key;
      Text = childrenList_ParentReadonly.Text;
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


  #region ChildrenList_ParentReadonlyReader
  //      ---------------------------------

  /// <summary>
  /// Reads from a CSV file containing ChildrenList_ParentReadonly instances. Note that the keys of linked objects will be returned 
  /// and not the linked objects themselves, since the data context will not be involved.
  /// </summary>
  public class ChildrenList_ParentReadonlyReader: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvReader csvReader;


    /// <summary>
    /// Constructor, will read and verify the ChildrenList_ParentReadonly header line. You need to dispose ChildrenList_ParentReadonlyReader once
    /// you are done, except when you have read the whole file, then ChildrenList_ParentReadonlyReader.ReadLine() disposes automatically.
    /// </summary>
    public ChildrenList_ParentReadonlyReader(string fileNamePath, CsvConfig csvConfig) {
      this.csvConfig = csvConfig;
      csvReader = new CsvReader(fileNamePath, csvConfig, ChildrenList_ParentReadonly.EstimatedLineLength);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_ParentReadonly.Headers, csvConfig.Delimiter);
      var headerLine = csvReader.ReadLine();
      if (csvHeaderString!=headerLine) throw new Exception($"Error reading file {csvReader.FileName}{Environment.NewLine}'" +
        headerLine + "' should be '" + csvHeaderString + "'.");
    }


    /// <summary>
    /// Reads the details of one ChildrenList_ParentReadonly from the CSV file. Returns false when all lines are
    /// read and disposes the reader.
    /// </summary>
    public bool ReadLine([NotNullWhen(true)] out ChildrenList_ParentReadonlyRaw? childrenList_ParentReadonlyRaw){
      if (csvReader.IsEndOfFileReached()) {
        csvReader.Dispose();
        childrenList_ParentReadonlyRaw = null;
        return false;
      }
      childrenList_ParentReadonlyRaw = new ChildrenList_ParentReadonlyRaw();
      var firstLineChar = csvReader.ReadFirstLineChar();
      if (firstLineChar==csvConfig.LineCharAdd) {
        childrenList_ParentReadonlyRaw.RawState = RawStateEnum.Read;
      } else if (firstLineChar==csvConfig.LineCharUpdate) {
        childrenList_ParentReadonlyRaw.RawState = RawStateEnum.Updated;
      } else if (firstLineChar==csvConfig.LineCharDelete) {
        childrenList_ParentReadonlyRaw.RawState = RawStateEnum.Deleted;
      } else {
        throw new NotSupportedException($"Illegal first line character '{firstLineChar}' found in '{csvReader.GetPresentContent()}'.");
      }
      childrenList_ParentReadonlyRaw.Key = csvReader.ReadInt();
      childrenList_ParentReadonlyRaw.Text = csvReader.ReadString();
      csvReader.ReadEndOfLine();
      return true;
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_ParentReadonlyReader exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_ParentReadonlyReader already exposed ?
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


  #region ChildrenList_ParentReadonlyWriter
  //      ---------------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as ChildrenList_ParentReadonly. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class ChildrenList_ParentReadonlyWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int lastKey = int.MinValue;


    /// <summary>
    /// Constructor, will write the ChildrenList_ParentReadonly header line into the CSV file. Dispose ChildrenList_ParentReadonlyWriter once done.
    /// </summary>
    public ChildrenList_ParentReadonlyWriter(string fileNamePath, CsvConfig csvConfig){
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, ChildrenList_ParentReadonly.EstimatedLineLength, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_ParentReadonly.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one ChildrenList_ParentReadonlyRaw to the CSV file
    /// </summary>
    public void Write(ChildrenList_ParentReadonlyRaw childrenList_ParentReadonlyRaw){
      if (childrenList_ParentReadonlyRaw.Key<0) {
        throw new Exception($"ChildrenList_ParentReadonlyRaw's key {childrenList_ParentReadonlyRaw.Key} needs to be greater equal 0.");
      }
      if (childrenList_ParentReadonlyRaw.Key<=lastKey) {
        throw new Exception($"ChildrenList_ParentReadonlyRaw's key {childrenList_ParentReadonlyRaw.Key} must be greater than the last written ChildrenList_ParentReadonly's key {lastKey}.");
      }
      lastKey = childrenList_ParentReadonlyRaw.Key;
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(childrenList_ParentReadonlyRaw.Key);
      csvWriter.Write(childrenList_ParentReadonlyRaw.Text);
      csvWriter.WriteEndOfLine();
    }


    /// <summary>
    /// Writes the details of one ChildrenList_ParentReadonly to the CSV file
    /// </summary>
    public void Write(int key, string text) {
      if (key<0) {
        throw new Exception($"ChildrenList_ParentReadonly's key {key} needs to be greater equal 0.");
      }
      if (key<=lastKey) {
        throw new Exception($"ChildrenList_ParentReadonly's key {key} must be greater than the last written ChildrenList_ParentReadonly's key {lastKey}.");
      }
      lastKey = key;
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(key);
      csvWriter.Write(text);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_ParentReadonlyWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_ParentReadonlyWriter already exposed ?
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
