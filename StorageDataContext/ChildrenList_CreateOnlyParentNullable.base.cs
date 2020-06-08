//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenList_CreateOnlyParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of none deletable parent using a List for its children. The child can be deletable or none deletable. The 
    /// child might have a parent (the Parent property is nullable).
    /// </summary>
  public partial class ChildrenList_CreateOnlyParentNullable: IStorage<ChildrenList_CreateOnlyParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenList_CreateOnlyParentNullable. Gets set once ChildrenList_CreateOnlyParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ChildrenList_CreateOnlyParentNullable childrenList_CreateOnlyParentNullable, int key) { childrenList_CreateOnlyParentNullable.Key = key; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// These deletable children might or might not have a parent
    /// </summary>
    public IReadOnlyList<ChildrenList_Child> ChildrenList_Children => childrenList_Children;
    readonly List<ChildrenList_Child> childrenList_Children;


    /// <summary>
    /// These none deletable children might or might not have a parent
    /// </summary>
    public IReadOnlyList<ChildrenList_CreateOnlyChild> ChildrenList_CreateOnlyChildren => childrenList_CreateOnlyChildren;
    readonly List<ChildrenList_CreateOnlyChild> childrenList_CreateOnlyChildren;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text"};


    /// <summary>
    /// None existing ChildrenList_CreateOnlyParentNullable
    /// </summary>
    internal static ChildrenList_CreateOnlyParentNullable NoChildrenList_CreateOnlyParentNullable = new ChildrenList_CreateOnlyParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<ChildrenList_CreateOnlyParentNullable>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenList_CreateOnlyParentNullable Constructor. If isStoring is true, adds ChildrenList_CreateOnlyParentNullable to DC.Data.ChildrenList_CreateOnlyParentNullables.
    /// </summary>
    public ChildrenList_CreateOnlyParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      childrenList_Children = new List<ChildrenList_Child>();
      childrenList_CreateOnlyChildren = new List<ChildrenList_CreateOnlyChild>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ChildrenList_CreateOnlyParentNullable read from CSV file
    /// </summary>
    private ChildrenList_CreateOnlyParentNullable(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      childrenList_Children = new List<ChildrenList_Child>();
      childrenList_CreateOnlyChildren = new List<ChildrenList_CreateOnlyChild>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ChildrenList_CreateOnlyParentNullable read from CSV file
    /// </summary>
    internal static ChildrenList_CreateOnlyParentNullable Create(int key, CsvReader csvReader, DC context) {
      return new ChildrenList_CreateOnlyParentNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenList_CreateOnlyParentNullable to DC.Data.ChildrenList_CreateOnlyParentNullables. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenList_CreateOnlyParentNullable cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.ChildrenList_CreateOnlyParentNullables.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ChildrenList_CreateOnlyParentNullable to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write ChildrenList_CreateOnlyParentNullable to CSV file
    /// </summary>
    internal static void Write(ChildrenList_CreateOnlyParentNullable childrenList_CreateOnlyParentNullable, CsvWriter csvWriter) {
      childrenList_CreateOnlyParentNullable.onCsvWrite();
      csvWriter.Write(childrenList_CreateOnlyParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Add childrenList_Child to ChildrenList_Children.
    /// </summary>
    internal void AddToChildrenList_Children(ChildrenList_Child childrenList_Child) {
      childrenList_Children.Add(childrenList_Child);
      onAddedToChildrenList_Children(childrenList_Child);
    }
    partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Removes childrenList_Child from ChildrenList_CreateOnlyParentNullable.
    /// </summary>
    internal void RemoveFromChildrenList_Children(ChildrenList_Child childrenList_Child) {
#if DEBUG
      if (!childrenList_Children.Remove(childrenList_Child)) throw new Exception();
#else
        childrenList_Children.Remove(childrenList_Child);
#endif
      onRemovedFromChildrenList_Children(childrenList_Child);
    }
    partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child);


    /// <summary>
    /// Add childrenList_CreateOnlyChild to ChildrenList_CreateOnlyChildren.
    /// </summary>
    internal void AddToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild) {
      childrenList_CreateOnlyChildren.Add(childrenList_CreateOnlyChild);
      onAddedToChildrenList_CreateOnlyChildren(childrenList_CreateOnlyChild);
    }
    partial void onAddedToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild);


    /// <summary>
    /// Removing ChildrenList_CreateOnlyParentNullable from DC.Data.ChildrenList_CreateOnlyParentNullables is not supported.
    /// </summary>
    public void Remove() {
      throw new NotSupportedException("StorageClass attribute AreInstancesDeletable is false.");
    }


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
        $"Key: {Key}," +
        $" Text: {Text}," +
        $" ChildrenList_Children: {ChildrenList_Children.Count}," +
        $" ChildrenList_CreateOnlyChildren: {ChildrenList_CreateOnlyChildren.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region ChildrenList_CreateOnlyParentNullableRaw
  //      ----------------------------------------

  /// <summary>
  /// ChildrenList_CreateOnlyParentNullableRaw is used instead ChildrenList_CreateOnlyParentNullable and DC.Data to read an instance from a CSV file with 
  /// ChildrenList_CreateOnlyParentNullableReader or write with ChildrenList_CreateOnlyParentNullableWriter.
  /// </summary>
  public class ChildrenList_CreateOnlyParentNullableRaw {

    /// <summary>
    /// Unique identifier for ChildrenList_CreateOnlyParentNullableRaw.
    /// </summary>
    public int Key { get; set; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; set; } ="";


    /// <summary>
    /// Default Constructor.
    /// </summary>
    public ChildrenList_CreateOnlyParentNullableRaw() {
    }


    /// <summary>
    /// Constructor, will replace links to parents with the parents' key.
    /// </summary>
    public ChildrenList_CreateOnlyParentNullableRaw(ChildrenList_CreateOnlyParentNullable childrenList_CreateOnlyParentNullable) {
      Key = childrenList_CreateOnlyParentNullable.Key;
      Text = childrenList_CreateOnlyParentNullable.Text;
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


  #region ChildrenList_CreateOnlyParentNullableReader
  //      -------------------------------------------

  /// <summary>
  /// Reads from a CSV file containing ChildrenList_CreateOnlyParentNullable instances. Note that the keys of linked objects will be returned 
  /// and not the linked objects themselves, since the data context will not be involved.
  /// </summary>
  public class ChildrenList_CreateOnlyParentNullableReader: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvReader csvReader;
    int nextKey = 0;


    /// <summary>
    /// Constructor, will read and verify the ChildrenList_CreateOnlyParentNullable header line. You need to dispose ChildrenList_CreateOnlyParentNullableReader once
    /// you are done, except when you have read the whole file, then ChildrenList_CreateOnlyParentNullableReader.ReadLine() disposes automatically.
    /// </summary>
    public ChildrenList_CreateOnlyParentNullableReader(string fileNamePath, CsvConfig csvConfig) {
      this.csvConfig = csvConfig;
      csvReader = new CsvReader(fileNamePath, csvConfig, ChildrenList_CreateOnlyParentNullable.MaxLineLength);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_CreateOnlyParentNullable.Headers, csvConfig.Delimiter);
      var headerLine = csvReader.ReadLine();
      if (csvHeaderString!=headerLine) throw new Exception($"Error reading file {csvReader.FileName}{Environment.NewLine}'" +
        headerLine + "' should be " + csvHeaderString + ".");
    }


    /// <summary>
    /// Reads the details of one ChildrenList_CreateOnlyParentNullable from the CSV file. Returns false when all lines are
    /// read and disposes the reader.
    /// </summary>
    public bool ReadLine([NotNullWhen(true)] out ChildrenList_CreateOnlyParentNullableRaw? childrenList_CreateOnlyParentNullableRaw){
      if (csvReader.IsEndOfFileReached()) {
        csvReader.Dispose();
        childrenList_CreateOnlyParentNullableRaw = null;
        return false;
      }
      childrenList_CreateOnlyParentNullableRaw = new ChildrenList_CreateOnlyParentNullableRaw();
      childrenList_CreateOnlyParentNullableRaw.Key = nextKey++;
      childrenList_CreateOnlyParentNullableRaw.Text = csvReader.ReadString();
      csvReader.ReadEndOfLine();
      return true;
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_CreateOnlyParentNullableReader exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_CreateOnlyParentNullableReader already exposed ?
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


  #region ChildrenList_CreateOnlyParentNullableWriter
  //      -------------------------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as ChildrenList_CreateOnlyParentNullable. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class ChildrenList_CreateOnlyParentNullableWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int nextKey = 0;


    /// <summary>
    /// Constructor, will write the ChildrenList_CreateOnlyParentNullable header line into the CSV file. Dispose ChildrenList_CreateOnlyParentNullableWriter once done.
    /// </summary>
    public ChildrenList_CreateOnlyParentNullableWriter(string fileNamePath, CsvConfig csvConfig){
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, ChildrenList_CreateOnlyParentNullable.MaxLineLength, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(ChildrenList_CreateOnlyParentNullable.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one ChildrenList_CreateOnlyParentNullableRaw to the CSV file
    /// </summary>
    public void Write(ChildrenList_CreateOnlyParentNullableRaw childrenList_CreateOnlyParentNullableRaw){
      if (childrenList_CreateOnlyParentNullableRaw.Key!=nextKey) {
        throw new Exception($"ChildrenList_CreateOnlyParentNullableRaw's key {childrenList_CreateOnlyParentNullableRaw.Key} should be {nextKey}.");
      }
      nextKey++;
      csvWriter.StartNewLine();
      csvWriter.Write(childrenList_CreateOnlyParentNullableRaw.Text);
      csvWriter.WriteEndOfLine();
    }


    /// <summary>
    /// Writes the details of one ChildrenList_CreateOnlyParentNullable to the CSV file
    /// </summary>
    public void Write(int key, string text) {
      if (key!=nextKey) {
        throw new Exception($"ChildrenList_CreateOnlyParentNullable's key {key} should be {nextKey}.");
      }
      nextKey++;
      csvWriter.StartNewLine();
      csvWriter.Write(text);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of ChildrenList_CreateOnlyParentNullableWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ChildrenList_CreateOnlyParentNullableWriter already exposed ?
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
