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
    where TItemCSV : class, IStorageCSV<TItemCSV> {

    #region Properties
    //      ----------

    /// <summary>
    /// Configuration data of CSV files
    /// </summary>
    public readonly CsvConfig CsvConfig;


    /// <summary>
    /// Maximal lenght of TItemCSV when stored as string. Can be too long, but not to short. 10*MaxLineLenght should be shorter
    /// than CsvConfig.BufferSize
    /// </summary>
    public int MaxLineLenght { get; private set; }


    /// <summary>
    /// Will only ASCII characters be written ? Throws an error if by chance a none ASCII character gets written.
    /// </summary>
    public bool IsAsciiOnly { get; }

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

    readonly Func<CsvReader, char, StringBuilder, TItemCSV?> readCsvLine;
    readonly Action<CsvReader, char, StorageDictionary<TItemCSV>, StringBuilder>? updateFromCsvLine;
    readonly bool isInitialReading;
    FileStream? fileStream;
    CsvWriter? csvWriter;

    Timer? flushTimer;


    public StorageDictionaryCSV(
      CsvConfig csvConfig,
      int maxLineLenght,
      string[] headers,
      Func<CsvReader, char, StringBuilder, TItemCSV?> readCsvLine,
      bool areItemsUpdatable = false,
      bool areItemsDeletable = false,
      bool isCompactDuringDispose = true,
      bool isAsciiOnly = true,
      Action<CsvReader, char, StorageDictionary<TItemCSV>, StringBuilder>? updateFromCsvLine = null,
      int capacity = 0,
      int flushDelay = 200) : base(areItemsUpdatable, areItemsDeletable, capacity) 
    {
      CsvConfig = csvConfig;
      MaxLineLenght = maxLineLenght;
      IsCompactDuringDispose = isCompactDuringDispose;
      CsvHeaderString = Csv.ToCsvHeaderString(headers, csvConfig.Delimiter);
      IsAsciiOnly = isAsciiOnly;
      this.readCsvLine = readCsvLine;
      if (areItemsUpdatable) {
        if (updateFromCsvLine==null) throw new Exception();

        this.updateFromCsvLine = updateFromCsvLine;
      }

      PathFileName = Csv.ToPathFileName(csvConfig, typeof(TItemCSV).Name);
      var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, csvConfig.BufferSize, FileOptions.SequentialScan);
      if (fileStream.Length>0) {
        using (var csvReader = new CsvReader(null, CsvConfig, maxLineLenght, fileStream)) {
          isInitialReading = true;
          readFromCsvFile(csvReader);
          isInitialReading = false;
          fileStream.Position = fileStream.Length;
        }
        csvWriter = new CsvWriter("", csvConfig, maxLineLenght, fileStream, isAsciiOnly);
      } else {
        //there is no file yet. Write an empty file with just the CSV header
        csvWriter = new CsvWriter("", csvConfig, maxLineLenght, fileStream, isAsciiOnly);
        WriteToCsvFile(csvWriter);
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

        var wasCsvWriter = Interlocked.Exchange(ref csvWriter, null);
        if (wasCsvWriter!=null) {
          try {
            lock (wasCsvWriter) {
              wasCsvWriter.Dispose();

              fileStream?.Dispose();
              fileStream = null;
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
            using var streamWriter = new CsvWriter("", CsvConfig, MaxLineLenght, fileStream, IsAsciiOnly);
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


    private void readFromCsvFile(CsvReader csvReader) {
      //verify headers line
      var headerLine = csvReader.ReadLine();
      if (CsvHeaderString!=headerLine) throw new Exception("'" + headerLine + "' should be " + CsvHeaderString + ".");

      //read data lines
      var errorStringBuilder = new StringBuilder();
      while (!csvReader.IsEndOfFileReached()) {
        var firstLineChar = csvReader.ReadFirstLineChar();
        if (firstLineChar==CsvConfig.LineCharUpdate) {
          //update
          updateFromCsvLine!(csvReader, CsvConfig.Delimiter, this, errorStringBuilder);//throws an exception when updateFromCsvLine==null

        } else if (firstLineChar==CsvConfig.LineCharDelete) {
          //delete
          int key = csvReader.ReadInt();
          csvReader.SkipToEndOfLine();
          if (!Remove(key)) {
            errorStringBuilder.AppendLine($"Deletion Line with key '{key}' did not exist in StorageDictonary.");
          }
#if DEBUG
        } else if (firstLineChar!=CsvConfig.LineCharAdd) {
          throw new Exception();
#endif
        } else {
          //add
          var item = readCsvLine(csvReader, CsvConfig.Delimiter, errorStringBuilder);
          if (errorStringBuilder.Length==0) {
            Add(item!);
          }
        }

      }
      if (errorStringBuilder.Length>0) {
        throw new Exception($"Errors reading file {csvReader.FileName}, wrong formatting on following lines:" + Environment.NewLine +
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


    public void WriteToCsvFile(CsvWriter csvWriter) {
      csvWriter.WriteLine(CsvHeaderString);
      foreach (TItemCSV item in this) {
        if (item!=null) {
          csvWriter.WriteFirstLineChar(CsvConfig.LineCharAdd);
          item.Write(csvWriter);
          csvWriter.WriteEndOfLine();
        }
      }
    }


#region Overrides
    //      ---------

    protected override void OnItemAdded(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (csvWriter!) {
          csvWriter.WriteFirstLineChar(CsvConfig.LineCharAdd);
          item.Write(csvWriter);
          csvWriter.WriteEndOfLine();
        }
        kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemHasChanged(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (csvWriter!) {
          csvWriter.WriteFirstLineChar('*');
          item.Write(csvWriter);
          csvWriter.WriteEndOfLine();
        }
        kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemRemoved(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (csvWriter!) {
          csvWriter.WriteFirstLineChar('-');
          item.Write(csvWriter);
          csvWriter.WriteEndOfLine();
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
      var wasCsvWriter = csvWriter;
      if (IsDisposed) return;

      if (wasCsvWriter!=null) {
        lock (wasCsvWriter) {
          wasCsvWriter.Flush();
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
