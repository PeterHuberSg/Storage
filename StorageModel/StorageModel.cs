﻿/**************************************************************************************

StorageModel
============

Shows how Data Model classes can be defined for storage compiler

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.Collections.Generic;
using Storage;


#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
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
  [StorageClass(areItemsDeletable: false, isCompactDuringDispose: false)]
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
    /// PreciseDecimal with about 20 digits precision, takes a lot of storage space
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
  }


  /// <summary>
  /// Some comment for SampleDetail
  /// </summary>
  [StorageClass(maxLineLength: 151, isCompactDuringDispose: false)]
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