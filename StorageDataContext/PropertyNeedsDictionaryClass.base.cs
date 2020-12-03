//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into PropertyNeedsDictionaryClass.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Some comment for PropertyNeedsDictionaryClass
    /// </summary>
  public partial class PropertyNeedsDictionaryClass: IStorageItemGeneric<PropertyNeedsDictionaryClass> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for PropertyNeedsDictionaryClass. Gets set once PropertyNeedsDictionaryClass gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem propertyNeedsDictionaryClass, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release PropertyNeedsDictionaryClass key @{propertyNeedsDictionaryClass.Key} #{propertyNeedsDictionaryClass.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store PropertyNeedsDictionaryClass key @{key} #{propertyNeedsDictionaryClass.GetHashCode()}");
        }
      }
#endif
      ((PropertyNeedsDictionaryClass)propertyNeedsDictionaryClass).Key = key;
    }


    /// <summary>
    /// Used as key into dictionary PropertyNeedsDictionaryClassesByIdInt
    /// </summary>
    public int IdInt { get; private set; }


    /// <summary>
    /// Used as key into dictionary PropertyNeedsDictionaryClassesByIdString
    /// </summary>
    public string? IdString { get; private set; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Lower case version of Text
    /// </summary>
    public string TextLower { get; private set; }


    /// <summary>
    /// Some Text comment which can be null
    /// </summary>
    public string? TextNullable { get; private set; }


    /// <summary>
    /// Lower case version of TextNullable
    /// </summary>
    public string? TextNullableLower { get; private set; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string TextReadonly { get; }


    /// <summary>
    /// Lower case version of TextReadonly
    /// </summary>
    public string TextReadonlyLower { get; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {
      "Key", 
      "IdInt", 
      "IdString", 
      "Text", 
      "TextLower", 
      "TextNullable", 
      "TextNullableLower", 
      "TextReadonly", 
      "TextReadonlyLower"
    };


    /// <summary>
    /// None existing PropertyNeedsDictionaryClass
    /// </summary>
    internal static PropertyNeedsDictionaryClass NoPropertyNeedsDictionaryClass = new PropertyNeedsDictionaryClass(int.MinValue, null, "NoText", null, "NoTextReadonly", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of PropertyNeedsDictionaryClass has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/PropertyNeedsDictionaryClass, /*new*/PropertyNeedsDictionaryClass>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// PropertyNeedsDictionaryClass Constructor. If isStoring is true, adds PropertyNeedsDictionaryClass to DC.Data.PropertyNeedsDictionaryClasses.
    /// </summary>
    public PropertyNeedsDictionaryClass(
      int idInt, 
      string? idString, 
      string text, 
      string? textNullable, 
      string textReadonly, 
      bool isStoring = true)
    {
      Key = StorageExtensions.NoKey;
      IdInt = idInt;
      IdString = idString;
      Text = text;
      TextLower = Text.ToLowerInvariant();
      TextNullable = textNullable;
      TextNullableLower = TextNullable?.ToLowerInvariant();
      TextReadonly = textReadonly;
      TextReadonlyLower = TextReadonly.ToLowerInvariant();
#if DEBUG
      DC.Trace?.Invoke($"new PropertyNeedsDictionaryClass: {ToTraceString()}");
#endif
      onConstruct();
      if (DC.Data.IsTransaction) {
        DC.Data.AddTransaction(new TransactionItem(2,TransactionActivityEnum.New, Key, this));
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
    public PropertyNeedsDictionaryClass(PropertyNeedsDictionaryClass original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      IdInt = original.IdInt;
      IdString = original.IdString;
      Text = original.Text;
      TextLower = original.TextLower;
      TextNullable = original.TextNullable;
      TextNullableLower = original.TextNullableLower;
      TextReadonly = original.TextReadonly;
      TextReadonlyLower = original.TextReadonlyLower;
      onCloned(this);
    }
    partial void onCloned(PropertyNeedsDictionaryClass clone);


    /// <summary>
    /// Constructor for PropertyNeedsDictionaryClass read from CSV file
    /// </summary>
    private PropertyNeedsDictionaryClass(int key, CsvReader csvReader){
      Key = key;
      IdInt = csvReader.ReadInt();
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(IdInt, this);
      IdString = csvReader.ReadStringNull();
      if (IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(IdString, this);
      }
      Text = csvReader.ReadString();
      TextLower = Text.ToLowerInvariant();
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(TextLower, this);
      TextNullable = csvReader.ReadStringNull();
      TextNullableLower = TextNullable?.ToLowerInvariant();
      if (TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(TextNullableLower, this);
      }
      TextReadonly = csvReader.ReadString();
      TextReadonlyLower = TextReadonly.ToLowerInvariant();
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Add(TextReadonlyLower, this);
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New PropertyNeedsDictionaryClass read from CSV file
    /// </summary>
    internal static PropertyNeedsDictionaryClass Create(int key, CsvReader csvReader) {
      return new PropertyNeedsDictionaryClass(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds PropertyNeedsDictionaryClass to DC.Data.PropertyNeedsDictionaryClasses.<br/>
    /// Throws an Exception when PropertyNeedsDictionaryClass is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"PropertyNeedsDictionaryClass cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(IdInt, this);
      if (IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(IdString, this);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(TextLower, this);
      if (TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(TextNullableLower, this);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Add(TextReadonlyLower, this);
      DC.Data.PropertyNeedsDictionaryClasses.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored PropertyNeedsDictionaryClass #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write PropertyNeedsDictionaryClass to CSV file
    /// </summary>
    public const int EstimatedLineLength = 1061;


    /// <summary>
    /// Write PropertyNeedsDictionaryClass to CSV file
    /// </summary>
    internal static void Write(PropertyNeedsDictionaryClass propertyNeedsDictionaryClass, CsvWriter csvWriter) {
      propertyNeedsDictionaryClass.onCsvWrite();
      csvWriter.Write(propertyNeedsDictionaryClass.IdInt);
      csvWriter.Write(propertyNeedsDictionaryClass.IdString);
      csvWriter.Write(propertyNeedsDictionaryClass.Text);
      csvWriter.Write(propertyNeedsDictionaryClass.TextNullable);
      csvWriter.Write(propertyNeedsDictionaryClass.TextReadonly);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates PropertyNeedsDictionaryClass with the provided values
    /// </summary>
    public void Update(int idInt, string? idString, string text, string? textNullable) {
      var clone = new PropertyNeedsDictionaryClass(this);
      var isCancelled = false;
      onUpdating(idInt, idString, text, textNullable, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating PropertyNeedsDictionaryClass: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (IdInt!=idInt) {
        if (Key>=0) {
            DC.Data._PropertyNeedsDictionaryClassesByIdInt.Remove(IdInt);
        }
        IdInt = idInt;
        if (Key>=0) {
            DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(IdInt, this);
        }
        isChangeDetected = true;
      }
      if (IdString!=idString) {
        if (Key>=0) {
            if (IdString!=null) {
              DC.Data._PropertyNeedsDictionaryClassesByIdString.Remove(IdString);
            }
        }
        IdString = idString;
        if (Key>=0) {
            if (IdString!=null) {
              DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(IdString, this);
            }
        }
        isChangeDetected = true;
      }
      if (Text!=text) {
        if (Key>=0) {
            DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(TextLower);
        }
        Text = text;
        TextLower = Text.ToLowerInvariant();
        if (Key>=0) {
            DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(TextLower, this);
        }
        isChangeDetected = true;
      }
      if (TextNullable!=textNullable) {
        if (Key>=0) {
            if (TextNullableLower!=null) {
              DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(TextNullableLower);
            }
        }
        TextNullable = textNullable;
        TextNullableLower = TextNullable?.ToLowerInvariant();
        if (Key>=0) {
            if (TextNullableLower!=null) {
              DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(TextNullableLower, this);
            }
        }
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.PropertyNeedsDictionaryClasses.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(2, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated PropertyNeedsDictionaryClass: {ToTraceString()}");
#endif
    }
    partial void onUpdating(
      int idInt, 
      string? idString, 
      string text, 
      string? textNullable, 
      ref bool isCancelled);
    partial void onUpdated(PropertyNeedsDictionaryClass old);


    /// <summary>
    /// Updates this PropertyNeedsDictionaryClass with values from CSV file
    /// </summary>
    internal static void Update(PropertyNeedsDictionaryClass propertyNeedsDictionaryClass, CsvReader csvReader){
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Remove(propertyNeedsDictionaryClass.IdInt);
      propertyNeedsDictionaryClass.IdInt = csvReader.ReadInt();
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(propertyNeedsDictionaryClass.IdInt, propertyNeedsDictionaryClass);
      if (propertyNeedsDictionaryClass.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Remove(propertyNeedsDictionaryClass.IdString);
      }
      propertyNeedsDictionaryClass.IdString = csvReader.ReadStringNull();
      if (propertyNeedsDictionaryClass.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(propertyNeedsDictionaryClass.IdString, propertyNeedsDictionaryClass);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(propertyNeedsDictionaryClass.TextLower);
      propertyNeedsDictionaryClass.Text = csvReader.ReadString();
      propertyNeedsDictionaryClass.TextLower = propertyNeedsDictionaryClass.Text.ToLowerInvariant();
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(propertyNeedsDictionaryClass.TextLower, propertyNeedsDictionaryClass);
      if (propertyNeedsDictionaryClass.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(propertyNeedsDictionaryClass.TextNullableLower);
      }
      propertyNeedsDictionaryClass.TextNullable = csvReader.ReadStringNull();
      propertyNeedsDictionaryClass.TextNullableLower = propertyNeedsDictionaryClass.TextNullable?.ToLowerInvariant();
      if (propertyNeedsDictionaryClass.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(propertyNeedsDictionaryClass.TextNullableLower, propertyNeedsDictionaryClass);
      }
      var textReadonly = csvReader.ReadString();
      if (propertyNeedsDictionaryClass.TextReadonly!=textReadonly) {
        throw new Exception($"PropertyNeedsDictionaryClass.Update(): Property TextReadonly '{propertyNeedsDictionaryClass.TextReadonly}' is " +
          $"readonly, textReadonly '{textReadonly}' read from the CSV file should be the same." + Environment.NewLine + 
          propertyNeedsDictionaryClass.ToString());
      }
      propertyNeedsDictionaryClass.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes PropertyNeedsDictionaryClass from DC.Data.PropertyNeedsDictionaryClasses.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"PropertyNeedsDictionaryClass.Release(): PropertyNeedsDictionaryClass '{this}' is not stored in DC.Data, key is {Key}.");
      }
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Remove(IdInt);
      if (IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Remove(IdString);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(TextLower);
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(TextLower);
      if (TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(TextNullableLower);
      }
      if (TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(TextNullableLower);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Remove(TextReadonlyLower);
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Remove(TextReadonlyLower);
      onReleased();
      DC.Data.PropertyNeedsDictionaryClasses.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released PropertyNeedsDictionaryClass @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Undoes the new() statement as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var propertyNeedsDictionaryClass = (PropertyNeedsDictionaryClass) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new PropertyNeedsDictionaryClass(): {propertyNeedsDictionaryClass.ToTraceString()}");
#endif
      propertyNeedsDictionaryClass.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases PropertyNeedsDictionaryClass from DC.Data.PropertyNeedsDictionaryClasses as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var propertyNeedsDictionaryClass = (PropertyNeedsDictionaryClass) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback PropertyNeedsDictionaryClass.Store(): {propertyNeedsDictionaryClass.ToTraceString()}");
#endif
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Remove(propertyNeedsDictionaryClass.IdInt);
      if (propertyNeedsDictionaryClass.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Remove(propertyNeedsDictionaryClass.IdString);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(propertyNeedsDictionaryClass.TextLower);
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(propertyNeedsDictionaryClass.TextLower);
      if (propertyNeedsDictionaryClass.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(propertyNeedsDictionaryClass.TextNullableLower);
      }
      if (propertyNeedsDictionaryClass.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(propertyNeedsDictionaryClass.TextNullableLower);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Remove(propertyNeedsDictionaryClass.TextReadonlyLower);
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Remove(propertyNeedsDictionaryClass.TextReadonlyLower);
      propertyNeedsDictionaryClass.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the PropertyNeedsDictionaryClass item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (PropertyNeedsDictionaryClass) oldStorageItem;
      var newItem = (PropertyNeedsDictionaryClass) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back PropertyNeedsDictionaryClass.Update(): {newItem.ToTraceString()}");
#endif
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Remove(newItem.IdInt);
      newItem.IdInt = oldItem.IdInt;
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(newItem.IdInt, newItem);
      if (newItem.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Remove(newItem.IdString);
      }
      newItem.IdString = oldItem.IdString;
      if (newItem.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(newItem.IdString, newItem);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Remove(newItem.TextLower);
      newItem.Text = oldItem.Text;
      newItem.TextLower = newItem.Text.ToLowerInvariant();
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(newItem.TextLower, newItem);
      if (newItem.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Remove(newItem.TextNullableLower);
      }
      newItem.TextNullable = oldItem.TextNullable;
      newItem.TextNullableLower = newItem.TextNullable?.ToLowerInvariant();
      if (newItem.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(newItem.TextNullableLower, newItem);
      }
      if (newItem.TextReadonly!=oldItem.TextReadonly) {
        throw new Exception($"PropertyNeedsDictionaryClass.Update(): Property TextReadonly '{newItem.TextReadonly}' is " +
          $"readonly, TextReadonly '{oldItem.TextReadonly}' read from the CSV file should be the same." + Environment.NewLine + 
          newItem.ToString());
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back PropertyNeedsDictionaryClass.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(PropertyNeedsDictionaryClass oldPropertyNeedsDictionaryClass);


    /// <summary>
    /// Adds PropertyNeedsDictionaryClass to DC.Data.PropertyNeedsDictionaryClasses as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var propertyNeedsDictionaryClass = (PropertyNeedsDictionaryClass) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback PropertyNeedsDictionaryClass.Release(): {propertyNeedsDictionaryClass.ToTraceString()}");
#endif
      DC.Data._PropertyNeedsDictionaryClassesByIdInt.Add(propertyNeedsDictionaryClass.IdInt, propertyNeedsDictionaryClass);
      if (propertyNeedsDictionaryClass.IdString!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByIdString.Add(propertyNeedsDictionaryClass.IdString, propertyNeedsDictionaryClass);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextLower.Add(propertyNeedsDictionaryClass.TextLower, propertyNeedsDictionaryClass);
      if (propertyNeedsDictionaryClass.TextNullableLower!=null) {
        DC.Data._PropertyNeedsDictionaryClassesByTextNullableLower.Add(propertyNeedsDictionaryClass.TextNullableLower, propertyNeedsDictionaryClass);
      }
      DC.Data._PropertyNeedsDictionaryClassesByTextReadonlyLower.Add(propertyNeedsDictionaryClass.TextReadonlyLower, propertyNeedsDictionaryClass);
      propertyNeedsDictionaryClass.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {IdInt}|" +
        $" {IdString}|" +
        $" {Text}|" +
        $" {TextLower}|" +
        $" {TextNullable}|" +
        $" {TextNullableLower}|" +
        $" {TextReadonly}|" +
        $" {TextReadonlyLower}";
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
        $" {IdInt}," +
        $" {IdString}," +
        $" {Text}," +
        $" {TextLower}," +
        $" {TextNullable}," +
        $" {TextNullableLower}," +
        $" {TextReadonly}," +
        $" {TextReadonlyLower}";
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
        $" IdInt: {IdInt}," +
        $" IdString: {IdString}," +
        $" Text: {Text}," +
        $" TextLower: {TextLower}," +
        $" TextNullable: {TextNullable}," +
        $" TextNullableLower: {TextNullableLower}," +
        $" TextReadonly: {TextReadonly}," +
        $" TextReadonlyLower: {TextReadonlyLower};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
