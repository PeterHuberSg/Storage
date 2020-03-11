//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into MinimalRef.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


  public partial class MinimalRef: IStorage<MinimalRef> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for MinimalRef. Gets set once MinimalRef gets added to DL.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(MinimalRef minimalRef, int key) { minimalRef.Key = key; }


    public int Number { get; private set; }


    public Minimal MinimalLookup { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Number", "MinimalLookup"};


    /// <summary>
    /// None existing MinimalRef
    /// </summary>
    internal static MinimalRef NoMinimalRef = new MinimalRef(int.MinValue, Minimal.NoMinimal, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of MinimalRef has changed. Gets only raised for changes occurring after loading DL.Data with previously stored data.
    /// </summary>
    public event Action<MinimalRef>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// MinimalRef Constructor. If isStoring is true, adds MinimalRef to DL.Data.MinimalRefs
    /// and adds MinimalRef to minimal.MinimalRefs.
    /// </summary>
    public MinimalRef(int number, Minimal minimalLookup, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Number = number;
      MinimalLookup = minimalLookup;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for MinimalRef read from CSV file
    /// </summary>
    private MinimalRef(int key, CsvReader csvReader, DL context) {
      Key = key;
      Number = csvReader.ReadInt();
      if (context.Minimals.TryGetValue(csvReader.ReadInt(), out var minimalLookup)) {
        MinimalLookup = minimalLookup;
      } else {
        MinimalLookup = Minimal.NoMinimal;
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New MinimalRef read from CSV file
    /// </summary>
    internal static MinimalRef Create(int key, CsvReader csvReader, DL context) {
      return new MinimalRef(key, csvReader, context);
    }


    /// <summary>
    /// Verify that minimalRef.MinimalLookup exists.
    /// </summary>
    internal static bool Verify(MinimalRef minimalRef) {
      if (minimalRef.MinimalLookup==Minimal.NoMinimal) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds MinimalRef to DL.Data.MinimalRefs. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"MinimalRef 'Class MinimalRef' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      onStore();
      DL.Data.MinimalRefs.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write MinimalRef to CSV file
    /// </summary>
    internal const int MaxLineLength = 11;


    /// <summary>
    /// Write MinimalRef to CSV file
    /// </summary>
    internal static void Write(MinimalRef minimalRef, CsvWriter csvWriter) {
      minimalRef.onCsvWrite();
      csvWriter.Write(minimalRef.Number);
      if (minimalRef.MinimalLookup.Key<0) throw new Exception($"Cannot write minimalRef '{minimalRef}' to CSV File, because MinimalLookup is not stored in DL.Data.Minimals.");

      csvWriter.Write(minimalRef.MinimalLookup.Key.ToString());
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removing MinimalRef from DL.Data.MinimalRefs is not supported.
    /// </summary>
    public void Remove() {
      throw new NotSupportedException();
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {Number}," +
        $" {MinimalLookup.ToShortString()}";
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
        $" Number: {Number}," +
        $" MinimalLookup: {MinimalLookup.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
