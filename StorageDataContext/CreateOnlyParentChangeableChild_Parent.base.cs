//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into CreateOnlyParentChangeableChild_Parent.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
    /// deleted, but it is still possible to add and remove children.
    /// </summary>
  public partial class CreateOnlyParentChangeableChild_Parent: IStorage<CreateOnlyParentChangeableChild_Parent> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for CreateOnlyParentChangeableChild_Parent. Gets set once CreateOnlyParentChangeableChild_Parent gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(CreateOnlyParentChangeableChild_Parent createOnlyParentChangeableChild_Parent, int key) { createOnlyParentChangeableChild_Parent.Key = key; }


    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text { get; }


    /// <summary>
    /// List of children
    /// </summary>
    public IReadOnlyList<CreateOnlyParentChangeableChild_Child> CreateOnlyParentChangeableChild_Children => createOnlyParentChangeableChild_Children;
    readonly List<CreateOnlyParentChangeableChild_Child> createOnlyParentChangeableChild_Children;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Text"};


    /// <summary>
    /// None existing CreateOnlyParentChangeableChild_Parent
    /// </summary>
    internal static CreateOnlyParentChangeableChild_Parent NoCreateOnlyParentChangeableChild_Parent = new CreateOnlyParentChangeableChild_Parent("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<CreateOnlyParentChangeableChild_Parent>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// CreateOnlyParentChangeableChild_Parent Constructor. If isStoring is true, adds CreateOnlyParentChangeableChild_Parent to DC.Data.CreateOnlyParentChangeableChild_Parents.
    /// </summary>
    public CreateOnlyParentChangeableChild_Parent(string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      createOnlyParentChangeableChild_Children = new List<CreateOnlyParentChangeableChild_Child>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for CreateOnlyParentChangeableChild_Parent read from CSV file
    /// </summary>
    private CreateOnlyParentChangeableChild_Parent(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      createOnlyParentChangeableChild_Children = new List<CreateOnlyParentChangeableChild_Child>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New CreateOnlyParentChangeableChild_Parent read from CSV file
    /// </summary>
    internal static CreateOnlyParentChangeableChild_Parent Create(int key, CsvReader csvReader, DC context) {
      return new CreateOnlyParentChangeableChild_Parent(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds CreateOnlyParentChangeableChild_Parent to DC.Data.CreateOnlyParentChangeableChild_Parents. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"CreateOnlyParentChangeableChild_Parent cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.CreateOnlyParentChangeableChild_Parents.Add(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write CreateOnlyParentChangeableChild_Parent to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write CreateOnlyParentChangeableChild_Parent to CSV file
    /// </summary>
    internal static void Write(CreateOnlyParentChangeableChild_Parent createOnlyParentChangeableChild_Parent, CsvWriter csvWriter) {
      createOnlyParentChangeableChild_Parent.onCsvWrite();
      csvWriter.Write(createOnlyParentChangeableChild_Parent.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Add createOnlyParentChangeableChild_Child to CreateOnlyParentChangeableChild_Children.
    /// </summary>
    internal void AddToCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child) {
      createOnlyParentChangeableChild_Children.Add(createOnlyParentChangeableChild_Child);
      onAddedToCreateOnlyParentChangeableChild_Children(createOnlyParentChangeableChild_Child);
    }
    partial void onAddedToCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child);


    /// <summary>
    /// Removes createOnlyParentChangeableChild_Child from CreateOnlyParentChangeableChild_Parent.
    /// </summary>
    internal void RemoveFromCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child) {
#if DEBUG
      if (!createOnlyParentChangeableChild_Children.Remove(createOnlyParentChangeableChild_Child)) throw new Exception();
#else
        createOnlyParentChangeableChild_Children.Remove(createOnlyParentChangeableChild_Child);
#endif
      onRemovedFromCreateOnlyParentChangeableChild_Children(createOnlyParentChangeableChild_Child);
    }
    partial void onRemovedFromCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child);


    /// <summary>
    /// Removing CreateOnlyParentChangeableChild_Parent from DC.Data.CreateOnlyParentChangeableChild_Parents is not supported.
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
        $" CreateOnlyParentChangeableChild_Children: {CreateOnlyParentChangeableChild_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
