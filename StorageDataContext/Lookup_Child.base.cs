//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into Lookup_Child.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a child with a none nullable and a nullable lookup parent. The child maintains links
    /// to its parents, but the parents don't have children collections.
    /// </summary>
  public partial class Lookup_Child: IStorageItemGeneric<Lookup_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for Lookup_Child. Gets set once Lookup_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem lookup_Child, int key) {
      ((Lookup_Child)lookup_Child).Key = key;
    }


    /// <summary>
    /// Some info
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Parent does not have a collection for LookupChild, because the child wants to use it only for
    /// lookup. This property requires a parent.
    /// </summary>
    public Lookup_Parent LookupParent { get; private set; }


    /// <summary>
    /// Parent does not have a collection for LookupChild, because the child wants to use it only for
    /// lookup. This property does not require a parent.
    /// </summary>
    public Lookup_ParentNullable? LookupParentNullable { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text", "LookupParent", "LookupParentNullable"};


    /// <summary>
    /// None existing Lookup_Child
    /// </summary>
    internal static Lookup_Child NoLookup_Child = new Lookup_Child("NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of Lookup_Child has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/Lookup_Child, /*new*/Lookup_Child>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Lookup_Child Constructor. If isStoring is true, adds Lookup_Child to DC.Data.Lookup_Children, 
    /// adds Lookup_Child to lookup_Parent.Lookup_Children
    /// and if there is a LookupParentNullable adds Lookup_Child to lookup_ParentNullable.Lookup_Children.
    /// </summary>
    public Lookup_Child(string text, Lookup_Parent lookupParent, Lookup_ParentNullable? lookupParentNullable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      LookupParent = lookupParent;
      LookupParentNullable = lookupParentNullable;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Cloning constructor. It will copy all data from original except any collection (children).
    /// </summary>
    #pragma warning disable CS8618 // Children collections are uninitialized.
    public Lookup_Child(Lookup_Child original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      LookupParent = original.LookupParent;
      LookupParentNullable = original.LookupParentNullable;
      onCloned(this);
    }
    partial void onCloned(Lookup_Child clone);


    /// <summary>
    /// Constructor for Lookup_Child read from CSV file
    /// </summary>
    private Lookup_Child(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      var lookup_ParentKey = csvReader.ReadInt();
      if (DC.Data.Lookup_Parents.TryGetValue(lookup_ParentKey, out var lookupParent)) {
          LookupParent = lookupParent;
      } else {
        throw new Exception($"Read Lookup_Child from CSV file: Cannot find LookupParent with key {lookup_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var lookupParentNullableKey = csvReader.ReadIntNull();
      if (lookupParentNullableKey.HasValue) {
        if (DC.Data.Lookup_ParentNullables.TryGetValue(lookupParentNullableKey.Value, out var lookupParentNullable)) {
          LookupParentNullable = lookupParentNullable;
        } else {
          LookupParentNullable = Lookup_ParentNullable.NoLookup_ParentNullable;
        }
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New Lookup_Child read from CSV file
    /// </summary>
    internal static Lookup_Child Create(int key, CsvReader csvReader) {
      return new Lookup_Child(key, csvReader);
    }


    /// <summary>
    /// Verify that lookup_Child.LookupParent exists.
    /// Verify that lookup_Child.LookupParentNullable exists.
    /// </summary>
    internal static bool Verify(Lookup_Child lookup_Child) {
      if (lookup_Child.LookupParent==Lookup_Parent.NoLookup_Parent) return false;
      if (lookup_Child.LookupParentNullable==Lookup_ParentNullable.NoLookup_ParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds Lookup_Child to DC.Data.Lookup_Children, Lookup_Parent and Lookup_ParentNullable. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"Lookup_Child cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.Lookup_Children.Add(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write Lookup_Child to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write Lookup_Child to CSV file
    /// </summary>
    internal static void Write(Lookup_Child lookup_Child, CsvWriter csvWriter) {
      lookup_Child.onCsvWrite();
      csvWriter.Write(lookup_Child.Text);
      if (lookup_Child.LookupParent.Key<0) throw new Exception($"Cannot write lookup_Child '{lookup_Child}' to CSV File, because LookupParent is not stored in DC.Data.Lookup_Parents.");

      csvWriter.Write(lookup_Child.LookupParent.Key.ToString());
      if (lookup_Child.LookupParentNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (lookup_Child.LookupParentNullable.Key<0) throw new Exception($"Cannot write lookup_Child '{lookup_Child}' to CSV File, because LookupParentNullable is not stored in DC.Data.Lookup_ParentNullables.");

        csvWriter.Write(lookup_Child.LookupParentNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates Lookup_Child with the provided values
    /// </summary>
    public void Update(string text, Lookup_Parent lookupParent, Lookup_ParentNullable? lookupParentNullable) {
      var clone = new Lookup_Child(this);
      var isCancelled = false;
      onUpdating(text, lookupParent, lookupParentNullable, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (LookupParent!=lookupParent) {
        LookupParent = lookupParent;
        isChangeDetected = true;
      }
      if (LookupParentNullable is null) {
        if (lookupParentNullable is null) {
          //nothing to do
        } else {
          LookupParentNullable = lookupParentNullable;
          isChangeDetected = true;
        }
      } else {
        if (lookupParentNullable is null) {
          LookupParentNullable = null;
          isChangeDetected = true;
        } else {
          if (LookupParentNullable!=lookupParentNullable) {
            LookupParentNullable = lookupParentNullable;
            isChangeDetected = true;
          }
        }
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.Lookup_Children.ItemHasChanged(clone, this);
        }
        HasChanged?.Invoke(clone, this);
      }
    }
    partial void onUpdating(string text, Lookup_Parent lookupParent, Lookup_ParentNullable? lookupParentNullable, ref bool isCancelled);
    partial void onUpdated(Lookup_Child old);


    /// <summary>
    /// Updates this Lookup_Child with values from CSV file
    /// </summary>
    internal static void Update(Lookup_Child lookup_Child, CsvReader csvReader){
      lookup_Child.Text = csvReader.ReadString();
      if (!DC.Data.Lookup_Parents.TryGetValue(csvReader.ReadInt(), out var lookupParent)) {
        lookupParent = Lookup_Parent.NoLookup_Parent;
      }
      if (lookup_Child.LookupParent!=lookupParent) {
        lookup_Child.LookupParent = lookupParent;
      }
      var lookupParentNullableKey = csvReader.ReadIntNull();
      Lookup_ParentNullable? lookupParentNullable;
      if (lookupParentNullableKey is null) {
        lookupParentNullable = null;
      } else {
        if (!DC.Data.Lookup_ParentNullables.TryGetValue(lookupParentNullableKey.Value, out lookupParentNullable)) {
          lookupParentNullable = Lookup_ParentNullable.NoLookup_ParentNullable;
        }
      }
      if (lookup_Child.LookupParentNullable is null) {
        if (lookupParentNullable is null) {
          //nothing to do
        } else {
          lookup_Child.LookupParentNullable = lookupParentNullable;
        }
      } else {
        if (lookupParentNullable is null) {
          lookup_Child.LookupParentNullable = null;
        } else {
          lookup_Child.LookupParentNullable = lookupParentNullable;
        }
      }
      lookup_Child.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes Lookup_Child from DC.Data.Lookup_Children, 
    /// disconnects Lookup_Child from Lookup_Parent because of LookupParent and 
    /// disconnects Lookup_Child from Lookup_ParentNullable because of LookupParentNullable.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"Lookup_Child.Remove(): Lookup_Child 'Class Lookup_Child' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.Lookup_Children.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects Lookup_Child from Lookup_Parent because of LookupParent and 
    /// disconnects Lookup_Child from Lookup_ParentNullable because of LookupParentNullable.
    /// </summary>
    internal static void Disconnect(Lookup_Child lookup_Child) {
    }


    /// <summary>
    /// Removes lookup_ParentNullable from LookupParentNullable
    /// </summary>
    internal void RemoveLookupParentNullable(Lookup_ParentNullable lookup_ParentNullable) {
      if (lookup_ParentNullable!=LookupParentNullable) throw new Exception();

      var clone = new Lookup_Child(this);
      LookupParentNullable = null;
      HasChanged?.Invoke(clone, this);
    }


    /// <summary>
    /// Removes Lookup_Child from possible parents as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var lookup_Child = (Lookup_Child) item;
      lookup_Child.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the Lookup_Child item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldItem, IStorageItem newItem) {
      var lookup_ChildOld = (Lookup_Child) oldItem;
      var lookup_ChildNew = (Lookup_Child) newItem;
      lookup_ChildNew.Text = lookup_ChildOld.Text;
      if (lookup_ChildNew.LookupParent!=lookup_ChildOld.LookupParent) {
        lookup_ChildNew.LookupParent = lookup_ChildOld.LookupParent;
      }
      if (lookup_ChildNew.LookupParentNullable is null) {
        if (lookup_ChildOld.LookupParentNullable is null) {
          //nothing to do
        } else {
          lookup_ChildNew.LookupParentNullable = lookup_ChildOld.LookupParentNullable;
        }
      } else {
        if (lookup_ChildOld.LookupParentNullable is null) {
          lookup_ChildNew.LookupParentNullable = null;
        } else {
          lookup_ChildNew.LookupParentNullable = lookup_ChildOld.LookupParentNullable;
        }
      }
      lookup_ChildNew.onRollbackItemUpdated(lookup_ChildOld);
    }
    partial void onRollbackItemUpdated(Lookup_Child oldLookup_Child);


    /// <summary>
    /// Adds Lookup_Child item to possible parents again as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemRemove(IStorageItem item) {
      var lookup_Child = (Lookup_Child) item;
      lookup_Child.onRollbackItemRemoved();
    }
    partial void onRollbackItemRemoved();


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {LookupParent.ToShortString()}," +
        $" {LookupParentNullable?.ToShortString()}";
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
        $" LookupParent: {LookupParent.ToShortString()}," +
        $" LookupParentNullable: {LookupParentNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
