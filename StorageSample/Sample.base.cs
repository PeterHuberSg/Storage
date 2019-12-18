using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


  /// <summary>
  /// Some comment for Sample
  /// </summary>
  /// 
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
    /// Some Flag comment
    /// </summary>
    public bool Flag { get; private set; }


    /// <summary>
    /// Some Amount comment
    /// </summary>
    public int Number { get; private set; }


    /// <summary>
    /// Amount with 2 digits after comma comment
    /// Stores decimal with 2 digits after comma.
    /// </summary>
    public decimal Amount { get; private set; }


    /// <summary>
    /// PreciseDecimal with about 20 digits precission, takes a lot of storage space
    /// Stores date and time with maximum precission.
    /// </summary>
    public decimal PreciseDecimal { get; private set; }


    /// <summary>
    /// Some SampleState comment
    /// </summary>
    public SampleStateEnum SampleState { get; private set; }


    /// <summary>
    /// Stores dates but not times
    /// Stores only dates but no times.
    /// </summary>
    public DateTime DateOnly { get; private set; }


    /// <summary>
    /// Stores times (24 hour timespan) but not date
    /// Stores less than 24 hours with second precission.
    /// </summary>
    public TimeSpan TimeOnly { get; private set; }


    /// <summary>
    /// Stores date and time precisely to a tick
    /// Stores date and time with tick precission.
    /// </summary>
    public DateTime DateAndTime { get; private set; }


    /// <summary>
    /// Some OneMaster comment
    /// </summary>
    public SampleMaster? OneMaster { get; private set; }


    /// <summary>
    /// Some OtherMaster comment
    /// </summary>
    public SampleMaster? OtherMaster { get; private set; }


    /// <summary>
    /// Some Optional comment
    /// </summary>
    public string? Optional { get; private set; }


    /// <summary>
    /// Some SampleDetails comment
    /// </summary>
    public IReadOnlyList<SampleDetail> SampleDetails => sampleDetails;
    readonly List<SampleDetail> sampleDetails;


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {
      "Key", 
      "Text", 
      "Flag", 
      "Number", 
      "Amount", 
      "PreciseDecimal", 
      "SampleState", 
      "DateOnly", 
      "TimeOnly", 
      "DateAndTime", 
      "OneMaster", 
      "OtherMaster", 
      "Optional"
    };


    /// <summary>
    /// None existing Sample
    /// </summary>
    internal static Sample NoSample = new Sample("NoText", false, int.MinValue, Decimal.MinValue, Decimal.MinValue, 0, DateTime.MinValue.Date, TimeSpan.MinValue, DateTime.MinValue, null, null, null, isStoring: false);
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
    /// Sample Constructor. If isStoring is true, adds Sample to DL.Data.SampleX, 
    /// if there is a sampleMaster adds Sample to sampleMaster.SampleX
    /// and if there is a sampleMaster adds Sample to sampleMaster.SampleX.
    /// </summary>
    public Sample(
      string text, 
      bool flag, 
      int number, 
      decimal amount, 
      decimal preciseDecimal, 
      SampleStateEnum sampleState, 
      DateTime dateOnly, 
      TimeSpan timeOnly, 
      DateTime dateAndTime, 
      SampleMaster? oneMaster, 
      SampleMaster? otherMaster, 
      string? optional, 
      bool isStoring = true)
    {
      Key = Storage.Storage.NoKey;
      Text = text;
      Flag = flag;
      Number = number;
      Amount = amount;
      PreciseDecimal = preciseDecimal;
      SampleState = sampleState;
      DateOnly = dateOnly;
      TimeOnly = timeOnly;
      DateAndTime = dateAndTime;
      OneMaster = oneMaster;
      OtherMaster = otherMaster;
      Optional = optional;
      sampleDetails = new List<SampleDetail>();
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for Sample read from CSV file
    /// </summary>
    private Sample(int key, CsvReader csvReader, DL context) {
      Key = key;
      Text = csvReader.ReadString()!;
      Flag = csvReader.ReadBool();
      Number = csvReader.ReadInt();
      Amount = csvReader.ReadDecimal();
      PreciseDecimal = csvReader.ReadDecimal();
      SampleState = (SampleStateEnum)csvReader.ReadInt();
      DateOnly = csvReader.ReadDate();
      TimeOnly = csvReader.ReadTime();
      DateAndTime = csvReader.ReadDateTime();
      var oneMasterKey = csvReader.ReadIntNull();
      if (oneMasterKey.HasValue) {
        if (context.SampleMasters.TryGetValue(oneMasterKey.Value, out var oneMaster)) {
          OneMaster = oneMaster;
          OneMaster.AddToSampleX(this);
        } else {
          OneMaster = SampleMaster.NoSampleMaster;
        }
      }
      var otherMasterKey = csvReader.ReadIntNull();
      if (otherMasterKey.HasValue) {
        if (context.SampleMasters.TryGetValue(otherMasterKey.Value, out var otherMaster)) {
          OtherMaster = otherMaster;
          OtherMaster.AddToSampleX(this);
        } else {
          OtherMaster = SampleMaster.NoSampleMaster;
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
    /// Verify that sample.OneMaster exists.
    /// Verify that sample.OtherMaster exists.
    /// </summary>
    internal static bool Verify(Sample sample) {
      if (sample.OneMaster==SampleMaster.NoSampleMaster) return false;
      if (sample.OtherMaster==SampleMaster.NoSampleMaster) return false;
      return true;
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds Sample to DL.Data.SampleX and SampleMaster.SampleX. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"Sample 'Class Sample' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      onStore();
      DL.Data!.SampleX.Add(this);
      OneMaster?.AddToSampleX(this);
      OtherMaster?.AddToSampleX(this);
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
      csvWriter.Write(sample.Flag);
      csvWriter.Write(sample.Number);
      csvWriter.WriteDecimal2(sample.Amount);
      csvWriter.Write(sample.PreciseDecimal);
      csvWriter.Write((int)sample.SampleState);
      csvWriter.WriteDate(sample.DateOnly);
      csvWriter.WriteTime(sample.TimeOnly);
      csvWriter.WriteDateTime(sample.DateAndTime);
      if (sample.OneMaster is null) {
        csvWriter.Write("");
      } else {
        if (sample.OneMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because OneMaster is not stored in DL.Data.SampleMasters.");

        csvWriter.Write(sample.OneMaster.Key.ToString());
      }
      if (sample.OtherMaster is null) {
        csvWriter.Write("");
      } else {
        if (sample.OtherMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because OtherMaster is not stored in DL.Data.SampleMasters.");

        csvWriter.Write(sample.OtherMaster.Key.ToString());
      }
      csvWriter.Write(sample.Optional);
    }


    /// <summary>
    /// Updates Sample with the provided values
    /// </summary>
    public void Update(
      string text, 
      bool flag, 
      int number, 
      decimal amount, 
      decimal preciseDecimal, 
      SampleStateEnum sampleState, 
      DateTime dateOnly, 
      TimeSpan timeOnly, 
      DateTime dateAndTime, 
      SampleMaster? oneMaster, 
      SampleMaster? otherMaster, 
      string? optional)
    {
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (Flag!=flag) {
        Flag = flag;
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
      if (PreciseDecimal!=preciseDecimal) {
        PreciseDecimal = preciseDecimal;
        isChangeDetected = true;
      }
      if (SampleState!=sampleState) {
        SampleState = sampleState;
        isChangeDetected = true;
      }
      if (DateOnly!=dateOnly) {
        DateOnly = dateOnly;
        isChangeDetected = true;
      }
      if (TimeOnly!=timeOnly) {
        TimeOnly = timeOnly;
        isChangeDetected = true;
      }
      if (DateAndTime!=dateAndTime) {
        DateAndTime = dateAndTime;
        isChangeDetected = true;
      }
      if (OneMaster is null) {
        if (oneMaster is null) {
          //nothing to do
        } else {
          OneMaster = oneMaster;
          OneMaster.AddToSampleX(this);
          isChangeDetected = true;
        }
      } else {
        if (oneMaster is null) {
          OneMaster.RemoveFromSampleX(this);
          OneMaster = null;
          isChangeDetected = true;
        } else {
          if (OneMaster!=oneMaster) {
            OneMaster.RemoveFromSampleX(this);
            OneMaster = oneMaster;
            OneMaster.AddToSampleX(this);
            isChangeDetected = true;
          }
        }
      }
      if (OtherMaster is null) {
        if (otherMaster is null) {
          //nothing to do
        } else {
          OtherMaster = otherMaster;
          OtherMaster.AddToSampleX(this);
          isChangeDetected = true;
        }
      } else {
        if (otherMaster is null) {
          OtherMaster.RemoveFromSampleX(this);
          OtherMaster = null;
          isChangeDetected = true;
        } else {
          if (OtherMaster!=otherMaster) {
            OtherMaster.RemoveFromSampleX(this);
            OtherMaster = otherMaster;
            OtherMaster.AddToSampleX(this);
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
      sample.Flag = csvReader.ReadBool();
      sample.Number = csvReader.ReadInt();
      sample.Amount = csvReader.ReadDecimal();
      sample.PreciseDecimal = csvReader.ReadDecimal();
      sample.SampleState = (SampleStateEnum)csvReader.ReadInt();
      sample.DateOnly = csvReader.ReadDate();
      sample.TimeOnly = csvReader.ReadTime();
      sample.DateAndTime = csvReader.ReadDateTime();
      var oneMasterKey = csvReader.ReadIntNull();
      SampleMaster? oneMaster;
      if (oneMasterKey is null) {
        oneMaster = null;
      } else {
        if (!context.SampleMasters.TryGetValue(oneMasterKey.Value, out oneMaster)) {
          oneMaster = SampleMaster.NoSampleMaster;
        }
      }
      if (sample.OneMaster is null) {
        if (oneMaster is null) {
          //nothing to do
        } else {
          sample.OneMaster = oneMaster;
          sample.OneMaster.AddToSampleX(sample);
        }
      } else {
        if (oneMaster is null) {
          if (sample.OneMaster!=SampleMaster.NoSampleMaster) {
            sample.OneMaster.RemoveFromSampleX(sample);
          }
          sample.OneMaster = null;
        } else {
          if (sample.OneMaster!=SampleMaster.NoSampleMaster) {
            sample.OneMaster.RemoveFromSampleX(sample);
          }
          sample.OneMaster = oneMaster;
          sample.OneMaster.AddToSampleX(sample);
        }
      }
      var otherMasterKey = csvReader.ReadIntNull();
      SampleMaster? otherMaster;
      if (otherMasterKey is null) {
        otherMaster = null;
      } else {
        if (!context.SampleMasters.TryGetValue(otherMasterKey.Value, out otherMaster)) {
          otherMaster = SampleMaster.NoSampleMaster;
        }
      }
      if (sample.OtherMaster is null) {
        if (otherMaster is null) {
          //nothing to do
        } else {
          sample.OtherMaster = otherMaster;
          sample.OtherMaster.AddToSampleX(sample);
        }
      } else {
        if (otherMaster is null) {
          if (sample.OtherMaster!=SampleMaster.NoSampleMaster) {
            sample.OtherMaster.RemoveFromSampleX(sample);
          }
          sample.OtherMaster = null;
        } else {
          if (sample.OtherMaster!=SampleMaster.NoSampleMaster) {
            sample.OtherMaster.RemoveFromSampleX(sample);
          }
          sample.OtherMaster = otherMaster;
          sample.OtherMaster.AddToSampleX(sample);
        }
      }
      sample.Optional = csvReader.ReadString()!;
    }


    /// <summary>
    /// Add sampleDetail to SampleDetails.
    /// </summary>
    internal void AddToSampleDetails(SampleDetail sampleDetail) {
      sampleDetails.Add(sampleDetail);
      OnAddedToSampleDetails(sampleDetail);
    }
    partial void OnAddedToSampleDetails(SampleDetail sampleDetail);


    /// <summary>
    /// Removes sampleDetail from SampleDetails.
    /// </summary>
    internal void RemoveFromSampleDetails(SampleDetail sampleDetail) {
#if DEBUG
      if (!sampleDetails.Remove(sampleDetail)) throw new Exception();
#else
        sampleDetails.Remove(sampleDetail));
#endif
      OnRemovedFromSampleDetails(sampleDetail);
    }
    partial void OnRemovedFromSampleDetails(SampleDetail sampleDetail);


    /// <summary>
    /// Removes Sample from DL.Data.SampleX, disconnects Sample from SampleMaster, disconnects Sample from SampleMaster and deletes all SampleDetail where Sample links to this Sample.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"Sample.Remove(): Sample 'Class Sample' is not stored in DL.Data, key is {Key}.");
      }
      onRemove();
      DL.Data!.SampleX.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects Sample from SampleMaster, disconnects Sample from SampleMaster and deletes all SampleDetail where Sample links to this Sample.
    /// </summary>
    internal static void Disconnect(Sample sample) {
      if (sample.OneMaster!=null && sample.OneMaster!=SampleMaster.NoSampleMaster) {
        sample.OneMaster.RemoveFromSampleX(sample);
      }
      if (sample.OtherMaster!=null && sample.OtherMaster!=SampleMaster.NoSampleMaster) {
        sample.OtherMaster.RemoveFromSampleX(sample);
      }
      foreach (var sampleDetail in sample.SampleDetails) {
         if (sampleDetail.Key>=0) {
           sampleDetail.Remove();
         }
      }
    }


    /// <summary>
    /// Removes sampleMaster from OneMaster
    /// </summary>
    internal void RemoveOneMaster(SampleMaster sampleMaster) {
      if (sampleMaster!=OneMaster) throw new Exception();
      OneMaster = null;
      HasChanged?.Invoke(this);
    }


    /// <summary>
    /// Removes sampleMaster from OtherMaster
    /// </summary>
    internal void RemoveOtherMaster(SampleMaster sampleMaster) {
      if (sampleMaster!=OtherMaster) throw new Exception();
      OtherMaster = null;
      HasChanged?.Invoke(this);
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {Text}," +
        $" {Flag}," +
        $" {Number}," +
        $" {Amount}," +
        $" {PreciseDecimal}," +
        $" {SampleState}," +
        $" {DateOnly.ToShortDateString()}," +
        $" {TimeOnly}," +
        $" {DateAndTime}," +
        $" {OneMaster?.ToShortString()}," +
        $" {OtherMaster?.ToShortString()}," +
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
        $" Text: {Text}," +
        $" Flag: {Flag}," +
        $" Number: {Number}," +
        $" Amount: {Amount}," +
        $" PreciseDecimal: {PreciseDecimal}," +
        $" SampleState: {SampleState}," +
        $" DateOnly: {DateOnly.ToShortDateString()}," +
        $" TimeOnly: {TimeOnly}," +
        $" DateAndTime: {DateAndTime}," +
        $" OneMaster: {OneMaster?.ToShortString()}," +
        $" OtherMaster: {OtherMaster?.ToShortString()}," +
        $" Optional: {Optional}," +
        $" SampleDetails: {SampleDetails.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
