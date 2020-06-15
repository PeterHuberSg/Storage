//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into CreateOnlyParentChangeableChild_ParentNullable.cs.
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
    /// deleted, but it is still possible to add and remove children. The parent property in the child 
    /// is nullable.
    /// </summary>
  public partial class CreateOnlyParentChangeableChild_ParentNullable: IStorage<CreateOnlyParentChangeableChild_ParentNullable> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for CreateOnlyParentChangeableChild_ParentNullable. Gets set once CreateOnlyParentChangeableChild_ParentNullable gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(CreateOnlyParentChangeableChild_ParentNullable createOnlyParentChangeableChild_ParentNullable, int key) { createOnlyParentChangeableChild_ParentNullable.Key = key; }


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
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing CreateOnlyParentChangeableChild_ParentNullable
    /// </summary>
    internal static CreateOnlyParentChangeableChild_ParentNullable NoCreateOnlyParentChangeableChild_ParentNullable = new CreateOnlyParentChangeableChild_ParentNullable("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// This event will never be raised, but is needed to comply with IStorage.
    /// </summary>
#pragma warning disable 67
    public event Action<CreateOnlyParentChangeableChild_ParentNullable>? HasChanged;
#pragma warning restore 67
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// CreateOnlyParentChangeableChild_ParentNullable Constructor. If isStoring is true, adds CreateOnlyParentChangeableChild_ParentNullable to DC.Data.CreateOnlyParentChangeableChild_ParentNullables.
    /// </summary>
    public CreateOnlyParentChangeableChild_ParentNullable(string text, bool isStoring = true) {
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
    /// Constructor for CreateOnlyParentChangeableChild_ParentNullable read from CSV file
    /// </summary>
    private CreateOnlyParentChangeableChild_ParentNullable(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      createOnlyParentChangeableChild_Children = new List<CreateOnlyParentChangeableChild_Child>();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New CreateOnlyParentChangeableChild_ParentNullable read from CSV file
    /// </summary>
    internal static CreateOnlyParentChangeableChild_ParentNullable Create(int key, CsvReader csvReader, DC context) {
      return new CreateOnlyParentChangeableChild_ParentNullable(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds CreateOnlyParentChangeableChild_ParentNullable to DC.Data.CreateOnlyParentChangeableChild_ParentNullables. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"CreateOnlyParentChangeableChild_ParentNullable cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.CreateOnlyParentChangeableChild_ParentNullables.Add(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write CreateOnlyParentChangeableChild_ParentNullable to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write CreateOnlyParentChangeableChild_ParentNullable to CSV file
    /// </summary>
    internal static void Write(CreateOnlyParentChangeableChild_ParentNullable createOnlyParentChangeableChild_ParentNullable, CsvWriter csvWriter) {
      createOnlyParentChangeableChild_ParentNullable.onCsvWrite();
      csvWriter.Write(createOnlyParentChangeableChild_ParentNullable.Text);
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
    /// Removes createOnlyParentChangeableChild_Child from CreateOnlyParentChangeableChild_ParentNullable.
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
    /// Removes CreateOnlyParentChangeableChild_ParentNullable from DC.Data.CreateOnlyParentChangeableChild_ParentNullables and 
    /// disconnects CreateOnlyParentChangeableChild_Child.ParentNullable from CreateOnlyParentChangeableChild_Children.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"CreateOnlyParentChangeableChild_ParentNullable.Remove(): CreateOnlyParentChangeableChild_ParentNullable 'Class CreateOnlyParentChangeableChild_ParentNullable' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.CreateOnlyParentChangeableChild_ParentNullables.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects CreateOnlyParentChangeableChild_Child.ParentNullable from CreateOnlyParentChangeableChild_Children.
    /// </summary>
    internal static void Disconnect(CreateOnlyParentChangeableChild_ParentNullable createOnlyParentChangeableChild_ParentNullable) {
      for (int createOnlyParentChangeableChild_ChildIndex = createOnlyParentChangeableChild_ParentNullable.CreateOnlyParentChangeableChild_Children.Count-1; createOnlyParentChangeableChild_ChildIndex>= 0; createOnlyParentChangeableChild_ChildIndex--) {
        var createOnlyParentChangeableChild_Child = createOnlyParentChangeableChild_ParentNullable.CreateOnlyParentChangeableChild_Children[createOnlyParentChangeableChild_ChildIndex];
        createOnlyParentChangeableChild_Child.RemoveParentNullable(createOnlyParentChangeableChild_ParentNullable);
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
        $" CreateOnlyParentChangeableChild_Children: {CreateOnlyParentChangeableChild_Children.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
