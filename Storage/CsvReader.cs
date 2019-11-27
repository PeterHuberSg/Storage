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
    /// How many chars can a line max contain ?
    /// </summary>
    public int MaxLineCharLenght { get; }

    /// <summary>
    /// How many bytes can a line max contain ?
    /// </summary>
    public int MaxLineByteLenght { get; }


    public bool IsEof { get; private set; }
    #endregion


    #region Constructor
    //      -----------
#pragma warning disable IDE0069 // Disposable fields should be disposed
    FileStream? fileStream;
#pragma warning restore IDE0069 // Disposable fields should be disposed
    readonly bool isFileStreamOwner;
    readonly byte[] byteArray;
    int readPos;
    int endPos;
    readonly int delimiter;
    readonly byte[] tempBytes;
    readonly char[] tempChars;


    public CsvReader(string? fileName, CsvConfig csvConfig, int maxLineLenght, FileStream? existingFileStream = null) {
      if (!string.IsNullOrEmpty(fileName) && existingFileStream!=null) throw new Exception();

      CsvConfig = csvConfig;
      if (csvConfig.Encoding!=Encoding.UTF8) 
        throw new Exception($"Only reading from UTF8 files is supported, but the Encoding was {csvConfig.Encoding.EncodingName}.");
      
      delimiter = (int)csvConfig.Delimiter;
      if (maxLineLenght>CsvConfig.BufferSize/Csv.LineToBufferRatio)
        throw new Exception($"Buffersize {CsvConfig.BufferSize} should be at least {Csv.LineToBufferRatio} times bigger than MaxLineCharLenght {MaxLineCharLenght} for file {fileName}.");

      MaxLineCharLenght = maxLineLenght;
      MaxLineByteLenght = maxLineLenght * Csv.Utf8BytesPerChar;
      if (existingFileStream is null) {
        isFileStreamOwner = true;
        if (string.IsNullOrEmpty(fileName)) throw new Exception();
        FileName = fileName;
        fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
      } else {
        isFileStreamOwner = false;
        fileStream = existingFileStream;
        FileName = fileStream.Name;
      }
      byteArray = new byte[CsvConfig.BufferSize + MaxLineByteLenght];
      readPos = 0;
      endPos = 0;
      IsEof = false;
      tempBytes = new byte[MaxLineByteLenght];
      tempChars = new char[maxLineLenght];
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
      if (!isFileStreamOwner) return;

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
      if (!fillBufferFromFileStream(0)) {
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
      var remainingBytesCount = endPos - readPos;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      }

      // test for Carriage Return
      if (byteArray[readPos++]!=0x0D) { //carriage return) {
        throw new Exception();
      }

      //test for line feed
      if (byteArray[readPos++]!=0x0A) { //line feed) {
        throw new Exception();
      }
#if DEBUG
      if ((readPos-lineStart)%CsvConfig.BufferSize > MaxLineByteLenght) throw new Exception();
      lineStart = readPos;
#endif
    }


    public void SkipToEndOfLine() {
      while (true) {
        if (byteArray[readPos++]==0x0D) { //carriage return) {
          if (byteArray[readPos++]==0x0A) { //line feed) {
            return;
          } else {
            throw new Exception();
          }
        }
      }
      // test for Carriage Return

      //test for line feed
    }


    bool areAllBytesRead = false;


    private bool fillBufferFromFileStream(int remainingBytesCount) {
      if (areAllBytesRead) {
        return remainingBytesCount>0;
      }

      if (remainingBytesCount>0) {
        Array.Copy(byteArray, readPos, byteArray, 0, remainingBytesCount);
      }
      readPos = 0;
      var bytesRead = fileStream!.Read(byteArray, remainingBytesCount, CsvConfig.BufferSize);
      if (bytesRead<CsvConfig.BufferSize) areAllBytesRead = true;

      endPos = remainingBytesCount + bytesRead;
      if (endPos<=0) {
        IsEof = true;
        return false;
      }
      return true;
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int ReadInt() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

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


    ///// <summary>
    ///// Read integer from UTF8 filestream including delimiter.
    ///// </summary>
    //public int ReadInt() {
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  if (byteLength<MaxLineLenght) {
    //    //maybe not enough bytes in the buffer, need to check before each read

    //    //check for minus sign
    //    int readByteAsInt = (int)byteArray[readPos++];
    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var i = 0;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      i = 10*i + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        i = 10*i + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -i;
    //        } else {
    //          return i;
    //        }
    //      }
    //      throw new Exception();
    //    }

    //  } else {
    //    //enough bytes in the buffer, no need to check before each read
    //    //check for minus sign
    //    int readByteAsInt = (int)byteArray[readPos++];
    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var i = 0;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      i = 10*i + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        i = 10*i + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -i;
    //        } else {
    //          return i;
    //        }
    //      }
    //      throw new Exception();
    //    }
    //  }
    //}


    ///// <summary>
    ///// Read integer from UTF8 filestream including delimiter.
    ///// </summary>
    //public int? ReadIntNull() {
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  if (byteLength<MaxLineLenght) {
    //    //maybe not enough bytes in the buffer, need to check before each read

    //    int readByteAsInt = (int)byteArray[readPos++];
    //    //check for null
    //    if (readByteAsInt==delimiter) {
    //      return null;
    //    }
    //    //check for minus sign
    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var i = 0;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      i = 10*i + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        i = 10*i + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -i;
    //        } else {
    //          return i;
    //        }
    //      }
    //      throw new Exception();
    //    }

    //  } else {
    //    //enough bytes in the buffer, no need to check before each read
    //    //check for minus sign
    //    int readByteAsInt = (int)byteArray[readPos++];
    //    //check for null
    //    if (readByteAsInt==delimiter) {
    //      return null;
    //    }

    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var i = 0;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      i = 10*i + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        i = 10*i + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -i;
    //        } else {
    //          return i;
    //        }
    //      }
    //      throw new Exception();
    //    }
    //  }
    //}


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int? ReadIntNull() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

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

    ///// <summary>
    ///// Read long from UTF8 filestream including delimiter.
    ///// </summary>
    //public long ReadLong() {
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  if (byteLength<MaxLineLenght) {
    //    //maybe not enough bytes in the buffer, need to check before each read

    //    //check for minus sign
    //    int readByteAsInt = (int)byteArray[readPos++];
    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var l = 0L;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      l = 10*l + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        l = 10*l + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -l;
    //        } else {
    //          return l;
    //        }
    //      }
    //      throw new Exception();
    //    }

    //  } else {
    //    //enough bytes in the buffer, no need to check before each read
    //    //check for minus sign
    //    int readByteAsInt = (int)byteArray[readPos++];
    //    var isMinus = readByteAsInt=='-';
    //    if (isMinus) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //    }

    //    //read first digit. There must be at least 1
    //    var l = 0L;
    //    if (readByteAsInt>='0' && readByteAsInt<='9') {
    //      l = 10*l + readByteAsInt - '0';
    //    }

    //    //read other digits until delimiter is reached
    //    while (true) {
    //      readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>='0' && readByteAsInt<='9') {
    //        l = 10*l + readByteAsInt - '0';
    //        continue;
    //      }

    //      if (readByteAsInt==delimiter) {
    //        if (isMinus) {
    //          return -l;
    //        } else {
    //          return l;
    //        }
    //      }
    //      throw new Exception();
    //    }
    //  }
    //}


    /// <summary>
    /// Read long from UTF8 filestream including delimiter.
    /// </summary>
    public long ReadLong() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

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


    ///// <summary>
    ///// Read long from UTF8 filestream including delimiter.
    ///// </summary>
    //public decimal ReadDecimal() {
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  if (byteLength<MaxLineLenght) {
    //    //maybe not enough bytes in the buffer, need to check before each read
    //    var tempCharsIndex = 0;
    //    while (true) {
    //      int readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>=0x80) throw new Exception();

    //      if (readByteAsInt==delimiter) {
    //        var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
    //        //return Decimal.Parse(tempCharsSpan);
    //        var sw = new Stopwatch();
    //        sw.Restart();
    //        var d = Decimal.Parse(tempCharsSpan);
    //        sw.Stop();
    //        return d;
    //      }
    //      tempChars[tempCharsIndex++] = (char)readByteAsInt;
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //    }

    //  } else {
    //    //enough bytes in the buffer, no need to check before each read
    //    var tempCharsIndex = 0;
    //    while (true) {
    //      int readByteAsInt = (int)byteArray[readPos++];
    //      if (readByteAsInt>=0x80) throw new Exception();

    //      if (readByteAsInt==delimiter) {
    //        var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
    //        return Decimal.Parse(tempCharsSpan);
    //      }
    //      tempChars[tempCharsIndex++] = (char)readByteAsInt;
    //    }
    //  }
    //}


    /// <summary>
    /// Read long from UTF8 filestream including delimiter.
    /// </summary>
    public decimal ReadDecimal() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

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


    //public char ReadChar() {
    //  char returnChar;
    //  if (readPos>=endPos) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //  }
    //  byte readByte = byteArray[readPos++];
    //  if (readByte<0x80) {
    //    returnChar = (char)readByte;
    //    if (readPos>=endPos) {
    //      if (!fillBufferFromFileStream()) throw new Exception();
    //    }
    //    readByte = byteArray[readPos++];
    //    if (readByte!=delimiter) throw new Exception();
    //    return returnChar;

    //  } else {
    //    var charBytesIndex = 0;
    //    do {
    //      tempBytes[charBytesIndex++] = readByte;
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      readByte = byteArray[readPos++];
    //    } while (readByte!=delimiter);
    //    var length = Encoding.UTF8.GetChars(tempBytes, 0, charBytesIndex, tempChars, 0);
    //    if (length>1) throw new Exception();
    //    return tempChars[0];
    //  }
    //}


    public char ReadChar() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

      char returnChar;
      byte readByte = byteArray[readPos++];
      if (readByte<0x80) {
        returnChar = (char)readByte;
        readByte = byteArray[readPos++];
        if (readByte!=delimiter) throw new Exception();
        return returnChar;

      } else {
        var charBytesIndex = 0;
        do {
          tempBytes[charBytesIndex++] = readByte;
          readByte = byteArray[readPos++];
        } while (readByte!=delimiter);
        var length = Encoding.UTF8.GetChars(tempBytes, 0, charBytesIndex, tempChars, 0);
        if (length>1) throw new Exception();
        return tempChars[0];
      }
    }


    /// <summary>
    /// Reads the very first character from a new line. It also ensures that enough bytes are in read from the file 
    /// that the whole line can be read. ReadLeadingLineChar() must be called before any other Readxxx() except ReadLine().
    /// </summary>
    public char ReadFirstLineChar() {
      var remainingBytesCount = endPos - readPos;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      }

      char readByteAsChar = (char)byteArray[readPos++];
      if (readByteAsChar>=0x80) throw new Exception();

      return readByteAsChar;
    }


    //public string? ReadString() {
    //  var tempCharsIndex = 0;
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  if (byteLength<MaxLineLenght) {
    //    //maybe not enough bytes in the buffer, need to check before each read
    //    while (true) {
    //      if (readPos>=endPos) {
    //        if (!fillBufferFromFileStream()) throw new Exception();
    //      }
    //      var readByte = byteArray[readPos++];
    //      var readChar = (char)readByte;
    //      if (readChar==delimiter) {
    //        if (tempCharsIndex==0) {
    //          return null;
    //        }
    //        return new string(tempChars, 0, tempCharsIndex);
    //      }
    //      if (readChar<0x80) {
    //        tempChars[tempCharsIndex++] = readChar;
    //      } else {
    //        var tempBytesIndex = 0;
    //        for (int tempCharsIndex2 = 0; tempCharsIndex2 < tempCharsIndex; tempCharsIndex2++) {
    //          tempBytes[tempBytesIndex++] = (byte)tempChars[tempCharsIndex2];
    //        }
    //        tempBytes[tempBytesIndex++] = readByte;
    //        while (true) {
    //          if (readPos>=endPos) {
    //            if (!fillBufferFromFileStream()) throw new Exception();
    //          }
    //          readByte = byteArray[readPos++];
    //          readChar = (char)readByte;
    //          if (readChar==delimiter) {
    //            return Encoding.UTF8.GetString(tempBytes, 0, tempBytesIndex);
    //          }
    //          tempBytes[tempBytesIndex++] = readByte;
    //        }
    //      }
    //    }

    //  } else {
    //    //enough bytes in the buffer, no need to check before each read
    //    var startReadPos = readPos;
    //    while (true) {
    //      var readByte = byteArray[readPos++];
    //      var readChar = (char)readByte;
    //      if (readChar==delimiter) {
    //        if (tempCharsIndex==0) {
    //          return null;
    //        }
    //        return new string(tempChars, 0, tempCharsIndex);
    //      }
    //      if (readChar<0x80) {
    //        tempChars[tempCharsIndex++] = readChar;
    //      } else {
    //        var tempBytesIndex = 0;
    //        //for (int tempCharsIndex2 = 0; tempCharsIndex2 < tempCharsIndex; tempCharsIndex2++) {
    //        //  tempBytes[tempBytesIndex++] = (byte)tempChars[tempCharsIndex2];
    //        //}
    //        //tempBytes[tempBytesIndex++] = readByte;
    //        var bytesCount = readPos-startReadPos;
    //        if (bytesCount>0) {
    //          Array.Copy(byteArray, startReadPos, tempBytes, 0, bytesCount);
    //          tempBytesIndex += bytesCount;
    //        }
    //        while (true) {
    //          readByte = byteArray[readPos++];
    //          readChar = (char)readByte;
    //          if (readChar==delimiter) {
    //            return Encoding.UTF8.GetString(tempBytes, 0, tempBytesIndex);
    //          }
    //          tempBytes[tempBytesIndex++] = readByte;
    //        }
    //      }
    //    }
    //  }
    //}


    public string? ReadString() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

      var tempCharsIndex = 0;
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


    //public DateTime ReadDate() {
    //  var byteLength = endPos - readPos;
    //  if (byteLength<=0) {
    //    if (!fillBufferFromFileStream()) throw new Exception();
    //    byteLength = endPos - readPos;
    //  }

    //  //if (byteLength<MaxLineLenght) {
    //  //  //maybe not enough bytes in the buffer, need to check before each read
    //  //  //if (readPos>=endPos) {
    //  //  //  if (!fillBufferFromFileStream()) throw new Exception();
    //  //  //}

    //  //} else {
    //    //enough bytes in the buffer, no need to check before each read
    //    var day = (int)(byteArray[readPos++] - '0');
    //    var readByteAsChar = (char)byteArray[readPos++];
    //    if (readByteAsChar!='.') {
    //      day = day*10 + (int)(readByteAsChar - '0');
    //      if ((char)byteArray[readPos++]!='.') throw new Exception();
    //    }

    //    var month = (int)(byteArray[readPos++] - '0');
    //    readByteAsChar = (char)byteArray[readPos++];
    //    if (readByteAsChar!='.') {
    //      month = month*10 + (int)(readByteAsChar - '0');
    //      if ((char)byteArray[readPos++]!='.') throw new Exception();
    //    }

    //    var year = (int)(byteArray[readPos++] - '0'); 
    //    year = 10*year + (int)(byteArray[readPos++] - '0');
    //    year = 10*year + (int)(byteArray[readPos++] - '0');
    //    year = 10*year + (int)(byteArray[readPos++] - '0');

    //    if ((char)byteArray[readPos++]!=CsvConfig.Delimiter) throw new Exception();

    //    return new DateTime(year, month, day);
    //  //}
    //}


    public DateTime ReadDate() {
      //var remainingBytesCount = endPos - readPos;
      //if (remainingBytesCount<=MaxLineByteLenght) {
      //  if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      //}

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


    /// <summary>
    /// reads one complete line as string. ReadLine() should be avoided, because of the string creation overhead.
    /// </summary>
    /// <returns></returns>
    public string ReadLine() {
      var remainingBytesCount = endPos - readPos;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception();
      }

      var tempCharsIndex = 0;
      var startReadPos = readPos;
      while (true) {
        var readByteAsChar = (char)byteArray[readPos++];
        if (readByteAsChar==0x0D) { //carriage return) {
          if (byteArray[readPos++]==0x0A) { //line feed) {
            return new string(tempChars, 0, tempCharsIndex);
          } else {
            throw new Exception();
          }
        }
        if (readByteAsChar<0x80) {
          tempChars[tempCharsIndex++] = readByteAsChar;
        } else {
          var tempBytesIndex = 0;
          var bytesCount = readPos-startReadPos;
          if (bytesCount>0) {
            Array.Copy(byteArray, startReadPos, tempBytes, 0, bytesCount);
            tempBytesIndex += bytesCount;
          }
          while (true) {
            var readByte = byteArray[readPos++];
            if (readByte==0x0D) { //carriage return) {
              if (byteArray[readPos++]==0x0A) { //line feed) {
                return Encoding.UTF8.GetString(tempBytes, 0, tempBytesIndex);
              } else {
                throw new Exception();
              }
            }
            tempBytes[tempBytesIndex++] = readByte;
          }
        }

      }
    }
    #endregion
  }
}
