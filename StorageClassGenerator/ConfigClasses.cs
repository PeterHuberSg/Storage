using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {
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
    /// <param name="isCompactDuringDispose">Should during StorageDirectory.Dispose the CSV file get overwritten without deleted or old values.</param>
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
    public StoragePropertyAttribute(string? defaultValue = null) { }
  }


  /// <summary>
  /// Stores only dates, but no time
  /// </summary>
  public class Date {}


  /// <summary>
  /// Stores only times shorter than 24 hours and only with seconds precission
  /// </summary>
  public class Time {}


  /// <summary>
  /// Stores dates and time with a precission of seconds
  /// </summary>
  public class DateMinutes { }


  /// <summary>
  /// Stores dates and time with a precission of seconds
  /// </summary>
  public class DateSeconds { }


  /// <summary>
  /// Stores only 2 digits after comma
  /// </summary>
  public class Decimal2 { }
}
