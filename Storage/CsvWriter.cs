using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace Storage {


  public class CsvWriter: IDisposable {

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

    public bool IsAsciiOnly { get; }


    /// <summary>
    /// Dealy in millisecond before flush gets executed after the last write
    /// </summary>
    public readonly int FlushDelay;
    #endregion


    #region Constructor
    //      -----------
#pragma warning disable IDE0069 // Disposable fields should be disposed
    FileStream? fileStream;
#pragma warning restore IDE0069 // Disposable fields should be disposed
    readonly bool isFileStreamOwner;
    readonly byte[] byteArray;
    int writePos;
    readonly int maxBufferWriteLength;
    readonly byte delimiter;
    Timer? flushTimer;
    readonly byte[] tempBytes;
    readonly char[] tempChars;


    public CsvWriter(
      string fileName, 
      CsvConfig csvConfig, 
      int maxLineLenght, 
      FileStream? existingFileStream = null, 
      bool isAsciiOnly = true, 
      int flushDelay = 200) 
    {
      if (!string.IsNullOrEmpty(fileName) && existingFileStream!=null) throw new Exception();

      FileName = fileName;
      CsvConfig = csvConfig;
      if (csvConfig.Encoding!=Encoding.UTF8) 
        throw new Exception($"Only reading from UTF8 files is supported, but the Encoding was {csvConfig.Encoding.EncodingName} for file {fileName}.");
      
      delimiter = (byte)csvConfig.Delimiter;
      if (maxLineLenght>CsvConfig.BufferSize/Csv.LineToBufferRatio)
        throw new Exception($"Buffersize {CsvConfig.BufferSize} should be at least {Csv.LineToBufferRatio} times bigger than MaxLineCharLenght {MaxLineCharLenght} for file {fileName}.");

      MaxLineCharLenght = maxLineLenght;
      MaxLineByteLenght = maxLineLenght * Csv.Utf8BytesPerChar;
      tempBytes = new byte[MaxLineByteLenght];
      tempChars = new char[maxLineLenght];
      IsAsciiOnly = isAsciiOnly;
      FlushDelay = flushDelay;
      if (existingFileStream is null) {
        isFileStreamOwner = true;
        //fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan | FileOptions.WriteThrough);
        fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, CsvConfig.BufferSize);
      } else {
        isFileStreamOwner = false;
        fileStream = existingFileStream;
      }
      //byteArray = new byte[csvConfig.BufferSize];
      //writePos = 0;
      //maxBufferWriteLength = CsvConfig.BufferSize - maxLineLenght;
      byteArray = new byte[csvConfig.BufferSize + maxLineLenght];
      writePos = 0;
      maxBufferWriteLength = CsvConfig.BufferSize;
      //flushTimer = new Timer(flushTimerMethod, null, Timeout.Infinite, Timeout.Infinite);
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
      var wasflushTimer = Interlocked.Exchange(ref flushTimer, null);
      if (wasflushTimer!=null) {
        flushTimer = null;
        try {
          wasflushTimer.Dispose();//Dispose() is multithreading safe
        } catch (Exception ex) {
          CsvConfig.ReportException?.Invoke(ex);
        }
      }

      releaseFileStream();
    }


    private void releaseFileStream() {
      lock (byteArray) {
        if (fileStream==null) return;

        if (writePos>0) {
          fileStream.Write(byteArray, 0, writePos);
          writePos = 0;
        }
        if (isFileStreamOwner) {
          fileStream.Dispose();
        }
        fileStream = null;
      }
    }
    #endregion


    #region Methods
    //      -------

#if DEBUG
    bool isLocked = false;
    int lineStart = 0;
#endif


    public void WriteEndOfLine() {
#if DEBUG
      if (!isLocked) {
        throw new Exception();
      }
      isLocked = false;
#endif
      try {
        byteArray[writePos++] = 0x0D;
        byteArray[writePos++] = 0x0A;

#if DEBUG
        if (writePos-lineStart>MaxLineByteLenght) throw new Exception($"MaxLineByteLenght {MaxLineByteLenght} should be at least {writePos-lineStart}.");
#endif

        if (writePos>maxBufferWriteLength) {
          //var sw = new Stopwatch();
          //sw.Restart();

          //fileStream!.Write(byteArray, 0, writePos);
          fileStream!.Write(byteArray, 0, maxBufferWriteLength);
          var numberOfBytesToCopy = writePos - maxBufferWriteLength;
          if (numberOfBytesToCopy>0) {
            Array.Copy(byteArray, maxBufferWriteLength, byteArray, 0, numberOfBytesToCopy);
            writePos = numberOfBytesToCopy;
          } else {
            writePos = 0;
          }
          kickFlushTimer();
          //sw.Stop();
        }
#if DEBUG
        lineStart = writePos;
#endif

      } finally {
        Monitor.Exit(byteArray);
      }
    }


    const byte minusByte = (byte)'-';


    /// <summary>
    /// Write integer to UTF8 filestream including delimiter.
    /// </summary>
    public void Write(int i) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}
      int start;
      if (i<0) {
        byteArray[writePos++] = minusByte;
        start = writePos;
        //since -int.MinValue is bigger than int.MaxValue, i=-i does not work for int.Minvalue.
        //therfore write 1 digit first and guarantee that i>int.MinValue
        byteArray[writePos++] = (byte)(-(i % 10) + '0');
        i /= 10;
        if (i==0) {
          byteArray[writePos++] = delimiter;
          return;
        }
        i = -i;
      } else {
        start = writePos;
      }

      while (i>9) {
        byteArray[writePos++] = (byte)((i % 10) + '0');
        i /= 10;
      }
      byteArray[writePos++] = (byte)(i + '0');
      var end = writePos-1;
      while (end>start) {
        var temp = byteArray[end];
        byteArray[end--] = byteArray[start];
        byteArray[start++] = temp;
      }
      byteArray[writePos++] = delimiter;
    }


    public void Write(int? i) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}
      if (i is null) {
        byteArray[writePos++] = delimiter;
      } else {
        Write(i.Value);
      }
    }


    /// <summary>
    /// Write integer to UTF8 filestream including delimiter.
    /// </summary>
    public void Write(long l) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}
      int start;
      if (l<0) {
        byteArray[writePos++] = minusByte;
        start = writePos;
        //since -long.MinValue is bigger than long.MaxValue, l=-l does not work for long.Minvalue.
        //therfore write 1 digit first and guarantee that l>long.MinValue
        byteArray[writePos++] = (byte)(-(l % 10) + '0');
        l /= 10;
        if (l==0) {
          byteArray[writePos++] = delimiter;
          return;
        }
        l = -l;
      } else {
        start = writePos;
      }

      while (l>9) {
        byteArray[writePos++] = (byte)((l % 10) + '0');
        l /= 10;
      }
      byteArray[writePos++] = (byte)(l + '0');
      var end = writePos-1;
      while (end>start) {
        var temp = byteArray[end];
        byteArray[end--] = byteArray[start];
        byteArray[start++] = temp;
      }
      byteArray[writePos++] = delimiter;
    }


    static readonly string[] formats = {
      ".",
      ".#",
      ".##",
      ".###",
      ".####",
      ".#####",
      ".######",
      ".#######",
      ".########",
    };


    /// <summary>
    /// writes at most number of digitsAfterComma, if they are not zero. Trailing zeros get trancated.
    /// </summary>
    public void Write(decimal d, int digitsAfterComma = int.MaxValue) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}

      int charsWritten;
      if (digitsAfterComma<=8) {
        if (!d.TryFormat(tempChars, out charsWritten, formats[digitsAfterComma])) throw new Exception();
      } else {
        if (!d.TryFormat(tempChars, out charsWritten)) throw new Exception();
      }

      //deal with zero here, this simplifies the code for removing trailing 0. Any other single digit value
      //can also be handled here.
      if (charsWritten==0) {
        //code like csvWriter.Write(0.4m, 0) results in charsWritten==0
        byteArray[writePos++] = (byte)'0';
        byteArray[writePos++] = delimiter;
        return;
      } else if (charsWritten==1) {
        byteArray[writePos++] = (byte)tempChars[0];
        byteArray[writePos++] = delimiter;
        return;
      }

      //remove trailing '0' and '.'
      var charIndex = charsWritten - 1;
      while (true) {
        var tempChar = tempChars[charIndex--];
        if (tempChar>='1' && tempChar<='9') {
          charIndex++;
          break;
        }
        if (tempChar=='0') continue;

        if (tempChar=='.') break;
        throw new Exception();
      }

      for (int copyIndex = 0; copyIndex <= charIndex; copyIndex++) {
        byteArray[writePos++] = (byte)tempChars[copyIndex];
      }
      byteArray[writePos++] = delimiter;
    }


    public void Write(char c) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}
      if (c==delimiter) throw new Exception();

      if (c<0x80) {
        byteArray[writePos++] = (byte)c;
      } else {
        foreach (var utf8Byte in Encoding.UTF8.GetBytes(new string(c, 1))) {
          byteArray[writePos++] = utf8Byte;
        }
      }
      byteArray[writePos++] = delimiter;
    }


    public void WriteFirstLineChar(char c) {
      Monitor.Enter(byteArray);
#if DEBUG
      if (isLocked) {
        throw new Exception();
      }
      isLocked = true;
#endif
      if (c==delimiter) throw new Exception();

      if (c<0x80) {
        byteArray[writePos++] = (byte)c;
      } else {
        throw new Exception();
      }
    }


    public void Write(string? s) {
      //if (isNotLocked) {
      //  Monitor.Enter(byteArray);
      //  isNotLocked = false;
      //}
      if (s!=null) {
        for (int readIndex = 0; readIndex < s.Length; readIndex++) {
          var c = s[readIndex];
          if (c==delimiter) throw new Exception();

          if (c<0x80) {
            byteArray[writePos++] = (byte)c;
          } else {
            var readOnlySpan = ((ReadOnlySpan<char>)s).Slice(readIndex, s.Length-readIndex);
            var byteLength = Encoding.UTF8.GetBytes(readOnlySpan, tempBytes);
            Array.Copy(tempBytes, 0, byteArray, writePos, byteLength);
            writePos += byteLength;
            break;
          }
        }
      }
      byteArray[writePos++] = delimiter;
    }


    public void WriteDate(DateTime date) {
      if (date!=date.Date) throw new Exception();

      var day = date.Day;
      if (day>=30) {
        byteArray[writePos++] = (byte)'3';
        day -= 30;
      } else if (day>=20) {
        byteArray[writePos++] = (byte)'2';
        day -= 20;
      } else if (day>=10) {
        byteArray[writePos++] = (byte)'1';
        day -= 10;
      }
      byteArray[writePos++] = (byte)(day + '0');
      byteArray[writePos++] = (byte)('.');

      var month = date.Month;
      if (month>=10) {
        byteArray[writePos++] = (byte)'1';
        month -= 10;
      }
      byteArray[writePos++] = (byte)(month + '0');
      byteArray[writePos++] = (byte)('.');

      var year = date.Year;
      byteArray[writePos++] = (byte)(('0') + year / 1000);
      year %= 1000;
      byteArray[writePos++] = (byte)(('0') + year / 100);
      year %= 100;
      byteArray[writePos++] = (byte)(('0') + year / 10);
      year %= 10;
      byteArray[writePos++] = (byte)(('0') + year);
      byteArray[writePos++] = delimiter;
    }


    public void WriteLine(string line) {
      lock (byteArray) {
        for (int readIndex = 0; readIndex < line.Length; readIndex++) {
          var c = line[readIndex];
          if (c==delimiter) throw new Exception();

          if (c<0x80) {
            byteArray[writePos++] = (byte)c;
          } else {
            var readOnlySpan = ((ReadOnlySpan<char>)line).Slice(readIndex, line.Length-readIndex);
            var byteLength = Encoding.UTF8.GetBytes(readOnlySpan, tempBytes);
            Array.Copy(tempBytes, 0, byteArray, writePos, byteLength);
            writePos += byteLength;
            break;
          }
        }
        byteArray[writePos++] = 0x0D;
        byteArray[writePos++] = 0x0A;
      }
    }
    #endregion


    #region Flushing
    //      --------

    private void kickFlushTimer() {
      flushTimer?.Change(FlushDelay, Timeout.Infinite);
    }


    private void flushTimerMethod(object? state) {
      try {
        flush();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    private void flush() {
      lock (byteArray) {
        if (IsDisposed || fileStream==null || writePos<=0) return;

        fileStream.Write(byteArray, 0, writePos);
        writePos = 0;
        fileStream.Flush();
        stopFlushTimer();
      }
    }


    /// <summary>
    /// Asks the OS to flush the data immediately to the disk. Normally, this happens FlushDelay miliseconds after last write. 
    /// Flush() is multithreading safe.
    /// </summary>
    public void Flush() {
      flush();
      stopFlushTimer();
    }


    private void stopFlushTimer() {
      //var wasFlushTimer = flushTimer;
      //if (wasFlushTimer!=null) {
      //  wasFlushTimer.Change(Timeout.Infinite, Timeout.Infinite);//change is multithreading safe
      //}
      flushTimer?.Change(Timeout.Infinite, Timeout.Infinite);//change is multithreading safe
    }
    #endregion
  }
}