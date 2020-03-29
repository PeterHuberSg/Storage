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
    /// Parent
    /// </summary>
    public ReadOnlyParent ReadOnlyParent { get; private set; }


    /// <summary>
    /// Some info
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"ReadOnlyParent", "Text"};


    /// <summary>
    /// None existing ReadOnlyChild
    /// </summary>
    internal static ReadOnlyChild NoReadOnlyChild = new ReadOnlyChild(ReadOnlyParent.NoReadOnlyParent, "NoText", isStoring: false);
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
    /// ReadOnlyChild Constructor. If isStoring is true, adds ReadOnlyChild to DL.Data.ReadOnlyChildren
    /// and adds ReadOnlyChild to readOnlyParent.ReadOnlyChildren.
    /// </summary>
    public ReadOnlyChild(ReadOnlyParent readOnlyParent, string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      ReadOnlyParent = readOnlyParent;
      Text = text;
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
      if (context.ReadOnlyParents.TryGetValue(csvReader.ReadInt(), out var readOnlyParent)) {
        ReadOnlyParent = readOnlyParent;
      } else {
        ReadOnlyParent = ReadOnlyParent.NoReadOnlyParent;
      }
      Text = csvReader.ReadString();
      if (ReadOnlyParent!=ReadOnlyParent.NoReadOnlyParent) {
        ReadOnlyParent.AddToReadOnlyChildren(this);
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
    /// </summary>
    internal static bool Verify(ReadOnlyChild readOnlyChild) {
      if (readOnlyChild.ReadOnlyParent==ReadOnlyParent.NoReadOnlyParent) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ReadOnlyChild to DL.Data.ReadOnlyChildren and ReadOnlyParent.ReadOnlyChildren. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ReadOnlyChild 'Class ReadOnlyChild' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      onStore();
      DL.Data.ReadOnlyChildren.Add(this);
      ReadOnlyParent.AddToReadOnlyChildren(this);
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
      if (readOnlyChild.ReadOnlyParent.Key<0) throw new Exception($"Cannot write readOnlyChild '{readOnlyChild}' to CSV File, because ReadOnlyParent is not stored in DL.Data.ReadOnlyParents.");

      csvWriter.Write(readOnlyChild.ReadOnlyParent.Key.ToString());
      csvWriter.Write(readOnlyChild.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Removing ReadOnlyChild from DL.Data.ReadOnlyChildren is not supported.
    /// </summary>
    public void Remove() {
      throw new NotSupportedException("Attribute AreInstancesDeletable is false.");
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {ReadOnlyParent.ToShortString()}," +
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
        $" ReadOnlyParent: {ReadOnlyParent.ToShortString()}," +
        $" Text: {Text};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
