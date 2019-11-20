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

    public int MaxLineLenght { get; }

    public bool IsAsciiOnly { get; }


    /// <summary>
    /// Dealy in millisecond before flush gets executed after the last write
    /// </summary>
    public readonly int FlushDelay;
    #endregion


    #region Constructor
    //      ----------
#pragma warning disable IDE0069 // Disposable fields should be disposed
    FileStream? fileStream;
#pragma warning restore IDE0069 // Disposable fields should be disposed
    readonly byte[] byteArray;
    int writePos;
    readonly int maxBufferWriteLength;
    readonly byte delimiter;
    Timer? flushTimer;



    public CsvWriter(string fileName, CsvConfig csvConfig, int maxLineLenght, bool isAsciiOnly = true, int flushDelay = 200) {
      FileName = fileName;
      CsvConfig = csvConfig;
      if (csvConfig.Encoding!=Encoding.UTF8) {
        throw new Exception($"Only reading from UTF8 files is supported, but the Encoding was {csvConfig.Encoding.EncodingName}.");
      }
      delimiter = (byte)csvConfig.Delimiter;
      MaxLineLenght = maxLineLenght;
      IsAsciiOnly = isAsciiOnly;
      FlushDelay = flushDelay;
      //fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan | FileOptions.WriteThrough);
      fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
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
        fileStream.Dispose();
        fileStream = null;
      }
    }
    #endregion


    #region Methods
    //      -------

    bool isNotLocked = true;


    public void WriteEndOfLine() {
      try {
        byteArray[writePos++] = 0x0D;
        byteArray[writePos++] = 0x0A;

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
      } finally {
        Monitor.Exit(byteArray);
        isNotLocked = true;
      }
    }


    const byte minusByte = (byte)'-';


    /// <summary>
    /// Write integer to UTF8 filestream including delimiter.
    /// </summary>
    public void Write(int i) {
      if (isNotLocked) {
        Monitor.Enter(byteArray);
        isNotLocked = false;
      }
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


    public void Write(char c) {
      if (isNotLocked) {
        Monitor.Enter(byteArray);
        isNotLocked = false;
      }
      if (c<0x80) {
        byteArray[writePos++] = (byte)c;
      } else {
        foreach (var utf8Byte in Encoding.UTF8.GetBytes(new string(c, 1))) {
          byteArray[writePos++] = utf8Byte;
        }
      }
      byteArray[writePos++] = delimiter;
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