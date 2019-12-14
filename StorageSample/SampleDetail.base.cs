using System;
using System.Collections.Generic;
using Storage;


namespace StorageSample  {


  /// <summary>
  /// Some comment for SampleDetail
  /// </summary>
  public partial class SampleDetail: IStorage<SampleDetail> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleDetail. Gets set once SampleDetail gets added to DL.Data.
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
    /// Content of SampleDetail has changed. Gets only raised for changes occuring after loading DL.Data with previously stored data.
    /// </summary>
    public event Action<SampleDetail>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// SampleDetail Constructor. If isStoring is true, adds SampleDetail to DL.Data.SampleDetails
    /// and adds SampleDetail to sample.SampleDetails.
    /// </summary>
    public SampleDetail(string text, Sample sample, bool isStoring = true) {
      Key = Storage.Storage.NoKey;
      Text = text;
      Sample = sample;
      if (isStoring) {
        Store();
      }
    }


    /// <summary>
    /// Constructor for SampleDetail read from CSV file
    /// </summary>
    private SampleDetail(int key, CsvReader csvReader, DL context) {
      Key = key;
      Text = csvReader.ReadString()!;
      if (context.Samples.TryGetValue(csvReader.ReadInt(), out var sample)) {
        Sample = sample;
        Sample.AddToSampleDetails(this);
      } else {
        Sample = Sample.NoSample;
      }
    }


    /// <summary>
    /// New SampleDetail read from CSV file
    /// </summary>
    internal static SampleDetail Create(int key, CsvReader csvReader, DL context) {
      return new SampleDetail(key, csvReader, context);
    }


    /// <summary>
    /// Verify that sampleDetail.Sample exists
    /// </summary>
    internal static bool Verify(SampleDetail sampleDetail) {
      return sampleDetail.Sample!=Sample.NoSample;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds SampleDetail to DL.Data.SampleDetails and Sample.SampleDetails. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleDetail 'Class SampleDetail' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      DL.Data!.SampleDetails.Add(this);
      Sample.AddToSampleDetails(this);
    }


    /// <summary>
    /// Maximal number of UTF8 characters needed to write SampleDetail to CSV file
    /// </summary>
    internal const int MaxLineLength = 50;


    /// <summary>
    /// Write SampleDetail to CSV file
    /// </summary>
    internal static void Write(SampleDetail sampleDetail, CsvWriter csvWriter) {
      csvWriter.Write(sampleDetail.Text);
      if (sampleDetail.Sample.Key<0) throw new Exception($"Cannot write sampleDetail '{sampleDetail}' to CSV File, because Sample is not stored in DL.Data.Samples.");

      csvWriter.Write(sampleDetail.Sample.Key.ToString());
    }


    /// <summary>
    /// Updates SampleDetail with the provided values
    /// </summary>
    public void Update(string text, Sample sample) {
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
      if (isChangeDetected) HasChanged?.Invoke(this);
    }


    /// <summary>
    /// Updates this SampleDetail with values from CSV file
    /// </summary>
    internal static void Update(SampleDetail sampleDetail, CsvReader csvReader, DL context) {
      sampleDetail.Text = csvReader.ReadString()!;
      if (!context.Samples.TryGetValue(csvReader.ReadInt(), out var sample)) {
        sample = Sample.NoSample;
      }
      if (sampleDetail.Sample!=sample) {
        if (sampleDetail.Sample!=Sample.NoSample) {
          sampleDetail.Sample.RemoveFromSampleDetails(sampleDetail);
        }
        sampleDetail.Sample = sample;
        sampleDetail.Sample.AddToSampleDetails(sampleDetail);
      }
    }


    /// <summary>
    /// Removes SampleDetail from DL.Data.SampleDetails and disconnects SampleDetail from Sample.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleDetail.Remove(): SampleDetail 'Class SampleDetail' is not stored in DL.Data, key is {Key}.");
      }
      DL.Data!.SampleDetails.Remove(Key);
    }


    /// <summary>
    /// Disconnects SampleDetail from Sample.
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
      return
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {Sample.ToShortString()}";
    }


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      return
        $"Key: {Key}," +
        $" {Text}," +
        $" {Sample.ToShortString()};";
    }
    #endregion
  }
}
