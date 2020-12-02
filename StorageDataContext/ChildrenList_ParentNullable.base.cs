//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenList_ParentNullable.cs.
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
    /// Example of deletable parent using a List for its children. It can have only deletable children. The child might have a 
    /// parent (the child.Parent property is nullable).
    /// </summary>
  public partial class ChildrenList_ParentNullable: IStorageItemGeneric<ChildrenList_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenList_ParentNullable. Gets set once ChildrenList_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenList_ParentNullable, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenList_ParentNullable key @{childrenList_ParentNullable.Key} #{childrenList_ParentNullable.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenList_ParentNullable key @{key} #{childrenList_ParentNullable.GetHashCode()}");
        }
      }
#endif
      ((ChildrenList_ParentNullable)childrenList_ParentNullable).Key = key;
    }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Deletable children which might or might not have a parent
    /// </summary>
    public IReadOnlyList<ChildrenList_Child> ChildrenList_Children => childrenList_Children;
    readonly List<ChildrenList_Child> childrenList_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ChildrenList_ParentNullable
    /// </summary>
    internal static ChildrenList_ParentNullable NoChildrenList_ParentNullable = new ChildrenList_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenList_ParentNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenList_ParentNullable, /*new*/ChildrenList_ParentNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenList_ParentNullable Constructor. If isStoring is true, adds ChildrenList_ParentNullable to DC.Data.ChildrenList_ParentNullables.
    /// </summary>
    public ChildrenList_ParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      childrenList_Children = new List<ChildrenList_Child>();
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenList_ParentNullable: {ToTraceString()}");
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
    public ChildrenList_ParentNullable(ChildrenList_ParentNullable original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      onCloned(this);
    }
    partial void onCloned(ChildrenList_ParentNullable clone);


    /// <summary>
    /// Constructor for ChildrenList_ParentNullable read from CSV file
    /// </summary>
    private ChildrenList_ParentNullable(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      childrenList_Children = new List<ChildrenList_Child>();
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenList_ParentNullable read from CSV file
    /// </summary>
    internal static ChildrenList_ParentNullable Create(int key, CsvReader csvReader) {
      return new ChildrenList_ParentNullable(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenList_ParentNullable to DC.Data.ChildrenList_ParentNullables.<br/>
    /// Throws an Exception when ChildrenList_ParentNullable is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenList_ParentNullable cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenList_ParentNullables.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenList_ParentNullable #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenList_ParentNullable to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write ChildrenList_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ChildrenList_ParentNullable childrenList_ParentNullable, CsvWriter csvWriter) {
      childrenList_ParentNullable.onCsvWrite();
      csvWriter.Write(childrenList_ParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenList_ParentNullable with the provided values
    /// </summary>
    public void Update(string text) {
      var clone = new ChildrenList_ParentNullable(this);
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ChildrenList_ParentNullable: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenList_ParentNullables.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(13, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ChildrenList_ParentNullable: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated(ChildrenList_ParentNullable old);


    /// <summary>
    /// Updates this ChildrenList_ParentNullable with values from CSV file
    /// </summary>
    internal static void Update(ChildrenList_ParentNullable childrenList_ParentNullable, CsvReader csvReader){
      childrenList_ParentNullable.Text = csvReader.ReadString();
      childrenList_ParentNullable.onCsvUpdate();
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
        $"{this.GetKeyOrHash()} ChildrenList_ParentNullable.ChildrenList_Children");
#endif
    }
    partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes childrenList_Child from ChildrenList_ParentNullable.
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
        $"{this.GetKeyOrHash()} ChildrenList_ParentNullable.ChildrenList_Children");
#endif
    }
    partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes ChildrenList_ParentNullable from DC.Data.ChildrenList_ParentNullables.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ChildrenList_ParentNullable.Release(): ChildrenList_ParentNullable '{this}' is not stored in DC.Data, key is {Key}.");
      }
      foreach (var childrenList_Child in ChildrenList_Children) {
        if (childrenList_Child?.Key>=0) {
          throw new Exception($"Cannot release ChildrenList_ParentNullable '{this}' " + Environment.NewLine + 
            $"because '{childrenList_Child}' in ChildrenList_ParentNullable.ChildrenList_Children is still stored.");
        }
      }
      onReleased();
      DC.Data.ChildrenList_ParentNullables.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ChildrenList_ParentNullable @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenList_ParentNullable = (ChildrenList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenList_ParentNullable(): {childrenList_ParentNullable.ToTraceString()}");
#endif
      childrenList_ParentNullable.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenList_ParentNullable from DC.Data.ChildrenList_ParentNullables as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenList_ParentNullable = (ChildrenList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_ParentNullable.Store(): {childrenList_ParentNullable.ToTraceString()}");
#endif
      childrenList_ParentNullable.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenList_ParentNullable item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenList_ParentNullable) oldStorageItem;
      var newItem = (ChildrenList_ParentNullable) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenList_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenList_ParentNullable.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenList_ParentNullable oldChildrenList_ParentNullable);


    /// <summary>
    /// Adds ChildrenList_ParentNullable to DC.Data.ChildrenList_ParentNullables as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenList_ParentNullable = (ChildrenList_ParentNullable) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenList_ParentNullable.Release(): {childrenList_ParentNullable.ToTraceString()}");
#endif
      childrenList_ParentNullable.onRollbackItemRelease();
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


  #region ChildrenList_ParentNullableRaw
  //      ------------------------------

  /// <summary>
  /// ChildrenList_ParentNullableRaw is used instead ChildrenList_ParentNullable and DC.Data to read an instance from a CSV file with 
  /// ChildrenList_ParentNullableReader or write with ChildrenList_ParentNullableWriter.
  /// </summary>
  public class ChildrenList_ParentNullableRaw {

    /// <summary>
    /// Unique identifier for ChildrenList_ParentNullableRaw.
    /// </summary>
    public int Key { get; set; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; set; } ="";


    /// <summary>
    /// How was ChildrenList_ParentNullableRaw marked in CSV file (read, update, delete) ? If not read from CSV file, RawState is new.
    /// </summary>
    public RawStateEnum RawState { get; set; }


    /// <summary>
    /// Default Constructor.
    /// </summary>
    public ChildrenList_ParentNullableRaw() {
    }


    /// <summary>
    /// Constructor, will replace links to parents with the parents' key.
    /// </summary>
    public ChildrenList_ParentNullableRaw(ChildrenList_ParentNullable childrenList_ParentNullable) {
      Key = childrenList_ParentNullable.Key;
      Text = childrenList_ParentNullable.Text;
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


  #region ChildrenList_ParentNullableReader
  //      ---------------------------------

  /// <summary>
  /// Reads from a CSV file containing ChildrenList_ParentNullable instances. Note that the keys of linked objects will be returned 
  /// and not the linked objects themselves, since the data context will not be involved.
  /// </summary>
  public class ChildrenList_ParentNullableReader: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvReader csvReader;


    /// <summary>
    /// Constructor, will read and verify the ChildrenList_ParentNullable header line. You need to dispose ChildrenList_ParentNullableReader once
    /// you are done, except when you have read the whole file, then ChildrenList_ParentNullableReader.ReadLine() disposes automatically.
    /// </summary>
    public ChildrenList_ParentNullableReader(string fileNamePath, CsvConfig csvConfig) {
      this.csvConfig = csvConfig;
      csvReader = new CsvReader(fileNamePath, csvConfig, ChildrenList_ParentNullable.EstimatedLineLength);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_ParentNullable.Headers, csvConfig.Delimiter);
      var headerLine = csvReader.ReadLine();
      if (csvHeaderString!=headerLine) throw new Exception($"Error reading file {csvReader.FileName}{Environment.NewLine}'" +
        headerLine + "' should be '" + csvHeaderString + "'.");
    }


    /// <summary>
    /// Reads the details of one ChildrenList_ParentNullable from the CSV file. Returns false when all lines are
    /// read and disposes the reader.
    /// </summary>
    public bool ReadLine([NotNullWhen(true)] out ChildrenList_ParentNullableRaw? childrenList_ParentNullableRaw){
      if (csvReader.IsEndOfFileReached()) {
        csvReader.Dispose();
        childrenList_ParentNullableRaw = null;
        return false;
      }
      childrenList_ParentNullableRaw = new ChildrenList_ParentNullableRaw();
      var firstLineChar = csvReader.ReadFirstLineChar();
      if (firstLineChar==csvConfig.LineCharAdd) {
        childrenList_ParentNullableRaw.RawState = RawStateEnum.Read;
      } else if (firstLineChar==csvConfig.LineCharUpdate) {
        childrenList_ParentNullableRaw.RawState = RawStateEnum.Updated;
      } else if (firstLineChar==csvConfig.LineCharDelete) {
        childrenList_ParentNullableRaw.RawState = RawStateEnum.Deleted;
      } else {
        throw new NotSupportedException($"Illegal first line character '{firstLineChar}' found in '{csvReader.GetPresentContent()}'.");
      }
      childrenList_ParentNullableRaw.Key = csvReader.ReadInt();
      childrenList_ParentNullableRaw.Text = csvReader.ReadString();
      csvReader.ReadEndOfLine();
      return true;
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_ParentNullableReader exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_ParentNullableReader already exposed ?
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


  #region ChildrenList_ParentNullableWriter
  //      ---------------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as ChildrenList_ParentNullable. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class ChildrenList_ParentNullableWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int lastKey = int.MinValue;


    /// <summary>
    /// Constructor, will write the ChildrenList_ParentNullable header line into the CSV file. Dispose ChildrenList_ParentNullableWriter once done.
    /// </summary>
    public ChildrenList_ParentNullableWriter(string fileNamePath, CsvConfig csvConfig){
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, ChildrenList_ParentNullable.EstimatedLineLength, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_ParentNullable.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one ChildrenList_ParentNullableRaw to the CSV file
    /// </summary>
    public void Write(ChildrenList_ParentNullableRaw childrenList_ParentNullableRaw){
      if (childrenList_ParentNullableRaw.Key<0) {
        throw new Exception($"ChildrenList_ParentNullableRaw's key {childrenList_ParentNullableRaw.Key} needs to be greater equal 0.");
      }
      if (childrenList_ParentNullableRaw.Key<=lastKey) {
        throw new Exception($"ChildrenList_ParentNullableRaw's key {childrenList_ParentNullableRaw.Key} must be greater than the last written ChildrenList_ParentNullable's key {lastKey}.");
      }
      lastKey = childrenList_ParentNullableRaw.Key;
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(childrenList_ParentNullableRaw.Key);
      csvWriter.Write(childrenList_ParentNullableRaw.Text);
      csvWriter.WriteEndOfLine();
    }


    /// <summary>
    /// Writes the details of one ChildrenList_ParentNullable to the CSV file
    /// </summary>
    public void Write(int key, string text) {
      if (key<0) {
        throw new Exception($"ChildrenList_ParentNullable's key {key} needs to be greater equal 0.");
      }
      if (key<=lastKey) {
        throw new Exception($"ChildrenList_ParentNullable's key {key} must be greater than the last written ChildrenList_ParentNullable's key {lastKey}.");
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
    /// Executes disposal of ChildrenList_ParentNullableWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_ParentNullableWriter already exposed ?
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
