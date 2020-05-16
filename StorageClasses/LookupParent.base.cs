//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into LookupParent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Parent of children who use lookup, i.e. parent has no children collection
    /// </summary>
  public partial class LookupParent: IStorage<LookupParent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for LookupParent. Gets set once LookupParent gets added to DL.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(LookupParent lookupParent, int key) { lookupParent.Key = key; }


    /// <summary>
    /// Stores only dates but no times.
    ///  </summary>
    public DateTime Date { get; }


    /// <summary>
    /// Stores decimal with 2 digits after comma.
    ///  </summary>
    public decimal SomeValue { get; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Date", "SomeValue"};


    /// <summary>
    /// None existing LookupParent
    /// </summary>
    internal static LookupParent NoLookupParent = new LookupParent(DateTime.MinValue.Date, Decimal.MinValue, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<LookupParent>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// LookupParent Constructor. If isStoring is true, adds LookupParent to DL.Data.LookupParents.
    /// </summary>
    public LookupParent(DateTime date, decimal someValue, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Date = date.Floor(Rounding.Days);
      SomeValue = someValue.Round(2);
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for LookupParent read from CSV file
    /// </summary>
    private LookupParent(int key, CsvReader csvReader, DL context) {
      Key = key;
      Date = csvReader.ReadDate();
      SomeValue = csvReader.ReadDecimal();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DL context);


    /// <summary>
    /// New LookupParent read from CSV file
    /// </summary>
    internal static LookupParent Create(int key, CsvReader csvReader, DL context) {
      return new LookupParent(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds LookupParent to DL.Data.LookupParents. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"LookupParent can not be stored again in DL.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DL.Data.LookupParents.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write LookupParent to CSV file
    /// </summary>
    public const int MaxLineLength = 23;


    /// <summary>
    /// Write LookupParent to CSV file
    /// </summary>
    internal static void Write(LookupParent lookupParent, CsvWriter csvWriter) {
      lookupParent.onCsvWrite();
      csvWriter.WriteDate(lookupParent.Date);
      csvWriter.WriteDecimal2(lookupParent.SomeValue);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removing LookupParent from DL.Data.LookupParents is not supported.
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
        $" {Date.ToShortDateString()}," +
        $" {SomeValue}";
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
        $" Date: {Date.ToShortDateString()}," +
        $" SomeValue: {SomeValue};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region LookupParentWriter
  //      ------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as LookupParent. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class LookupParentWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;


    /// <summary>
    /// Constructor, will write the LookupParent header line into the CSV file. Dispose LookupParentWriter once done.
    /// </summary>
    public LookupParentWriter(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(LookupParent.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one LookupParent to the CSV file
    /// </summary>
    public void Write(DateTime date, decimal someValue) {
      csvWriter.StartNewLine();
      csvWriter.WriteDate(date);
      csvWriter.WriteDecimal2(someValue);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of LookupParentWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is LookupParentWriter already exposed ?
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
