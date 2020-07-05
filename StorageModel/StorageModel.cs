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


#region Documentation
//      -------------

// Structure of this file
// ----------------------
//
// The code in this file is used to auto generated classes, properties and their parent child relationships.
//
// Simple comments '//' will not be included in the generated files. They can be used to comment the model only.
// XML comments '///' will be included in the generated files as comments for the classes or properties they comment.
// #region will not be included in the generated files. They are used to navigation among the different Storage models
//
// using and namespace will be included in the generated files
// #pragma are used only to prevent compiler warnings in this file
// [StorageClass] and [StorageProperty] attributes are used for generating code

// How to setup your own project
// -----------------------------
//
// Create a .NET Core Console application project for your model and a .dll project for your generated code.
// Change Program.cs to something like this:
/*
using System;
using System.IO;
namespace YourNameSpace {

  class Program {
    public static void Main(string[] _) {
      var solutionDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent;
      var sourceDirectoryPath = solutionDirectory.FullName + @"\Model";
      var targetDirectoryPath = solutionDirectory.FullName + @"\DataContext";
      new StorageClassGenerator(
        sourceDirectoryString: sourceDirectoryPath, //directory from where the .cs files get read.
        targetDirectoryString: targetDirectoryPath, //directory where the new .cs files get written.
        context: "DC"); //>Name of Data Context class, which gives static access to all data stored.
    }
  }
}
*/
// It defines the model project and generated code projects and then calls the StorageClassGenerator.
// Run the console application each time you have made a change to the model

// Add a .CS file following the structure of the file you read presently. A simple model could look like this:
/*
using System;
using System.Collections.Generic;
using Storage;

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace YourNameSpace {

  /// <summary>
  /// Some comment for Parent.
  /// </summary>
  [StorageClass(areInstancesUpdatable:false, areInstancesDeletable: false)]
  public class Parent {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;

    /// <summary>
    /// List representing parent child relationship
    /// </summary>
    public List<Child> Children;
  }

  /// <summary>
  /// Some comment for Child
  /// </summary>
  [StorageClass(pluralName: "Children")]
  public class Child {
    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;
  }
}*/

// Generated Code
// --------------
//
// 2 files will be created for every class Xxx: Xxx.base.cs containing the generated code as partial class and Xxx.cs
// where you can add your own functionality. The code generator only makes a Xxx.cs when none exist. Xxx.base.cs gets
// overwritten each time the generator runs.

// One additional file gets generated as data context. It's name is defined in new StorageClassGenerator() call of the
// Console program. Create in your application a new data context, which gives access to all classes in the model and
// reads the persisted data from the .CSV files. Dispose it before closing your application, which ensures that all
// changes are written to the files.

#endregion


using System;
using System.Collections.Generic;
using Storage;


#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
namespace StorageModel {

  #region Class using all supported data types 
  //      ------------------------------------

  // check this class to find all available data types.

  // The goal is to use as little storage space in the CSV file as possible. It is better to use the data type Date
  // then DateTime, if the time portion is not use. It is better to use Decimal2, which stores maximally 2 digits
  // after the decimal point than decimal, which gets stored with full precision.

  // In general, it is better to use none nullable value types, they give the garbage collector less work to do.


  /// <summary>
  /// Class having every possible data type used for a property
  /// </summary>
  public class DataTypeSample {
    //A DateTime with only Date, but no Time
    public Date ADate;
    public Date? ANullableDate;

    //A TimeSpan covering only positive 23 hours, 59 minutes and 59 seconds 
    public Time ATime;
    public Time? ANullableTime;

    //A DateTime with a precision of minutes
    public DateMinutes ADateMinutes;
    public DateMinutes? ANullableDateMinutes;

    //A DateTime with a precision of seconds
    public DateSeconds ADateSeconds;
    public DateSeconds? ANullableDateSeconds;

    //A DateTime with a precision of ticks
    public DateTimeTicks ADateTime;
    public DateTimeTicks? ANullableDateTime;

    //A TimeSpan with a precision of ticks
    public TimeSpanTicks ATimeSpan;
    public TimeSpanTicks? ANullableTimeSpan;

    //A decimal with full precision. If possible, use DecimalX, which uses less CSV file storage space
    public decimal ADecimal;
    public decimal? ANullableDecimal;

    //A decimal with up to 2 digits after decimal point
    public Decimal2 ADecimal2;
    public Decimal2? ANullableDecimal2;

    //A decimal with up to 4 digits after decimal point
    public Decimal4 ADecimal4;
    public Decimal4? ANullableDecimal4;

    //A decimal with up to 5 digits after decimal point
    public Decimal5 ADecimal5;
    public Decimal5? ANullableDecimal5;

    //A boolean
    public bool ABool;
    public bool? ANullableBool;

    //An integer
    public int AInt;
    public int? ANullableInt;

    //A long
    public long ALong;
    public long? ANullableLong;

    //A character
    public char AChar;
    public char? ANullableChar;

    //A string, full Unicode supported, but ASCII only strings get faster processed
    public string AString;
    public string? ANullableString;

    //any enum defined in this file
    public SampleStateEnum AEnum;
    public SampleStateEnum? ANullableEnum;
  }
  #endregion


  #region SampleMaster -> Sample -> SampleDetail, using List for children
  //      ---------------------------------------------------------------

  //Sample.SampleMaster is nullable
  //SampleDetail.Sample is NOT nullable, it is not possible to store a SampleDetail without a parent Sample
  //shows in Sample also most data types supported

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
  [StorageClass(pluralName: "SampleX")]
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
    public DateTimeTicks DateTimeTicks;

    /// <summary>
    /// Stores date and time precisely to a minute
    /// </summary>
    public DateMinutes DateTimeMinute;

    /// <summary>
    /// Stores date and time precisely to a second
    /// </summary>
    public DateSeconds DateTimeSecond;

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


  #region Parent with zero or maximal 1 child
  //      -----------------------------------

  // The parent might or might not have a child.
  //
  // The parent class has a nullable property with the child class type, while the child class has a property
  // with the parent class type.
  //
  // Since the parent class is deletable, it cannot have a child which is not deletable, because the deletion of
  // the parent forces also the deletion of the child.

  /// <summary>
  /// Example for parent which can have at most 1 child and the parent property in the child is not nullable.
  /// </summary>
  public class ParentOneChild_Parent {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;


    /// <summary>
    /// Links to conditional child. Parent might or might not have a child, since the parent always gets
    /// created before the child.
    /// </summary>
    [StorageProperty(isParentOneChild: true)]
    public ParentOneChild_Child? Child;


    ///// <summary>
    ///// Links to conditional readonly child. Parent might or might not have a child, since the parent always gets
    ///// created before the child.
    ///// </summary>
    //[StorageProperty(isParentOneChild: true)]
    //public ParentOneChild_ReadonlyChild? ReadonlyChild;
  }


  /// <summary>
  /// Example for parent which can have at most 1 child and the parent property in the child is nullable.
  /// </summary>
  public class ParentOneChild_ParentNullable {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;


    /// <summary>
    /// Links to conditional child. Parent might or might not have a child, since the parent always gets
    /// created before the child.
    /// </summary>
    [StorageProperty(isParentOneChild: true)]
    public ParentOneChild_Child? Child;


    ///// <summary>
    ///// Links to conditional readonly child. Parent might or might not have a child, since the parent always gets
    ///// created before the child.
    ///// </summary>
    //[StorageProperty(isParentOneChild: true)]
    //public ParentOneChild_ReadonlyChild? ReadonlyChild;
  }


  /// <summary>
  /// Child class with one parent property which is not nullable and one property to a different parent 
  /// which is nullable
  /// </summary>
  [StorageClass(pluralName: "ParentOneChild_Children")]
  public class ParentOneChild_Child {

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;


    /// <summary>
    /// Links to parent
    /// </summary>
    public ParentOneChild_Parent Parent;


    /// <summary>
    /// Links to parent conditionally
    /// </summary>
    public ParentOneChild_ParentNullable? ParentNullable;
  }


  ///// <summary>
  ///// Readonly Child class with one parent property which is not nullable and one property to a different parent 
  ///// which is nullable
  ///// </summary>
  //[StorageClass(pluralName: "ParentOneChild_ReadonlyChildren")]
  //public class ParentOneChild_ReadonlyChild {

  //  /// <summary>
  //  /// Some Text comment
  //  /// </summary>
  //  public string Text;


  //  /// <summary>
  //  /// Links to parent
  //  /// </summary>
  //  public ParentOneChild_Parent Parent;


  //  /// <summary>
  //  /// Links to parent conditionally
  //  /// </summary>
  //  public ParentOneChild_ParentNullable? ParentNullable;
  //}
  #endregion


  #region Class where the value of one property are used to build a dictionary for that class
  //      -----------------------------------------------------------------------------------

  // Often, a class has one property which can be used to identify one particular instance. 
  // StorageProperty(needsDictionary: true) adds a Dictionary to the data context, which gets updated whenever
  // an instance get added, that property updated or the instance deleted.

  /// <summary>
  /// Some comment for PropertyNeedsDictionaryClass
  /// </summary>
  [StorageClass(pluralName: "PropertyNeedsDictionaryClasses")]
  public class PropertyNeedsDictionaryClass {

    /// <summary>
    /// Used as key into dictionary SampleWithDictionaryByIdInt
    /// </summary>
    [StorageProperty(needsDictionary: true)]
    public int IdInt;

    /// <summary>
    /// Used as key into dictionary SampleWithDictionaryByIdString
    /// </summary>
    [StorageProperty(needsDictionary: true)]
    public string? IdString;

    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text;
  }
  #endregion


  #region LookupParent -> LookupChild, parent has no collection for children
  //      ------------------------------------------------------------------

  //an example for lookup, only the child linking to parent but the parent having no child collection
  //this can be useful for example if parent holds exchange rates for every day. The child links to
  //one particular exchange rate, but the exchange rate does not know which child links to it. In this
  //scenario, the parent can never be deleted.

  /// <summary>
  /// Parent of children who use lookup, i.e. parent has no children collection
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false)]
  public class Lookup_Parent {
    public Date Date;
    public Decimal2 SomeValue;
  }


  /// <summary>
  /// Parent of children who use lookup, i.e. parent has no children collection,  where the child's 
  /// parent property is nullable.
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: true)]
  public class Lookup_ParentNullable {
    public Date Date;
    public Decimal2 SomeValue;
  }


  /// <summary>
  /// Some comment for Lookup_Child
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false, pluralName: "Lookup_Children")]
  public class Lookup_Child {
    public int Number;
    [StorageProperty(isLookupOnly: true)] //parent LookupParent does not have a collection for LookupChild
    public Lookup_Parent LookupParent;
    [StorageProperty(isLookupOnly: true)] //parent LookupParentNullable does not have a collection for LookupChild
    public Lookup_ParentNullable? LookupParentNullable;
  }
  #endregion


  #region ChildrenList, using List for children
  //      -------------------------------------

  //Example where the parent uses a List for its children. 
  //If the child is not deletable, the parent must not be not deletable too. It's not possible to delete a parent and
  //leave the child with a link to that deleted parent.
  //The child.Parent property can be nullable (conditional parent) or not nullable (parent required)
  //[StorageClass(isGenerateReaderWriter: true)] creates ClassXyzReader and ClassXyzWriter, which allow to read and write 
  //the CSV file without using a data context nor StorageDictionary. This is useful for administrative tasks, like deleting
  //of data which is not deletable within the data context.

  /// <summary>
  /// Example of deletable parent using a List for its children. It can have only deletable children. The child must have a 
  /// parent (the child.Parent property is not nullable).
  /// </summary>
  [StorageClass(isGenerateReaderWriter: true)]
  public class ChildrenList_Parent {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// Deletable children which must have a parent
    /// </summary>
    public List<ChildrenList_Child> ChildrenList_Children;
  }


  /// <summary>
  /// Example of deletable parent using a List for its children. It can have only deletable children. The child might have a 
  /// parent (the child.Parent property is nullable).
  /// </summary>
  [StorageClass(isGenerateReaderWriter: true)]
  public class ChildrenList_ParentNullable {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// Deletable children which might or might not have a parent
    /// </summary>
    public List<ChildrenList_Child> ChildrenList_Children;
  }


  /// <summary>
  /// Example of none deletable parent using a List for its children. It can have deletable and none
  /// deletable children. The child must have a parent (the Parent property is not nullable).
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false, isGenerateReaderWriter: true)]
  public class ChildrenList_CreateOnlyParent {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// These deletable children must have a parent
    /// </summary>
    public List<ChildrenList_Child> ChildrenList_Children;

    /// <summary>
    /// These none deletable children must have a none deletable parent
    /// </summary>
    public List<ChildrenList_CreateOnlyChild> ChildrenList_CreateOnlyChildren;
  }


  /// <summary>
  /// Example of none deletable parent using a List for its children. The child can be deletable or none deletable. The 
  /// child might have a parent (the Parent property is nullable).
  /// </summary>
  [StorageClass(areInstancesDeletable:false, areInstancesUpdatable: false, isGenerateReaderWriter: true)]
  public class ChildrenList_CreateOnlyParentNullable {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// These deletable children might or might not have a parent
    /// </summary>
    public List<ChildrenList_Child> ChildrenList_Children;

    /// <summary>
    /// These none deletable children might or might not have a parent
    /// </summary>
    public List<ChildrenList_CreateOnlyChild> ChildrenList_CreateOnlyChildren;
  }


  /// <summary>
  /// This deletable child has links to 4 different types of parents
  /// </summary>
  [StorageClass(pluralName: "ChildrenList_Children", isGenerateReaderWriter: true)]
  public class ChildrenList_Child {
    /// <summary>
    /// Some info
    /// </summary>
    public string Text;

    /// <summary>
    /// Deletable parent for deletable children which must have a parent
    /// </summary>
    public ChildrenList_Parent Parent;

    /// <summary>
    /// Deletable parent for deletable children which might or might not have a parent
    /// </summary>
    public ChildrenList_ParentNullable? ParentNullable;

    /// <summary>
    /// None deletable parent for deletable children which must have a parent
    /// </summary>
    public ChildrenList_CreateOnlyParent CreateOnlyParent;

    /// <summary>
    /// None deletable parent for deletable children which might or might not have a parent
    /// </summary>
    public ChildrenList_CreateOnlyParentNullable? CreateOnlyParentNullable;
  }


  /// <summary>
  /// This none deletable child has links to 2 different types of parents, which must be none deletable
  /// </summary>
  [StorageClass(pluralName: "ChildrenList_CreateOnlyChildren", areInstancesDeletable: false, 
    areInstancesUpdatable: false, isGenerateReaderWriter: true)]
  public class ChildrenList_CreateOnlyChild {
    /// <summary>
    /// Some info
    /// </summary>
    public string Text;

    /// <summary>
    /// None deletable parent for none deletable child which must have a none deletable parent
    /// </summary>
    public ChildrenList_CreateOnlyParent CreateOnlyParent;

    /// <summary>
    /// None deletable parent for deletable child which might or might not have a parent which must be none deletable
    /// </summary>
    public ChildrenList_CreateOnlyParentNullable? CreateOnlyParentNullable;
  }
  #endregion


  #region ParentChildrenDictionary, using Dictionary for children
  //      -------------------------------------------------------

  //Example where parent has a Dictionary instead a List for its children. The child needs an additional field which
  //can be used as Key for the Dictionary. 

  /// <summary>
  /// Example of a parent child relationship using a Dictionary.
  /// </summary>
  public class ChildrenDictionary_Parent {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    /// use as key
    /// </summary>
    public Dictionary<Date /*DateKey*/, ChildrenDictionary_Child> ChildrenDictionary_Children;
  }


  /// <summary>
  /// Example of a parent child relationship using a Dictionary where the child's parent property is nullable.
  /// </summary>
  public class ChildrenDictionary_ParentNullable {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;

    /// <summary>
    /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    /// use as key
    /// </summary>
    public Dictionary<Date /*DateKey*/, ChildrenDictionary_Child> ChildrenDictionary_Children;
  }


  /// <summary>
  /// DictionaryChild has a member providing the key value needed to add DictionaryChild to  
  /// ParentWithDictionary and ParentWithDictionaryNullable
  /// </summary>
  [StorageClass(pluralName: "ChildrenDictionary_Children")]
  public class ChildrenDictionary_Child {
    /// <summary>
    /// Key field used in ParentWithDictionary.DictionaryChildren and 
    /// ParentWithDictionaryNullable.DictionaryChildrenDictionary
    /// </summary>
    public Date DateKey;

    /// <summary>
    /// Some info
    /// </summary>
    public string Text;

    /// <summary>
    /// Parent
    /// </summary>
    public ChildrenDictionary_Parent ParentWithDictionary;

    /// <summary>
    /// Nullable parent
    /// </summary>
    public ChildrenDictionary_ParentNullable? ParentWithDictionaryNullable;
  }
  #endregion


  #region ParentChildrenSortedList, using SortedList for children
  //      -------------------------------------------------------

  //Example where parent has a SortedList instead a List for its children. The child needs an additional field which
  //can be used as Key for the SortedList.

  //It's better to use a SortedList than a SortedDictionary, because in a SortedList, an item can be accessed
  //by its place in the SortedList like the last item:
  //key = sortedList.Keys[sortedList.Lenght];
  //item = sortedList[key];

  /// <summary>
  /// Example of a parent child relationship using a SortedList.
  /// </summary>
  public class ChildrenSortedList_Parent {

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
    public SortedList<Date /*DateKey*/, ChildrenSortedList_Child> ChildrenSortedList_Children;
  }


  /// <summary>
  /// Example of a parent child relationship using a SortedList where the child's parent property is nullable.
  /// </summary>
  public class ChildrenSortedList_ParentNullable {

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
    public SortedList<Date /*DateKey*/, ChildrenSortedList_Child> ChildrenSortedList_Children;
  }


  /// <summary>
  /// SortedListChild has a member providing the key value needed to add SortedListChild to  
  /// ParentWithSortedList and ParentWithSortedListNullable
  /// </summary>
  [StorageClass(pluralName: "ChildrenSortedList_Children")]
  public class ChildrenSortedList_Child {
    /// <summary>
    /// Key field used in ParentWithSortedList.SortedListChildren SortedList
    /// </summary>
    public Date DateKey;

    /// <summary>
    /// Some info
    /// </summary>
    public string Text;

    /// <summary>
    /// Parent
    /// </summary>
    public ChildrenSortedList_Parent ParentWithSortedList;

    /// <summary>
    /// Nullable Parent
    /// </summary>
    public ChildrenSortedList_ParentNullable? ParentWithSortedListNullable;
  }
  #endregion


  #region CreateOnlyParent -> CreateOnlyChild, using List for children
  //      ------------------------------------------------------------

  //Example where parent and children are CreateOnly, meaning not updatable and not deletable. This should lead to
  //the smallest amount of generated code.

  /// <summary>
  /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class CreateOnly_Parent {

    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<CreateOnly_Child> CreateOnly_Children;
  }


  /// <summary>
  /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them. The parent property in the child 
  /// is nullable.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class CreateOnly_ParentNullable {

    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<CreateOnly_Child> CreateOnly_Children;
  }


  /// <summary>
  /// Example of a "CreateOnly" Child, i.e. the child's properties will not change. If it is added to a parent during its
  /// creation, it cannot be removed from the parent, because the Parent property of the child cannot be changed
  /// either.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false, pluralName: "CreateOnly_Children")]
  public class CreateOnly_Child {

    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// CreateOnlyParent will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public CreateOnly_Parent CreateOnlyParent;

    /// <summary>
    /// CreateOnlyParentNullable will be readonly even it is not marked as such, because class is not updatable
    /// </summary>          
    public CreateOnly_ParentNullable? CreateOnlyParentNullable;
  }
  #endregion


  #region CreateOnlyParent -> ChangeableChild (updatable and or deletable), using List for children
  //      -----------------------------------------------------------------------------------------

  //Example where parent is not updatable and not deletable. Child can be updated and/or deleted. 

  /// <summary>
  /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add and remove children.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class CreateOnlyParentChangeableChild_Parent {

    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<CreateOnlyParentChangeableChild_Child> CreateOnlyParentChangeableChild_Children;
  }


  /// <summary>
  /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add and remove children. The parent property in the child 
  /// is nullable.
  /// </summary>
  [StorageClass(areInstancesUpdatable: false)]
  public class CreateOnlyParentChangeableChild_ParentNullable {

    /// <summary>
    /// Text will be readonly even it is not marked as such, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<CreateOnlyParentChangeableChild_Child> CreateOnlyParentChangeableChild_Children;
  }


  /// <summary>
  /// Example of an updatable and deletable Child, i.e. the child's properties will not change and once it is added to its parent
  /// and therefore it also cannot be removed from parent, because the Parent property of the child cannot be changed
  /// either.
  /// </summary>
  [StorageClass(pluralName: "CreateOnlyParentChangeableChild_Children")]
  public class CreateOnlyParentChangeableChild_Child {

    /// <summary>
    /// Readonly Text
    /// </summary>
    public readonly string ReadonlyText;

    /// <summary>
    /// Updatable Text
    /// </summary>
    public string UpdatableText;

    /// <summary>
    /// Parent
    /// </summary>
    public CreateOnlyParentChangeableChild_Parent Parent;

    /// <summary>
    /// Parent
    /// </summary>          
    public CreateOnlyParentChangeableChild_ParentNullable? ParentNullable;
  }
  #endregion


  #region Private Constructor
  //      -------------------

  // Example where constructor is private instead of public. This is convenient when another public constructor
  // is defined in Xxx.cs, additional to the private constructor in Xxx.base.cs, which is now hidden.

  /// <summary>
  /// Example with private constructor.
  /// </summary>
  [StorageClass(isConstructorPrivate: true)]
  public class PrivateConstructor {

    /// <summary>
    /// Some Text
    /// </summary>
    public string Text;
  }
  #endregion

}