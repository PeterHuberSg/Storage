#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel {


  /// <summary>
  /// Some comment for SampleMaster.
  /// With an additional line.
  /// </summary>
  public class SampleMaster {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;

    /// <summary>
    /// Some Samples comment
    /// </summary>
    public List<Sample> Samples;


    //the following confirguration constants define the storage behaviour of
    //SampleMaster's StorageDirectory. They will not be included in SampleMaster. 
    public const int MaxLineLenght = 50;
    //public const bool AreItemsUpdatable = true;
    //public const bool AreItemsDeletable = false;
    //public const bool IsCompactDuringDispose = false;
  }


  /// <summary>
  /// Some comment for Sample
  /// </summary>
  public class Sample {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;


    /// <summary>
    /// Some Number comment
    /// </summary>
    public int Number;


    /// <summary>
    /// Some Amount comment
    /// </summary>
    public decimal Amount;


    /// <summary>
    /// Some Date comment
    /// </summary>
    public DateTime Date;


    /// <summary>
    /// Some SampleMaster comment
    /// </summary>
    public SampleMaster? SampleMaster;


    /// <summary>
    /// Some Optional comment
    /// </summary>
    public string? Optional;


    /// <summary>
    /// Some SampleDetails comment
    /// </summary>
    public List<SampleDetail> SampleDetails;


    public const int MaxLineLenght = 200;
    public const bool AreItemsUpdatable = false;
    public const bool AreItemsDeletable = true;
    public const bool IsCompactDuringDispose = false;
  }


  /// <summary>
  /// Some comment for SampleDetail
  /// </summary>
  public class SampleDetail {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;


    /// <summary>
    /// Link to parent Sample
    /// </summary>
    public Sample Sample;


    public const int MaxLineLenght = 50;
  }
}