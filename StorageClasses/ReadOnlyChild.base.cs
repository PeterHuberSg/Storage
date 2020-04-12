//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ReadOnlyChild.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a "readonly" Child, i.e. the child's properties will not change and once it is added to its parent
    /// and therefore it also cannot be removed from parent, because the Parent property of the child cannot be changed
    /// either.
    /// </summary>
  public partial class ReadOnlyChild: IStorage<ReadOnlyChild> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ReadOnlyChild. Gets set once ReadOnlyChild gets added to DL.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ReadOnlyChild readOnlyChild, int key) { readOnlyChild.Key = key; }


    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// Parent
    /// </summary>
    public ReadOnlyParent ReadOnlyParent { get; }


    /// <summary>
    /// Parent
    /// </summary>          
    public ReadOnlyParentNullable? ReadOnlyParentNullable { get; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text", "ReadOnlyParent", "ReadOnlyParentNullable"};


    /// <summary>
    /// None existing ReadOnlyChild
    /// </summary>
    internal static ReadOnlyChild NoReadOnlyChild = new ReadOnlyChild("NoText", ReadOnlyParent.NoReadOnlyParent, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<ReadOnlyChild>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ReadOnlyChild Constructor. If isStoring is true, adds ReadOnlyChild to DL.Data.ReadOnlyChildren, 
    /// adds ReadOnlyChild to readOnlyParent.ReadOnlyChildren
    /// and if there is a ReadOnlyParentNullable adds ReadOnlyChild to readOnlyParentNullable.ReadOnlyChildren.
    /// </summary>
    public ReadOnlyChild(string text, ReadOnlyParent readOnlyParent, ReadOnlyParentNullable? readOnlyParentNullable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      ReadOnlyParent = readOnlyParent;
      ReadOnlyParentNullable = readOnlyParentNullable;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ReadOnlyChild read from CSV file
    /// </summary>
    private ReadOnlyChild(int key, CsvReader csvReader, DL context) {
      Key = key;
      Text = csvReader.ReadString();
      var readOnlyParentKey = csvReader.ReadInt();
      if (context.ReadOnlyParents.TryGetValue(readOnlyParentKey, out var readOnlyParent)) {
          ReadOnlyParent = readOnlyParent;
      } else {
        throw new Exception($"Read ReadOnlyChild from CSV file: Cannot find ReadOnlyParent with key {readOnlyParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var readOnlyParentNullableKey = csvReader.ReadIntNull();
      if (readOnlyParentNullableKey.HasValue) {
        if (context.ReadOnlyParentNullables.TryGetValue(readOnlyParentNullableKey.Value, out var readOnlyParentNullable)) {
          ReadOnlyParentNullable = readOnlyParentNullable;
        } else {
          ReadOnlyParentNullable = ReadOnlyParentNullable.NoReadOnlyParentNullable;
        }
      }
      if (ReadOnlyParent!=ReadOnlyParent.NoReadOnlyParent) {
        ReadOnlyParent.AddToReadOnlyChildren(this);
      }
      if (readOnlyParentNullableKey.HasValue && ReadOnlyParentNullable!=ReadOnlyParentNullable.NoReadOnlyParentNullable) {
        ReadOnlyParentNullable!.AddToReadOnlyChildren(this);
      }
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DL context);


    /// <summary>
    /// New ReadOnlyChild read from CSV file
    /// </summary>
    internal static ReadOnlyChild Create(int key, CsvReader csvReader, DL context) {
      return new ReadOnlyChild(key, csvReader, context);
    }


    /// <summary>
    /// Verify that readOnlyChild.ReadOnlyParent exists.
    /// Verify that readOnlyChild.ReadOnlyParentNullable exists.
    /// </summary>
    internal static bool Verify(ReadOnlyChild readOnlyChild) {
      if (readOnlyChild.ReadOnlyParent==ReadOnlyParent.NoReadOnlyParent) return false;
      if (readOnlyChild.ReadOnlyParentNullable==ReadOnlyParentNullable.NoReadOnlyParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ReadOnlyChild to DL.Data.ReadOnlyChildren, ReadOnlyParent.ReadOnlyChildren and ReadOnlyParentNullable.ReadOnlyChildren. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ReadOnlyChild can not be stored in DL.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      if (ReadOnlyParent.Key<0) {
        throw new Exception($"ReadOnlyChild can not be stored in DL.Data, ReadOnlyParent is missing." + Environment.NewLine + ToString());
      }
      onStore();
      DL.Data.ReadOnlyChildren.Add(this);
      ReadOnlyParent.AddToReadOnlyChildren(this);
      ReadOnlyParentNullable?.AddToReadOnlyChildren(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ReadOnlyChild to CSV file
    /// </summary>
    internal const int MaxLineLength = 150;


    /// <summary>
    /// Write ReadOnlyChild to CSV file
    /// </summary>
    internal static void Write(ReadOnlyChild readOnlyChild, CsvWriter csvWriter) {
      readOnlyChild.onCsvWrite();
      csvWriter.Write(readOnlyChild.Text);
      if (readOnlyChild.ReadOnlyParent.Key<0) throw new Exception($"Cannot write readOnlyChild '{readOnlyChild}' to CSV File, because ReadOnlyParent is not stored in DL.Data.ReadOnlyParents.");

      csvWriter.Write(readOnlyChild.ReadOnlyParent.Key.ToString());
      if (readOnlyChild.ReadOnlyParentNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (readOnlyChild.ReadOnlyParentNullable.Key<0) throw new Exception($"Cannot write readOnlyChild '{readOnlyChild}' to CSV File, because ReadOnlyParentNullable is not stored in DL.Data.ReadOnlyParentNullables.");

        csvWriter.Write(readOnlyChild.ReadOnlyParentNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removing ReadOnlyChild from DL.Data.ReadOnlyChildren is not supported.
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
        $" {Text}," +
        $" {ReadOnlyParent.ToShortString()}," +
        $" {ReadOnlyParentNullable?.ToShortString()}";
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
        $" ReadOnlyParent: {ReadOnlyParent.ToShortString()}," +
        $" ReadOnlyParentNullable: {ReadOnlyParentNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
