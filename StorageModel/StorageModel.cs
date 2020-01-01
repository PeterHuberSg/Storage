#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel {


  /// <summary>
  /// Some SampleStateEnum comment
  /// </summary>
  public enum SampleStateEnum {
    /// <summary>
    /// Recommendation while creating your own enums: use value 0 as undefined
    /// </summary>
    None,
    Some
  }


  /// <summary>
  /// Some comment for SampleMaster.
  /// With an additional line.
  /// </summary>
  [StorageClass(maxLineLength: 50, areItemsDeletable: false, isCompactDuringDispose: false)]
  public class SampleMaster {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;
    
    /// <summary>
    /// Some Samples comment
    /// </summary>
    public List<Sample> SampleX;


    /// <summary>
    /// Integer property with int.MinValue as default
    /// </summary>
    [StorageProperty(defaultValue: "int.MinValue")]
    public int NumberWithDefault;
  }


  /// <summary>
  /// Some comment for Sample
  /// </summary>
  /// 
  [StorageClass(maxLineLength: 200, pluralName: "SampleX", isCompactDuringDispose: false)]
  public class Sample {
    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;

    /// <summary>
    /// Some Flag comment
    /// </summary>
    public bool Flag;

    /// <summary>
    /// Some Amount comment
    /// </summary>
    public int Number;

    /// <summary>
    /// Amount with 2 digits after comma comment
    /// </summary>
    public Decimal2 Amount;

    /// <summary>
    /// PreciseDecimal with about 20 digits precission, takes a lot of storage space
    /// </summary>
    public decimal PreciseDecimal;

    /// <summary>
    /// Some SampleState comment
    /// </summary>
    public SampleStateEnum SampleState;

    /// <summary>
    /// Stores dates but not times
    /// </summary>
    public Date DateOnly;

    /// <summary>
    /// Stores times (24 hour timespan) but not date
    /// </summary>
    public Time TimeOnly;

    /// <summary>
    /// Stores date and time precisely to a tick
    /// </summary>
    public DateTime DateTimeTicks;

    /// <summary>
    /// Stores date and time precisely to a minute
    /// </summary>
    public DateTime DateTimeMinute;

    /// <summary>
    /// Stores date and time precisely to a second
    /// </summary>
    public DateTime DateTimeSecond;

    /// <summary>
    /// Some OneMaster comment
    /// </summary>
    public SampleMaster? OneMaster;

    /// <summary>
    /// Some OtherMaster comment
    /// </summary>
    public SampleMaster? OtherMaster;

    /// <summary>
    /// Some Optional comment
    /// </summary>
    public string? Optional;

    /// <summary>
    /// Some SampleDetails comment
    /// </summary>
    public List<SampleDetail> SampleDetails;

    //public const int MaxLineLenght = 200;
    //public const bool AreItemsUpdatable = false;
    //public const bool AreItemsDeletable = true;
    //public const bool IsCompactDuringDispose = false;
  }


  /// <summary>
  /// Some comment for SampleDetail
  /// </summary>
  [StorageClass(maxLineLength: 50, isCompactDuringDispose: false)]
  public class SampleDetail {
    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;

    /// <summary>
    /// Link to parent Sample
    /// </summary>
    public Sample Sample;
  }
}