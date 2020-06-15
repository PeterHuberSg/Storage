//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenSortedList_Parent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a parent child relationship using a SortedList.
    /// </summary>
  public partial class ChildrenSortedList_Parent: IStorage<ChildrenSortedList_Parent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenSortedList_Parent. Gets set once ChildrenSortedList_Parent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ChildrenSortedList_Parent childrenSortedList_Parent, int key) { childrenSortedList_Parent.Key = key; }


    /// <summary>
    /// This text is readonly. Readonly only matters when [StorageClass(areInstancesUpdatable: true)]
    /// </summary>
    public string TextReadOnly { get; }


    /// <summary>
    /// This text can be updated
    /// </summary>
    public string TextUpdateable { get; private set; }


    /// <summary>
    /// SortedList used instead of List. Comment is required and indicates which property of the SortedListChild to 
    /// use as key
    /// </summary>
    public IReadOnlyDictionary<DateTime, ChildrenSortedList_Child> ChildrenSortedList_Children => childrenSortedList_Children;
    readonly SortedList<DateTime, ChildrenSortedList_Child> childrenSortedList_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "TextReadOnly", "TextUpdateable"};


    /// <summary>
    /// None existing ChildrenSortedList_Parent
    /// </summary>
    internal static ChildrenSortedList_Parent NoChildrenSortedList_Parent = new ChildrenSortedList_Parent("NoTextReadOnly", "NoTextUpdateable", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenSortedList_Parent has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action<ChildrenSortedList_Parent>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenSortedList_Parent Constructor. If isStoring is true, adds ChildrenSortedList_Parent to DC.Data.ChildrenSortedList_Parents.
    /// </summary>
    public ChildrenSortedList_Parent(string textReadOnly, string textUpdateable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      TextReadOnly = textReadOnly;
      TextUpdateable = textUpdateable;
      childrenSortedList_Children = new SortedList<DateTime, ChildrenSortedList_Child>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ChildrenSortedList_Parent read from CSV file
    /// </summary>
    private ChildrenSortedList_Parent(int key, CsvReader csvReader, DC context) {
      Key = key;
      TextReadOnly = csvReader.ReadString();
      TextUpdateable = csvReader.ReadString();
      childrenSortedList_Children = new SortedList<DateTime, ChildrenSortedList_Child>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ChildrenSortedList_Parent read from CSV file
    /// </summary>
    internal static ChildrenSortedList_Parent Create(int key, CsvReader csvReader, DC context) {
      return new ChildrenSortedList_Parent(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenSortedList_Parent to DC.Data.ChildrenSortedList_Parents. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenSortedList_Parent cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenSortedList_Parents.Add(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ChildrenSortedList_Parent to CSV file
    /// </summary>
    public const int MaxLineLength = 300;


    /// <summary>
    /// Write ChildrenSortedList_Parent to CSV file
    /// </summary>
    internal static void Write(ChildrenSortedList_Parent childrenSortedList_Parent, CsvWriter csvWriter) {
      childrenSortedList_Parent.onCsvWrite();
      csvWriter.Write(childrenSortedList_Parent.TextReadOnly);
      csvWriter.Write(childrenSortedList_Parent.TextUpdateable);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenSortedList_Parent with the provided values
    /// </summary>
    public void Update(string textUpdateable) {
      var isCancelled = false;
      onUpdating(textUpdateable, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      if (TextUpdateable!=textUpdateable) {
        TextUpdateable = textUpdateable;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdating(string textUpdateable, ref bool isCancelled);
    partial void onUpdated();


    /// <summary>
    /// Updates this ChildrenSortedList_Parent with values from CSV file
    /// </summary>
    internal static void Update(ChildrenSortedList_Parent childrenSortedList_Parent, CsvReader csvReader, DC _) {
      var textReadOnly = csvReader.ReadString();
      if (childrenSortedList_Parent.TextReadOnly!=textReadOnly) {
        throw new Exception($"ChildrenSortedList_Parent.Update(): Property TextReadOnly '{childrenSortedList_Parent.TextReadOnly}' is " +
          $"readonly, textReadOnly '{textReadOnly}' read from the CSV file should be the same." + Environment.NewLine + 
          childrenSortedList_Parent.ToString());
      }
      childrenSortedList_Parent.TextUpdateable = csvReader.ReadString();
      childrenSortedList_Parent.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add childrenSortedList_Child to ChildrenSortedList_Children.
    /// </summary>
    internal void AddToChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child) {
      childrenSortedList_Children.Add(childrenSortedList_Child.DateKey, childrenSortedList_Child);
      onAddedToChildrenSortedList_Children(childrenSortedList_Child);
    }
    partial void onAddedToChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child);


    /// <summary>
    /// Removes childrenSortedList_Child from ChildrenSortedList_Parent.
    /// </summary>
    internal void RemoveFromChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child) {
#if DEBUG
      if (!childrenSortedList_Children.Remove(childrenSortedList_Child.DateKey)) throw new Exception();
#else
        childrenSortedList_Children.Remove(childrenSortedList_Child.DateKey);
#endif
      onRemovedFromChildrenSortedList_Children(childrenSortedList_Child);
    }
    partial void onRemovedFromChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child);


    /// <summary>
    /// Removes ChildrenSortedList_Parent from DC.Data.ChildrenSortedList_Parents and 
    /// deletes any ChildrenSortedList_Child where ChildrenSortedList_Child.ParentWithSortedList links to this ChildrenSortedList_Parent.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"ChildrenSortedList_Parent.Remove(): ChildrenSortedList_Parent 'Class ChildrenSortedList_Parent' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.ChildrenSortedList_Parents.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Deletes any ChildrenSortedList_Child where ChildrenSortedList_Child.ParentWithSortedList links to this ChildrenSortedList_Parent.
    /// </summary>
    internal static void Disconnect(ChildrenSortedList_Parent childrenSortedList_Parent) {
      var childrenSortedList_Children = new ChildrenSortedList_Child[childrenSortedList_Parent.ChildrenSortedList_Children.Count];
      childrenSortedList_Parent.childrenSortedList_Children.Values.CopyTo(childrenSortedList_Children, 0);
      foreach (var childrenSortedList_Child in childrenSortedList_Children) {
         if (childrenSortedList_Child.Key>=0) {
           childrenSortedList_Child.Remove();
         }
      }
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {TextReadOnly}," +
        $" {TextUpdateable}";
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
        $" TextReadOnly: {TextReadOnly}," +
        $" TextUpdateable: {TextUpdateable}," +
        $" ChildrenSortedList_Children: {ChildrenSortedList_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
