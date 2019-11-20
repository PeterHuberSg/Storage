using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Storage {


  public static class FileStreamExtensions {


    #region Integer
    //      -------

    ///// <summary>
    ///// Write i to filestream as UTF8 bytes including delimiter
    ///// </summary>
    //public static void Write(this FileStream filestream, int i, int delimiter) {
    //  Span<byte> bytes = stackalloc byte[maxLengthInt];
    //  var index = 0;
    //  if (i<0) {
    //    filestream.WriteByte((byte)'-');
    //    //since -int.MinValue is bigger than int.MaxValue, i=-i does not work of int.Minvalue.
    //    //therfore write 1 character first and guarantee that i>int.MinValue
    //    bytes[index++] = (byte)(-(i % 10) + '0');
    //    i /= 10;
    //    if (i==0) {
    //      filestream.WriteByte(bytes[0]);
    //      filestream.WriteByte((byte)delimiter);
    //      return;
    //    }
    //    i = -i;
    //  }

    //  while (i>9) {
    //    bytes[index++] = (byte)((i % 10) + '0');
    //    i /= 10;
    //  }
    //  bytes[index] = (byte)(i + '0');

    //  for (; index >= 0; index--) {
    //    filestream.WriteByte(bytes[index]);
    //  }
    //  filestream.WriteByte((byte)delimiter);
    //}


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter. Returns IsEof=true, if there is nothing left to read. If the EOF happened
    /// when reading the first character, no error message gets added to errorStringBuilder. 
    /// </summary>
    public static int ReadInt(
      this byte[] byteArray,
      int Index,
      int lenght,
      string fieldName,
      StringBuilder errorStringBuilder) 
    {
      var i = 0;
      var isMinus = false;
      var isFirstByte = true;

      while (Index<lenght) {
        var readByte = byteArray[Index++];
        if (isFirstByte) {
          isFirstByte = false;
          if (readByte=='-') {
            isMinus = true;
            continue;
          }
        }
        if (readByte<'0' || readByte>'9') {
          errorStringBuilder.AppendLine($"{fieldName} illegal character '{(char)readByte}' encountered while readin integer {(isMinus ? -i : i)}.");
          return int.MinValue;
        }
        i = 10*i + readByte - '0';
      }
      if (isMinus) {
        return -i;
      } else {
        return i;
      }
    }
    #endregion


    #region Integer Rad byte
    //      -------

    const int maxLengthInt = 10;


    /// <summary>
    /// Write i to filestream as UTF8 bytes including delimiter
    /// </summary>
    public static void Write(this FileStream filestream, int i, int delimiter) {
      Span<byte> bytes = stackalloc byte[maxLengthInt];
      var index = 0;
      if (i<0) {
        filestream.WriteByte((byte)'-');
        //since -int.MinValue is bigger than int.MaxValue, i=-i does not work of int.Minvalue.
        //therfore write 1 character first and guarantee that i>int.MinValue
        bytes[index++] = (byte)(-(i % 10) + '0');
        i /= 10;
        if (i==0) {
          filestream.WriteByte(bytes[0]);
          filestream.WriteByte((byte)delimiter);
          return;
        }
        i = -i;
      }

      while (i>9) {
        bytes[index++] = (byte)((i % 10) + '0');
        i /= 10;
      }
      bytes[index] = (byte)(i + '0');

      for (; index >= 0; index--) {
        filestream.WriteByte(bytes[index]);
      }
      filestream.WriteByte((byte)delimiter);
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter. Returns IsEof=true, if there is nothing left to read. If the EOF happened
    /// when reading the first character, no error message gets added to errorStringBuilder. 
    /// </summary>
    public static (bool IsEof, int I) ReadInt(
      this FileStream filestream,
      int delimiter,
      string fieldName,
      StringBuilder errorStringBuilder) {
      var i = 0;
      var isMinus = false;
      var isFirstbyte = true;

      while (true) {
        var readByte = filestream.ReadByte();
        if (readByte<0) {
          if (!isFirstbyte) {
            errorStringBuilder.AppendLine($"{fieldName}: EOF encountered while reading integer {(isMinus ? -i : i)} from file {filestream.Name}.");
          }
          return (true, int.MinValue);
        }
        if (readByte==delimiter) {
          if (isMinus) {
            return (false, -i);
          } else {
            return (false, i);
          }
        }
        if (isFirstbyte) {
          isFirstbyte = false;
          if (readByte=='-') {
            isMinus = true;
            continue;
          }
        }
        if (readByte<'0' || readByte>'9') {
          errorStringBuilder.AppendLine($"{fieldName} illegal character '{(char)readByte}' encountered while readin integer {(isMinus ? -i : i)} from file {filestream.Name}.");
          while (readByte!=delimiter) {
            readByte = filestream.ReadByte();
            if (readByte<0) return (true, int.MinValue);
          }
          return (false, int.MinValue);
        }
        i = 10*i + readByte - '0';
      }
    }
    #endregion

  }
}
