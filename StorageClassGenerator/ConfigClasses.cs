using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {

  /// <summary>
  /// Sores only dates, but no time
  /// </summary>
  public class Date {}

  /// <summary>
  /// Stores only times shorter than 24 hours and only with seconds precission
  /// </summary>
  public class Time {}

  /// <summary>
  /// Stores only 2 digits after comma
  /// </summary>
  public class Decimal2 { }


  /// <summary>
  /// Gives a class a plural name which is different from just appending an s per default. Example: Activity => Activities
  /// </summary>
  public class StorageClassAttribute: Attribute {
    public StorageClassAttribute(
      int maxLineLength = 0, 
      string? pluralName = null, 
      bool areItemsUpdatable = true, 
      bool areItemsDeletable = true, 
      bool isCompactDuringDispose = true){}
  }
}
