//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into LookupParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Parent of children who use lookup, i.e. parent has no children collection,  where the child's 
    /// parent property is nullable.
    /// </summary>
  public partial class LookupParentNullable: IStorage<LookupParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for LookupParentNullable. Gets set once LookupParentNullable gets added to DL.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(LookupParentNullable lookupParentNullable, int key) { lookupParentNullable.Key = key; }


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
    internal static readonly string[] Headers = {"Key", "Date", "SomeValue"};


    /// <summary>
    /// None existing LookupParentNullable
    /// </summary>
    internal static LookupParentNullable NoLookupParentNullable = new LookupParentNullable(DateTime.MinValue.Date, Decimal.MinValue, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<LookupParentNullable>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// LookupParentNullable Constructor. If isStoring is true, adds LookupParentNullable to DL.Data.LookupParentNullables.
    /// </summary>
    public LookupParentNullable(DateTime date, decimal someValue, bool isStoring = true) {
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
    /// Constructor for LookupParentNullable read from CSV file
    /// </summary>
    private LookupParentNullable(int key, CsvReader csvReader, DL context) {
      Key = key;
      Date = csvReader.ReadDate();
      SomeValue = csvReader.ReadDecimal();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DL context);


    /// <summary>
    /// New LookupParentNullable read from CSV file
    /// </summary>
    internal static LookupParentNullable Create(int key, CsvReader csvReader, DL context) {
      return new LookupParentNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds LookupParentNullable to DL.Data.LookupParentNullables. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"LookupParentNullable can not be stored again in DL.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DL.Data.LookupParentNullables.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write LookupParentNullable to CSV file
    /// </summary>
    internal const int MaxLineLength = 23;


    /// <summary>
    /// Write LookupParentNullable to CSV file
    /// </summary>
    internal static void Write(LookupParentNullable lookupParentNullable, CsvWriter csvWriter) {
      lookupParentNullable.onCsvWrite();
      csvWriter.WriteDate(lookupParentNullable.Date);
      csvWriter.WriteDecimal2(lookupParentNullable.SomeValue);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removes LookupParentNullable from DL.Data.LookupParentNullables.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"LookupParentNullable.Remove(): LookupParentNullable 'Class LookupParentNullable' is not stored in DL.Data, key is {Key}.");
      }
      onRemove();
      DL.Data.LookupParentNullables.Remove(Key);
    }
    partial void onRemove();


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
}
