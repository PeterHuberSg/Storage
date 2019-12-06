using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  /// <summary>
  /// Some comment for Sample
  /// </summary>
  public partial class Sample: IStorage<Sample> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for Sample. Gets set once Sample gets added to DL.Data
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(Sample sample, int key) { sample.Key = key; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Some Number comment
    /// </summary>
    public int Number { get; private set; }


    /// <summary>
    /// Some Amount comment
    /// </summary>
    public decimal Amount { get; private set; }


    /// <summary>
    /// Some Date comment
    /// </summary>
    public DateTime Date { get; private set; }


    /// <summary>
    /// Some SampleMaster comment
    /// </summary>
    public SampleMaster? SampleMaster { get; private set; }


    /// <summary>
    /// Some Optional comment
    /// </summary>
    public string? Optional { get; private set; }


    /// <summary>
    /// Some SampleDetails comment
    /// </summary>
    public IReadOnlyList<SampleDetail> SampleDetails { get { return sampleDetails; } }
    readonly List<SampleDetail> sampleDetails;


    /// <summary>
    /// Headers written to first line in csv file
    /// </summary>
    public static readonly string[] Headers = { 
      "Key", 
      "Text", 
      "Number", 
      "Amount", 
      "Date",
      "SampleMaster", 
      "Optional" 
    };


    /// <summary>
    /// None existing Sample
    /// </summary>
    internal static Sample NoSample = new Sample("NoSample", int.MinValue, decimal.MinValue, DateTime.MinValue.Date, isStoring:false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of Sample has changed. Gets only raised for changes occuring after loading DL.Data with previously stored data.
    /// </summary>
    public event Action<Sample>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Sample Constructor. If isStoring is true, adds Sample to DL.Data.Samples and if there is a sampleMaster to sampleMaster.Samples.
    /// </summary>
    public Sample(
      string text, 
      int number, 
      decimal amount, 
      DateTime date,
      SampleMaster? sampleMaster = null,
      string? optional = null,
      bool isStoring = true) 
    {
      Key = Storage.NoKey;
      Text = text;
      Number = number;
      Amount = amount;
      Date = date;
      SampleMaster = sampleMaster;
      Optional = optional;
      sampleDetails = new List<SampleDetail>();
      if (isStoring) {
        Store();
      }
    }


    /// <summary>
    /// Constructor for Sample read from csv file
    /// </summary>
    private Sample(int key, CsvReader csvReader, DLData dlData) {
      Key = key;
      Text = csvReader.ReadString()!;
      Number = csvReader.ReadInt();
      Amount = csvReader.ReadDecimal();
      Date = csvReader.ReadDate();
      var masterKey = csvReader.ReadIntNull();
      if (masterKey.HasValue) {
        if (dlData.SampleMasters.TryGetValue(masterKey.Value, out var sampleMaster)) {
          SampleMaster = sampleMaster;
          SampleMaster.AddToSamples(this);
        } else {
          SampleMaster = SampleMaster.NoSampleMaster;
        }
      }
      Optional = csvReader.ReadString();
      sampleDetails = new List<SampleDetail>();
    }


    /// <summary>
    /// New Sample read from csv file
    /// </summary>
    internal static Sample Create(int key, CsvReader csvReader, DLData dlData) {
      return new Sample(key, csvReader, dlData);
    }


    /// <summary>
    /// Verify that sampleDetail.Sample exists
    /// </summary>
    internal static bool Verify(Sample sample) {
      return sample.SampleMaster!=SampleMaster.NoSampleMaster;
    }
    #endregion


    #region Methods
    //      -------


    /// <summary>
    /// Adds Sample to DL.Data.Samples and SampleMaster.Samples. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"Sample '{this}' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      DL.Data!.Samples.Add(this);
      if (SampleMaster!=null) {
        SampleMaster.AddToSamples(this);
      }

    }


    /// <summary>
    /// Maximal number of UTF8 characters needed to write Sample to csv file
    /// </summary>
    internal const int MaxLineLength = 150;


    /// <summary>
    /// Write Sample to csv file
    /// </summary>
    internal static void Write(Sample sample, CsvWriter csvWriter) {
      csvWriter.Write(sample.Text);
      csvWriter.Write(sample.Number);
      csvWriter.Write(sample.Amount);
      csvWriter.WriteDate(sample.Date);
      if (sample.SampleMaster is null) {
        csvWriter.Write("");
      } else {
        if (sample.SampleMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because MasterSample is not stored in DL.Data.MasterSamples.");
 
        csvWriter.Write(sample.SampleMaster.Key.ToString());
      }
      csvWriter.Write(sample.Optional);
    }


    /// <summary>
    /// Updates Sample with the provided values
    /// </summary>
    public void Update(
      string text, 
      int number, 
      decimal amount, 
      DateTime date,
      SampleMaster? sampleMaster,
      string? optional) 
    {
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (Number!=number) {
        Number = number;
        isChangeDetected = true;
      }
      if (Amount!=amount) {
        Amount = amount;
        isChangeDetected = true;
      }
      if (Date!=date) {
        Date = date;
        isChangeDetected = true;
      }
      if (SampleMaster is null) {
        if (sampleMaster is null) {
          //nothing to do
        } else {
          SampleMaster = sampleMaster;
          SampleMaster.AddToSamples(this);
          isChangeDetected = true;
        }
      } else {
        if (sampleMaster is null) {
          SampleMaster.RemoveFromSamples(this);
          SampleMaster = null;
          isChangeDetected = true;
        } else {
          if (SampleMaster!=sampleMaster) {
            SampleMaster.RemoveFromSamples(this);
            SampleMaster = sampleMaster;
            SampleMaster.AddToSamples(this);
            isChangeDetected = true;
          }
        }
      }
      if (Optional!=optional) {
        Optional = optional;
        isChangeDetected = true;
      }
      if (isChangeDetected) HasChanged?.Invoke(this);
    }


//    /// <summary>
//    /// Copies the values from sampleChanged to this Sample.
//    /// </summary>
//    public void Update(int key, Sample sampleChanged) {
//      if (key!=Key) throw new Exception();

//#if DEBUG
//      if (sampleChanged.Key!=Storage.NoKey) throw new Exception();
//#endif

//      Update(
//        sampleChanged.Text,
//        sampleChanged.Number,
//        sampleChanged.Amount,
//        sampleChanged.Date,
//        sampleChanged.SampleMaster,
//        sampleChanged.Optional);
//    }


    /// <summary>
    /// Updates this Sample with values from csvReader
    /// </summary>
    internal static void Update(Sample sample, CsvReader csvReader, DLData dlData) {
      sample.Text = csvReader.ReadString()!;
      sample.Number = csvReader.ReadInt();
      sample.Amount = csvReader.ReadDecimal();
      sample.Date = csvReader.ReadDate();
      var sampleMasterKey = csvReader.ReadIntNull();
      //SampleMaster? sampleMaster = sampleMasterKey.HasValue ? dlData.SampleMasters[sampleMasterKey.Value] : null;
      SampleMaster? sampleMaster;
      if (sampleMasterKey is null) {
        sampleMaster = null;
      } else {
        if (!dlData.SampleMasters.TryGetValue(sampleMasterKey.Value, out sampleMaster)) {
          sampleMaster = SampleMaster.NoSampleMaster;
        }
      }
      if (sample.SampleMaster is null) {
        if (sampleMaster is null) {
          //nothing to do
        } else {
          sample.SampleMaster = sampleMaster;
          sample.SampleMaster.AddToSamples(sample);
        }
      } else {
        if (sampleMaster is null) {
          if (sample.SampleMaster!=SampleMaster.NoSampleMaster) {
            sample.SampleMaster.RemoveFromSamples(sample);
          }
          sample.SampleMaster = null;
        } else {
          if (sample.SampleMaster!=SampleMaster.NoSampleMaster) {
            sample.SampleMaster.RemoveFromSamples(sample);
          }
          sample.SampleMaster = sampleMaster;
          sample.SampleMaster.AddToSamples(sample);
        }
      }
      sample.Optional = csvReader.ReadString()!;
    }


    /// <summary>
    /// Add sampleDetail to SampleDetails.
    /// </summary>
    internal void AddToSampleDetails(SampleDetail sampleDetail) {
      sampleDetails.Add(sampleDetail);
    }


    /// <summary>
    /// Removes sampleDetail from SampleDetails.
    /// </summary>
    internal void RemoveFromSamples(SampleDetail sampleDetail) {
#if DEBUG
      if (!sampleDetails.Remove(sampleDetail)) throw new Exception();
#else
        sampleDetails.Remove(sampleDetail));
#endif
    }


    /// <summary>
    /// Removes Sample from DL.Data.Samples, deletes all SampleDetails and disconnects Sample from SampleMaster.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"Sample.Remove(): Sample '{this}' is not stored in DL.Data, key is {Key}.");
      }
      DL.Data!.Samples.Remove(Key);
    }


    /// <summary>
    /// Removes Sample from SampleMaster. Removes all SampleDetails.
    /// </summary>
    internal static void Disconnect(Sample sample) {
      foreach (var sampleDetail in sample.SampleDetails) {
        sampleDetail.Remove();
      }

      if (sample.SampleMaster!=null && sample.SampleMaster!=SampleMaster.NoSampleMaster) {
        sample.SampleMaster?.RemoveFromSamples(sample);
      }
    }


    /// <summary>
    /// Removes sampleMaster from SampleMaster
    /// </summary>
    internal void RemoveSampleMaster(SampleMaster sampleMaster) {
      if (sampleMaster!=SampleMaster) throw new Exception();
      SampleMaster = null;
      HasChanged?.Invoke(this);
    }


    /// <summary>
    /// Returns some property values
    /// </summary>
    public string ToShortString() {
      return
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {Number}," +
        $" {Amount}," +
        $" {Date.ToShortDateString()}," +
        $" {SampleMaster?.ToShortString()}," +
        $" {Optional}";
    }


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      return
        $"Key: {Key.ToKeyString()};" +
        $" Text: {Text};" +
        $" Number: {Number};" +
        $" Amount: {Amount};" +
        $" Date: {Date.ToShortDateString()};" +
        $" SampleMaster: {SampleMaster?.ToShortString()};" +
        $" Optional: {Optional};" +
        $" SampleDetails: {SampleDetails.Count};";
    }
    #endregion
  }
}
