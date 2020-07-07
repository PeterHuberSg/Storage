//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenSortedList_Child.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// SortedListChild has a member providing the key value needed to add SortedListChild to  
    /// ParentWithSortedList and ParentWithSortedListNullable
    /// </summary>
  public partial class ChildrenSortedList_Child: IStorageItemGeneric<ChildrenSortedList_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenSortedList_Child. Gets set once ChildrenSortedList_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenSortedList_Child, int key) {
      ((ChildrenSortedList_Child)childrenSortedList_Child).Key = key;
    }


    /// <summary>
    /// Key field used in ParentWithSortedList.SortedListChildren SortedList
    /// Stores only dates but no times.
    /// </summary>
    public DateTime DateKey { get; private set; }


    /// <summary>
    /// Some info
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Parent
    /// </summary>
    public ChildrenSortedList_Parent ParentWithSortedList { get; private set; }


    /// <summary>
    /// Nullable Parent
    /// </summary>
    public ChildrenSortedList_ParentNullable? ParentWithSortedListNullable { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "DateKey", "Text", "ParentWithSortedList", "ParentWithSortedListNullable"};


    /// <summary>
    /// None existing ChildrenSortedList_Child
    /// </summary>
    internal static ChildrenSortedList_Child NoChildrenSortedList_Child = new ChildrenSortedList_Child(DateTime.MinValue.Date, "NoText", ChildrenSortedList_Parent.NoChildrenSortedList_Parent, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenSortedList_Child has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenSortedList_Child, /*new*/ChildrenSortedList_Child>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenSortedList_Child Constructor. If isStoring is true, adds ChildrenSortedList_Child to DC.Data.ChildrenSortedList_Children, 
    /// adds ChildrenSortedList_Child to childrenSortedList_Parent.ChildrenSortedList_Children
    /// and if there is a ParentWithSortedListNullable adds ChildrenSortedList_Child to childrenSortedList_ParentNullable.ChildrenSortedList_Children.
    /// </summary>
    public ChildrenSortedList_Child(
      DateTime dateKey, 
      string text, 
      ChildrenSortedList_Parent parentWithSortedList, 
      ChildrenSortedList_ParentNullable? parentWithSortedListNullable, 
      bool isStoring = true)
    {
      Key = StorageExtensions.NoKey;
      DateKey = dateKey.Floor(Rounding.Days);
      Text = text;
      ParentWithSortedList = parentWithSortedList;
      ParentWithSortedListNullable = parentWithSortedListNullable;
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
    public ChildrenSortedList_Child(ChildrenSortedList_Child original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      DateKey = original.DateKey;
      Text = original.Text;
      ParentWithSortedList = original.ParentWithSortedList;
      ParentWithSortedListNullable = original.ParentWithSortedListNullable;
      onCloned(this);
    }
    partial void onCloned(ChildrenSortedList_Child clone);


    /// <summary>
    /// Constructor for ChildrenSortedList_Child read from CSV file
    /// </summary>
    private ChildrenSortedList_Child(int key, CsvReader csvReader){
      Key = key;
      DateKey = csvReader.ReadDate();
      Text = csvReader.ReadString();
      var childrenSortedList_ParentKey = csvReader.ReadInt();
      if (DC.Data.ChildrenSortedList_Parents.TryGetValue(childrenSortedList_ParentKey, out var parentWithSortedList)) {
          ParentWithSortedList = parentWithSortedList;
      } else {
        throw new Exception($"Read ChildrenSortedList_Child from CSV file: Cannot find ParentWithSortedList with key {childrenSortedList_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var parentWithSortedListNullableKey = csvReader.ReadIntNull();
      if (parentWithSortedListNullableKey.HasValue) {
        if (DC.Data.ChildrenSortedList_ParentNullables.TryGetValue(parentWithSortedListNullableKey.Value, out var parentWithSortedListNullable)) {
          ParentWithSortedListNullable = parentWithSortedListNullable;
        } else {
          ParentWithSortedListNullable = ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable;
        }
      }
      if (ParentWithSortedList!=ChildrenSortedList_Parent.NoChildrenSortedList_Parent) {
        ParentWithSortedList.AddToChildrenSortedList_Children(this);
      }
      if (parentWithSortedListNullableKey.HasValue && ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
        ParentWithSortedListNullable!.AddToChildrenSortedList_Children(this);
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenSortedList_Child read from CSV file
    /// </summary>
    internal static ChildrenSortedList_Child Create(int key, CsvReader csvReader) {
      return new ChildrenSortedList_Child(key, csvReader);
    }


    /// <summary>
    /// Verify that childrenSortedList_Child.ParentWithSortedList exists.
    /// Verify that childrenSortedList_Child.ParentWithSortedListNullable exists.
    /// </summary>
    internal static bool Verify(ChildrenSortedList_Child childrenSortedList_Child) {
      if (childrenSortedList_Child.ParentWithSortedList==ChildrenSortedList_Parent.NoChildrenSortedList_Parent) return false;
      if (childrenSortedList_Child.ParentWithSortedListNullable==ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenSortedList_Child to DC.Data.ChildrenSortedList_Children, ChildrenSortedList_Parent and ChildrenSortedList_ParentNullable. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenSortedList_Child cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      if (ParentWithSortedList.Key<0) {
        throw new Exception($"ChildrenSortedList_Child cannot be stored in DC.Data, ParentWithSortedList is missing or not stored yet." + Environment.NewLine + ToString());
      }
      if (ParentWithSortedListNullable!=null && ParentWithSortedListNullable.Key<0) {
        throw new Exception($"ChildrenSortedList_Child cannot be stored in DC.Data, ParentWithSortedListNullable is not stored yet." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.ChildrenSortedList_Children.Add(this);
      ParentWithSortedList.AddToChildrenSortedList_Children(this);
      ParentWithSortedListNullable?.AddToChildrenSortedList_Children(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenSortedList_Child to CSV file
    /// </summary>
    public const int EstimatedLineLength = 161;


    /// <summary>
    /// Write ChildrenSortedList_Child to CSV file
    /// </summary>
    internal static void Write(ChildrenSortedList_Child childrenSortedList_Child, CsvWriter csvWriter) {
      childrenSortedList_Child.onCsvWrite();
      csvWriter.WriteDate(childrenSortedList_Child.DateKey);
      csvWriter.Write(childrenSortedList_Child.Text);
      if (childrenSortedList_Child.ParentWithSortedList.Key<0) throw new Exception($"Cannot write childrenSortedList_Child '{childrenSortedList_Child}' to CSV File, because ParentWithSortedList is not stored in DC.Data.ChildrenSortedList_Parents.");

      csvWriter.Write(childrenSortedList_Child.ParentWithSortedList.Key.ToString());
      if (childrenSortedList_Child.ParentWithSortedListNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (childrenSortedList_Child.ParentWithSortedListNullable.Key<0) throw new Exception($"Cannot write childrenSortedList_Child '{childrenSortedList_Child}' to CSV File, because ParentWithSortedListNullable is not stored in DC.Data.ChildrenSortedList_ParentNullables.");

        csvWriter.Write(childrenSortedList_Child.ParentWithSortedListNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenSortedList_Child with the provided values
    /// </summary>
    public void Update(DateTime dateKey, string text, ChildrenSortedList_Parent parentWithSortedList, ChildrenSortedList_ParentNullable? parentWithSortedListNullable) {
      var clone = new ChildrenSortedList_Child(this);
      var isCancelled = false;
      onUpdating(dateKey, text, parentWithSortedList, parentWithSortedListNullable, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      var dateKeyRounded = dateKey.Floor(Rounding.Days);
      if (DateKey!=dateKeyRounded) {
        DateKey = dateKeyRounded;
        isChangeDetected = true;
      }
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (ParentWithSortedList!=parentWithSortedList) {
        if (Key>=0) {
          ParentWithSortedList.RemoveFromChildrenSortedList_Children(this);
        }
        ParentWithSortedList = parentWithSortedList;
        if (Key>=0) {
          ParentWithSortedList.AddToChildrenSortedList_Children(this);
        }
        isChangeDetected = true;
      }
      if (ParentWithSortedListNullable is null) {
        if (parentWithSortedListNullable is null) {
          //nothing to do
        } else {
          ParentWithSortedListNullable = parentWithSortedListNullable;
          if (Key>=0) {
            ParentWithSortedListNullable.AddToChildrenSortedList_Children(this);
          }
          isChangeDetected = true;
        }
      } else {
        if (parentWithSortedListNullable is null) {
          if (Key>=0) {
            ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(this);
          }
          ParentWithSortedListNullable = null;
          isChangeDetected = true;
        } else {
          if (ParentWithSortedListNullable!=parentWithSortedListNullable) {
            if (Key>=0) {
              ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(this);
            }
            ParentWithSortedListNullable = parentWithSortedListNullable;
            if (Key>=0) {
              ParentWithSortedListNullable.AddToChildrenSortedList_Children(this);
            }
            isChangeDetected = true;
          }
        }
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenSortedList_Children.ItemHasChanged(clone, this);
        }
        HasChanged?.Invoke(clone, this);
      }
    }
    partial void onUpdating(
      DateTime dateKey, 
      string text, 
      ChildrenSortedList_Parent parentWithSortedList, 
      ChildrenSortedList_ParentNullable? parentWithSortedListNullable, 
      ref bool isCancelled);
    partial void onUpdated(ChildrenSortedList_Child old);


    /// <summary>
    /// Updates this ChildrenSortedList_Child with values from CSV file
    /// </summary>
    internal static void Update(ChildrenSortedList_Child childrenSortedList_Child, CsvReader csvReader){
      childrenSortedList_Child.DateKey = csvReader.ReadDate();
      childrenSortedList_Child.Text = csvReader.ReadString();
      if (!DC.Data.ChildrenSortedList_Parents.TryGetValue(csvReader.ReadInt(), out var parentWithSortedList)) {
        parentWithSortedList = ChildrenSortedList_Parent.NoChildrenSortedList_Parent;
      }
      if (childrenSortedList_Child.ParentWithSortedList!=parentWithSortedList) {
        if (childrenSortedList_Child.ParentWithSortedList!=ChildrenSortedList_Parent.NoChildrenSortedList_Parent) {
          childrenSortedList_Child.ParentWithSortedList.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
        }
        childrenSortedList_Child.ParentWithSortedList = parentWithSortedList;
        childrenSortedList_Child.ParentWithSortedList.AddToChildrenSortedList_Children(childrenSortedList_Child);
      }
      var parentWithSortedListNullableKey = csvReader.ReadIntNull();
      ChildrenSortedList_ParentNullable? parentWithSortedListNullable;
      if (parentWithSortedListNullableKey is null) {
        parentWithSortedListNullable = null;
      } else {
        if (!DC.Data.ChildrenSortedList_ParentNullables.TryGetValue(parentWithSortedListNullableKey.Value, out parentWithSortedListNullable)) {
          parentWithSortedListNullable = ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable;
        }
      }
      if (childrenSortedList_Child.ParentWithSortedListNullable is null) {
        if (parentWithSortedListNullable is null) {
          //nothing to do
        } else {
          childrenSortedList_Child.ParentWithSortedListNullable = parentWithSortedListNullable;
          childrenSortedList_Child.ParentWithSortedListNullable.AddToChildrenSortedList_Children(childrenSortedList_Child);
        }
      } else {
        if (parentWithSortedListNullable is null) {
          if (childrenSortedList_Child.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
            childrenSortedList_Child.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
          }
          childrenSortedList_Child.ParentWithSortedListNullable = null;
        } else {
          if (childrenSortedList_Child.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
            childrenSortedList_Child.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
          }
          childrenSortedList_Child.ParentWithSortedListNullable = parentWithSortedListNullable;
          childrenSortedList_Child.ParentWithSortedListNullable.AddToChildrenSortedList_Children(childrenSortedList_Child);
        }
      }
      childrenSortedList_Child.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes ChildrenSortedList_Child from DC.Data.ChildrenSortedList_Children, 
    /// disconnects ChildrenSortedList_Child from ChildrenSortedList_Parent because of ParentWithSortedList and 
    /// disconnects ChildrenSortedList_Child from ChildrenSortedList_ParentNullable because of ParentWithSortedListNullable.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"ChildrenSortedList_Child.Remove(): ChildrenSortedList_Child 'Class ChildrenSortedList_Child' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.ChildrenSortedList_Children.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects ChildrenSortedList_Child from ChildrenSortedList_Parent because of ParentWithSortedList and 
    /// disconnects ChildrenSortedList_Child from ChildrenSortedList_ParentNullable because of ParentWithSortedListNullable.
    /// </summary>
    internal static void Disconnect(ChildrenSortedList_Child childrenSortedList_Child) {
      if (childrenSortedList_Child.ParentWithSortedList!=ChildrenSortedList_Parent.NoChildrenSortedList_Parent) {
        childrenSortedList_Child.ParentWithSortedList.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
      }
      if (childrenSortedList_Child.ParentWithSortedListNullable!=null && childrenSortedList_Child.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
        childrenSortedList_Child.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
      }
    }


    /// <summary>
    /// Removes childrenSortedList_ParentNullable from ParentWithSortedListNullable
    /// </summary>
    internal void RemoveParentWithSortedListNullable(ChildrenSortedList_ParentNullable childrenSortedList_ParentNullable) {
      if (childrenSortedList_ParentNullable!=ParentWithSortedListNullable) throw new Exception();

      var clone = new ChildrenSortedList_Child(this);
      ParentWithSortedListNullable = null;
      HasChanged?.Invoke(clone, this);
    }


    /// <summary>
    /// Removes ChildrenSortedList_Child from possible parents as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenSortedList_Child = (ChildrenSortedList_Child) item;
      if (childrenSortedList_Child.ParentWithSortedList!=ChildrenSortedList_Parent.NoChildrenSortedList_Parent) {
        childrenSortedList_Child.ParentWithSortedList.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
      }
      if (childrenSortedList_Child.ParentWithSortedListNullable!=null && childrenSortedList_Child.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
        childrenSortedList_Child.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_Child);
      }
      childrenSortedList_Child.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenSortedList_Child item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldItem, IStorageItem newItem) {
      var childrenSortedList_ChildOld = (ChildrenSortedList_Child) oldItem;
      var childrenSortedList_ChildNew = (ChildrenSortedList_Child) newItem;
      childrenSortedList_ChildNew.DateKey = childrenSortedList_ChildOld.DateKey;
      childrenSortedList_ChildNew.Text = childrenSortedList_ChildOld.Text;
      if (childrenSortedList_ChildNew.ParentWithSortedList!=childrenSortedList_ChildOld.ParentWithSortedList) {
        if (childrenSortedList_ChildNew.ParentWithSortedList!=ChildrenSortedList_Parent.NoChildrenSortedList_Parent) {
          childrenSortedList_ChildNew.ParentWithSortedList.RemoveFromChildrenSortedList_Children(childrenSortedList_ChildNew);
        }
        childrenSortedList_ChildNew.ParentWithSortedList = childrenSortedList_ChildOld.ParentWithSortedList;
        childrenSortedList_ChildNew.ParentWithSortedList.AddToChildrenSortedList_Children(childrenSortedList_ChildNew);
      }
      if (childrenSortedList_ChildNew.ParentWithSortedListNullable is null) {
        if (childrenSortedList_ChildOld.ParentWithSortedListNullable is null) {
          //nothing to do
        } else {
          childrenSortedList_ChildNew.ParentWithSortedListNullable = childrenSortedList_ChildOld.ParentWithSortedListNullable;
          childrenSortedList_ChildNew.ParentWithSortedListNullable.AddToChildrenSortedList_Children(childrenSortedList_ChildNew);
        }
      } else {
        if (childrenSortedList_ChildOld.ParentWithSortedListNullable is null) {
          if (childrenSortedList_ChildNew.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
            childrenSortedList_ChildNew.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_ChildNew);
          }
          childrenSortedList_ChildNew.ParentWithSortedListNullable = null;
        } else {
          if (childrenSortedList_ChildNew.ParentWithSortedListNullable!=ChildrenSortedList_ParentNullable.NoChildrenSortedList_ParentNullable) {
            childrenSortedList_ChildNew.ParentWithSortedListNullable.RemoveFromChildrenSortedList_Children(childrenSortedList_ChildNew);
          }
          childrenSortedList_ChildNew.ParentWithSortedListNullable = childrenSortedList_ChildOld.ParentWithSortedListNullable;
          childrenSortedList_ChildNew.ParentWithSortedListNullable.AddToChildrenSortedList_Children(childrenSortedList_ChildNew);
        }
      }
      childrenSortedList_ChildNew.onRollbackItemUpdated(childrenSortedList_ChildOld);
    }
    partial void onRollbackItemUpdated(ChildrenSortedList_Child oldChildrenSortedList_Child);


    /// <summary>
    /// Adds ChildrenSortedList_Child item to possible parents again as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemRemove(IStorageItem item) {
      var childrenSortedList_Child = (ChildrenSortedList_Child) item;
      childrenSortedList_Child.ParentWithSortedList.AddToChildrenSortedList_Children(childrenSortedList_Child);
      childrenSortedList_Child.ParentWithSortedListNullable?.AddToChildrenSortedList_Children(childrenSortedList_Child);
      childrenSortedList_Child.onRollbackItemRemoved();
    }
    partial void onRollbackItemRemoved();


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {DateKey.ToShortDateString()}," +
        $" {Text}," +
        $" {ParentWithSortedList.ToShortString()}," +
        $" {ParentWithSortedListNullable?.ToShortString()}";
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
        $" DateKey: {DateKey.ToShortDateString()}," +
        $" Text: {Text}," +
        $" ParentWithSortedList: {ParentWithSortedList.ToShortString()}," +
        $" ParentWithSortedListNullable: {ParentWithSortedListNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
