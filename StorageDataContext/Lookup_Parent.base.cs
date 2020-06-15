//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into Lookup_Parent.cs.
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
  public partial class Lookup_Parent: IStorage<Lookup_Parent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for Lookup_Parent. Gets set once Lookup_Parent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(Lookup_Parent lookup_Parent, int key) { lookup_Parent.Key = key; }


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
    /// None existing Lookup_Parent
    /// </summary>
    internal static Lookup_Parent NoLookup_Parent = new Lookup_Parent(DateTime.MinValue.Date, Decimal.MinValue, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<Lookup_Parent>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Lookup_Parent Constructor. If isStoring is true, adds Lookup_Parent to DC.Data.Lookup_Parents.
    /// </summary>
    public Lookup_Parent(DateTime date, decimal someValue, bool isStoring = true) {
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
    /// Constructor for Lookup_Parent read from CSV file
    /// </summary>
    private Lookup_Parent(int key, CsvReader csvReader, DC context) {
      Key = key;
      Date = csvReader.ReadDate();
      SomeValue = csvReader.ReadDecimal();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New Lookup_Parent read from CSV file
    /// </summary>
    internal static Lookup_Parent Create(int key, CsvReader csvReader, DC context) {
      return new Lookup_Parent(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds Lookup_Parent to DC.Data.Lookup_Parents. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"Lookup_Parent cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.Lookup_Parents.Add(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write Lookup_Parent to CSV file
    /// </summary>
    public const int MaxLineLength = 23;


    /// <summary>
    /// Write Lookup_Parent to CSV file
    /// </summary>
    internal static void Write(Lookup_Parent lookup_Parent, CsvWriter csvWriter) {
      lookup_Parent.onCsvWrite();
      csvWriter.WriteDate(lookup_Parent.Date);
      csvWriter.WriteDecimal2(lookup_Parent.SomeValue);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removing Lookup_Parent from DC.Data.Lookup_Parents is not supported.
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
}
