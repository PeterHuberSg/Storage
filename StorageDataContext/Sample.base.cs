//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into Sample.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Some comment for Sample
    /// </summary>
  public partial class Sample: IStorageItemGeneric<Sample> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for Sample. Gets set once Sample gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(IStorageItem sample, int key) {
      ((Sample)sample).Key = key;
    }


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
    /// Amount with 4 digits after comma comment
    /// Stores decimal with 4 digits after comma.
    /// </summary>
    public decimal Amount4 { get; private set; }


    /// <summary>
    /// Nullable amount with 5 digits after comma comment
    /// Stores decimal with 5 digits after comma.
    /// </summary>
    public decimal? Amount5 { get; private set; }


    /// <summary>
    /// PreciseDecimal with about 20 digits precision, takes a lot of storage space
    /// Stores date and time with maximum precision.
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
    /// Stores less than 24 hours with second precision.
    /// </summary>
    public TimeSpan TimeOnly { get; private set; }


    /// <summary>
    /// Stores date and time precisely to a tick
    /// Stores date and time with tick precision.
    /// </summary>
    public DateTime DateTimeTicks { get; private set; }


    /// <summary>
    /// Stores date and time precisely to a minute
    /// Stores date and time with minute preclusion.
    /// </summary>
    public DateTime DateTimeMinute { get; private set; }


    /// <summary>
    /// Stores date and time precisely to a second
    /// Stores date and time with seconds precision.
    /// </summary>
    public DateTime DateTimeSecond { get; private set; }


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
      "Amount4", 
      "Amount5", 
      "PreciseDecimal", 
      "SampleState", 
      "DateOnly", 
      "TimeOnly", 
      "DateTimeTicks", 
      "DateTimeMinute", 
      "DateTimeSecond", 
      "OneMaster", 
      "OtherMaster", 
      "Optional"
    };


    /// <summary>
    /// None existing Sample
    /// </summary>
    internal static Sample NoSample = new Sample("NoText", false, int.MinValue, Decimal.MinValue, Decimal.MinValue, null, Decimal.MinValue, 0, DateTime.MinValue.Date, TimeSpan.MinValue, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, null, null, null, isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of Sample has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action</*old*/Sample, /*new*/Sample>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Sample Constructor. If isStoring is true, adds Sample to DC.Data.SampleX, 
    /// if there is a OneMaster adds Sample to sampleMaster.SampleX
    /// and if there is a OtherMaster adds Sample to sampleMaster.SampleX.
    /// </summary>
    public Sample(
      string text, 
      bool flag, 
      int number, 
      decimal amount, 
      decimal amount4, 
      decimal? amount5, 
      decimal preciseDecimal, 
      SampleStateEnum sampleState, 
      DateTime dateOnly, 
      TimeSpan timeOnly, 
      DateTime dateTimeTicks, 
      DateTime dateTimeMinute, 
      DateTime dateTimeSecond, 
      SampleMaster? oneMaster, 
      SampleMaster? otherMaster, 
      string? optional, 
      bool isStoring = true)
    {
      Key = StorageExtensions.NoKey;
      Text = text;
      Flag = flag;
      Number = number;
      Amount = amount.Round(2);
      Amount4 = amount4.Round(4);
      Amount5 = amount5.Round(5);
      PreciseDecimal = preciseDecimal;
      SampleState = sampleState;
      DateOnly = dateOnly.Floor(Rounding.Days);
      TimeOnly = timeOnly.Round(Rounding.Seconds);
      DateTimeTicks = dateTimeTicks;
      DateTimeMinute = dateTimeMinute.Round(Rounding.Minutes);
      DateTimeSecond = dateTimeSecond.Round(Rounding.Seconds);
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
    /// Cloning constructor. It will copy all data from original except any collection (children).
    /// </summary>
    #pragma warning disable CS8618 // Children collections are uninitialized.
    public Sample(Sample original) {
    #pragma warning restore CS8618 //
      Key = StorageExtensions.NoKey;
      Text = original.Text;
      Flag = original.Flag;
      Number = original.Number;
      Amount = original.Amount;
      Amount4 = original.Amount4;
      Amount5 = original.Amount5;
      PreciseDecimal = original.PreciseDecimal;
      SampleState = original.SampleState;
      DateOnly = original.DateOnly;
      TimeOnly = original.TimeOnly;
      DateTimeTicks = original.DateTimeTicks;
      DateTimeMinute = original.DateTimeMinute;
      DateTimeSecond = original.DateTimeSecond;
      OneMaster = original.OneMaster;
      OtherMaster = original.OtherMaster;
      Optional = original.Optional;
      onCloned(this);
    }
    partial void onCloned(Sample clone);


    /// <summary>
    /// Constructor for Sample read from CSV file
    /// </summary>
    private Sample(int key, CsvReader csvReader){
      Key = key;
      Text = csvReader.ReadString();
      Flag = csvReader.ReadBool();
      Number = csvReader.ReadInt();
      Amount = csvReader.ReadDecimal();
      Amount4 = csvReader.ReadDecimal();
      Amount5 = csvReader.ReadDecimalNull();
      PreciseDecimal = csvReader.ReadDecimal();
      SampleState = (SampleStateEnum)csvReader.ReadInt();
      DateOnly = csvReader.ReadDate();
      TimeOnly = csvReader.ReadTime();
      DateTimeTicks = csvReader.ReadDateTimeTicks();
      DateTimeMinute = csvReader.ReadDateSeconds();
      DateTimeSecond = csvReader.ReadDateSeconds();
      var oneMasterKey = csvReader.ReadIntNull();
      if (oneMasterKey.HasValue) {
        if (DC.Data.SampleMasters.TryGetValue(oneMasterKey.Value, out var oneMaster)) {
          OneMaster = oneMaster;
        } else {
          OneMaster = SampleMaster.NoSampleMaster;
        }
      }
      var otherMasterKey = csvReader.ReadIntNull();
      if (otherMasterKey.HasValue) {
        if (DC.Data.SampleMasters.TryGetValue(otherMasterKey.Value, out var otherMaster)) {
          OtherMaster = otherMaster;
        } else {
          OtherMaster = SampleMaster.NoSampleMaster;
        }
      }
      Optional = csvReader.ReadStringNull();
      sampleDetails = new List<SampleDetail>();
      if (oneMasterKey.HasValue && OneMaster!=SampleMaster.NoSampleMaster) {
        OneMaster!.AddToSampleX(this);
      }
      if (otherMasterKey.HasValue && OtherMaster!=SampleMaster.NoSampleMaster) {
        OtherMaster!.AddToSampleX(this);
      }
      onCsvConstruct();
    }
    partial void onCsvConstruct();


    /// <summary>
    /// New Sample read from CSV file
    /// </summary>
    internal static Sample Create(int key, CsvReader csvReader) {
      return new Sample(key, csvReader);
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
    /// Adds Sample to DC.Data.SampleX and SampleMaster. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"Sample cannot be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      if (OneMaster!=null && OneMaster.Key<0) {
        throw new Exception($"Sample cannot be stored in DC.Data, OneMaster is not stored yet." + Environment.NewLine + ToString());
      }
      if (OtherMaster!=null && OtherMaster.Key<0) {
        throw new Exception($"Sample cannot be stored in DC.Data, OtherMaster is not stored yet." + Environment.NewLine + ToString());
      }
      var isCancelled = false;
      onStoring(ref isCancelled);
      if (isCancelled) return;

      DC.Data.SampleX.Add(this);
      OneMaster?.AddToSampleX(this);
      OtherMaster?.AddToSampleX(this);
      onStored();
    }
    partial void onStoring(ref bool isCancelled);
    partial void onStored();


    /// <summary>
    /// Estimated number of UTF8 characters needed to write Sample to CSV file
    /// </summary>
    public const int EstimatedLineLength = 457;


    /// <summary>
    /// Write Sample to CSV file
    /// </summary>
    internal static void Write(Sample sample, CsvWriter csvWriter) {
      sample.onCsvWrite();
      csvWriter.Write(sample.Text);
      csvWriter.Write(sample.Flag);
      csvWriter.Write(sample.Number);
      csvWriter.WriteDecimal2(sample.Amount);
      csvWriter.WriteDecimal4(sample.Amount4);
      csvWriter.WriteDecimal5(sample.Amount5);
      csvWriter.Write(sample.PreciseDecimal);
      csvWriter.Write((int)sample.SampleState);
      csvWriter.WriteDate(sample.DateOnly);
      csvWriter.WriteTime(sample.TimeOnly);
      csvWriter.WriteDateTimeTicks(sample.DateTimeTicks);
      csvWriter.WriteDateMinutes(sample.DateTimeMinute);
      csvWriter.WriteDateSeconds(sample.DateTimeSecond);
      if (sample.OneMaster is null) {
        csvWriter.WriteNull();
      } else {
        if (sample.OneMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because OneMaster is not stored in DC.Data.SampleMasters.");

        csvWriter.Write(sample.OneMaster.Key.ToString());
      }
      if (sample.OtherMaster is null) {
        csvWriter.WriteNull();
      } else {
        if (sample.OtherMaster.Key<0) throw new Exception($"Cannot write sample '{sample}' to CSV File, because OtherMaster is not stored in DC.Data.SampleMasters.");

        csvWriter.Write(sample.OtherMaster.Key.ToString());
      }
      csvWriter.Write(sample.Optional);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates Sample with the provided values
    /// </summary>
    public void Update(
      string text, 
      bool flag, 
      int number, 
      decimal amount, 
      decimal amount4, 
      decimal? amount5, 
      decimal preciseDecimal, 
      SampleStateEnum sampleState, 
      DateTime dateOnly, 
      TimeSpan timeOnly, 
      DateTime dateTimeTicks, 
      DateTime dateTimeMinute, 
      DateTime dateTimeSecond, 
      SampleMaster? oneMaster, 
      SampleMaster? otherMaster, 
      string? optional)
    {
      var clone = new Sample(this);
      var isCancelled = false;
      onUpdating(
        text, 
        flag, 
        number, 
        amount, 
        amount4, 
        amount5, 
        preciseDecimal, 
        sampleState, 
        dateOnly, 
        timeOnly, 
        dateTimeTicks, 
        dateTimeMinute, 
        dateTimeSecond, 
        oneMaster, 
        otherMaster, 
        optional, 
        ref isCancelled);
      if (isCancelled) return;

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
      var amountRounded = amount.Round(2);
      if (Amount!=amountRounded) {
        Amount = amountRounded;
        isChangeDetected = true;
      }
      var amount4Rounded = amount4.Round(4);
      if (Amount4!=amount4Rounded) {
        Amount4 = amount4Rounded;
        isChangeDetected = true;
      }
      var amount5Rounded = amount5.Round(5);
      if (Amount5!=amount5Rounded) {
        Amount5 = amount5Rounded;
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
      var dateOnlyRounded = dateOnly.Floor(Rounding.Days);
      if (DateOnly!=dateOnlyRounded) {
        DateOnly = dateOnlyRounded;
        isChangeDetected = true;
      }
      var timeOnlyRounded = timeOnly.Round(Rounding.Seconds);
      if (TimeOnly!=timeOnlyRounded) {
        TimeOnly = timeOnlyRounded;
        isChangeDetected = true;
      }
      if (DateTimeTicks!=dateTimeTicks) {
        DateTimeTicks = dateTimeTicks;
        isChangeDetected = true;
      }
      var dateTimeMinuteRounded = dateTimeMinute.Round(Rounding.Minutes);
      if (DateTimeMinute!=dateTimeMinuteRounded) {
        DateTimeMinute = dateTimeMinuteRounded;
        isChangeDetected = true;
      }
      var dateTimeSecondRounded = dateTimeSecond.Round(Rounding.Seconds);
      if (DateTimeSecond!=dateTimeSecondRounded) {
        DateTimeSecond = dateTimeSecondRounded;
        isChangeDetected = true;
      }
      if (OneMaster is null) {
        if (oneMaster is null) {
          //nothing to do
        } else {
          OneMaster = oneMaster;
          if (Key>=0) {
            OneMaster.AddToSampleX(this);
          }
          isChangeDetected = true;
        }
      } else {
        if (oneMaster is null) {
          if (Key>=0) {
            OneMaster.RemoveFromSampleX(this);
          }
          OneMaster = null;
          isChangeDetected = true;
        } else {
          if (OneMaster!=oneMaster) {
            if (Key>=0) {
              OneMaster.RemoveFromSampleX(this);
            }
            OneMaster = oneMaster;
            if (Key>=0) {
              OneMaster.AddToSampleX(this);
            }
            isChangeDetected = true;
          }
        }
      }
      if (OtherMaster is null) {
        if (otherMaster is null) {
          //nothing to do
        } else {
          OtherMaster = otherMaster;
          if (Key>=0) {
            OtherMaster.AddToSampleX(this);
          }
          isChangeDetected = true;
        }
      } else {
        if (otherMaster is null) {
          if (Key>=0) {
            OtherMaster.RemoveFromSampleX(this);
          }
          OtherMaster = null;
          isChangeDetected = true;
        } else {
          if (OtherMaster!=otherMaster) {
            if (Key>=0) {
              OtherMaster.RemoveFromSampleX(this);
            }
            OtherMaster = otherMaster;
            if (Key>=0) {
              OtherMaster.AddToSampleX(this);
            }
            isChangeDetected = true;
          }
        }
      }
      if (Optional!=optional) {
        Optional = optional;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdated(clone);
        if (Key>=0) {
          DC.Data.SampleX.ItemHasChanged(clone, this);
        }
        HasChanged?.Invoke(clone, this);
      }
    }
    partial void onUpdating(
      string text, 
      bool flag, 
      int number, 
      decimal amount, 
      decimal amount4, 
      decimal? amount5, 
      decimal preciseDecimal, 
      SampleStateEnum sampleState, 
      DateTime dateOnly, 
      TimeSpan timeOnly, 
      DateTime dateTimeTicks, 
      DateTime dateTimeMinute, 
      DateTime dateTimeSecond, 
      SampleMaster? oneMaster, 
      SampleMaster? otherMaster, 
      string? optional, 
      ref bool isCancelled);
    partial void onUpdated(Sample old);


    /// <summary>
    /// Updates this Sample with values from CSV file
    /// </summary>
    internal static void Update(Sample sample, CsvReader csvReader){
      sample.Text = csvReader.ReadString();
      sample.Flag = csvReader.ReadBool();
      sample.Number = csvReader.ReadInt();
      sample.Amount = csvReader.ReadDecimal();
      sample.Amount4 = csvReader.ReadDecimal();
      sample.Amount5 = csvReader.ReadDecimalNull();
      sample.PreciseDecimal = csvReader.ReadDecimal();
      sample.SampleState = (SampleStateEnum)csvReader.ReadInt();
      sample.DateOnly = csvReader.ReadDate();
      sample.TimeOnly = csvReader.ReadTime();
      sample.DateTimeTicks = csvReader.ReadDateTimeTicks();
      sample.DateTimeMinute = csvReader.ReadDateSeconds();
      sample.DateTimeSecond = csvReader.ReadDateSeconds();
      var oneMasterKey = csvReader.ReadIntNull();
      SampleMaster? oneMaster;
      if (oneMasterKey is null) {
        oneMaster = null;
      } else {
        if (!DC.Data.SampleMasters.TryGetValue(oneMasterKey.Value, out oneMaster)) {
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
        if (!DC.Data.SampleMasters.TryGetValue(otherMasterKey.Value, out otherMaster)) {
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
      sample.Optional = csvReader.ReadStringNull();
      sample.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Add sampleDetail to SampleDetails.
    /// </summary>
    internal void AddToSampleDetails(SampleDetail sampleDetail) {
#if DEBUG
      if (sampleDetail==SampleDetail.NoSampleDetail) throw new Exception();
#endif
      sampleDetails.Add(sampleDetail);
      onAddedToSampleDetails(sampleDetail);
    }
    partial void onAddedToSampleDetails(SampleDetail sampleDetail);


    /// <summary>
    /// Removes sampleDetail from Sample.
    /// </summary>
    internal void RemoveFromSampleDetails(SampleDetail sampleDetail) {
#if DEBUG
      if (!sampleDetails.Remove(sampleDetail)) throw new Exception();
#else
        sampleDetails.Remove(sampleDetail);
#endif
      onRemovedFromSampleDetails(sampleDetail);
    }
    partial void onRemovedFromSampleDetails(SampleDetail sampleDetail);


    /// <summary>
    /// Removes Sample from DC.Data.SampleX, 
    /// disconnects Sample from SampleMaster because of OneMaster, 
    /// disconnects Sample from SampleMaster because of OtherMaster and 
    /// deletes any SampleDetail where SampleDetail.Sample links to this Sample.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"Sample.Remove(): Sample 'Class Sample' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.SampleX.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects Sample from SampleMaster because of OneMaster, 
    /// disconnects Sample from SampleMaster because of OtherMaster and 
    /// deletes any SampleDetail where SampleDetail.Sample links to this Sample.
    /// </summary>
    internal static void Disconnect(Sample sample) {
      if (sample.OneMaster!=null && sample.OneMaster!=SampleMaster.NoSampleMaster) {
        sample.OneMaster.RemoveFromSampleX(sample);
      }
      if (sample.OtherMaster!=null && sample.OtherMaster!=SampleMaster.NoSampleMaster) {
        sample.OtherMaster.RemoveFromSampleX(sample);
      }
      for (int sampleDetailIndex = sample.SampleDetails.Count-1; sampleDetailIndex>= 0; sampleDetailIndex--) {
        var sampleDetail = sample.SampleDetails[sampleDetailIndex];
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

      var clone = new Sample(this);
      OneMaster = null;
      HasChanged?.Invoke(clone, this);
    }


    /// <summary>
    /// Removes sampleMaster from OtherMaster
    /// </summary>
    internal void RemoveOtherMaster(SampleMaster sampleMaster) {
      if (sampleMaster!=OtherMaster) throw new Exception();

      var clone = new Sample(this);
      OtherMaster = null;
      HasChanged?.Invoke(clone, this);
    }


    /// <summary>
    /// Removes Sample from possible parents as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemStore(IStorageItem item) {
      var sample = (Sample) item;
      if (sample.OneMaster!=null && sample.OneMaster!=SampleMaster.NoSampleMaster) {
        sample.OneMaster.RemoveFromSampleX(sample);
      }
      if (sample.OtherMaster!=null && sample.OtherMaster!=SampleMaster.NoSampleMaster) {
        sample.OtherMaster.RemoveFromSampleX(sample);
      }
      sample.onRollbackItemStored();
    }
    partial void onRollbackItemStored();


    /// <summary>
    /// Restores the Sample item data as it was before the last update as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemUpdate(IStorageItem oldItem, IStorageItem newItem) {
      var sampleOld = (Sample) oldItem;
      var sampleNew = (Sample) newItem;
      sampleNew.Text = sampleOld.Text;
      sampleNew.Flag = sampleOld.Flag;
      sampleNew.Number = sampleOld.Number;
      sampleNew.Amount = sampleOld.Amount;
      sampleNew.Amount4 = sampleOld.Amount4;
      sampleNew.Amount5 = sampleOld.Amount5;
      sampleNew.PreciseDecimal = sampleOld.PreciseDecimal;
      sampleNew.SampleState = sampleOld.SampleState;
      sampleNew.DateOnly = sampleOld.DateOnly;
      sampleNew.TimeOnly = sampleOld.TimeOnly;
      sampleNew.DateTimeTicks = sampleOld.DateTimeTicks;
      sampleNew.DateTimeMinute = sampleOld.DateTimeMinute;
      sampleNew.DateTimeSecond = sampleOld.DateTimeSecond;
      if (sampleNew.OneMaster is null) {
        if (sampleOld.OneMaster is null) {
          //nothing to do
        } else {
          sampleNew.OneMaster = sampleOld.OneMaster;
          sampleNew.OneMaster.AddToSampleX(sampleNew);
        }
      } else {
        if (sampleOld.OneMaster is null) {
          if (sampleNew.OneMaster!=SampleMaster.NoSampleMaster) {
            sampleNew.OneMaster.RemoveFromSampleX(sampleNew);
          }
          sampleNew.OneMaster = null;
        } else {
          if (sampleNew.OneMaster!=SampleMaster.NoSampleMaster) {
            sampleNew.OneMaster.RemoveFromSampleX(sampleNew);
          }
          sampleNew.OneMaster = sampleOld.OneMaster;
          sampleNew.OneMaster.AddToSampleX(sampleNew);
        }
      }
      if (sampleNew.OtherMaster is null) {
        if (sampleOld.OtherMaster is null) {
          //nothing to do
        } else {
          sampleNew.OtherMaster = sampleOld.OtherMaster;
          sampleNew.OtherMaster.AddToSampleX(sampleNew);
        }
      } else {
        if (sampleOld.OtherMaster is null) {
          if (sampleNew.OtherMaster!=SampleMaster.NoSampleMaster) {
            sampleNew.OtherMaster.RemoveFromSampleX(sampleNew);
          }
          sampleNew.OtherMaster = null;
        } else {
          if (sampleNew.OtherMaster!=SampleMaster.NoSampleMaster) {
            sampleNew.OtherMaster.RemoveFromSampleX(sampleNew);
          }
          sampleNew.OtherMaster = sampleOld.OtherMaster;
          sampleNew.OtherMaster.AddToSampleX(sampleNew);
        }
      }
      sampleNew.Optional = sampleOld.Optional;
      sampleNew.onRollbackItemUpdated(sampleOld);
    }
    partial void onRollbackItemUpdated(Sample oldSample);


    /// <summary>
    /// Adds Sample item to possible parents again as part of a transaction rollback.
    /// </summary>
    internal static void RollbackItemRemove(IStorageItem item) {
      var sample = (Sample) item;
      sample.OneMaster?.AddToSampleX(sample);
      sample.OtherMaster?.AddToSampleX(sample);
      sample.onRollbackItemRemoved();
    }
    partial void onRollbackItemRemoved();


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
        $" {Amount4}," +
        $" {Amount5}," +
        $" {PreciseDecimal}," +
        $" {SampleState}," +
        $" {DateOnly.ToShortDateString()}," +
        $" {TimeOnly}," +
        $" {DateTimeTicks}," +
        $" {DateTimeMinute}," +
        $" {DateTimeSecond}," +
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
        $" Amount4: {Amount4}," +
        $" Amount5: {Amount5}," +
        $" PreciseDecimal: {PreciseDecimal}," +
        $" SampleState: {SampleState}," +
        $" DateOnly: {DateOnly.ToShortDateString()}," +
        $" TimeOnly: {TimeOnly}," +
        $" DateTimeTicks: {DateTimeTicks}," +
        $" DateTimeMinute: {DateTimeMinute}," +
        $" DateTimeSecond: {DateTimeSecond}," +
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
