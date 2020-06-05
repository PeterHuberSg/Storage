//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ReadOnlyParentUpdatableChild_ParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
    /// deleted, but it is still possible to add children, but not to remove them. The parent property in the child 
    /// is nullable.
    /// </summary>
  public partial class ReadOnlyParentUpdatableChild_ParentNullable: IStorage<ReadOnlyParentUpdatableChild_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ReadOnlyParentUpdatableChild_ParentNullable. Gets set once ReadOnlyParentUpdatableChild_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ReadOnlyParentUpdatableChild_ParentNullable readOnlyParentUpdatableChild_ParentNullable, int key) { readOnlyParentUpdatableChild_ParentNullable.Key = key; }


    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// List of children
    /// </summary>
    public IReadOnlyList<ReadOnlyParentUpdatableChild_Child> ReadOnlyParentUpdatableChild_Children => readOnlyParentUpdatableChild_Children;
    readonly List<ReadOnlyParentUpdatableChild_Child> readOnlyParentUpdatableChild_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text"};


    /// <summary>
    /// None existing ReadOnlyParentUpdatableChild_ParentNullable
    /// </summary>
    internal static ReadOnlyParentUpdatableChild_ParentNullable NoReadOnlyParentUpdatableChild_ParentNullable = new ReadOnlyParentUpdatableChild_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<ReadOnlyParentUpdatableChild_ParentNullable>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ReadOnlyParentUpdatableChild_ParentNullable Constructor. If isStoring is true, adds ReadOnlyParentUpdatableChild_ParentNullable to DC.Data.ReadOnlyParentUpdatableChild_ParentNullables.
    /// </summary>
    public ReadOnlyParentUpdatableChild_ParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      readOnlyParentUpdatableChild_Children = new List<ReadOnlyParentUpdatableChild_Child>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ReadOnlyParentUpdatableChild_ParentNullable read from CSV file
    /// </summary>
    private ReadOnlyParentUpdatableChild_ParentNullable(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      readOnlyParentUpdatableChild_Children = new List<ReadOnlyParentUpdatableChild_Child>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ReadOnlyParentUpdatableChild_ParentNullable read from CSV file
    /// </summary>
    internal static ReadOnlyParentUpdatableChild_ParentNullable Create(int key, CsvReader csvReader, DC context) {
      return new ReadOnlyParentUpdatableChild_ParentNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ReadOnlyParentUpdatableChild_ParentNullable to DC.Data.ReadOnlyParentUpdatableChild_ParentNullables. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ReadOnlyParentUpdatableChild_ParentNullable cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.ReadOnlyParentUpdatableChild_ParentNullables.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ReadOnlyParentUpdatableChild_ParentNullable to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write ReadOnlyParentUpdatableChild_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ReadOnlyParentUpdatableChild_ParentNullable readOnlyParentUpdatableChild_ParentNullable, CsvWriter csvWriter) {
      readOnlyParentUpdatableChild_ParentNullable.onCsvWrite();
      csvWriter.Write(readOnlyParentUpdatableChild_ParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Add readOnlyParentUpdatableChild_Child to ReadOnlyParentUpdatableChild_Children.
    /// </summary>
    internal void AddToReadOnlyParentUpdatableChild_Children(ReadOnlyParentUpdatableChild_Child readOnlyParentUpdatableChild_Child) {
      readOnlyParentUpdatableChild_Children.Add(readOnlyParentUpdatableChild_Child);
      onAddedToReadOnlyParentUpdatableChild_Children(readOnlyParentUpdatableChild_Child);
    }
    partial void onAddedToReadOnlyParentUpdatableChild_Children(ReadOnlyParentUpdatableChild_Child readOnlyParentUpdatableChild_Child);


    /// <summary>
    /// Removes readOnlyParentUpdatableChild_Child from ReadOnlyParentUpdatableChild_ParentNullable.
    /// </summary>
    internal void RemoveFromReadOnlyParentUpdatableChild_Children(ReadOnlyParentUpdatableChild_Child readOnlyParentUpdatableChild_Child) {
#if DEBUG
      if (!readOnlyParentUpdatableChild_Children.Remove(readOnlyParentUpdatableChild_Child)) throw new Exception();
#else
        readOnlyParentUpdatableChild_Children.Remove(readOnlyParentUpdatableChild_Child);
#endif
      onRemovedFromReadOnlyParentUpdatableChild_Children(readOnlyParentUpdatableChild_Child);
    }
    partial void onRemovedFromReadOnlyParentUpdatableChild_Children(ReadOnlyParentUpdatableChild_Child readOnlyParentUpdatableChild_Child);


    /// <summary>
    /// Removing ReadOnlyParentUpdatableChild_ParentNullable from DC.Data.ReadOnlyParentUpdatableChild_ParentNullables is not supported.
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
        $" ReadOnlyParentUpdatableChild_Children: {ReadOnlyParentUpdatableChild_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
