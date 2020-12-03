//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into SampleDetail.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Some comment for SampleDetail
    /// </summary>
  public partial class SampleDetail: IStorageItemGeneric<SampleDetail> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleDetail. Gets set once SampleDetail gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem sampleDetail, int key, bool isRollback) {
#if DEBUG
      if (isRollback) {
        if (key==StorageExtensions.NoKey) {
          DC.Trace?.Invoke($"Release SampleDetail key @{sampleDetail.Key} #{sampleDetail.GetHashCode()}");
        } else {
          DC.Trace?.Invoke($"Store SampleDetail key @{key} #{sampleDetail.GetHashCode()}");
        }
      }
#endif
      ((SampleDetail)sampleDetail).Key = key;
    }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Link to parent Sample
    /// </summary>
    public Sample Sample { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text", "Sample"};


    /// <summary>
    /// None existing SampleDetail
    /// </summary>
    internal static SampleDetail NoSampleDetail = new SampleDetail("NoText", Sample.NoSample, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of SampleDetail has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/SampleDetail, /*new*/SampleDetail>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// SampleDetail Constructor. If isStoring is true, adds SampleDetail to DC.Data.SampleDetails.
    /// </summary>
    public SampleDetail(string text, Sample sample, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      Sample = sample;
#if DEBUG
      DC.Trace?.Invoke($"new SampleDetail: {ToTraceString()}");
#endif
      Sample.AddToSampleDetails(this);
      onConstruct();
      if (DC.Data.IsTransaction) {
        DC.Data.AddTransaction(new TransactionItem(8,TransactionActivityEnum.New, Key, this));
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
    public SampleDetail(SampleDetail original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      Sample = original.Sample;
      onCloned(this);
    }
    partial void onCloned(SampleDetail clone);


    /// <summary>
    /// Constructor for SampleDetail read from CSV file
    /// </summary>
    private SampleDetail(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      var sampleKey = csvReader.ReadInt();
      if (DC.Data.SampleX.TryGetValue(sampleKey, out var sample)) {
          Sample = sample;
      } else {
        throw new Exception($"Read SampleDetail from CSV file: Cannot find Sample with key {sampleKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      if (Sample!=Sample.NoSample) {
        Sample.AddToSampleDetails(this);
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New SampleDetail read from CSV file
    /// </summary>
    internal static SampleDetail Create(int key, CsvReader csvReader) {
      return new SampleDetail(key, csvReader);
    }


    /// <summary>
    /// Verify that sampleDetail.Sample exists.
    /// </summary>
    internal static bool Verify(SampleDetail sampleDetail) {
      if (sampleDetail.Sample==Sample.NoSample) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds SampleDetail to DC.Data.SampleDetails.<br/>
    /// Throws an Exception when SampleDetail is already stored.
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleDetail cannot be stored again in DC.Data, key {Key} is greater equal 0." + Environment.NewLine + ToString());
      }

      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      if (Sample.Key<0) {
        throw new Exception($"Cannot store child SampleDetail '{this}'.Sample to Sample '{Sample}' because parent is not stored yet.");
      }
      DC.Data.SampleDetails.Add(this);
      onStored();
#if DEBUG
      DC.Trace?.Invoke($"Stored SampleDetail #{GetHashCode()} @{Key}");
#endif
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write SampleDetail to CSV file
    /// </summary>
    public const int EstimatedLineLength = 150;


    /// <summary>
    /// Write SampleDetail to CSV file
    /// </summary>
    internal static void Write(SampleDetail sampleDetail, CsvWriter csvWriter) {
      sampleDetail.onCsvWrite();
      csvWriter.Write(sampleDetail.Text);
      if (sampleDetail.Sample.Key<0) throw new Exception($"Cannot write sampleDetail '{sampleDetail}' to CSV File, because Sample is not stored in DC.Data.SampleX.");

      csvWriter.Write(sampleDetail.Sample.Key.ToString());
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates SampleDetail with the provided values
    /// </summary>
    public void Update(string text, Sample sample) {
      if (Key>=0){
        if (sample.Key<0) {
          throw new Exception($"SampleDetail.Update(): It is illegal to add stored SampleDetail '{this}'" + Environment.NewLine + 
            $"to Sample '{sample}', which is not stored.");
        }
      }
      var clone = new SampleDetail(this);
      var isCancelled = false;
      onUpdating(text, sample, ref isCancelled);
      if (isCancelled) return;

#if DEBUG
      DC.Trace?.Invoke($"Updating SampleDetail: {ToTraceString()}");
#endif
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (Sample!=sample) {
        Sample.RemoveFromSampleDetails(this);
        Sample = sample;
        Sample.AddToSampleDetails(this);
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.SampleDetails.ItemHasChanged(clone, this);
        } else if (DC.Data.IsTransaction) {
          DC.Data.AddTransaction(new TransactionItem(8, TransactionActivityEnum.Update, Key, this, oldItem: clone));
        }
        HasChanged?.Invoke(clone, this);
      }
#if DEBUG
      DC.Trace?.Invoke($"Updated SampleDetail: {ToTraceString()}");
#endif
    }
    partial void onUpdating(string text, Sample sample, ref bool isCancelled);
    partial void onUpdated(SampleDetail old);


    /// <summary>
    /// Updates this SampleDetail with values from CSV file
    /// </summary>
    internal static void Update(SampleDetail sampleDetail, CsvReader csvReader){
      sampleDetail.Text = csvReader.ReadString();
      if (!DC.Data.SampleX.TryGetValue(csvReader.ReadInt(), out var sample)) {
        sample = Sample.NoSample;
      }
      if (sampleDetail.Sample!=sample) {
        if (sampleDetail.Sample!=Sample.NoSample) {
          sampleDetail.Sample.RemoveFromSampleDetails(sampleDetail);
        }
        sampleDetail.Sample = sample;
        sampleDetail.Sample.AddToSampleDetails(sampleDetail);
      }
      sampleDetail.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes SampleDetail from DC.Data.SampleDetails.
    /// </summary>
    public void Release() {
      if (Key<0) {
        throw new Exception($"SampleDetail.Release(): SampleDetail '{this}' is not stored in DC.Data, key is {Key}.");
      }
      onReleased();
      DC.Data.SampleDetails.Remove(Key);
#if DEBUG
      DC.Trace?.Invoke($"Released SampleDetail @{Key} #{GetHashCode()}");
#endif
    }
    partial void onReleased();


    /// <summary>
    /// Removes SampleDetail from parents as part of a transaction rollback of the new() statement.
    /// </summary>
    internal static void RollbackItemNew(IStorageItem item) {
      var sampleDetail = (SampleDetail) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback new SampleDetail(): {sampleDetail.ToTraceString()}");
#endif
      if (sampleDetail.Sample!=Sample.NoSample) {
        sampleDetail.Sample.RemoveFromSampleDetails(sampleDetail);
      }
      sampleDetail.onRollbackItemNew();
    }
    partial void onRollbackItemNew();


    /// <summary>
    /// Releases SampleDetail from DC.Data.SampleDetails as part of a transaction rollback of Store().
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var sampleDetail = (SampleDetail) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback SampleDetail.Store(): {sampleDetail.ToTraceString()}");
#endif
      sampleDetail.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the SampleDetail item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldStorageItem, IStorageItem newStorageItem) {
      var oldItem = (SampleDetail) oldStorageItem;
      var newItem = (SampleDetail) newStorageItem;
#if DEBUG
      DC.Trace?.Invoke($"Rolling back SampleDetail.Update(): {newItem.ToTraceString()}");
#endif
      newItem.Text = oldItem.Text;
      if (newItem.Sample!=oldItem.Sample) {
        if (newItem.Sample!=Sample.NoSample) {
            newItem.Sample.RemoveFromSampleDetails(newItem);
        }
        newItem.Sample = oldItem.Sample;
        newItem.Sample.AddToSampleDetails(newItem);
      }
      newItem.onRollbackItemUpdated(oldItem);
#if DEBUG
      DC.Trace?.Invoke($"Rolled back SampleDetail.Update(): {newItem.ToTraceString()}");
#endif
    }
    partial void onRollbackItemUpdated(SampleDetail oldSampleDetail);


    /// <summary>
    /// Adds SampleDetail to DC.Data.SampleDetails as part of a transaction rollback of Release().
    /// </summary>
    internal static void RollbackItemRelease(IStorageItem item) {
      var sampleDetail = (SampleDetail) item;
#if DEBUG
      DC.Trace?.Invoke($"Rollback SampleDetail.Release(): {sampleDetail.ToTraceString()}");
#endif
      sampleDetail.onRollbackItemRelease();
    }
    partial void onRollbackItemRelease();


    /// <summary>
    /// Returns property values for tracing. Parents are shown with their key instead their content.
    /// </summary>
    public string ToTraceString() {
      var returnString =
        $"{this.GetKeyOrHash()}|" +
        $" {Text}|" +
        $" Sample {Sample.GetKeyOrHash()}";
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
        $" {Sample.ToShortString()}";
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
        $" Sample: {Sample.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
