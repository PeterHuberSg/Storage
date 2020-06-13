//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ParentOneChild_Child.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Child class with one parent property which is not nullable and one property to a different parent 
    /// which is nullable
    /// </summary>
  public partial class ParentOneChild_Child: IStorage<ParentOneChild_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ParentOneChild_Child. Gets set once ParentOneChild_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(ParentOneChild_Child parentOneChild_Child, int key) { parentOneChild_Child.Key = key; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Links to parent
    /// </summary>
    public ParentOneChild_Parent Parent { get; private set; }


    /// <summary>
    /// Links to parent conditionally
    /// </summary>
    public ParentOneChild_ParentNullable? ParentNullable { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text", "Parent", "ParentNullable"};


    /// <summary>
    /// None existing ParentOneChild_Child
    /// </summary>
    internal static ParentOneChild_Child NoParentOneChild_Child = new ParentOneChild_Child("NoText", ParentOneChild_Parent.NoParentOneChild_Parent, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ParentOneChild_Child has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action<ParentOneChild_Child>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ParentOneChild_Child Constructor. If isStoring is true, adds ParentOneChild_Child to DC.Data.ParentOneChild_Children, 
    /// adds ParentOneChild_Child to parentOneChild_Parent.ParentOneChild_Children
    /// and if there is a ParentNullable adds ParentOneChild_Child to parentOneChild_ParentNullable.ParentOneChild_Children.
    /// </summary>
    public ParentOneChild_Child(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      Parent = parent;
      ParentNullable = parentNullable;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for ParentOneChild_Child read from CSV file
    /// </summary>
    private ParentOneChild_Child(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      var parentOneChild_ParentKey = csvReader.ReadInt();
      if (context.ParentOneChild_Parents.TryGetValue(parentOneChild_ParentKey, out var parent)) {
          Parent = parent;
      } else {
        throw new Exception($"Read ParentOneChild_Child from CSV file: Cannot find Parent with key {parentOneChild_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var parentNullableKey = csvReader.ReadIntNull();
      if (parentNullableKey.HasValue) {
        if (context.ParentOneChild_ParentNullables.TryGetValue(parentNullableKey.Value, out var parentNullable)) {
          ParentNullable = parentNullable;
        } else {
          ParentNullable = ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable;
        }
      }
      if (Parent!=ParentOneChild_Parent.NoParentOneChild_Parent) {
        Parent.AddToChild(this);
      }
      if (parentNullableKey.HasValue && ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
        ParentNullable!.AddToChild(this);
      }
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New ParentOneChild_Child read from CSV file
    /// </summary>
    internal static ParentOneChild_Child Create(int key, CsvReader csvReader, DC context) {
      return new ParentOneChild_Child(key, csvReader, context);
    }


    /// <summary>
    /// Verify that parentOneChild_Child.Parent exists.
    /// Verify that parentOneChild_Child.ParentNullable exists.
    /// </summary>
    internal static bool Verify(ParentOneChild_Child parentOneChild_Child) {
      if (parentOneChild_Child.Parent==ParentOneChild_Parent.NoParentOneChild_Parent) return false;
      if (parentOneChild_Child.ParentNullable==ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ParentOneChild_Child to DC.Data.ParentOneChild_Children, ParentOneChild_Parent and ParentOneChild_ParentNullable. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ParentOneChild_Child cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      if (Parent.Key<0) {
        throw new Exception($"ParentOneChild_Child cannot be stored in DC.Data, Parent is missing or not stored yet." + Environment.NewLine + ToString());
      }
      if (ParentNullable!=null && ParentNullable.Key<0) {
        throw new Exception($"ParentOneChild_Child cannot be stored in DC.Data, ParentNullable is not stored yet." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.ParentOneChild_Children.Add(this);
      Parent.AddToChild(this);
      ParentNullable?.AddToChild(this);
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write ParentOneChild_Child to CSV file
    /// </summary>
    public const int MaxLineLength = 150;


    /// <summary>
    /// Write ParentOneChild_Child to CSV file
    /// </summary>
    internal static void Write(ParentOneChild_Child parentOneChild_Child, CsvWriter csvWriter) {
      parentOneChild_Child.onCsvWrite();
      csvWriter.Write(parentOneChild_Child.Text);
      if (parentOneChild_Child.Parent.Key<0) throw new Exception($"Cannot write parentOneChild_Child '{parentOneChild_Child}' to CSV File, because Parent is not stored in DC.Data.ParentOneChild_Parents.");

      csvWriter.Write(parentOneChild_Child.Parent.Key.ToString());
      if (parentOneChild_Child.ParentNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (parentOneChild_Child.ParentNullable.Key<0) throw new Exception($"Cannot write parentOneChild_Child '{parentOneChild_Child}' to CSV File, because ParentNullable is not stored in DC.Data.ParentOneChild_ParentNullables.");

        csvWriter.Write(parentOneChild_Child.ParentNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ParentOneChild_Child with the provided values
    /// </summary>
    public void Update(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable) {
      var isCancelled = false;
      onUpdating(text, parent, parentNullable, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (Parent!=parent) {
        Parent.RemoveFromChild(this);
        Parent = parent;
        Parent.AddToChild(this);
        isChangeDetected = true;
      }
      if (ParentNullable is null) {
        if (parentNullable is null) {
          //nothing to do
        } else {
          ParentNullable = parentNullable;
          ParentNullable.AddToChild(this);
          isChangeDetected = true;
        }
      } else {
        if (parentNullable is null) {
          ParentNullable.RemoveFromChild(this);
          ParentNullable = null;
          isChangeDetected = true;
        } else {
          if (ParentNullable!=parentNullable) {
            ParentNullable.RemoveFromChild(this);
            ParentNullable = parentNullable;
            ParentNullable.AddToChild(this);
            isChangeDetected = true;
          }
        }
      }
      if (isChangeDetected) {
        onUpdated();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdating(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable, ref bool isCancelled);
    partial void onUpdated();


    /// <summary>
    /// Updates this ParentOneChild_Child with values from CSV file
    /// </summary>
    internal static void Update(ParentOneChild_Child parentOneChild_Child, CsvReader csvReader, DC context) {
      parentOneChild_Child.Text = csvReader.ReadString();
      if (!context.ParentOneChild_Parents.TryGetValue(csvReader.ReadInt(), out var parent)) {
        parent = ParentOneChild_Parent.NoParentOneChild_Parent;
      }
      if (parentOneChild_Child.Parent!=parent) {
        if (parentOneChild_Child.Parent!=ParentOneChild_Parent.NoParentOneChild_Parent) {
          parentOneChild_Child.Parent.RemoveFromChild(parentOneChild_Child);
        }
        parentOneChild_Child.Parent = parent;
        parentOneChild_Child.Parent.AddToChild(parentOneChild_Child);
      }
      var parentNullableKey = csvReader.ReadIntNull();
      ParentOneChild_ParentNullable? parentNullable;
      if (parentNullableKey is null) {
        parentNullable = null;
      } else {
        if (!context.ParentOneChild_ParentNullables.TryGetValue(parentNullableKey.Value, out parentNullable)) {
          parentNullable = ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable;
        }
      }
      if (parentOneChild_Child.ParentNullable is null) {
        if (parentNullable is null) {
          //nothing to do
        } else {
          parentOneChild_Child.ParentNullable = parentNullable;
          parentOneChild_Child.ParentNullable.AddToChild(parentOneChild_Child);
        }
      } else {
        if (parentNullable is null) {
          if (parentOneChild_Child.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
            parentOneChild_Child.ParentNullable.RemoveFromChild(parentOneChild_Child);
          }
          parentOneChild_Child.ParentNullable = null;
        } else {
          if (parentOneChild_Child.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
            parentOneChild_Child.ParentNullable.RemoveFromChild(parentOneChild_Child);
          }
          parentOneChild_Child.ParentNullable = parentNullable;
          parentOneChild_Child.ParentNullable.AddToChild(parentOneChild_Child);
        }
      }
      parentOneChild_Child.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes ParentOneChild_Child from DC.Data.ParentOneChild_Children, 
    /// disconnects ParentOneChild_Child from ParentOneChild_Parent because of Parent and 
    /// disconnects ParentOneChild_Child from ParentOneChild_ParentNullable because of ParentNullable.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"ParentOneChild_Child.Remove(): ParentOneChild_Child 'Class ParentOneChild_Child' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.ParentOneChild_Children.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects ParentOneChild_Child from ParentOneChild_Parent because of Parent and 
    /// disconnects ParentOneChild_Child from ParentOneChild_ParentNullable because of ParentNullable.
    /// </summary>
    internal static void Disconnect(ParentOneChild_Child parentOneChild_Child) {
      if (parentOneChild_Child.Parent!=ParentOneChild_Parent.NoParentOneChild_Parent) {
        parentOneChild_Child.Parent.RemoveFromChild(parentOneChild_Child);
      }
      if (parentOneChild_Child.ParentNullable!=null && parentOneChild_Child.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
        parentOneChild_Child.ParentNullable.RemoveFromChild(parentOneChild_Child);
      }
    }


    /// <summary>
    /// Removes parentOneChild_ParentNullable from ParentNullable
    /// </summary>
    internal void RemoveParentNullable(ParentOneChild_ParentNullable parentOneChild_ParentNullable) {
      if (parentOneChild_ParentNullable!=ParentNullable) throw new Exception();
      ParentNullable = null;
      HasChanged?.Invoke(this);
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {Parent.ToShortString()}," +
        $" {ParentNullable?.ToShortString()}";
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
        $" Parent: {Parent.ToShortString()}," +
        $" ParentNullable: {ParentNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}