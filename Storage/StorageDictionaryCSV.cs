using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Storage {


  /// <summary>
  /// Stores records in a StorageDictionary and in a CSV (comma separated value) file. When starting, the file content gets read into the 
  /// StorageDictionary, if a file exists, otherwise an empty file with only the header definition gets written. Records added to or deleted
  /// from StorageDictionary and items with changed content get continuously writtento the file. If there is no write activity, a flush timer
  /// ensures that the writes are committed to the hard disk.
  /// Disposing the StorageDictionary ensures that all data is flushed to the file if only new files were added or the complete 
  /// StorageDictionaryCSV is rewritten, which eliminates all the updated and deleted lines.
  /// </summary>
  public class StorageDictionaryCSV<TItemCSV>: StorageDictionary<TItemCSV>, IDisposable
    where TItemCSV: class, IStorageCSV<TItemCSV>
  {

    #region Properties
    //      ----------

    /// <summary>
    /// Configuration data of CSV files
    /// </summary>
    public readonly CsvConfig CsvConfig;


    /// <summary>
    /// Path and file name 
    /// </summary>
    public readonly string PathFileName;


    /// <summary>
    /// Headers of the CsvRecord, separated by delimiter 
    /// </summary>
    public readonly string CsvHeaderString;


    /// <summary>
    /// Dealy in millisecond before flush gets executed after the last write
    /// </summary>
    public readonly int FlushDelay;


    /// <summary>
    /// During Dispoe(), should a new file be written when some items have changed their content or some items were deleted ? 
    /// This compacts the file.
    /// </summary>
    public bool IsCompactDuringDispose { get; }


    //public readonly TData Data;
    #endregion

    #region Constructor
    //     ------------

    readonly Func<string, char, StringBuilder, TItemCSV?> readCsvLine;
    readonly Action<string, char, StorageDictionary<TItemCSV>, StringBuilder>? updateFromCsvLine;
    readonly bool isInitialReading;
    StreamWriter? streamWriter;

    Timer? flushTimer;


    public StorageDictionaryCSV(
      CsvConfig csvConfig,
      string[] headers,
      Func<string, char, StringBuilder, TItemCSV?> readCsvLine,
      bool areItemsUpdatable = false,
      bool areItemsDeletable = false,
      bool isCompactDuringDispose = true,
      Action<string, char, StorageDictionary<TItemCSV>, StringBuilder>? updateFromCsvLine = null,
      int capacity = 0,
      int flushDelay = 200): base(areItemsUpdatable, areItemsDeletable, capacity) 
    {
      CsvConfig = csvConfig;
      IsCompactDuringDispose = isCompactDuringDispose;
      CsvHeaderString = Csv.ToCsvHeaderString(headers, csvConfig.Delimiter);
      this.readCsvLine = readCsvLine;
      if (areItemsUpdatable) {
        if (updateFromCsvLine==null) throw new Exception();

        this.updateFromCsvLine = updateFromCsvLine;
      }

      PathFileName = Csv.ToPathFileName(csvConfig, typeof(TItemCSV).Name);
      var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, csvConfig.BufferSize, FileOptions.SequentialScan);
      if (fileStream.Length>0) {
        //var streamReader = new StreamReader(fileStream, CsvConfig.Encoding, detectEncodingFromByteOrderMarks: false, bufferSize: csvConfig.BufferSize, leaveOpen: true);
        using (var streamReader = new StreamReader(fileStream, CsvConfig.Encoding, detectEncodingFromByteOrderMarks: false, bufferSize: csvConfig.BufferSize, leaveOpen: true)) {
          isInitialReading = true;
          readFromCsvFile(streamReader);
          isInitialReading = false;
          fileStream.Position = fileStream.Length;
        }
        streamWriter = new StreamWriter(fileStream);
      } else {
        //there is no file yet. Write an empty file with just the CSV header
        streamWriter = new StreamWriter(fileStream);
        WriteToCsvFile(streamWriter);
      }
      flushTimer = new Timer(flushTimerMethod, null, Timeout.Infinite, Timeout.Infinite);
      FlushDelay = flushDelay;
    }
    #endregion


    #region IDispose interface
    //      ------------------

    readonly object disposingLock = new object();


    protected override void OnDispose(bool disposing) {
      lock (disposingLock) {
        var wasflushTimer = Interlocked.Exchange(ref flushTimer, null);
        if (wasflushTimer!=null) {
          try {
            wasflushTimer.Dispose();//Dispose() is multithreading safe
          } catch (Exception ex) {
            CsvConfig.ReportException?.Invoke(ex);
          }
        }

        var wasStreamWriter = Interlocked.Exchange(ref streamWriter, null);
        if (wasStreamWriter!=null) {
          try {
            lock (wasStreamWriter) {
              if (wasStreamWriter.BaseStream!=null) {
                wasStreamWriter.Dispose();
              }
            }
          } catch (Exception ex) {
            CsvConfig.ReportException?.Invoke(ex);
          }
        }

        if (IsCompactDuringDispose && (AreItemsUpdated || AreItemsDeleted)) {
          //backup file and create a new one with only the latest items, but no deletions nor updates
          try {
            var backupFileName = PathFileName[..^3] + "bak";
            if (File.Exists(backupFileName)) {
              File.Delete(backupFileName);
            }
            File.Move(PathFileName, backupFileName);
            var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
            using var streamWriter = new StreamWriter(fileStream);
            WriteToCsvFile(streamWriter);
          } catch (Exception ex) {
            CsvConfig.ReportException?.Invoke(ex);
          }
        }
      }
    }
    #endregion


    #region Methods
    //      -------


    private void readFromCsvFile(StreamReader streamReader) {
      //verify headers line
      var line = streamReader.ReadLine();
      if (CsvHeaderString!=line) throw new Exception("'" + line + "' should be " + CsvHeaderString + ".");

      //read data lines
      var errorStringBuilder = new StringBuilder();
      while ((line = streamReader.ReadLine())!=null) {
        if (line.StartsWith('*')) {
          //update
          line = line.Substring(1);
          updateFromCsvLine!(line, CsvConfig.Delimiter, this, errorStringBuilder);//throws an exception when updateFromCsvLine==null

        } else if (line.StartsWith('-')) {
          //delete
          int key = readKey(line, CsvConfig.Delimiter, errorStringBuilder);
          if (key>=0 && !Remove(key)) {
            errorStringBuilder.AppendLine($"Line '{line}' with key '{key}' did not exist in StorageDictonary.");
          }
        } else {
          //add
          var item = readCsvLine(line, CsvConfig.Delimiter, errorStringBuilder);
          if (errorStringBuilder.Length==0) {
            Add(item!);
          }
        }

      }
      if (errorStringBuilder.Length>0) {
        throw new Exception("Errors reading file " + ((FileStream)streamReader.BaseStream).Name + ", wrong formatting on following lines:" + Environment.NewLine +
          errorStringBuilder.ToString());
      }
    }


    private int readKey(string line, char delimiter, StringBuilder errorStringBuilder) {
      try {
        var delimiterPos = line.IndexOf(delimiter);
        var keyString = line[1..delimiterPos ];
        var key = int.Parse(keyString);
        return key;
      } catch {
        errorStringBuilder.AppendLine($"Could not read key in line '{line}'.");
        return -1;
      }
    }


    public void WriteToCsvFile(StreamWriter streamWriter) {
      streamWriter.WriteLine(CsvHeaderString);
      foreach (TItemCSV item in this) {
        streamWriter.WriteLine(item.ToCsvString(CsvConfig.Delimiter));
      }
    }


    #region Overrides
    //      ---------

    protected override void OnItemAdded(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (streamWriter!) {
          streamWriter.WriteLine(item.ToCsvString(CsvConfig.Delimiter));
        }
        kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemHasChanged(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (streamWriter!) {
          streamWriter.WriteLine("*" + item.ToCsvString(CsvConfig.Delimiter));
        }
        kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemRemoved(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (streamWriter!) {
          streamWriter.WriteLine("-" + item.ToCsvString(CsvConfig.Delimiter));
        }
        kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }
    #endregion


    #region Flushing
    //      --------

    private void kickFlushTimer() {
      flushTimer!.Change(FlushDelay, Timeout.Infinite); //the callers catch any exception
    }


    private void flushTimerMethod(object? state) {
      try {
        flush();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    private void flush() {
      var wasStreamWriter = streamWriter;
      if (IsDisposed) return;

      if (wasStreamWriter!=null) {
        lock (wasStreamWriter) {
          wasStreamWriter.Flush();
        }
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
      var wasFlushTimer = flushTimer;
      if (wasFlushTimer!=null) {
        wasFlushTimer.Change(Timeout.Infinite, Timeout.Infinite);//change is multithreading safe
      }
    }
    #endregion
    #endregion
  }
}
