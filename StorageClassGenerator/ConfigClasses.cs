/**************************************************************************************

Storage.ConfigClasses
=====================

Some small classes to define storage attributes and data types

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;


namespace Storage {


#pragma warning disable IDE0060 // Remove unused parameter
  /// <summary>
  /// Provides additional information about storing the class in a CSV file
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class StorageClassAttribute: Attribute {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pluralName">used if class name has an irregular plural. Example: Activity => Activities</param>
    /// <param name="areInstancesUpdatable">Can the properties of the class change ?</param>
    /// <param name="areInstancesDeletable">Can class instance be deleted from StorageDirectory ?</param>
    /// <param name="isConstructorPrivate">Should constructor be private instead of public ?</param>
    /// <param name="isGenerateReaderWriter">Should code get generated to read instances of that class from
    /// CSV files without using a data context ? This is mostly used for data administration use cases.</param>
    public StorageClassAttribute(
      string? pluralName = null,
      bool areInstancesUpdatable = true,
      bool areInstancesDeletable = true,
      bool isConstructorPrivate = false,
      bool isGenerateReaderWriter = false) { }
  }


  /// <summary>
  /// Provides additional information about a property of a class which can be written to a CSV file.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public class StoragePropertyAttribute: Attribute {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="defaultValue">Provides a default value for this property in the class constructor.</param>
    /// <param name="isLookupOnly">Normally, a parent has a child collection for every child type referencing it. If the
    /// child just wants to link to the parent without the parent having a collection for that child, set isLookupOnly = true.</param>
    /// <param name="needsDictionary">A dictionary gets created in the data context for quick access to an instance using
    /// the value of this property.</param>
    public StoragePropertyAttribute(
      string? defaultValue = null, 
      bool isLookupOnly = false, 
      bool needsDictionary = false,
      bool isParentOneChild = false) { }
  }
#pragma warning restore IDE0060 // Remove unused parameter


  /// <summary>
  /// Stores only dates, but no time
  /// </summary>
  public class Date {}


  /// <summary>
  /// Stores only times shorter than 24 hours and only with seconds precision
  /// </summary>
  public class Time {}


  /// <summary>
  /// Stores dates and time with a precision of seconds
  /// </summary>
  public class DateMinutes { }


  /// <summary>
  /// Stores dates and time with a precision of seconds
  /// </summary>
  public class DateSeconds { }


  /// <summary>
  /// Stores dates and time with a precision of ticks
  /// </summary>
  public class DateTimeTicks { }


  /// <summary>
  /// Stores TimeSpan with a precision of ticks
  /// </summary>
  public class TimeSpanTicks { }


  /// <summary>
  /// Stores only 2 digits after comma
  /// </summary>
  public class Decimal2 { }


  /// <summary>
  /// Stores only 4 digits after comma
  /// </summary>
  public class Decimal4 { }


  /// <summary>
  /// Stores only 5 digits after comma
  /// </summary>
  public class Decimal5 { }
}
