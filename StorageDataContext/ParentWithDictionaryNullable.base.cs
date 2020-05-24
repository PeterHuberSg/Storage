//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ParentWithDictionaryNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a parent child relationship using a Dictionary where the child's parent property is nullable.
    /// </summary>
  public partial class ParentWithDictionaryNullable: IStorage<ParentWithDictionaryNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ParentWithDictionaryNullable. Gets set once ParentWithDictionaryNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ParentWithDictionaryNullable parentWithDictionaryNullable, int key) { parentWithDictionaryNullable.Key = key; }


    /// <summary>
    /// Some Text
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    /// use as key
    /// </summary>
    public IReadOnlyDictionary<DateTime, DictionaryChild> DictionaryChildren => dictionaryChildren;
    readonly Dictionary<DateTime, DictionaryChild> dictionaryChildren;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ParentWithDictionaryNullable
    /// </summary>
    internal static ParentWithDictionaryNullable NoParentWithDictionaryNullable = new ParentWithDictionaryNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ParentWithDictionaryNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action<ParentWithDictionaryNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ParentWithDictionaryNullable Constructor. If isStoring is true, adds ParentWithDictionaryNullable to DC.Data.ParentsWithDictionaryNullable.
    /// </summary>
    public ParentWithDictionaryNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      dictionaryChildren = new Dictionary<DateTime, DictionaryChild>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ParentWithDictionaryNullable read from CSV file
    /// </summary>
    private ParentWithDictionaryNullable(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      dictionaryChildren = new Dictionary<DateTime, DictionaryChild>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ParentWithDictionaryNullable read from CSV file
    /// </summary>
    internal static ParentWithDictionaryNullable Create(int key, CsvReader csvReader, DC context) {
      return new ParentWithDictionaryNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ParentWithDictionaryNullable to DC.Data.ParentsWithDictionaryNullable. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ParentWithDictionaryNullable can not be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.ParentsWithDictionaryNullable.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ParentWithDictionaryNullable to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write ParentWithDictionaryNullable to CSV file
    /// </summary>
    internal static void Write(ParentWithDictionaryNullable parentWithDictionaryNullable, CsvWriter csvWriter) {
      parentWithDictionaryNullable.onCsvWrite();
      csvWriter.Write(parentWithDictionaryNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ParentWithDictionaryNullable with the provided values
    /// </summary>
    public void Update(string text) {
      var isCancelled = false;
      onUpdating(text, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdating(string text, ref bool isCancelled);
    partial void onUpdated();


    /// <summary>
    /// Updates this ParentWithDictionaryNullable with values from CSV file
    /// </summary>
    internal static void Update(ParentWithDictionaryNullable parentWithDictionaryNullable, CsvReader csvReader, DC _) {
      parentWithDictionaryNullable.Text = csvReader.ReadString();
      parentWithDictionaryNullable.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add dictionaryChild to DictionaryChildren.
    /// </summary>
    internal void AddToDictionaryChildren(DictionaryChild dictionaryChild) {
      dictionaryChildren.Add(dictionaryChild.DateKey, dictionaryChild);
      onAddedToDictionaryChildren(dictionaryChild);
    }
    partial void onAddedToDictionaryChildren(DictionaryChild dictionaryChild);


    /// <summary>
    /// Removes dictionaryChild from DictionaryChildren.
    /// </summary>
    internal void RemoveFromDictionaryChildren(DictionaryChild dictionaryChild) {
#if DEBUG
      if (!dictionaryChildren.Remove(dictionaryChild.DateKey)) throw new Exception();
#else
        dictionaryChildren.Remove(dictionaryChild.DateKey);
#endif
      onRemovedFromDictionaryChildren(dictionaryChild);
    }
    partial void onRemovedFromDictionaryChildren(DictionaryChild dictionaryChild);


    /// <summary>
    /// Removes ParentWithDictionaryNullable from DC.Data.ParentsWithDictionaryNullable and 
    /// disconnects DictionaryChild.ParentWithDictionaryNullable from DictionaryChildren.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"ParentWithDictionaryNullable.Remove(): ParentWithDictionaryNullable 'Class ParentWithDictionaryNullable' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.ParentsWithDictionaryNullable.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects DictionaryChild.ParentWithDictionaryNullable from DictionaryChildren.
    /// </summary>
    internal static void Disconnect(ParentWithDictionaryNullable parentWithDictionaryNullable) {
      foreach (var dictionaryChild in parentWithDictionaryNullable.DictionaryChildren.Values) {
        dictionaryChild.RemoveParentWithDictionaryNullable(parentWithDictionaryNullable);
      }
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
        $" DictionaryChildren: {DictionaryChildren.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region ParentWithDictionaryNullableWriter
  //      ----------------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as ParentWithDictionaryNullable. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class ParentWithDictionaryNullableWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int lastKey = int.MinValue;


    /// <summary>
    /// Constructor, will write the ParentWithDictionaryNullable header line into the CSV file. Dispose ParentWithDictionaryNullableWriter once done.
    /// </summary>
    public ParentWithDictionaryNullableWriter(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(ParentWithDictionaryNullable.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one ParentWithDictionaryNullable to the CSV file
    /// </summary>
    public void Write(int key, string text) {
      if (key<0) {
        throw new Exception($"ParentWithDictionaryNullable's key {key} needs to be greater equal 0.");
      }
      if (key<=lastKey) {
        throw new Exception($"ParentWithDictionaryNullable's key {key} must be greater than the last written ParentWithDictionaryNullable's key {lastKey}.");
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
    /// Executes disposal of ParentWithDictionaryNullableWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is ParentWithDictionaryNullableWriter already exposed ?
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
