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
    #endregion

  }
}
