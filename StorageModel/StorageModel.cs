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
        context: "DL"); //>Name of Context class, which gives static access to all data stored.
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
  /// Parent of children who use lookup, i.e. parent has no children collection
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false)]
  public class LookupParent {
    public Date Date;
    public Decimal2 SomeValue;
  }


  /// <summary>
  /// Parent of children who use lookup, i.e. parent has no children collection,  where the child's 
  /// parent property is nullable.
  /// </summary>
  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: true)]
  public class LookupParentNullable {
    public Date Date;
    public Decimal2 SomeValue;
  }


  [StorageClass(areInstancesUpdatable: false, areInstancesDeletable: false, pluralName: "LookupChildren")]
  public class LookupChild {
    public int Number;
    [StorageProperty(isLookupOnly: true)] //parent LookupParent does not have a collection for LookupChild
    public LookupParent LookupParent;
    [StorageProperty(isLookupOnly: true)] //parent LookupParentNullable does not have a collection for LookupChild
    public LookupParentNullable? LookupParentNullable;
  }
  #endregion


  #region ParentWithDictionary -> DictionaryChild, using Dictionary for children
  //      ----------------------------------------------------------------------

  //Example where parent has a Dictionary instead a List for its children. The child needs an additional field which
  //can be used as Key for the Dictionary. 

  /// <summary>
  /// Example of a parent child relationship using a Dictionary.
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
  /// Example of a parent child relationship using a Dictionary where the child's parent property is nullable.
  /// </summary>
  [StorageClass(pluralName: "ParentsWithDictionaryNullable")]
  public class ParentWithDictionaryNullable {

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
  /// DictionaryChild has a member providing the key value needed to add DictionaryChild to the 
  /// ParentWithDictionary.DictionaryChildren and ParentWithDictionaryNullable.DictionaryChildren
  /// </summary>
  [StorageClass(pluralName: "DictionaryChildren")]
  public class DictionaryChild {
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
    public ParentWithDictionary ParentWithDictionary;

    /// <summary>
    /// Nullable parent
    /// </summary>
    public ParentWithDictionaryNullable? ParentWithDictionaryNullable;
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
  /// Example of a parent child relationship using a SortedList.
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
  /// Example of a parent child relationship using a SortedList where the child's parent property is nullable.
  /// </summary>
  [StorageClass(pluralName: "ParentsWithSortedListNullable")]
  public class ParentWithSortedListNullable {

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
  /// SortedListChild has a member providing the key value needed to add SortedListChild to the 
  /// ParentWithSortedList.SortedListChildren and ParentWithSortedListNullable.SortedListChildren
  /// </summary>
  [StorageClass(pluralName: "SortedListChildren")]
  public class SortedListChild {
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
    public ParentWithSortedList ParentWithSortedList;

    /// <summary>
    /// Nullable Parent
    /// </summary>
    public ParentWithSortedListNullable? ParentWithSortedListNullable;
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
  /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them. The parent property in the child 
  /// is nullable.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class ReadOnlyParentNullable {

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
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// Parent
    /// </summary>
    public ReadOnlyParent ReadOnlyParent;

    /// <summary>
    /// Parent
    /// </summary>          
    public ReadOnlyParentNullable? ReadOnlyParentNullable;
  }
  #endregion


  #region ReadOnlyParent2 -> UpdatableChild, using List for children
  //      ----------------------------------------------------------

  //Example where parent is not updatable and not editable. Child can be updated, but not deleted. 

  /// <summary>
  /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class ReadOnlyParent2 {

    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<UpdatableChild> UpdatableChildren;
  }


  /// <summary>
  /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
  /// deleted, but it is still possible to add children, but not to remove them. The parent property in the child 
  /// is nullable.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: false)]
  public class ReadOnlyParent2Nullable {

    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// List of children
    /// </summary>
    public List<UpdatableChild> UpdatableChildren;
  }


  /// <summary>
  /// Example of a "readonly" Child, i.e. the child's properties will not change and once it is added to its parent
  /// and therefore it also cannot be removed from parent, because the Parent property of the child cannot be changed
  /// either.
  /// </summary>
  [StorageClass(areInstancesDeletable: false, areInstancesUpdatable: true, pluralName: "UpdatableChildren")]
  public class UpdatableChild {

    /// <summary>
    /// Readonly Text, because class is not updatable
    /// </summary>
    public string Text;

    /// <summary>
    /// Parent
    /// </summary>
    public ReadOnlyParent2 ReadOnlyParent2;

    /// <summary>
    /// Parent
    /// </summary>          
    public ReadOnlyParent2Nullable? ReadOnlyParent2Nullable;
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