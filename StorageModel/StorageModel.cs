/**************************************************************************************

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

  #region SampleMaster -> Sample -> SampleDetail, using List for children
  //      ---------------------------------------------------------------

  //Sample.SampleMaster is nullable
  //SampleDetail.Sample is NOT nullable, it is not possible to store a SampleDetail without a parent Sample
  //shows also in Sample most data types supported

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
  [StorageClass(areInstancesDeletable: false)]
  public class SampleMaster {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;

    /// <summary>
    /// List representing parent child relationship
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
  [StorageClass(maxLineLength: 200, pluralName: "SampleX")]
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
    /// Amount with 4 digits after comma comment
    /// </summary>
    public Decimal4 Amount4;

    /// <summary>
    /// Nullable amount with 5 digits after comma comment
    /// </summary>
    public Decimal5? Amount5;

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
  [StorageClass(maxLineLength: 151)]
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
  #endregion


  #region LookupParent -> LookupChild, parent has no collection for children
  //      ------------------------------------------------------------------

  //an example for lookup, only the child linking to parent but the parent having no child collection
  //this can be useful for example if parent holds exchange rates for every day. The child links to
  //one particular exchange rate, but the exchange rate does not know which child links to it. In this
  //scenario, the parent can never be deleted.

  /// <summary>
  /// Parent of children who uses lookup, i.e. parent has no children collection
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false)]
  public class LookupParent {
    public DateTime Date;
    public Decimal2 SomeValue;
  }


  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false, pluralName: "LookupChildren")]
  public class LookupChild {
    public int Number;
    [StorageProperty(isLookupOnly: true)] //parent LookupParent does not have a collection for LookupChild
    public LookupParent LookupParent;
  }
  #endregion


  #region ParentWithDictionary -> DictionaryChild, using Dictionary for children
  //      ----------------------------------------------------------------------

  //Example where parent has a Dictionary instead a List for its children. The child needs an additional field which
  //can be used as Key for the Dictionary. 

  /// <summary>
  /// Example of a Parent child relationship using a Dictionary.
  /// </summary>
  [StorageClass(pluralName: "ParentsWithDictionary")]
  public class ParentWithDictionary {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    /// use as key
    /// </summary>
    public Dictionary<Date /*DateKey*/, DictionaryChild> DictionaryChildren;
  }


  /// <summary>
  /// DictionaryChild has a member providing the key value needed to add DictionaryChild to the ParentWithDictionary.DictionaryChildren
  /// </summary>
  [StorageClass(pluralName: "DictionaryChildren")]
  public class DictionaryChild {
    /// <summary>
    /// Parent
    /// </summary>
    public ParentWithDictionary ParentWithDictionary;

    /// <summary>
    /// Key field used in ParentWithDictionary.DictionaryChildren Dictionary
    /// </summary>
    public Date DateKey;

    /// <summary>
    /// Some info
    /// </summary>
    public string Text;
  }
  #endregion


  #region ParentWithSortedList -> SortedListChild, using SortedList for children
  //      ----------------------------------------------------------------------

  //Example where parent has a SortedList instead a List for its children. The child needs an additional field which
  //can be used as Key for the SortedList.

  //It's better to use a SortedList than a SortedDictionary, because in a SortedList, an item can be accessed
  //by its place in the SortedList like the last item:
  //key = sortedList.Keys[sortedList.Lenght];
  //item = sortedList[key];

  /// <summary>
  /// Example of a Parent child relationship using a SortedList.
  /// </summary>
  [StorageClass(pluralName: "ParentsWithSortedList")]
  public class ParentWithSortedList {

    /// <summary>
    /// This text is readonly. Readonly only matters when [StorageClass(areInstancesUpdatable: true)]
    /// </summary>
    public readonly string TextReadOnly;

    /// <summary>
    /// This text can be updated
    /// </summary>
    public string TextUpdateable;

    /// <summary>
    /// SortedList used instead of List. Comment is required and indicates which property of the SortedListChild to 
    /// use as key
    /// </summary>
    public SortedList<Date /*DateKey*/, SortedListChild> SortedListChildren;
  }


  /// <summary>
  /// SortedListChild has a member providing the key value needed to add SortedListChild to the ParentWithSortedList.SortedListChildren
  /// </summary>
  [StorageClass(pluralName: "SortedListChildren")]
  public class SortedListChild {
    /// <summary>
    /// Parent
    /// </summary>
    public ParentWithSortedList ParentWithSortedList;

    /// <summary>
    /// Key field used in ParentWithSortedList.SortedListChildren SortedList
    /// </summary>
    public Date DateKey;

    /// <summary>
    /// Some info
    /// </summary>
    public string Text;
  }
  #endregion


  #region ReadOnlyParent -> ReadOnlyChild, using List for children
  //      --------------------------------------------------------

  //Example where parent and children are ReadOnly, meaning not updatable and not editable. This should lead to
  //the smallest amount of generated code.

  /// <summary>
  /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class ReadOnlyParent {

    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<ReadOnlyChild> ReadOnlyChildren;
  }


  /// <summary>
  /// Example of a "readonly" Child, i.e. the child's properties will not change and once it is added to its parent
  /// and therefore it also cannot be removed from parent, because the Parent property of the child cannot be changed
  /// either.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false, pluralName: "ReadOnlyChildren")]
  public class ReadOnlyChild {
    /// <summary>
    /// Parent
    /// </summary>
    public ReadOnlyParent ReadOnlyParent;

    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;
  }
  #endregion
}