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


namespace StorageModel  {


    /// <summary>
    /// Some comment for SampleDetail
    /// </summary>
  public partial class SampleDetail: IStorage<SampleDetail> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleDetail. Gets set once SampleDetail gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(SampleDetail sampleDetail, int key) { sampleDetail.Key = key; }


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
    public event Action<SampleDetail>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// SampleDetail Constructor. If isStoring is true, adds SampleDetail to DC.Data.SampleDetails
    /// and adds SampleDetail to sample.SampleDetails.
    /// </summary>
    public SampleDetail(string text, Sample sample, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      Text = text;
      Sample = sample;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for SampleDetail read from CSV file
    /// </summary>
    private SampleDetail(int key, CsvReader csvReader, DC context) {
      Key = key;
      Text = csvReader.ReadString();
      var sampleKey = csvReader.ReadInt();
      if (context.SampleX.TryGetValue(sampleKey, out var sample)) {
          Sample = sample;
      } else {
        throw new Exception($"Read SampleDetail from CSV file: Cannot find Sample with key {sampleKey}." + Environment.NewLine + 
          csvReader.PresentContent);
      }
      if (Sample!=Sample.NoSample) {
        Sample.AddToSampleDetails(this);
      }
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New SampleDetail read from CSV file
    /// </summary>
    internal static SampleDetail Create(int key, CsvReader csvReader, DC context) {
      return new SampleDetail(key, csvReader, context);
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
    /// Adds SampleDetail to DC.Data.SampleDetails and Sample. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleDetail cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      if (Sample.Key<0) {
        throw new Exception($"SampleDetail cannot be stored in DC.Data, Sample is missing or not stored yet." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.SampleDetails.Add(this);
      Sample.AddToSampleDetails(this);
      onStored();
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
      var isCancelled = false;
      onUpdating(text, sample, ref isCancelled);
      if (isCancelled) return;

      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (Sample!=sample) {
        if (Key>=0) {
          Sample.RemoveFromSampleDetails(this);
        }
        Sample = sample;
        if (Key>=0) {
          Sample.AddToSampleDetails(this);
        }
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdating(string text, Sample sample, ref bool isCancelled);
    partial void onUpdated();


    /// <summary>
    /// Updates this SampleDetail with values from CSV file
    /// </summary>
    internal static void Update(SampleDetail sampleDetail, CsvReader csvReader, DC context) {
      sampleDetail.Text = csvReader.ReadString();
      if (!context.SampleX.TryGetValue(csvReader.ReadInt(), out var sample)) {
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
    /// Removes SampleDetail from DC.Data.SampleDetails and 
    /// disconnects SampleDetail from Sample because of Sample.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleDetail.Remove(): SampleDetail 'Class SampleDetail' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.SampleDetails.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects SampleDetail from Sample because of Sample.
    /// </summary>
    internal static void Disconnect(SampleDetail sampleDetail) {
      if (sampleDetail.Sample!=Sample.NoSample) {
        sampleDetail.Sample.RemoveFromSampleDetails(sampleDetail);
      }
    }


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
        $"Key: {Key}," +
        $" Text: {Text}," +
        $" Sample: {Sample.ToShortString()};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
