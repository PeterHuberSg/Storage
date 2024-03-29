//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into ChildrenDictionary_Child.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// DictionaryChild has a member providing the key value needed to add DictionaryChild to  
    /// ParentWithDictionary and ParentWithDictionaryNullable
    /// </summary>
  public partial class ChildrenDictionary_Child: IStorageItemGeneric<ChildrenDictionary_Child> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for ChildrenDictionary_Child. Gets set once ChildrenDictionary_Child gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem childrenDictionary_Child, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release ChildrenDictionary_Child key @{childrenDictionary_Child.Key} #{childrenDictionary_Child.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store ChildrenDictionary_Child key @{key} #{childrenDictionary_Child.GetHashCode()}");
        }
      }
#endif
      ((ChildrenDictionary_Child)childrenDictionary_Child).Key = key;
    }


    /// <summary>
    /// Key field used in ParentWithDictionary.DictionaryChildren and 
    /// ParentWithDictionaryNullable.DictionaryChildrenDictionary
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
    public ChildrenDictionary_Parent ParentWithDictionary { get; private set; }


    /// <summary>
    /// Nullable parent
    /// </summary>
    public ChildrenDictionary_ParentNullable? ParentWithDictionaryNullable { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "DateKey", "Text", "ParentWithDictionary", "ParentWithDictionaryNullable"};


    /// <summary>
    /// None existing ChildrenDictionary_Child
    /// </summary>
    internal static ChildrenDictionary_Child NoChildrenDictionary_Child = new ChildrenDictionary_Child(DateTime.MinValue.Date, "NoText", ChildrenDictionary_Parent.NoChildrenDictionary_Parent, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of ChildrenDictionary_Child has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/ChildrenDictionary_Child, /*new*/ChildrenDictionary_Child>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// ChildrenDictionary_Child Constructor. If isStoring is true, adds ChildrenDictionary_Child to DC.Data.ChildrenDictionary_Children.
    /// </summary>
    public ChildrenDictionary_Child(
      DateTime dateKey, 
      string text, 
      ChildrenDictionary_Parent parentWithDictionary, 
      ChildrenDictionary_ParentNullable? parentWithDictionaryNullable, 
      bool isStoring = true)
    {
      Key = StorageExtensions.NoKey;
      DateKey = dateKey.Floor(Rounding.Days);
      Text = text;
      ParentWithDictionary = parentWithDictionary;
      ParentWithDictionaryNullable = parentWithDictionaryNullable;
#if DEBUG
      DC.Trace?.Invoke($"new ChildrenDictionary_Child: {ToTraceString()}");
#endif
      ParentWithDictionary.AddToChildrenDictionary_Children(this);
      if (ParentWithDictionaryNullable!=null) {
        ParentWithDictionaryNullable.AddToChildrenDictionary_Children(this);
      }
      onConstruct();
      if (DC.Data?.IsTransaction??false) {
        DC.Data.AddTransaction(new TransactionItem(24,TransactionActivityEnum.New, Key, this));
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
    public ChildrenDictionary_Child(ChildrenDictionary_Child original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      DateKey = original.DateKey;
      Text = original.Text;
      ParentWithDictionary = original.ParentWithDictionary;
      ParentWithDictionaryNullable = original.ParentWithDictionaryNullable;
      onCloned(this);
    }
    partial void onCloned(ChildrenDictionary_Child clone);


    /// <summary>
    /// Constructor for ChildrenDictionary_Child read from CSV file
    /// </summary>
    private ChildrenDictionary_Child(int key, CsvReader csvReader){
      Key = key;
      DateKey = csvReader.ReadDate();
      Text = csvReader.ReadString();
      var childrenDictionary_ParentKey = csvReader.ReadInt();
      if (DC.Data.ChildrenDictionary_Parents.TryGetValue(childrenDictionary_ParentKey, out var parentWithDictionary)) {
          ParentWithDictionary = parentWithDictionary;
      } else {
        throw new Exception($"Read ChildrenDictionary_Child from CSV file: Cannot find ParentWithDictionary with key {childrenDictionary_ParentKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      var parentWithDictionaryNullableKey = csvReader.ReadIntNull();
      if (parentWithDictionaryNullableKey.HasValue) {
        if (DC.Data.ChildrenDictionary_ParentNullables.TryGetValue(parentWithDictionaryNullableKey.Value, out var parentWithDictionaryNullable)) {
          ParentWithDictionaryNullable = parentWithDictionaryNullable;
        } else {
          ParentWithDictionaryNullable = ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable;
        }
      }
      if (ParentWithDictionary!=ChildrenDictionary_Parent.NoChildrenDictionary_Parent) {
        ParentWithDictionary.AddToChildrenDictionary_Children(this);
      }
      if (parentWithDictionaryNullableKey.HasValue && ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
        ParentWithDictionaryNullable!.AddToChildrenDictionary_Children(this);
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New ChildrenDictionary_Child read from CSV file
    /// </summary>
    internal static ChildrenDictionary_Child Create(int key, CsvReader csvReader) {
      return new ChildrenDictionary_Child(key, csvReader);
    }


    /// <summary>
    /// Verify that childrenDictionary_Child.ParentWithDictionary exists.
    /// Verify that childrenDictionary_Child.ParentWithDictionaryNullable exists.
    /// </summary>
    internal static bool Verify(ChildrenDictionary_Child childrenDictionary_Child) {
      if (childrenDictionary_Child.ParentWithDictionary==ChildrenDictionary_Parent.NoChildrenDictionary_Parent) return false;
      if (childrenDictionary_Child.ParentWithDictionaryNullable==ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds ChildrenDictionary_Child to DC.Data.ChildrenDictionary_Children.<br/>
    /// Throws an Exception when ChildrenDictionary_Child is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"ChildrenDictionary_Child cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      if (ParentWithDictionary.Key<0) {
        throw new Exception($"Cannot store child ChildrenDictionary_Child '{this}'.ParentWithDictionary to ChildrenDictionary_Parent '{ParentWithDictionary}' because parent is not stored yet.");
      }
      if (ParentWithDictionaryNullable?.Key<0) {
        throw new Exception($"Cannot store child ChildrenDictionary_Child '{this}'.ParentWithDictionaryNullable to ChildrenDictionary_ParentNullable '{ParentWithDictionaryNullable}' because parent is not stored yet.");
      }
      DC.Data.ChildrenDictionary_Children.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored ChildrenDictionary_Child #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write ChildrenDictionary_Child to CSV file
    /// </summary>
    public const int EstimatedLineLength = 161;


    /// <summary>
    /// Write ChildrenDictionary_Child to CSV file
    /// </summary>
    internal static void Write(ChildrenDictionary_Child childrenDictionary_Child, CsvWriter csvWriter) {
      childrenDictionary_Child.onCsvWrite();
      csvWriter.WriteDate(childrenDictionary_Child.DateKey);
      csvWriter.Write(childrenDictionary_Child.Text);
      if (childrenDictionary_Child.ParentWithDictionary.Key<0) throw new Exception($"Cannot write childrenDictionary_Child '{childrenDictionary_Child}' to CSV File, because ParentWithDictionary is not stored in DC.Data.ChildrenDictionary_Parents.");

      csvWriter.Write(childrenDictionary_Child.ParentWithDictionary.Key.ToString());
      if (childrenDictionary_Child.ParentWithDictionaryNullable is null) {
        csvWriter.WriteNull();
      } else {
        if (childrenDictionary_Child.ParentWithDictionaryNullable.Key<0) throw new Exception($"Cannot write childrenDictionary_Child '{childrenDictionary_Child}' to CSV File, because ParentWithDictionaryNullable is not stored in DC.Data.ChildrenDictionary_ParentNullables.");

        csvWriter.Write(childrenDictionary_Child.ParentWithDictionaryNullable.Key.ToString());
      }
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates ChildrenDictionary_Child with the provided values
    /// </summary>
    public void Update(DateTime dateKey, string text, ChildrenDictionary_Parent parentWithDictionary, ChildrenDictionary_ParentNullable? parentWithDictionaryNullable) {
      if (Key>=0){
        if (parentWithDictionary.Key<0) {
          throw new Exception($"ChildrenDictionary_Child.Update(): It is illegal to add stored ChildrenDictionary_Child '{this}'" + Environment.NewLine + 
            $"to ParentWithDictionary '{parentWithDictionary}', which is not stored.");
        }
        if (parentWithDictionaryNullable?.Key<0) {
          throw new Exception($"ChildrenDictionary_Child.Update(): It is illegal to add stored ChildrenDictionary_Child '{this}'" + Environment.NewLine + 
            $"to ParentWithDictionaryNullable '{parentWithDictionaryNullable}', which is not stored.");
        }
      }
      var clone = new ChildrenDictionary_Child(this);
      var isCancelled = false;
      onUpdating(dateKey, text, parentWithDictionary, parentWithDictionaryNullable, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating ChildrenDictionary_Child: {ToTraceString()}");
#endif
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
      if (ParentWithDictionary!=parentWithDictionary || clone.DateKey!=DateKey) {
        ParentWithDictionary.RemoveFromChildrenDictionary_Children(clone);
        ParentWithDictionary = parentWithDictionary;
        ParentWithDictionary.AddToChildrenDictionary_Children(this);
        isChangeDetected = true;
      }
      if (ParentWithDictionaryNullable is null) {
        if (parentWithDictionaryNullable is null) {
          //nothing to do
        } else {
          ParentWithDictionaryNullable = parentWithDictionaryNullable;
          ParentWithDictionaryNullable.AddToChildrenDictionary_Children(this);
          isChangeDetected = true;
        }
      } else {
        if (parentWithDictionaryNullable is null) {
          ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(clone);
          ParentWithDictionaryNullable = null;
          isChangeDetected = true;
        } else {
          if (ParentWithDictionaryNullable!=parentWithDictionaryNullable || clone.DateKey != DateKey) {
            ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(clone);
            ParentWithDictionaryNullable = parentWithDictionaryNullable;
            ParentWithDictionaryNullable.AddToChildrenDictionary_Children(this);
            isChangeDetected = true;
          }
        }
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.ChildrenDictionary_Children.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(24, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated ChildrenDictionary_Child: {ToTraceString()}");
#endif
    }
    partial void onUpdating(
      DateTime dateKey, 
      string text, 
      ChildrenDictionary_Parent parentWithDictionary, 
      ChildrenDictionary_ParentNullable? parentWithDictionaryNullable, 
      ref bool isCancelled);
    partial void onUpdated(ChildrenDictionary_Child old);


    /// <summary>
    /// Updates this ChildrenDictionary_Child with values from CSV file
    /// </summary>
    internal static void Update(ChildrenDictionary_Child childrenDictionary_Child, CsvReader csvReader){
      childrenDictionary_Child.DateKey = csvReader.ReadDate();
      childrenDictionary_Child.Text = csvReader.ReadString();
      if (!DC.Data.ChildrenDictionary_Parents.TryGetValue(csvReader.ReadInt(), out var parentWithDictionary)) {
        parentWithDictionary = ChildrenDictionary_Parent.NoChildrenDictionary_Parent;
      }
      if (childrenDictionary_Child.ParentWithDictionary!=parentWithDictionary) {
        if (childrenDictionary_Child.ParentWithDictionary!=ChildrenDictionary_Parent.NoChildrenDictionary_Parent) {
          childrenDictionary_Child.ParentWithDictionary.RemoveFromChildrenDictionary_Children(childrenDictionary_Child);
        }
        childrenDictionary_Child.ParentWithDictionary = parentWithDictionary;
        childrenDictionary_Child.ParentWithDictionary.AddToChildrenDictionary_Children(childrenDictionary_Child);
      }
      var parentWithDictionaryNullableKey = csvReader.ReadIntNull();
      ChildrenDictionary_ParentNullable? parentWithDictionaryNullable;
      if (parentWithDictionaryNullableKey is null) {
        parentWithDictionaryNullable = null;
      } else {
        if (!DC.Data.ChildrenDictionary_ParentNullables.TryGetValue(parentWithDictionaryNullableKey.Value, out parentWithDictionaryNullable)) {
          parentWithDictionaryNullable = ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable;
        }
      }
      if (childrenDictionary_Child.ParentWithDictionaryNullable is null) {
        if (parentWithDictionaryNullable is null) {
          //nothing to do
        } else {
          childrenDictionary_Child.ParentWithDictionaryNullable = parentWithDictionaryNullable;
          childrenDictionary_Child.ParentWithDictionaryNullable.AddToChildrenDictionary_Children(childrenDictionary_Child);
        }
      } else {
        if (parentWithDictionaryNullable is null) {
          if (childrenDictionary_Child.ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
            childrenDictionary_Child.ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(childrenDictionary_Child);
          }
          childrenDictionary_Child.ParentWithDictionaryNullable = null;
        } else {
          if (childrenDictionary_Child.ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
            childrenDictionary_Child.ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(childrenDictionary_Child);
          }
          childrenDictionary_Child.ParentWithDictionaryNullable = parentWithDictionaryNullable;
          childrenDictionary_Child.ParentWithDictionaryNullable.AddToChildrenDictionary_Children(childrenDictionary_Child);
        }
      }
      childrenDictionary_Child.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes ChildrenDictionary_Child from DC.Data.ChildrenDictionary_Children.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"ChildrenDictionary_Child.Release(): ChildrenDictionary_Child '{this}' is not stored in DC.Data, key is {Key}.");
      }
      onReleased();
      DC.Data.ChildrenDictionary_Children.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released ChildrenDictionary_Child @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Removes ChildrenDictionary_Child from parents as part of a transaction rollback of the new() statement.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var childrenDictionary_Child = (ChildrenDictionary_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new ChildrenDictionary_Child(): {childrenDictionary_Child.ToTraceString()}");
#endif
      if (childrenDictionary_Child.ParentWithDictionary!=ChildrenDictionary_Parent.NoChildrenDictionary_Parent) {
        childrenDictionary_Child.ParentWithDictionary.RemoveFromChildrenDictionary_Children(childrenDictionary_Child);
      }
      if (childrenDictionary_Child.ParentWithDictionaryNullable!=null && childrenDictionary_Child.ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
        childrenDictionary_Child.ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(childrenDictionary_Child);
      }
      childrenDictionary_Child.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases ChildrenDictionary_Child from DC.Data.ChildrenDictionary_Children as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var childrenDictionary_Child = (ChildrenDictionary_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenDictionary_Child.Store(): {childrenDictionary_Child.ToTraceString()}");
#endif
      childrenDictionary_Child.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the ChildrenDictionary_Child item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (ChildrenDictionary_Child) oldStorageItem;
      var newItem = (ChildrenDictionary_Child) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back ChildrenDictionary_Child.Update(): {newItem.ToTraceString()}");
#endif
      newItem.DateKey = oldItem.DateKey;
      newItem.Text = oldItem.Text;
      if (newItem.ParentWithDictionary!=oldItem.ParentWithDictionary) {
        if (newItem.ParentWithDictionary!=ChildrenDictionary_Parent.NoChildrenDictionary_Parent) {
            newItem.ParentWithDictionary.RemoveFromChildrenDictionary_Children(oldItem);
        }
        newItem.ParentWithDictionary = oldItem.ParentWithDictionary;
        newItem.ParentWithDictionary.AddToChildrenDictionary_Children(newItem);
      }
      if (newItem.ParentWithDictionaryNullable is null) {
        if (oldItem.ParentWithDictionaryNullable is null) {
          //nothing to do
        } else {
          newItem.ParentWithDictionaryNullable = oldItem.ParentWithDictionaryNullable;
          newItem.ParentWithDictionaryNullable.AddToChildrenDictionary_Children(newItem);
        }
      } else {
        if (oldItem.ParentWithDictionaryNullable is null) {
          if (newItem.ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
            newItem.ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(oldItem);
          }
          newItem.ParentWithDictionaryNullable = null;
        } else {
          if (oldItem.ParentWithDictionaryNullable!=newItem.ParentWithDictionaryNullable || oldItem.DateKey != newItem.DateKey) {
          if (newItem.ParentWithDictionaryNullable!=ChildrenDictionary_ParentNullable.NoChildrenDictionary_ParentNullable) {
            newItem.ParentWithDictionaryNullable.RemoveFromChildrenDictionary_Children(oldItem);
          }
          newItem.ParentWithDictionaryNullable = oldItem.ParentWithDictionaryNullable;
          newItem.ParentWithDictionaryNullable.AddToChildrenDictionary_Children(newItem);
          }
        }
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back ChildrenDictionary_Child.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(ChildrenDictionary_Child oldChildrenDictionary_Child);


    /// <summary>
    /// Adds ChildrenDictionary_Child to DC.Data.ChildrenDictionary_Children as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var childrenDictionary_Child = (ChildrenDictionary_Child) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback ChildrenDictionary_Child.Release(): {childrenDictionary_Child.ToTraceString()}");
#endif
      childrenDictionary_Child.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {DateKey.ToShortDateString()}|" +
        $" {Text}|" +
        $" ParentWithDictionary {ParentWithDictionary.GetKeyOrHash()}|" +
        $" ParentWithDictionaryNullable {ParentWithDictionaryNullable?.GetKeyOrHash()}";
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
        $" {DateKey.ToShortDateString()}," +
        $" {Text}," +
        $" {ParentWithDictionary.ToShortString()}," +
        $" {ParentWithDictionaryNullable?.ToShortString()}";
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
        $" DateKey: {DateKey.ToShortDateString()}," +
        $" Text: {Text}," +
        $" ParentWithDictionary: {ParentWithDictionary.ToShortString()}," +
        $" ParentWithDictionaryNullable: {ParentWithDictionaryNullable?.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
