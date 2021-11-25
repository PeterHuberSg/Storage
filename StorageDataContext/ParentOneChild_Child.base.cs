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


namespace StorageDataContext  {


    /// <summary>
    /// Child class with one parent property which is not nullable and one property to a different parent 
    /// which is nullable
    /// </summary>
  public partial class ParentOneChild_Child: IStorageItemGeneric<ParentOneChild_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ParentOneChild_Child. Gets set once ParentOneChild_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem parentOneChild_Child, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ParentOneChild_Child key @{parentOneChild_Child.Key} #{parentOneChild_Child.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ParentOneChild_Child key @{key} #{parentOneChild_Child.GetHashCode()}");
        }
      }
#endif
      ((ParentOneChild_Child)parentOneChild_Child).Key = key;
    }


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
    public event Action</*old*/ParentOneChild_Child, /*new*/ParentOneChild_Child>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ParentOneChild_Child Constructor. If isStoring is true, adds ParentOneChild_Child to DC.Data.ParentOneChild_Children.
    /// </summary>
    public ParentOneChild_Child(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      Parent = parent;
      ParentNullable = parentNullable;
#if DEBUG
      DC.Trace?.Invoke($"new ParentOneChild_Child: {ToTraceString()}");
#endif
      Parent.AddToChild(this);
      if (ParentNullable!=null) {
        ParentNullable.AddToChild(this);
      }
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(11,TransactionActivityEnum.New, Key, this));
      }

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Cloning constructor. It will copy all data from original except any collection (children).
    /// </summary>
    #pragma warning disable CS8618 // Children collections are uninitialized.
    public ParentOneChild_Child(ParentOneChild_Child original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      Parent = original.Parent;
      ParentNullable = original.ParentNullable;
      onCloned(this);
    }
    partial void onCloned(ParentOneChild_Child clone);


    /// <summary>
    /// Constructor for ParentOneChild_Child read from CSV file
    /// </summary>
    private ParentOneChild_Child(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      var parentOneChild_ParentKey = csvReader.ReadInt();
      if (DC.Data.ParentOneChild_Parents.TryGetValue(parentOneChild_ParentKey, out var parent)) {
          Parent = parent;
      } else {
        throw new Exception($"Read ParentOneChild_Child from CSV file: Cannot find Parent with key {parentOneChild_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var parentNullableKey = csvReader.ReadIntNull();
      if (parentNullableKey.HasValue) {
        if (DC.Data.ParentOneChild_ParentNullables.TryGetValue(parentNullableKey.Value, out var parentNullable)) {
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
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ParentOneChild_Child read from CSV file
    /// </summary>
    internal static ParentOneChild_Child Create(int key, CsvReader csvReader) {
      return new ParentOneChild_Child(key, csvReader);
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
    /// Adds ParentOneChild_Child to DC.Data.ParentOneChild_Children.<br/>
    /// Throws an Exception when ParentOneChild_Child is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ParentOneChild_Child cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      if (Parent.Key<0) {
        throw new Exception($"Cannot store child ParentOneChild_Child '{this}'.Parent to ParentOneChild_Parent '{Parent}' because parent is not stored yet.");
      }
      if (ParentNullable?.Key<0) {
        throw new Exception($"Cannot store child ParentOneChild_Child '{this}'.ParentNullable to ParentOneChild_ParentNullable '{ParentNullable}' because parent is not stored yet.");
      }
      DC.Data.ParentOneChild_Children.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ParentOneChild_Child #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ParentOneChild_Child to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


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
      if (Key>=0){
        if (parent.Key<0) {
          throw new Exception($"ParentOneChild_Child.Update(): It is illegal to add stored ParentOneChild_Child '{this}'" + Environment.NewLine + 
            $"to Parent '{parent}', which is not stored.");
        }
        if (parentNullable?.Key<0) {
          throw new Exception($"ParentOneChild_Child.Update(): It is illegal to add stored ParentOneChild_Child '{this}'" + Environment.NewLine + 
            $"to ParentNullable '{parentNullable}', which is not stored.");
        }
      }
      var clone = new ParentOneChild_Child(this);
      var isCancelled = false;
      onUpdating(text, parent, parentNullable, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ParentOneChild_Child: {ToTraceString()}");
#endif
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
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ParentOneChild_Children.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(11, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ParentOneChild_Child: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable, ref bool isCancelled);
    partial void onUpdated(ParentOneChild_Child old);


    /// <summary>
    /// Updates this ParentOneChild_Child with values from CSV file
    /// </summary>
    internal static void Update(ParentOneChild_Child parentOneChild_Child, CsvReader csvReader){
      parentOneChild_Child.Text = csvReader.ReadString();
      if (!DC.Data.ParentOneChild_Parents.TryGetValue(csvReader.ReadInt(), out var parent)) {
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
        if (!DC.Data.ParentOneChild_ParentNullables.TryGetValue(parentNullableKey.Value, out parentNullable)) {
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
    /// Removes ParentOneChild_Child from DC.Data.ParentOneChild_Children.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ParentOneChild_Child.Release(): ParentOneChild_Child '{this}' is not stored in DC.Data, key is {Key}.");
      }
      onReleased();
      DC.Data.ParentOneChild_Children.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ParentOneChild_Child @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Removes ParentOneChild_Child from parents as part of a transaction rollback of the new() statement.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var parentOneChild_Child = (ParentOneChild_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ParentOneChild_Child(): {parentOneChild_Child.ToTraceString()}");
#endif
      if (parentOneChild_Child.Parent!=ParentOneChild_Parent.NoParentOneChild_Parent) {
        parentOneChild_Child.Parent.RemoveFromChild(parentOneChild_Child);
      }
      if (parentOneChild_Child.ParentNullable!=null && parentOneChild_Child.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
        parentOneChild_Child.ParentNullable.RemoveFromChild(parentOneChild_Child);
      }
      parentOneChild_Child.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ParentOneChild_Child from DC.Data.ParentOneChild_Children as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var parentOneChild_Child = (ParentOneChild_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ParentOneChild_Child.Store(): {parentOneChild_Child.ToTraceString()}");
#endif
      parentOneChild_Child.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ParentOneChild_Child item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ParentOneChild_Child) oldStorageItem;
      var newItem = (ParentOneChild_Child) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ParentOneChild_Child.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      if (newItem.Parent!=oldItem.Parent) {
        if (newItem.Parent!=ParentOneChild_Parent.NoParentOneChild_Parent) {
            newItem.Parent.RemoveFromChild(newItem);
        }
        newItem.Parent = oldItem.Parent;
        newItem.Parent.AddToChild(newItem);
      }
      if (newItem.ParentNullable is null) {
        if (oldItem.ParentNullable is null) {
          //nothing to do
        } else {
          newItem.ParentNullable = oldItem.ParentNullable;
          newItem.ParentNullable.AddToChild(newItem);
        }
      } else {
        if (oldItem.ParentNullable is null) {
          if (newItem.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
            newItem.ParentNullable.RemoveFromChild(newItem);
          }
          newItem.ParentNullable = null;
        } else {
          if (oldItem.ParentNullable!=newItem.ParentNullable) {
          if (newItem.ParentNullable!=ParentOneChild_ParentNullable.NoParentOneChild_ParentNullable) {
            newItem.ParentNullable.RemoveFromChild(newItem);
          }
          newItem.ParentNullable = oldItem.ParentNullable;
          newItem.ParentNullable.AddToChild(newItem);
          }
        }
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ParentOneChild_Child.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ParentOneChild_Child oldParentOneChild_Child);


    /// <summary>
    /// Adds ParentOneChild_Child to DC.Data.ParentOneChild_Children as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var parentOneChild_Child = (ParentOneChild_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ParentOneChild_Child.Release(): {parentOneChild_Child.ToTraceString()}");
#endif
      parentOneChild_Child.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {Text}|" +
        $" Parent {Parent.GetKeyOrHash()}|" +
        $" ParentNullable {ParentNullable?.GetKeyOrHash()}";
      onToTraceString(ref returnString);
      return returnString;
    }
    partial void onToTraceString(ref string returnString);


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
        $"Key: {Key.ToKeyString()}," +
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
