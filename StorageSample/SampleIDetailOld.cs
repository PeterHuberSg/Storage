//using System;
//using System.Collections.Generic;
//using Storage;


//namespace StorageSample {


//  /// <summary>
//  /// Some comment for SampleDetail
//  /// </summary>
//  public partial class SampleDetail: IStorage<SampleDetail> {

//    #region Properties
//    //      ----------

//    /// <summary>
//    /// Unique identifier for SampleDetail. Gets set once SampleDetail gets added to DL.Data
//    /// </summary>
//    public int Key { get; private set; }
//    internal static void SetKey(SampleDetail sampleDetail, int key) { sampleDetail.Key = key; }


//    /// <summary>
//    /// Some Text comment
//    /// </summary>
//    public string Text { get; private set; }


//    /// <summary>
//    /// Link to parent Sample
//    /// </summary>
//    public Sample Sample { get; private set; }


//    /// <summary>
//    /// Headers written to first line in csv file
//    /// </summary>
//    public static readonly string[] Headers = { 
//      "Key", 
//      "Text",
//      "Sample"
//    };
//    #endregion


//    #region Events
//    //      ------

//    /// <summary>
//    /// Content of SampleDetail has changed. Gets only raised for changes occuring after loading DL.Data with previously stored data.
//    /// </summary>
//    public event Action<SampleDetail>? HasChanged;
//    #endregion


//    #region Constructors
//    //      ------------

//    /// <summary>
//    /// Constructor for SampleDetail. If isStoring is true, adds SampleDetail to DL.Data.sampleDetails and to Sample.SampleDetails.
//    /// </summary>
//    public SampleDetail(
//      string text,
//      Sample sample,
//      bool isStoring = true) 
//    {
//      Key = Storage.Storage.NoKey;
//      Text = text;
//      Sample = sample;
//      if (isStoring) {
//        Store();
//      }
//    }


//    /// <summary>
//    /// Constructor for SampleDetail read from csv file.
//    /// </summary>
//    private SampleDetail(int key, CsvReader csvReader, DL dLData) {
//      Key = key;
//      Text = csvReader.ReadString()!;
//      if (dLData.Samples.TryGetValue(csvReader.ReadInt(), out var sample)) {
//        Sample = sample;
//        Sample.AddToSampleDetails(this);
//      } else {
//        Sample = Sample.NoSample;
//      }
//    }


//    /// <summary>
//    /// New sampleDetails read from csv file
//    /// </summary>
//    internal static SampleDetail Create(int key, CsvReader csvReader, DL dLData) {
//      return new SampleDetail(key, csvReader, dLData);
//    }


//    /// <summary>
//    /// Verify that sampleDetail.Sample exists
//    /// </summary>
//    internal static bool Verify(SampleDetail sampleDetail) {
//      return sampleDetail.Sample!=Sample.NoSample;
//    }
//    #endregion


//    #region Methods
//    //      -------

//    /// <summary>
//    /// Adds SampleDetail to DL.Data.sampleDetails and to Sample.SampleDetails. 
//    /// </summary>
//    public void Store() {
//      if (Key>=0) {
//        throw new Exception($"SampleDetail '{this}' can not be stored in DL.Data, key is {Key} greater equal 0.");
//      }
//      DL.Data!.SampleDetails.Add(this);
//      Sample.AddToSampleDetails(this);
//    }


//    /// <summary>
//    /// Maximal number of UTF8 characters needed to write SampleDetail to csv file
//    /// </summary>
//    internal const int MaxLineLength = 40;


//    /// <summary>
//    /// Write SampleDetail to csv file
//    /// </summary>
//    internal static void Write(SampleDetail sampleDetail, CsvWriter csvWriter) {
//      csvWriter.Write(sampleDetail.Text);
//      if (sampleDetail.Sample.Key<0) throw new Exception($"Cannot write sampleDetail '{sampleDetail}' to CSV File, because Sample '{sampleDetail.Sample}' is not stored in DL.Data.MasterSamples.");

//      csvWriter.Write(sampleDetail.Sample.Key.ToString());
//    }


//    /// <summary>
//    /// Updates SampleDetail with the provided values
//    /// </summary>
//    public void Update(string text){
//      var isChangeDetected = false;
//      if (Text!=text) {
//        Text = text;
//        isChangeDetected = true;
//      }
//      if (isChangeDetected) HasChanged?.Invoke(this);
//    }


////    /// <summary>
////    /// Copies the values from sampleDetailChanged to this SampleDetail.
////    /// </summary>
////    public void Update(int key, SampleDetail sampleDetailChanged) {
////      if (key!=Key) throw new Exception();

////#if DEBUG
////      if (sampleDetailChanged.Key!=Storage.NoKey) throw new Exception();
////#endif
////      var isChangeDetected = false;
////      if (Text!=sampleDetailChanged.Text) {
////        Text = sampleDetailChanged.Text;
////        isChangeDetected = true;
////      }
////      if (isChangeDetected) HasChanged?.Invoke(this);
////    }


//    /// <summary>
//    /// Updates this SampleDetail with values from csvReader
//    /// </summary>
//    internal static void Update(SampleDetail sampleDetail, CsvReader csvReader, DL dlData) {
//      sampleDetail.Text = csvReader.ReadString()!;
//      var sampleKey = csvReader.ReadInt();
//      if (sampleDetail.Sample.Key!=sampleKey) {
//        throw new Exception();
//      }
//    }


//    /// <summary>
//    /// Removes SampleDetail and all SampleDetails from DL.Data.
//    /// </summary>
//    public void Remove() {
//      if (Key<0) {
//        throw new Exception($"SampleDetail.Remove(): SampleDetail '{this}' is not stored in DL.Data, key is {Key}.");
//      }
//      DL.Data!.SampleDetails.Remove(Key);
//    }


//    /// <summary>
//    /// Removes SampleDetail from Sample. 
//    /// </summary>
//    internal static void Disconnect(SampleDetail sampleDetail) {
//      if (sampleDetail.Sample!=Sample.NoSample) {
//        sampleDetail.Sample.RemoveFromSamples(sampleDetail);
//      }
//    }


//    /// <summary>
//    /// Returns some property values
//    /// </summary>
//    public string ToShortString() {
//      return
//        $"{Key.ToKeyString()}," +
//        $" {Text}";
//    }


//    /// <summary>
//    /// Return SampleDetail details
//    /// </summary>
//    public override string ToString() {
//      return
//        $"Key: {Key.ToKeyString()};" +
//        $" Text: {Text},";
//    }
//    #endregion
//  }
//}
