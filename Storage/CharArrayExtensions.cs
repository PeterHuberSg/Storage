using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public static class CharArrayExtensions {

    #region Integer
    //      -------

    public static void Write(this char[] charArray, int i, ref int index){
      int start;
      if (i<0) {
        charArray[index++] = '-';
        start = index;
        //since -int.MinValue is bigger than int.MaxValue, i=-i does not work of int.Minvalue.
        //therfore write 1 character first and guarantee that i>int.MinValue
        charArray[index++] = (char)(-(i % 10) + '0');
        i /= 10;
        if (i==0) return;
        i = -i;
      } else {
        start = index;
      }

      while (i>9) {
        charArray[index++] = (char)((i % 10) + '0');
        i /= 10;
      }
      charArray[index++] = (char)(i + '0');
      var end = index-1;
      while (end>start) {
        var temp = charArray[end];
        charArray[end--] = charArray[start];
        charArray[start++] = temp;
      }
    }


    /// <summary>
    /// Parse field into int
    /// </summary>
    public static int ReadInt(
      this char[] charArray, 
      ref int index, 
      int length,
      int lineStart, 
      int lineLength,
      string fieldName,
      StringBuilder errorStringBuilder) 
    {
      var startindex = index;
      if (index>=length) {
        if (index==length) {
          errorStringBuilder.AppendLine($"{fieldName} should be int, but was empty '' in line: '{new string(charArray[lineStart..lineLength])}'.");
        } else {
          errorStringBuilder.AppendLine($"{fieldName} invalid index {index}, field ends at {length} in line: '{new string(charArray[lineStart..lineLength])}'.");
        }
        return int.MinValue;
      }
      var i = 0;
      var isMinus = charArray[index]=='-';
      if (isMinus) {
        index++;
      }
      while (index<length) {
        var inChar = charArray[index++];
        if (inChar<'0' || inChar>'9') {
          errorStringBuilder.AppendLine($"{fieldName} should be int, but contained illegal character '{inChar}' in field '{new string(charArray[startindex..length])}' and line: '{new string(charArray[lineStart..lineLength])}'.");
          return int.MinValue;
        }
        i = 10*i + inChar - '0';
      }
      if (isMinus) {
        return -i;
      } else {
        return i;
      }
    }


    /// <summary>
    /// Parse field into int
    /// </summary>
    public static int? ReadIntNull(
      this char[] charArray,
      string fieldName,
      ref int index,
      int lenght,
      int lineStart,
      int lineLength,
      StringBuilder errorStringBuilder) 
    {
      if (lenght==0) return null;

      return ReadIntNull(charArray, fieldName, ref index, lenght, lineStart, lineLength, errorStringBuilder);
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

  }
}
