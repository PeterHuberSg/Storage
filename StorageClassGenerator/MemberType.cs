using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {

  /// <summary>
  /// Data types supported by storage compiler
  /// </summary>
  public enum MemberTypeEnum {
    Undefined = 0,
    Date,
    Time,
    DateMinutes,
    DateSeconds,
    DateTime,
    Decimal,
    Decimal2,
    Decimal4,
    Decimal5,
    Bool,
    Int,
    List,
    Dictionary,
    Parent,
    String,
    Enum,
    Lenght
  }


  //extension methods for MemberTypeEnum
  public static class MemberTypeExtensions {
    

    static string?[] baseTypeStrings = {
    null, //Undefined
    "DateTime", //Date,
    "DateTime", //Time,
    "DateTime", //DateMinutes,
    "DateTime", //DateSeconds,
    "DateTime", //DateTime,
    "Decimal", //Decimal,
    "Decimal", //Decimal2,
    "Decimal", //Decimal4,
    "Decimal", //Decimal5,
    "bool", //Bool,
    "int", //Int,
    null, //List,
    null, //Dictionary,
    null, //Parent,
    "string", //String,
    null, //Enum,
    };


    /// <summary>
    /// Converts a specialised CSV type to a .NET type, like Decimal2 to Decimal or DateMinutes to DateTime
    /// </summary>
    public static string? ToBaseType(this MemberTypeEnum memberType) {
      return baseTypeStrings[(int)memberType];
    }
  }
}
