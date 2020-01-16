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
    /// How many bytes can a line max contain ? (25% of CsvConfig.BufferSize)
    /// </summary>
    public int MaxLineByteLenght { get; }

    /// <summary>
    /// Is file completly read ?
    /// </summary>
    public bool IsEof { get; private set; }

    /// <summary>
    /// Used to see read buffer content as string in debugger
    /// </summary>
    public string PresentContent { get { return this.GetPresentContent(); } }
    #endregion


    #region Constructor
    //      -----------
#pragma warning disable IDE0069 // Disposable fields should be disposed
    FileStream? fileStream;
#pragma warning restore IDE0069 // Disposable fields should be disposed
    readonly bool isFileStreamOwner;
    readonly byte[] byteArray;
    int readIndex;
    int endIndex;
    readonly int delimiter;
    readonly char[] tempChars;


    public CsvReader(
      string? fileName, 
      CsvConfig csvConfig, 
      int maxLineCharLenght, 
      FileStream? existingFileStream = null) 
    {
      if (!string.IsNullOrEmpty(fileName) && existingFileStream!=null) throw new Exception("CsvReader constructor: There was neither an existingFileStream nor a fileName proided.");

      if (existingFileStream!=null) {
        FileName = existingFileStream.Name;
      } else {
        FileName = fileName!;
      }
      CsvConfig = csvConfig;
      if (csvConfig.Encoding!=Encoding.UTF8)
        throw new Exception($"CsvReader constructor '{FileName}': Only reading from UTF8 files is supported, but the Encoding was {csvConfig.Encoding.EncodingName}.");

      delimiter = (int)csvConfig.Delimiter;
      //if (maxLineLenght>CsvConfig.BufferSize/Csv.LineToBufferRatio)
      //  throw new Exception($"CsvReader constructor '{FileName}': Buffersize {CsvConfig.BufferSize} should be at least {Csv.LineToBufferRatio} times bigger than MaxLineCharLenght {MaxLineCharLenght} for file {fileName}.");

      //MaxLineCharLenght = maxLineLenght;
      //MaxLineByteLenght = maxLineLenght * Csv.Utf8BytesPerChar;
      MaxLineByteLenght = CsvConfig.BufferSize/Csv.LineToBufferRatio;
      if (maxLineCharLenght*Csv.Utf8BytesPerChar>MaxLineByteLenght)
        throw new Exception($"CsvReader constructor: Buffersize {CsvConfig.BufferSize} should be at least {Csv.LineToBufferRatio} times bigger than MaxLineCharLenght {MaxLineCharLenght} for file {fileName}.");

      MaxLineCharLenght = maxLineCharLenght;
      if (existingFileStream is null) {
        isFileStreamOwner = true;
        if (string.IsNullOrEmpty(fileName)) throw new Exception("CsvReader constructor: File name is missing.");
        FileName = fileName;
        fileStream = new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
      } else {
        isFileStreamOwner = false;
        fileStream = existingFileStream;
        FileName = fileStream.Name;
      }
      byteArray = new byte[CsvConfig.BufferSize + MaxLineByteLenght];
      readIndex = 0;
      endIndex = 0;
      IsEof = false;
      tempChars = new char[MaxLineByteLenght/Csv.Utf8BytesPerChar];
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
      if (readIndex<endIndex) {
        return false;
      }
      if (endIndex<CsvConfig.BufferSize) {
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
      var remainingBytesCount = endIndex - readIndex;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception($"CsvReader.ReadEndOfLine() '{FileName}': premature EOF found:" + Environment.NewLine + GetPresentContent());
      }

      // test for Carriage Return
      if (byteArray[readIndex++]!=0x0D) { //carriage return) {
        throw new Exception($"CsvReader.ReadEndOfLine() '{FileName}': Carriage return missing: " + Environment.NewLine + GetPresentContent());
      }

      //test for line feed
      if (byteArray[readIndex++]!=0x0A) { //line feed) {
        throw new Exception($"CsvReader.ReadEndOfLine() '{FileName}': Line feed missing: " + Environment.NewLine + GetPresentContent());
      }
#if DEBUG
      if ((readIndex-lineStart)%CsvConfig.BufferSize > MaxLineByteLenght) throw new Exception();
      lineStart = readIndex;
#endif
    }


    public void SkipToEndOfLine() {
      while (true) {
        if (byteArray[readIndex++]==0x0D) { //carriage return) {
          if (byteArray[readIndex++]==0x0A) { //line feed) {
            return;
          } else {
            throw new Exception($"CsvReader.SkipToEndOfLine() '{FileName}': Line feed missing: " + Environment.NewLine + GetPresentContent());
          }
        }
      }
    }


    bool areAllBytesRead = false;


    private bool fillBufferFromFileStream(int remainingBytesCount) {
      if (areAllBytesRead) {
        return remainingBytesCount>0;
      }

      if (remainingBytesCount>0) {
        Array.Copy(byteArray, readIndex, byteArray, 0, remainingBytesCount);
      }
      readIndex = 0;
      var bytesRead = fileStream!.Read(byteArray, remainingBytesCount, CsvConfig.BufferSize);
      if (bytesRead<CsvConfig.BufferSize) areAllBytesRead = true;

      endIndex = remainingBytesCount + bytesRead;
      if (endIndex<=0) {
        IsEof = true;
        return false;
      }
      return true;
    }


    /// <summary>
    /// Read boolean as 0 or 1 from UTF8 filestream including delimiter.
    /// </summary>
    public bool ReadBool() {
      bool b;
      int readByteAsInt = (int)byteArray[readIndex++];
      if (readByteAsInt=='0') {
        b = false;
      } else if (readByteAsInt=='1') {
        b = true;
      } else {
        throw new Exception($"CsvReader.ReadBool() '{FileName}': Illegal character found: " + Environment.NewLine + GetPresentContent());
      }
      readByteAsInt = (int)byteArray[readIndex++];
      if (readByteAsInt!=CsvConfig.Delimiter) {
        throw new Exception($"CsvReader.ReadBool() '{FileName}': Illegal character found" + Environment.NewLine + GetPresentContent());
      }
      return b;
    }


    /// <summary>
    /// Read boolean? as '', 0 or 1 from UTF8 filestream including delimiter.
    /// </summary>
    public bool? ReadBoolNull() {
      int readByteAsInt = (int)byteArray[readIndex++];
      if (readByteAsInt==CsvConfig.Delimiter) {
        return null;
      }
      bool b;
      if (readByteAsInt=='0') {
        b = false;
      } else if (readByteAsInt=='1') {
        b = true;
      } else {
        throw new Exception($"CsvReader.ReadBool() '{FileName}': Illegal character found: " + Environment.NewLine + GetPresentContent());
      }
      readByteAsInt = (int)byteArray[readIndex++];
      if (readByteAsInt!=CsvConfig.Delimiter) {
        throw new Exception($"CsvReader.ReadBool() '{FileName}': Illegal character found" + Environment.NewLine + GetPresentContent());
      }
      return b;
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int ReadInt() {
      //check for minus sign
      int readByteAsInt = (int)byteArray[readIndex++];
      var isMinus = readByteAsInt=='-';
      if (isMinus) {
        readByteAsInt = (int)byteArray[readIndex++];
      }

      //read first digit. There must be at least 1
      var i = 0;
      if (readByteAsInt>='0' && readByteAsInt<='9') {
        i = 10*i + readByteAsInt - '0';
      } else {
        throw new Exception($"CsvReader.ReadInt() '{FileName}': Illegal character found: " + Environment.NewLine + GetPresentContent());
      }

      //read other digits until delimiter is reached
      while (true) {
        readByteAsInt = (int)byteArray[readIndex++];
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
        throw new Exception($"CsvReader.ReadInt() '{FileName}': Illegal character found: " + Environment.NewLine + GetPresentContent());
      }
    }


    /// <summary>
    /// Read integer from UTF8 filestream including delimiter.
    /// </summary>
    public int? ReadIntNull() {
      //check for minus sign
      int readByteAsInt = (int)byteArray[readIndex++];
      //check for null
      if (readByteAsInt==delimiter) {
        return null;
      }

      var isMinus = readByteAsInt=='-';
      if (isMinus) {
        readByteAsInt = (int)byteArray[readIndex++];
      }

      //read first digit. There must be at least 1
      var i = 0;
      if (readByteAsInt>='0' && readByteAsInt<='9') {
        i = 10*i + readByteAsInt - '0';
      } else {
        throw new Exception($"CsvReader '{FileName}': Illegal integer: " + Environment.NewLine + GetPresentContent());
      }

      //read other digits until delimiter is reached
      while (true) {
        readByteAsInt = (int)byteArray[readIndex++];
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
        throw new Exception($"CsvReader '{FileName}': Illegal integer: " + Environment.NewLine + GetPresentContent());
      }
    }


    /// <summary>
    /// Read long from UTF8 filestream including delimiter.
    /// </summary>
    public long ReadLong() {
      //check for minus sign
      int readByteAsInt = (int)byteArray[readIndex++];
      var isMinus = readByteAsInt=='-';
      if (isMinus) {
        readByteAsInt = (int)byteArray[readIndex++];
      }

      //read first digit. There must be at least 1
      var l = 0L;
      if (readByteAsInt>='0' && readByteAsInt<='9') {
        l = 10*l + readByteAsInt - '0';
      } else {
        throw new Exception($"CsvReader.ReadLong() '{FileName}': Illegal character found:" + Environment.NewLine + GetPresentContent());
      }

      //read other digits until delimiter is reached
      while (true) {
        readByteAsInt = (int)byteArray[readIndex++];
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
        throw new Exception($"CsvReader.ReadLong() '{FileName}': Illegal character found" + Environment.NewLine + GetPresentContent());
      }
    }


    /// <summary>
    /// Read decimal from UTF8 filestream including delimiter.
    /// </summary>
    public decimal ReadDecimal() {
      var tempCharsIndex = 0;
      while (true) {
        int readByteAsInt = (int)byteArray[readIndex++];
        if (readByteAsInt>=0x80) throw new Exception($"CsvReader.ReadDecimal() '{FileName}': Illegal character found:" + Environment.NewLine + GetPresentContent());

        if (readByteAsInt==delimiter) {
          var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
          return Decimal.Parse(tempCharsSpan);
        }
        tempChars[tempCharsIndex++] = (char)readByteAsInt;
      }
    }


    /// <summary>
    /// Read decimal or null from UTF8 filestream including delimiter.
    /// </summary>
    public decimal? ReadDecimalNull() {
      var tempCharsIndex = 0;
      while (true) {
        int readByteAsInt = (int)byteArray[readIndex++];
        if (readByteAsInt==delimiter) {
          if (tempCharsIndex==0) return null;
          var tempCharsSpan = new ReadOnlySpan<char>(tempChars, 0, tempCharsIndex);
          return Decimal.Parse(tempCharsSpan);
        }
        if (readByteAsInt>=0x80) throw new Exception($"CsvReader.ReadDecimal() '{FileName}': Illegal character found:" + Environment.NewLine + GetPresentContent());

        tempChars[tempCharsIndex++] = (char)readByteAsInt;
      }
    }


    public char ReadChar() {
      char returnChar;
      byte readByte = byteArray[readIndex++];
      if (readByte<0x80) {
        returnChar = (char)readByte;
        readByte = byteArray[readIndex++];
        if (readByte!=delimiter) throw new Exception($"CsvReader.ReadChar() '{FileName}': More than 1 character found: " + Environment.NewLine + GetPresentContent());
        return returnChar;

      } else {
        var startIndex = readIndex-1;
        do {
          readByte = byteArray[readIndex++];
        } while (readByte!=delimiter);
        var length = Encoding.UTF8.GetChars(byteArray, startIndex, readIndex-startIndex-1, tempChars, 0);
        if (length>1) throw new Exception($"CsvReader.ReadChar() '{FileName}': More than 1 character found: " + Environment.NewLine + GetPresentContent());
        return tempChars[0];
      }
    }


    /// <summary>
    /// Reads the very first character from a new line. It also ensures that enough bytes are in read from the file 
    /// that the whole line can be read. ReadLeadingLineChar() must be called before any other Readxxx() except ReadLine().
    /// </summary>
    public char ReadFirstLineChar() {
      var remainingBytesCount = endIndex - readIndex;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception($"CsvReader.ReadFirstLineChar() '{FileName}': Premature EOF found: " + Environment.NewLine + GetPresentContent());
      }

      char readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar>=0x80) throw new Exception($"CsvReader.ReadFirstLineChar() '{FileName}': Illegal character found: " + Environment.NewLine + GetPresentContent());

      return readByteAsChar;
    }


    public string? ReadString() {
      var tempCharsIndex = 0;
      var startReadIndex = readIndex;
      while (true) {
        var readByte = byteArray[readIndex++];
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
          while (true) {
            readChar = (char)byteArray[readIndex++];
            if (readChar==delimiter) {
              return Encoding.UTF8.GetString(byteArray, startReadIndex, readIndex-startReadIndex-1);
            }
          }
        }
      }
    }


    public DateTime ReadDate() {
      var day = (int)(byteArray[readIndex++] - '0');
      var readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar!='.') {
        day = day*10 + (int)(readByteAsChar - '0');
        if ((char)byteArray[readIndex++]!='.') throw new Exception($"CsvReader.ReadDate() '{FileName}': Day has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var month = (int)(byteArray[readIndex++] - '0');
      readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar!='.') {
        month = month*10 + (int)(readByteAsChar - '0');
        if ((char)byteArray[readIndex++]!='.') throw new Exception($"CsvReader.ReadDate() '{FileName}': Month has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var year = (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');

      if ((char)byteArray[readIndex++]!=CsvConfig.Delimiter) throw new Exception($"CsvReader.ReadDate() '{FileName}': delimiter not found after 4 characters for year: " + Environment.NewLine + GetPresentContent());

      return new DateTime(year, month, day);
    }


    public DateTime ReadDateSeconds() {
      var day = (int)(byteArray[readIndex++] - '0');
      var readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar!='.') {
        day = day*10 + (int)(readByteAsChar - '0');
        if ((char)byteArray[readIndex++]!='.') throw new Exception($"CsvReader.ReadDate() '{FileName}': Day has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var month = (int)(byteArray[readIndex++] - '0');
      readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar!='.') {
        month = month*10 + (int)(readByteAsChar - '0');
        if ((char)byteArray[readIndex++]!='.') throw new Exception($"CsvReader.ReadDate() '{FileName}': Month has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var year = (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');
      year = 10*year + (int)(byteArray[readIndex++] - '0');
      if ((char)byteArray[readIndex++]!=' ') throw new Exception($"CsvReader.ReadDateSeconds() '{FileName}': ' ' missing after date: " + Environment.NewLine + GetPresentContent());

      var timeSpan = ReadTime();

      return new DateTime(year, month, day) + timeSpan;
    }


    // 0: 0: 0 => "0"
    // 0: 0: 1 => "0:0:1"
    // 0: 1: 1 => "0:1:1"
    // 1: 1: 1 => "1:1:1"
    //23: 0: 0 => "23"
    //23:59: 0 => "23:59"
    //23:59:59 => "23:59:59"
    public TimeSpan ReadTime() {
      var hour = (int)(byteArray[readIndex++] - '0');
      var readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar==CsvConfig.Delimiter) return TimeSpan.FromHours(hour);

      if (readByteAsChar!=':') {
        hour = hour*10 + (int)(readByteAsChar - '0');
        readByteAsChar = (char)byteArray[readIndex++];
        if (readByteAsChar==CsvConfig.Delimiter) return TimeSpan.FromHours(hour);

        if (readByteAsChar!=':') throw new Exception($"CsvReader.ReadTime() '{FileName}': Hour has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var minute = (int)(byteArray[readIndex++] - '0');
      readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar==CsvConfig.Delimiter) return new TimeSpan(hour, minute, 0);
      
      if (readByteAsChar!=':') {
        minute = minute*10 + (int)(readByteAsChar - '0');
        readByteAsChar = (char)byteArray[readIndex++];
        if (readByteAsChar==CsvConfig.Delimiter) return new TimeSpan(hour, minute, 0);
        
        if (readByteAsChar!=':') throw new Exception($"CsvReader.ReadTime() '{FileName}': Minute has more than 2 chars: " + Environment.NewLine + GetPresentContent());
      }

      var second = (int)(byteArray[readIndex++] - '0');
      readByteAsChar = (char)byteArray[readIndex++];
      if (readByteAsChar==CsvConfig.Delimiter) return new TimeSpan(hour, minute, second);

      if (readByteAsChar!=':') {
        second = second*10 + (int)(readByteAsChar - '0');
        readByteAsChar = (char)byteArray[readIndex++];
        if (readByteAsChar==CsvConfig.Delimiter) return new TimeSpan(hour, minute, second);
      }

      throw new Exception($"CsvReader.ReadTime() '{FileName}': Second has more than 2 chars: " + Environment.NewLine + GetPresentContent());
    }


    public DateTime ReadDateTimeTicks() {
      var ticks = ReadLong();
      return new DateTime(ticks);
    }


    /// <summary>
    /// reads one complete line as string. ReadLine() should be avoided, because of the string creation overhead.
    /// </summary>
    public string ReadLine() {
      var remainingBytesCount = endIndex - readIndex;
      if (remainingBytesCount<=MaxLineByteLenght) {
        if (!fillBufferFromFileStream(remainingBytesCount)) throw new Exception($"CsvReader.ReadLine() '{FileName}': Premature EOF found: " + Environment.NewLine + GetPresentContent());
      }

      var tempCharsIndex = 0;
      var startReadIndex = readIndex;
      while (true) {
        var readByteAsChar = (char)byteArray[readIndex++];
        if (readByteAsChar==0x0D) { //carriage return) {
          if (byteArray[readIndex++]==0x0A) { //line feed) {
            return new string(tempChars, 0, tempCharsIndex);
          } else {
            throw new Exception($"CsvReader.ReadLine() '{FileName}': Line feed missing after carriage return: " + Environment.NewLine + GetPresentContent());
          }
        }
        if (readByteAsChar<0x80) {
          tempChars[tempCharsIndex++] = readByteAsChar;
        } else {
          while (true) {
            readByteAsChar = (char)byteArray[readIndex++];
            if (readByteAsChar==0x0D) { //carriage return) {
              if (byteArray[readIndex++]==0x0A) { //line feed) {
                return Encoding.UTF8.GetString(byteArray, startReadIndex, readIndex-startReadIndex-2);
              } else {
                throw new Exception($"CsvReader.ReadLine() '{FileName}': Line feed missing after carriage return: " + Environment.NewLine + GetPresentContent());
              }
            }
          }
        }
      }
    }
    #endregion


    public string GetPresentContent() {
      int fromPos;
      if (readIndex>100) {
        fromPos = readIndex - 100;
      } else {
        fromPos = 0;
      }
      var presentPos = readIndex - fromPos;

      int toPos = readIndex + 30;
      toPos = Math.Max(toPos, endIndex);
      var byteString = UTF8Encoding.UTF8.GetString(byteArray, fromPos, toPos-fromPos+1).Replace(CsvConfig.Delimiter, '|');
      return byteString[..presentPos] + '^' + byteString[presentPos..];
    }
  }
}
