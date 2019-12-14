using System;
using System.Collections.Generic;
using Storage;


namespace StorageSample  {


  /// <summary>
  /// Some comment for Sample
  /// </summary>
  public partial class Sample: IStorage<Sample> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for Sample. Gets set once Sample gets added to DL.Data.
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
    /// Stores date and time with maximum precission.
    /// </summary>
    public decimal Amount { get; private set; }


    /// <summary>
    /// Some Date comment
    /// Stores date and time with tick precission.
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
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {
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
    internal static Sample NoSample = new Sample("NoText", int.MinValue, Decimal.MinValue, DateTime.MinValue, null, null, isStoring: false);
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
    /// Sample Constructor. If isStoring is true, adds Sample to DL.Data.Samples
    /// and if there is a sampleMaster adds Sample to sampleMaster.Samples.
    /// </summary>
    public Sample(
      string text, 
      int number, 
      decimal amount, 
      DateTime date, 
      SampleMaster? sampleMaster, 
      string? optional, 
      bool isStoring = true)
    {
      Key = Storage.Storage.NoKey;
      Text = text;
      Number = number;
      Amount = amount;
      Date = date;
      SampleMaster = sampleMaster;
      Optional = optional;
      sampleDetails = new List<SampleDetail>();
      onCreate();

      if (isStoring) {
        Store();
      }
    }
    partial void onCreate();


    /// <summary>
    /// Constructor for Sample read from CSV file
    /// </summary>
    private Sample(int key, CsvReader csvReader, DL context) {
      Key = key;
      Text = csvReader.ReadString()!;
      Number = csvReader.ReadInt();
      Amount = csvReader.ReadDecimal();
      Date = csvReader.ReadDateTime();
      var sampleMasterKey = csvReader.ReadIntNull();
      if (sampleMasterKey.HasValue) {
        if (context.SampleMasters.TryGetValue(sampleMasterKey.Value, out var sampleMaster)) {
          SampleMaster = sampleMaster;
          SampleMaster.AddToSamples(this);
        } else {
          SampleMaster = SampleMaster.NoSampleMaster;
        }
      }
      Optional = csvReader.ReadString()!;
      sampleDetails = new List<SampleDetail>();
    }


    /// <summary>
    /// New Sample read from CSV file
    /// </summary>
    internal static Sample Create(int key, CsvReader csvReader, DL context) {
      return new Sample(key, csvReader, context);
    }


    /// <summary>
    /// Verify that sample.SampleMaster exists
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
        throw new Exception($"Sample 'Class Sample' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      onStore();
      DL.Data!.Samples.Add(this);
      if (SampleMaster!=null) {
        SampleMaster.AddToSamples(this);
      }
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write Sample to CSV file
    /// </summary>
    internal const int MaxLineLength = 200;


    /// <summary>
    /// Write Sample to CSV file
    /// </summary>
    internal static void Write(Sample sample, CsvWriter csvWriter) {
      csvWriter.Write(sample.Text);
      csvWriter.Write(sample.Number);
      csvWriter.Write(sample.Amount);
      csvWriter.WriteDateTime(sample.Date);
      if (sample.SampleMaster is null) {
        csvWriter.Write("");
      } else {
        if (sample.SampleMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because SampleMaster is not stored in DL.Data.SampleMasters.");

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
      if (isChangeDetected) {
        onUpdate();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdate();


    /// <summary>
    /// Updates this Sample with values from CSV file
    /// </summary>
    internal static void Update(Sample sample, CsvReader csvReader, DL context) {
      sample.Text = csvReader.ReadString()!;
      sample.Number = csvReader.ReadInt();
      sample.Amount = csvReader.ReadDecimal();
      sample.Date = csvReader.ReadDateTime();
      var sampleMasterKey = csvReader.ReadIntNull();
      SampleMaster? sampleMaster;
      if (sampleMasterKey is null) {
        sampleMaster = null;
      } else {
        if (!context.SampleMasters.TryGetValue(sampleMasterKey.Value, out sampleMaster)) {
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
    internal void RemoveFromSampleDetails(SampleDetail sampleDetail) {
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
        throw new Exception($"Sample.Remove(): Sample 'Class Sample' is not stored in DL.Data, key is {Key}.");
      }
      onRemove();
      DL.Data!.Samples.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Deletes all SampleDetails and disconnects Sample from SampleMaster.
    /// </summary>
    internal static void Disconnect(Sample sample) {
      foreach (var sampleDetail in sample.SampleDetails) {
         if (sampleDetail.Key>=0) {
           sampleDetail.Remove();
         }
      }
      if (sample.SampleMaster!=null && sample.SampleMaster!=SampleMaster.NoSampleMaster) {
        sample.SampleMaster.RemoveFromSamples(sample);
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
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {Number}," +
        $" {Amount}," +
        $" {Date}," +
        $" {SampleMaster?.ToShortString()}," +
        $" {Optional}";
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
        $" {Text}," +
        $" {Number}," +
        $" {Amount}," +
        $" {Date}," +
        $" {SampleMaster?.ToShortString()}," +
        $" {Optional}," +
        $" SampleDetails: {SampleDetails.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
