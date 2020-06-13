//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ParentOneChild_ParentNullable.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example for parent which can have at most 1 child and the parent property in the child is nullable.
    /// </summary>
  public partial class ParentOneChild_ParentNullable: IStorage<ParentOneChild_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ParentOneChild_ParentNullable. Gets set once ParentOneChild_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ParentOneChild_ParentNullable parentOneChild_ParentNullable, int key) { parentOneChild_ParentNullable.Key = key; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Links to conditional child. Parent might or might not have a child, since the parent always gets
    /// created before the child.
    /// </summary>
    public ParentOneChild_Child? Child { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing ParentOneChild_ParentNullable
    /// </summary>
    internal static ParentOneChild_ParentNullable NoParentOneChild_ParentNullable = new ParentOneChild_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ParentOneChild_ParentNullable has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action<ParentOneChild_ParentNullable>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ParentOneChild_ParentNullable Constructor. If isStoring is true, adds ParentOneChild_ParentNullable to DC.Data.ParentOneChild_ParentNullables.
    /// </summary>
    public ParentOneChild_ParentNullable(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ParentOneChild_ParentNullable read from CSV file
    /// </summary>
    private ParentOneChild_ParentNullable(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ParentOneChild_ParentNullable read from CSV file
    /// </summary>
    internal static ParentOneChild_ParentNullable Create(int key, CsvReader csvReader, DC context) {
      return new ParentOneChild_ParentNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ParentOneChild_ParentNullable to DC.Data.ParentOneChild_ParentNullables. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ParentOneChild_ParentNullable cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.ParentOneChild_ParentNullables.Add(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ParentOneChild_ParentNullable to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write ParentOneChild_ParentNullable to CSV file
    /// </summary>
    internal static void Write(ParentOneChild_ParentNullable parentOneChild_ParentNullable, CsvWriter csvWriter) {
      parentOneChild_ParentNullable.onCsvWrite();
      csvWriter.Write(parentOneChild_ParentNullable.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ParentOneChild_ParentNullable with the provided values
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
    /// Updates this ParentOneChild_ParentNullable with values from CSV file
    /// </summary>
    internal static void Update(ParentOneChild_ParentNullable parentOneChild_ParentNullable, CsvReader csvReader, DC _) {
      parentOneChild_ParentNullable.Text = csvReader.ReadString();
      parentOneChild_ParentNullable.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add parentOneChild_Child to Child.
    /// </summary>
    internal void AddToChild(ParentOneChild_Child parentOneChild_Child) {
      if (Child!=null) {
        throw new Exception($"ParentOneChild_ParentNullable.AddToChild(): '{Child}' is already assigned to Child, it is not possible to assign now '{parentOneChild_Child}'.");
      }
      Child = parentOneChild_Child;
      onAddedToChild(parentOneChild_Child);
    }
    partial void onAddedToChild(ParentOneChild_Child parentOneChild_Child);


    /// <summary>
    /// Removes parentOneChild_Child from ParentOneChild_ParentNullable.
    /// </summary>
    internal void RemoveFromChild(ParentOneChild_Child parentOneChild_Child) {
#if DEBUG
      if (Child!=parentOneChild_Child) {
        throw new Exception($"ParentOneChild_ParentNullable.RemoveFromChild(): Child does not link to parentOneChild_Child '{parentOneChild_Child}' but '{Child}'.");
      }
#endif
      Child = null;
      onRemovedFromChild(parentOneChild_Child);
    }
    partial void onRemovedFromChild(ParentOneChild_Child parentOneChild_Child);


    /// <summary>
    /// Removes ParentOneChild_ParentNullable from DC.Data.ParentOneChild_ParentNullables and 
    /// disconnects ParentOneChild_Child.ParentNullable from ParentOneChild_Children.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"ParentOneChild_ParentNullable.Remove(): ParentOneChild_ParentNullable 'Class ParentOneChild_ParentNullable' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.ParentOneChild_ParentNullables.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects ParentOneChild_Child.ParentNullable from ParentOneChild_Children.
    /// </summary>
    internal static void Disconnect(ParentOneChild_ParentNullable parentOneChild_ParentNullable) {
      if (parentOneChild_ParentNullable.Child!=null) {
        parentOneChild_ParentNullable.Child.Remove();
        parentOneChild_ParentNullable.Child = null;
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
        $" Child: {Child?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
