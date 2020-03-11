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
  public class StorageClassAttribute: Attribute {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="maxLineLength">Maximum number of UTF8 bytes needed to write class to CSV file.</param>
    /// <param name="pluralName">used if class name has an irregular plural. Example: Activity => Activities</param>
    /// <param name="areItemsUpdatable">Can the properties of the class change ?</param>
    /// <param name="areItemsDeletable">Can class instance be deleted from StorageDirectory ?</param>
    /// <param name="isCompactDuringDispose">Should during StorageDirectory.Dispose the CSV file get overwritten without deleted or old values ?</param>
    public StorageClassAttribute(
      int maxLineLength = 0,
      string? pluralName = null,
      bool areItemsUpdatable = true,
      bool areItemsDeletable = true,
      bool isCompactDuringDispose = true) { }
  }


  /// <summary>
  /// Provides additional information about a property of a class which can be written to a CSV file.
  /// </summary>
  public class StoragePropertyAttribute: Attribute {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="defaultValue">Provides a default value for this property in the class constructor.</param>
    /// <param name="isLookupOnly">Normally, a parent has a child collection for every child type referencing it. If the
    /// child just wants to link to the parent without the parent having a collection for that child, set isLookupOnly = true.</param>
    public StoragePropertyAttribute(string? defaultValue = null, bool isLookupOnly = false) { }
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
  /// Stores only 2 digits after comma
  /// </summary>
  public class Decimal2 { }


  /// <summary>
  /// Stores only 4 digits after comma
  /// </summary>
  public class Decimal4 { }
}
