using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  /// <summary>
  /// Contains static helper methods for CommaSeparateValue file processing, like reading data from csv files
  /// </summary>
  public static class Csv {


    #region Parsing
    //      -------

    #region Integer
    //      -------

    /// <summary>
    /// Parse field into int
    /// </summary>
    public static int ParseInt(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (int.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be int, but was '" + field + "' in line: '" + line + "'.");
      return int.MinValue;
    }


    public static int ParseInt(String fieldName, String line, int lineLength, ref int charIndex, char delimiter, ref Boolean isLineError, StringBuilder errorStringBuilder) {
      if (isLineError) return int.MinValue;

      var number = 0;
      var isFoundNumber = false;
      var isNegative = 1;
      while (charIndex<lineLength) {
        var c = line[charIndex++];
        if (c>='0' && c<='9') {
          isFoundNumber = true;
          number = number * 10 + (c - '0');
        } else if (!isFoundNumber && c=='-') {
          isNegative = -1;
        } else if (c==delimiter && isFoundNumber) {
          return isNegative * number;
        } else {
          break;
        }
      };

      isLineError = true;
      errorStringBuilder.AppendLine(fieldName + " should be int in line '" + line + "' at position " + charIndex + ".");
      return int.MinValue;
    }



    /// <summary>
    /// Parse field into int?
    /// </summary>
    public static int? ParseIntNull(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (field.Length==0)
        return null;

      if (int.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be int?, but was '" + field + "' in line: '" + line + "'.");
      return null;
    }
    #endregion


    #region Long
    //      ----

    /// <summary>
    /// Parse field into long
    /// </summary>
    public static long ParseLong(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (long.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be long, but was '" + field + "' in line: '" + line + "'.");
      return long.MinValue;
    }
    #endregion


    #region Bool
    //      ----


    /// <summary>
    /// Parse field into bool
    /// </summary>
    public static bool ParseBoolYN(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (field=="y")
        return true;
      if (field=="n")
        return false;

      errorStringBuilder.AppendLine(fieldName + " should be 'y' or 'n' (boolean), but was '" + field + "' in line: '" + line + "'.");
      return false;
    }


    /// <summary>
    /// Parse y/n field into bool?
    /// </summary>
    public static bool? ParseBoolYNNull(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (field=="")
        return null;
      if (field=="y")
        return true;
      if (field=="n")
        return false;

      errorStringBuilder.AppendLine(fieldName + " should be 'y' or 'n' (boolean), but was '" + field + "' in line: '" + line + "'.");
      return null;
    }
    #endregion


    #region DateTime
    //      --------


    /// <summary>
    /// Parse field into DateTime
    /// </summary>
    public static DateTime ParseDateTime(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (DateTime.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be DateTime, but was '" + field + "' in line: '" + line + "'.");
      return DateTime.MinValue;
    }


    /// <summary>
    /// Parse field into DateTime?
    /// </summary>
    public static DateTime? ParseDateTimeNull(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (field.Length==0)
        return null;

      if (DateTime.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be DateTime, but was '" + field + "' in line: '" + line + "'.");
      return DateTime.MinValue;
    }


    enum dateStateEnum {
      day,
      month,
      year,
    }

    public static DateTime ParseDateTime(String fieldName, String line, Int32 lineLength, ref Int32 charIndex, Char delimiter, ref Boolean isLineError, StringBuilder errorStringBuilder) {
      if (isLineError) return default;

      var day = 0;
      var month = 0;
      var year = 0;
      var dateState = dateStateEnum.day;
      var digitCount = 0;
      var dotCount = 0;
      while (charIndex<lineLength) {
        var c = line[charIndex++];
        if (c>='0' && c<='9') {
          digitCount++;
          if (dateState==dateStateEnum.day) {
            day = day * 10 + (c - '0');
          } else if (dateState==dateStateEnum.month) {
            month = month * 10 + (c - '0');
          } else {
            year = year * 10 + (c - '0');
          }
        } else if (c=='.') {
          if (digitCount<1 || digitCount>2) {
            break;
          }
          digitCount = 0;
          dotCount++;
          dateState++;
        } else if (c==delimiter) {
          if (dotCount!=2 || (digitCount!=4 && digitCount!=2) || day==0 || month==0) {
            break;
          }
          if (digitCount==2) {
            year += 2000;
          }
          try {
            return new DateTime(year, month, day);

          } catch (Exception) {
            break;
          }
        } else {
          break;
        }
      };

      isLineError = true;
      errorStringBuilder.AppendLine(fieldName + " should be DateTime in line: '" + line + "' at position " + charIndex + ".");
      return default;
    }
    #endregion


    #region Decimal
    //      -------


    /// <summary>
    /// Parse field into Decimal
    /// </summary>
    public static Decimal ParseDecimal(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (Decimal.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be DateTime, but was '" + field + "' in line: '" + line + "'.");
      return Decimal.MinValue;
    }


    const int maxDigitsInt = 9;
    const int maxDigitsLong = 19;


    /// <summary>
    /// Parse field into Decimal, faster implementation for numbers with less than 19 digits
    /// </summary>
    public static Decimal ParseDecimalFast(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      int number = 0;
      long numberLong = 0;
      int offset = 0;
      int length = field.Length;
      var isFoundNumber = false;
      var isNegative = false;
      var isScale = false;
      var isLongNumber = false;
      byte scale = 0;
      int maxDigits = maxDigitsInt;
      if (length>maxDigitsLong) {
        if (Decimal.TryParse(field, out decimal value))
          return value;
      } else {
        while (offset<length) {
          var c = field[offset];
          offset++;
          if (c>='0' && c<='9') {
            isFoundNumber = true;
            if (isScale) {
              scale++;
            }
            if (isLongNumber) {
              numberLong = unchecked(numberLong * 10 + (c - '0'));
              if (offset==length) {
                int low = (int)(numberLong);
                int high = (int)(numberLong>>32);
                return new Decimal(low, high, 0, isNegative, scale);
              }
              if (offset>maxDigits) {
                break;
              }
            } else {
              number = unchecked(number * 10 + (c - '0'));
              if (offset==length) {
                return new Decimal(number, 0, 0, isNegative, scale);
              }
              if (offset>=maxDigits) {
                numberLong = number;
                isLongNumber = true;
                maxDigits += 10;
              }
            }
          } else if (c=='-' && !isFoundNumber && !isNegative) {
            isNegative = true;
            maxDigits++;
          } else if (c=='.' && !isScale) {
            isScale = true;
            maxDigits++;
          } else {
            break;
          }
        };
      }

      errorStringBuilder.AppendLine(fieldName + " should be Decimal, but was '" + field + "' in line: '" + line + "'.");
      return Decimal.MinValue;
    }


    /// <summary>
    /// Parse field into Decimal?
    /// </summary>
    public static Decimal? ParseDecimalNull(string fieldName, string field, string line, StringBuilder errorStringBuilder) {
      if (field.Length==0)
        return null;

      if (Decimal.TryParse(field, out var value))
        return value;

      errorStringBuilder.AppendLine(fieldName + " should be Decimal?, but was '" + field + "' in line: '" + line + "'.");
      return null;
    }


    //const int maxM = 10;
    //static Stopwatch sw = new Stopwatch();
    //static int i = 0;
    //static int j = 0;
    //static long[,] ts = new long[maxM,3];


    //public static Decimal ParseDecimal(String fieldName, String line, int lineLength, ref int charIndex, char delimiter, ref bool isLineError, StringBuilder errorStringBuilder) {
    //  //sw.Restart();
    //  if (isLineError) return decimal.MinValue;

    //  decimal number = 0;
    //  var isFoundNumber = false;
    //  decimal negativeMultiplier = 1;
    //  decimal fractionMultiplier = 1;
    //  //var t = sw.ElapsedTicks; ts[i, 0] = t; sw.Restart();
    //  //t = sw.ElapsedTicks; ts[i, 1] = t; sw.Restart();
    //  while (charIndex<lineLength) {
    //    var c = line[charIndex++];
    //    if (c>='0' && c<='9') {
    //      isFoundNumber = true;
    //      if (fractionMultiplier==1) {
    //        number = number * 10 + (c - '0');
    //      } else {
    //        number += (c - '0') * fractionMultiplier;
    //        fractionMultiplier /= 10;
    //      }
    //    } else if (!isFoundNumber && c=='-') {
    //      negativeMultiplier = -1;
    //    } else if (fractionMultiplier==1 && c=='.') {
    //      fractionMultiplier = 0.1m;
    //    } else if (c==delimiter && isFoundNumber) {
    //      //t = sw.ElapsedTicks; ts[i, 2] = t;
    //      //if (j % 100000==0) {
    //      //  i++;
    //      //}
    //      //j++;
    //      //if (i==10) {
    //      //  var f = Stopwatch.Frequency / 1000000.0;
    //      //  var sb = new StringBuilder();
    //      //  for (int ix = 0; ix < maxM; ix++) {
    //      //    sb.AppendLine($"{ix}: {ts[ix, 0]}, {ts[ix, 1]}, {ts[ix, 2]}");
    //      //  }
    //      //  var s = sb.ToString();
    //      //}
    //      return negativeMultiplier * number;
    //    } else {
    //      break;
    //    }
    //  };

    //  isLineError = true;
    //  errorStringBuilder.AppendLine(fieldName + " should be decmimal in line '" + line + "' at position " + charIndex + ".");
    //  return Decimal.MinValue;
    //}


    //public static Decimal? ParseDecimalNull(String fieldName, String line, int lineLength, ref int charIndex, Char delimiter, ref bool isLineError, StringBuilder errorStringBuilder) {
    //  if (isLineError) return null;

    //  if (charIndex>=lineLength) {
    //    isLineError = true;
    //    errorStringBuilder.AppendLine(fieldName + " is empty in line '" + line + "' at position " + charIndex + ".");
    //    return null;
    //  }

    //  if (line[charIndex]==delimiter) {
    //    charIndex++;
    //    return null;
    //  }

    //  return ParseDecimal(fieldName, line, lineLength, ref charIndex, delimiter, ref isLineError, errorStringBuilder);
    //}
    #endregion


    #region String
    //      ------

    public static String ParseString(String fieldName, String line, Int32 lineLength, ref Int32 charIndex, Char delimiter, ref Boolean isLineError, StringBuilder errorStringBuilder) {
      if (isLineError) return "";

      var startCharIndex = charIndex;
      while (charIndex<lineLength) {
        var c = line[charIndex++];
        if (c==delimiter) {
          if (charIndex==startCharIndex) return "";

          return line.Substring(startCharIndex, charIndex-startCharIndex - 1);
        }
      };

      isLineError = true;
      errorStringBuilder.AppendLine(fieldName + " is empty at position " + charIndex + " in line: '" + line + "'.");
      return "";
    }
    #endregion

    #endregion


    #region AreEqual
    //      --------

    //When reading back a null string from a CVS file, it becomes an empty string.

    /// <summary>
    /// Tests if both strings are equal. If one is null and the other is an empty string, they are considered equal, because 
    /// when reading back a null string from a CVS file, it becomes an empty string.
    /// </summary>
    public static bool AreEqual(string thisString, string thatString) {
      if (thisString==thatString) return true;
      if ((thisString==null || thisString=="") && (thatString==null || thatString=="")) return true;
      return false;
    }

    #endregion


    #region other helpers
    //      -------------

    public static bool SplitRecord(string line, char[] delimiters, int length, StringBuilder errorStringBuilder, string typeName, out string[] fields) {
      fields = line.Split(delimiters);
      if (fields.Length-1!=length) {
        errorStringBuilder.AppendLine(typeName + " should have " + length + " fields, but had " + (fields.Length-1) + ": '" + line + "'.");
        return false;
      }
      return true;
    }


    //internal static string ToPathFileName(CsvConfig csvConfig, string name) {
    //  return csvConfig.DirectoryPath + @"\"  + name + ".csv";
    //}


    public static string ToCsvHeaderString(string[] headers, char delimiter) {
      string csvHeaderString = "";
      foreach (string header in headers) {
        csvHeaderString += header + delimiter;
      }
      return csvHeaderString;
    }

    #endregion
  }
}
