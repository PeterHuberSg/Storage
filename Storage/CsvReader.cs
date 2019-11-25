using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Storage {


  public class CsvReader: IDisposable {

    #region Properties
    //      ----------

    public string FileName { get; }

    public CsvConfig CsvConfig { get; }

    /// <summary>
    /// How many chars can a line max contain ? int.MaxValue: don't use MaxLineLenght
    /// </summary>
    public int MaxLineLenght { get; }

    public bool IsEof { get; private set; }
    #endregion


    #region Constructor
    //      -----------
#pragma warning disable IDE0069 // Disposable fields should be disposed
    FileStream? fileStream;
#pragma warning restore IDE0069 // Disposable fields should be disposed
    readonly byte[] byteArray;
    int readPos;
    int endPos;
    readonly int delimiter;
    byte[] tempBytes;
    char[] tempChars;


    public CsvReader(string fileName, CsvConfig csvConfig, int maxLineLenght = int.MaxValue) {
      FileName = fileName;
      CsvConfig = csvConfig;
      if (csvConfig.Encoding!=Encoding.UTF8) {
        throw new Exception($"Only reading from UTF8 files is supported, but the Encoding was {csvConfig.Encoding.EncodingName}.");
      }
      delimiter = (int)csvConfig.Delimiter;
      MaxLineLenght = maxLineLenght;
      //fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
      fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan | FileOptions.WriteThrough);
      byteArray = new byte[CsvConfig.BufferSize];
      readPos = 0;
      endPos = 0;
      IsEof = false;
      if (maxLineLenght==int.MaxValue) {
        maxLineLenght = csvConfig.BufferSize;
      }
      tempBytes = new byte[maxLineLenght];
      tempChars = new char[maxLineLenght/2];
    }
    #endregion


    #region Disposable Interface
    //     ---------------------

    /// <summary>
    /// Executes disposal of CsvReader exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is CsvReader already exposed ?
    /// </summary>
    protected bool IsDisposed {
      get { return isDisposed==1; }
    }
    int isDisposed = 0;


    /// <summary>
    /// Inheritors should call Dispose(false) from their destructor
    /// </summary>
    protected void Dispose(bool disposing) {
      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
      if (wasDisposed==1) return; // already disposed

      OnDispose(disposing);
    }


    /// <summary>
    /// Inheritors should overwrite OnDispose() and put the diposal code in there. 
    /// </summary>
    /// <param name="disposing">is false if it is called from a destructor.</param>
    protected virtual void OnDispose(bool disposing) {
      releaseFileStream();
    }


    private void releaseFileStream() {
      var wasFileStream = Interlocked.Exchange(ref fileStream, null);//prevents that 2 threads release simultaneously
      if (wasFileStream!=null) {
        wasFileStream.Dispose();
      }
    }
    #endregion


    #region Methods
    //      -------

    public bool IsEndOfFileReached() {
      if (readPos<endPos) {
        return false;
      }
      if (endPos<CsvConfig.BufferSize) {
        IsEof = true;
        return true;
      }

      //in very rare cases the file fits exactly into the buffer. Read again to see if there are more bytes.
      if (!fillBufferFromFileStream()) {
        IsEof = true;
        return true;
      } else {
        return false;
      }
    }


#if DEBUG
    int lineStart = 0;
#endif


    public void ReadEndOfLine() {
      // test for Carriage Return
      if (readPos>=endPos) {
        if (!fillBufferFromFileStream()) throw new Exception();
      }
      if (byteArray[readPos++]!=0x0D) { //carriage return) {
        throw new Exception();
      }

      //test for line feed
      if (readPos>=endPos) {
        if (!fillBufferFromFileStream()) throw new Exception();
      }
      if (byteArray[readPos++]!=0x0A) { //line feed) {
        throw new Exception();
      }
#if DEBUG
      if ((readPos-lineStart)%CsvConfig.BufferSize > MaxLineLenght) throw new Exception();
      lineStart = readPos;
#endif
    }


    private bool fillBufferFromFileStream() {
      endPos = fileStream!.Read(byteArray, 0, CsvConfig.BufferSize);
      readPos = 0;
      if (endPos<0) {
        IsEof = true;
        return false;
      }
      return true;
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int ReadInt() {
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      if (byteLength<MaxLineLenght) {
        //maybe not enough bytes in the buffer, need to check before each read

        //check for minus sign
        int readByteAsInt = (int)byteArray[readPos++];
        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var i = 0;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          i = 10*i + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            i = 10*i + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -i;
            } else {
              return i;
            }
          }
          throw new Exception();
        }

      } else {
        //enough bytes in the buffer, no need to check before each read
        //check for minus sign
        int readByteAsInt = (int)byteArray[readPos++];
        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var i = 0;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          i = 10*i + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            i = 10*i + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -i;
            } else {
              return i;
            }
          }
          throw new Exception();
        }
      }
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int? ReadIntNull() {
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      if (byteLength<MaxLineLenght) {
        //maybe not enough bytes in the buffer, need to check before each read

        int readByteAsInt = (int)byteArray[readPos++];
        //check for null
        if (readByteAsInt==delimiter) {
          return null;
        }
        //check for minus sign
        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var i = 0;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          i = 10*i + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            i = 10*i + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -i;
            } else {
              return i;
            }
          }
          throw new Exception();
        }

      } else {
        //enough bytes in the buffer, no need to check before each read
        //check for minus sign
        int readByteAsInt = (int)byteArray[readPos++];
        //check for null
        if (readByteAsInt==delimiter) {
          return null;
        }

        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var i = 0;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          i = 10*i + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            i = 10*i + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -i;
            } else {
              return i;
            }
          }
          throw new Exception();
        }
      }
    }


    /// <summary>
    /// Read long from UTF8 filestream including delimiter.
    /// </summary>
    public long ReadLong() {
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      if (byteLength<MaxLineLenght) {
        //maybe not enough bytes in the buffer, need to check before each read

        //check for minus sign
        int readByteAsInt = (int)byteArray[readPos++];
        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var l = 0L;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          l = 10*l + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            l = 10*l + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -l;
            } else {
              return l;
            }
          }
          throw new Exception();
        }

      } else {
        //enough bytes in the buffer, no need to check before each read
        //check for minus sign
        int readByteAsInt = (int)byteArray[readPos++];
        var isMinus = readByteAsInt=='-';
        if (isMinus) {
          readByteAsInt = (int)byteArray[readPos++];
        }

        //read first digit. There must be at least 1
        var l = 0L;
        if (readByteAsInt>='0' && readByteAsInt<='9') {
          l = 10*l + readByteAsInt - '0';
        }

        //read other digits until delimiter is reached
        while (true) {
          readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>='0' && readByteAsInt<='9') {
            l = 10*l + readByteAsInt - '0';
            continue;
          }

          if (readByteAsInt==delimiter) {
            if (isMinus) {
              return -l;
            } else {
              return l;
            }
          }
          throw new Exception();
        }
      }
    }


    /// <summary>
    /// Read long from UTF8 filestream including delimiter.
    /// </summary>
    public decimal ReadDecimal() {
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      if (byteLength<MaxLineLenght) {
        //maybe not enough bytes in the buffer, need to check before each read
        var tempCharsIndex = 0;
        while (true) {
          int readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>=0x80) throw new Exception();

          if (readByteAsInt==delimiter) {
            var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
            //return Decimal.Parse(tempCharsSpan);
            var sw = new Stopwatch();
            sw.Restart();
            var d = Decimal.Parse(tempCharsSpan);
            sw.Stop();
            return d;
          }
          tempChars[tempCharsIndex++] = (char)readByteAsInt;
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
        }

      } else {
        //enough bytes in the buffer, no need to check before each read
        var tempCharsIndex = 0;
        while (true) {
          int readByteAsInt = (int)byteArray[readPos++];
          if (readByteAsInt>=0x80) throw new Exception();

          if (readByteAsInt==delimiter) {
            var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
            return Decimal.Parse(tempCharsSpan);
          }
          tempChars[tempCharsIndex++] = (char)readByteAsInt;
        }
      }
    }


    public char ReadChar() {
      char returnChar;
      if (readPos>=endPos) {
        if (!fillBufferFromFileStream()) throw new Exception();
      }
      byte readByte = byteArray[readPos++];
      if (readByte<0x80) {
        returnChar = (char)readByte;
        if (readPos>=endPos) {
          if (!fillBufferFromFileStream()) throw new Exception();
        }
        readByte = byteArray[readPos++];
        if (readByte!=delimiter) throw new Exception();
        return returnChar;

      } else {
        var charBytesIndex = 0;
        do {
          tempBytes[charBytesIndex++] = readByte;
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          readByte = byteArray[readPos++];
        } while (readByte!=delimiter);
        var length = Encoding.UTF8.GetChars(tempBytes, 0, charBytesIndex, tempChars, 0);
        if (length>1) throw new Exception();
        return tempChars[0];
      }
    }


    public string? ReadString() {
      var tempCharsIndex = 0;
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      if (byteLength<MaxLineLenght) {
        //maybe not enough bytes in the buffer, need to check before each read
        while (true) {
          if (readPos>=endPos) {
            if (!fillBufferFromFileStream()) throw new Exception();
          }
          var readByte = byteArray[readPos++];
          var readChar = (char)readByte;
          if (readChar==delimiter) {
            if (tempCharsIndex==0) {
              return null;
            }
            return new string(tempChars, 0, tempCharsIndex);
          }
          if (readChar<0x80) {
            tempChars[tempCharsIndex++] = readChar;
          } else {
            var tempBytesIndex = 0;
            for (int tempCharsIndex2 = 0; tempCharsIndex2 < tempCharsIndex; tempCharsIndex2++) {
              tempBytes[tempBytesIndex++] = (byte)tempChars[tempCharsIndex2];
            }
            tempBytes[tempBytesIndex++] = readByte;
            while (true) {
              if (readPos>=endPos) {
                if (!fillBufferFromFileStream()) throw new Exception();
              }
              readByte = byteArray[readPos++];
              readChar = (char)readByte;
              if (readChar==delimiter) {
                return Encoding.UTF8.GetString(tempBytes, 0, tempBytesIndex);
              }
              tempBytes[tempBytesIndex++] = readByte;
            }
          }
        }

      } else {
        //enough bytes in the buffer, no need to check before each read
        var startReadPos = readPos;
        while (true) {
          var readByte = byteArray[readPos++];
          var readChar = (char)readByte;
          if (readChar==delimiter) {
            if (tempCharsIndex==0) {
              return null;
            }
            return new string(tempChars, 0, tempCharsIndex);
          }
          if (readChar<0x80) {
            tempChars[tempCharsIndex++] = readChar;
          } else {
            var tempBytesIndex = 0;
            //for (int tempCharsIndex2 = 0; tempCharsIndex2 < tempCharsIndex; tempCharsIndex2++) {
            //  tempBytes[tempBytesIndex++] = (byte)tempChars[tempCharsIndex2];
            //}
            //tempBytes[tempBytesIndex++] = readByte;
            var bytesCount = readPos-startReadPos;
            if (bytesCount>0) {
              Array.Copy(byteArray, startReadPos, tempBytes, 0, bytesCount);
              tempBytesIndex += bytesCount;
            }
            while (true) {
              readByte = byteArray[readPos++];
              readChar = (char)readByte;
              if (readChar==delimiter) {
                return Encoding.UTF8.GetString(tempBytes, 0, tempBytesIndex);
              }
              tempBytes[tempBytesIndex++] = readByte;
            }
          }
        }
      }
    }


    public DateTime ReadDate() {
      var byteLength = endPos - readPos;
      if (byteLength<=0) {
        if (!fillBufferFromFileStream()) throw new Exception();
        byteLength = endPos - readPos;
      }

      //if (byteLength<MaxLineLenght) {
      //  //maybe not enough bytes in the buffer, need to check before each read
      //  //if (readPos>=endPos) {
      //  //  if (!fillBufferFromFileStream()) throw new Exception();
      //  //}

      //} else {
        //enough bytes in the buffer, no need to check before each read
        var day = (int)(byteArray[readPos++] - '0');
        var readByteAsChar = (char)byteArray[readPos++];
        if (readByteAsChar!='.') {
          day = day*10 + (int)(readByteAsChar - '0');
          if ((char)byteArray[readPos++]!='.') throw new Exception();
        }

        var month = (int)(byteArray[readPos++] - '0');
        readByteAsChar = (char)byteArray[readPos++];
        if (readByteAsChar!='.') {
          month = month*10 + (int)(readByteAsChar - '0');
          if ((char)byteArray[readPos++]!='.') throw new Exception();
        }

        var year = (int)(byteArray[readPos++] - '0'); 
        year = 10*year + (int)(byteArray[readPos++] - '0');
        year = 10*year + (int)(byteArray[readPos++] - '0');
        year = 10*year + (int)(byteArray[readPos++] - '0');

        if ((char)byteArray[readPos++]!=CsvConfig.Delimiter) throw new Exception();

        return new DateTime(year, month, day);
      //}
    }
    #endregion
  }
}
