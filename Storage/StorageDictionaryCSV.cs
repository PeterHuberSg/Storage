/**************************************************************************************

Storage.StorageDictionaryCSV
============================

Stores items in a StorageDictionary and permanently in a CSV (comma separated value) file. An item can be accessed by its 
key. When starting, the file content gets read into the StorageDictionary, if a file exists, otherwise an empty file with 
only the header definition gets written. Items added to or deleted from StorageDictionary and items with changed content get 
continuously written to the file. 

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;


namespace Storage {


  /// <summary>
  /// Stores items in a StorageDictionary and permanently in a CSV (comma separated value) file. When starting, the file 
  /// content gets read into the StorageDictionary, if a file exists, otherwise an empty file with only the header definition 
  /// gets written. Items added to or deleted from StorageDictionary and items with changed content get continuously written 
  /// to the file. If there is no write activity, a flush timer ensures that the writes are committed to the hard disk.
  /// Disposing the StorageDictionary ensures that all data is flushed to the file if only new files were added or the complete 
  /// StorageDictionaryCSV is rewritten, which eliminates all the updated and deleted lines.
  /// </summary>
  public class StorageDictionaryCSV<TItemCSV, TContext>: StorageDictionary<TItemCSV, TContext> 
    where TItemCSV : class, IStorage<TItemCSV> 
    where TContext: class 
  {

    #region Properties
    //      ----------

    /// <summary>
    /// Configuration data of CSV files
    /// </summary>
    public readonly CsvConfig CsvConfig;


    /// <summary>
    /// Maximal length of TItemCSV when stored as string. Can be too long, but not to short. 10*MaxLineLenght should be shorter
    /// than CsvConfig.BufferSize
    /// </summary>
    public int MaxLineLenght { get; private set; }


    /// <summary>
    /// Path and file name 
    /// </summary>
    public readonly string PathFileName;


    /// <summary>
    /// Headers of the item, separated by delimiter 
    /// </summary>
    public readonly string CsvHeaderString;


    /// <summary>
    /// Delay in millisecond before flush gets executed after the last write
    /// </summary>
    public readonly int FlushDelay;
    #endregion

    #region Constructor
    //     ------------

    readonly Func<int, CsvReader, TContext, TItemCSV?> create;
    readonly Func<TItemCSV, bool>? verify;
    readonly Action<TItemCSV, CsvReader, TContext>? update;
    readonly Action<TItemCSV, CsvWriter>? write;

    readonly bool isInitialReading;
    FileStream? fileStream;
    CsvWriter? csvWriter;

    //Timer? flushTimer;


    /// <summary>
    /// Constructs a readonly StorageDictionaryCSV. If a CSV file exists already, its content gets read at startup. If no CSV
    /// file exists, an empty one gets created with a header line.
    /// </summary>
    /// <param name="context">Should be parent holding all StorageDictionaries of the application</param>
    /// <param name="csvConfig">File name and other parameters for CSV file</param>
    /// <param name="maxLineLenght">Maximal number of bytes needed to write one line</param>
    /// <param name="headers">Name for each item property</param>
    /// <param name="setKey">Method to be called if item has no key yet</param>
    /// <param name="create">Creates a new item with one line read from the CSV file</param>
    /// <param name="verify">Verifies item, for example it parent(s) exist</param>
    /// <param name="write">Writes item to CSV file</param>
    /// <param name="capacity">How many items should StorageDictionaryCSV by able to hold initially ?</param>
    /// <param name="flushDelay">When the items in StorageDictionaryCSV are not changed for flushDelay milliseconds, the internal
    /// buffer gets written to the CSV file.</param>
    public StorageDictionaryCSV(
      TContext? context,
      CsvConfig csvConfig,
      int maxLineLenght,
      string[] headers,
      Action<TItemCSV, int> setKey,
      Func<int, CsvReader, TContext, TItemCSV> create,
      Func<TItemCSV, bool>? verify,
      Action<TItemCSV, CsvWriter> write,
      int capacity = 0,
      int flushDelay = 200) : this(context, csvConfig, maxLineLenght, headers, setKey, create, verify, null, write, 
        null, false, false, capacity, flushDelay) {}


    /// <summary>
    /// Constructs a StorageDictionaryCSV. If a CSV file exists already, its content gets read at startup. If no CSV file 
    /// exists, an empty one gets created with a header line.
    /// </summary>
    /// <param name="context">Should be parent holding all StorageDictionaries of the application</param>
    /// <param name="csvConfig">File name and other parameters for CSV file</param>
    /// <param name="maxLineLenght">Maximal number of bytes needed to write one line</param>
    /// <param name="headers">Name for each item property</param>
    /// <param name="setKey">Method to be called if item has no key yet</param>
    /// <param name="create">Creates a new item with one line read from the CSV file</param>
    /// <param name="verify">Verifies item, for example it parent(s) exist</param>
    /// <param name="update">Updates an item if an line with updates is read</param>
    /// <param name="write">Writes item to CSV file</param>
    /// <param name="disconnect">Called when an item gets removed (deleted). It might be necessary to disconnect also child
    /// items linked to this item and/or to remove item from parent(s)</param>
    /// <param name="areInstancesUpdatable">Can the property of an item change ?</param>
    /// <param name="areInstancesDeletable">Can an item be removed from StorageDictionaryCSV</param>
    /// <param name="capacity">How many items should StorageDictionaryCSV by able to hold initially ?</param>
    /// <param name="flushDelay">When the items in StorageDictionaryCSV are not changed for flushDelay milliseconds, the internal
    /// buffer gets written to the CSV file.</param>
    public StorageDictionaryCSV(
      TContext? context,
      CsvConfig csvConfig,
      int maxLineLenght,
      string[] headers,
      Action<TItemCSV, int> setKey,
      Func<int, CsvReader, TContext, TItemCSV> create,
      Func<TItemCSV, bool>? verify,
      Action<TItemCSV, CsvReader, TContext>? update,
      Action<TItemCSV, CsvWriter> write,
      Action<TItemCSV>? disconnect,
      bool areInstancesUpdatable = false,
      bool areInstancesDeletable = false,
      int capacity = 0,
      int flushDelay = 200) : base(context, setKey, disconnect, areInstancesUpdatable, areInstancesDeletable, capacity) 
    {
      CsvConfig = csvConfig;
      MaxLineLenght = maxLineLenght;
      CsvHeaderString = Csv.ToCsvHeaderString(headers, csvConfig.Delimiter);
      this.create = create;
      this.verify = verify;
      this.update= update;
      this.write = write;

      PathFileName = Csv.ToPathFileName(csvConfig, typeof(TItemCSV).Name);
      fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, csvConfig.BufferSize, FileOptions.SequentialScan);
      if (fileStream.Length>0) {
        using (var csvReader = new CsvReader(null, CsvConfig, maxLineLenght, fileStream)) {
          isInitialReading = true;
          readFromCsvFile(csvReader);
          isInitialReading = false;
          fileStream.Position = fileStream.Length;
        }
        csvWriter = new CsvWriter("", csvConfig, maxLineLenght, fileStream, flushDelay: flushDelay);
      } else {
        //there is no file yet. Write an empty file with just the CSV header
        csvWriter = new CsvWriter("", csvConfig, maxLineLenght, fileStream, flushDelay: flushDelay);
        WriteToCsvFile(csvWriter);
      }
      //flushTimer = new Timer(flushTimerMethod, null, Timeout.Infinite, Timeout.Infinite);
      FlushDelay = flushDelay;
    }
    #endregion


    #region IDispose interface
    //      ------------------

    readonly object disposingLock = new object();


    protected override void OnDispose(bool disposing) {
      lock (disposingLock) {
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

        if (AreItemsUpdated || AreItemsDeleted) {
          //backup file and create a new one with only the latest items, but no deletions nor updates
          try {
            var backupFileName = PathFileName[..^3] + "bak";
            if (File.Exists(backupFileName)) {
              File.Delete(backupFileName);
            }
            File.Move(PathFileName, backupFileName);
            using var fileStream = new FileStream(PathFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None, CsvConfig.BufferSize, FileOptions.SequentialScan);
            using var csvWriter = new CsvWriter("", CsvConfig, MaxLineLenght, fileStream);
            WriteToCsvFile(csvWriter);
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
      if (CsvHeaderString!=headerLine) throw new Exception($"Error reading file {csvReader.FileName}{Environment.NewLine}'" + 
        headerLine + "' should be " + CsvHeaderString + ".");

      //read data lines
      var errorStringBuilder = new StringBuilder();
      while (!csvReader.IsEndOfFileReached()) {
        if (IsReadOnly) {
          addItem(csvReader, errorStringBuilder);
        } else {
          var firstLineChar = csvReader.ReadFirstLineChar();
          if (firstLineChar==CsvConfig.LineCharAdd) {
            addItem(csvReader, errorStringBuilder);
            
          //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          //Todo: improve StorageDictionaryCSV.readFromCsvFile()
          //actually, there should never be the case where deletion or updates get read, since all
          //CSV files get now compacted on Dispose. Need to detect the case here when the Dispose()
          //did not work properly
          //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
          } else if (firstLineChar==CsvConfig.LineCharDelete) {
            //delete
            int key = csvReader.ReadInt();
            csvReader.SkipToEndOfLine();
            if (!Remove(key)) {
              errorStringBuilder.AppendLine($"Deletion Line with key '{key}' did not exist in StorageDictonary.");
            }
#if DEBUG
          } else if (firstLineChar!=CsvConfig.LineCharUpdate) {
            throw new Exception();
#endif
          } else {
            //update
            var key = csvReader.ReadInt();
            var item = this[key];
            update!(item, csvReader, Context!);
            csvReader.ReadEndOfLine();
          }
        }
      }

      if (verify!=null) {
        foreach (var item in this) {
          if (!verify(item)) {
            errorStringBuilder.AppendLine($"StorageDictionaryCSV<{typeof(TItemCSV).Name}>: item '{item}' could not be validated in {PathFileName}.");
          }
        }
      }

      if (errorStringBuilder.Length>0) {
        throw new Exception($"Errors reading file {csvReader.FileName}, wrong formatting on following lines:" + Environment.NewLine +
          errorStringBuilder.ToString());
      }
      UpdateAreKeysContinous();
    }


    private void addItem(CsvReader csvReader, StringBuilder errorStringBuilder) {
      TItemCSV? item;
      if (IsReadOnly) {
        item = create(LastItemIndex+1, csvReader, Context!);
      } else {
        item = create(csvReader.ReadInt(), csvReader, Context!);
      }
      if (errorStringBuilder.Length==0) {
        AddProtected(item!);
      }
      csvReader.ReadEndOfLine();
    }


    /// <summary>
    /// Writes all items in StorageDictionaryCSV to a CSV file
    /// </summary>
    public void WriteToCsvFile(CsvWriter csvWriter) {
      csvWriter.WriteLine(CsvHeaderString);
      foreach (TItemCSV item in this) {
        if (item!=null) {
          if (IsReadOnly) {
            csvWriter.StartNewLine();
          } else {
            csvWriter.WriteFirstLineChar(CsvConfig.LineCharAdd);
            csvWriter.Write(item.Key);
          }
          write!(item, csvWriter);
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
          if (IsReadOnly) {
            csvWriter.StartNewLine();
          } else {
            csvWriter.WriteFirstLineChar(CsvConfig.LineCharAdd);
            csvWriter.Write(item.Key);
          }
          write!(item, csvWriter);
          csvWriter.WriteEndOfLine();
        }
        //kickFlushTimer();
      } catch (Exception ex) {
        csvWriter!.CleanupAfterException();
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemHasChanged(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (csvWriter!) {
          csvWriter.WriteFirstLineChar(CsvConfig.LineCharUpdate);
          csvWriter.Write(item.Key);
          write!(item, csvWriter);
          csvWriter.WriteEndOfLine();
        }
        //kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    protected override void OnItemRemoved(TItemCSV item) {
      if (isInitialReading) return;

      try {
        lock (csvWriter!) {
          csvWriter.WriteFirstLineChar(CsvConfig.LineCharDelete);
          csvWriter.Write(item.Key);
          write!(item, csvWriter);
          csvWriter.WriteEndOfLine();
        }
        //kickFlushTimer();
      } catch (Exception ex) {
        CsvConfig.ReportException?.Invoke(ex);
      }
    }


    public override string ToString() {
      return base.ToString() + $"; MaxLine: {this.MaxLineLenght};";
    }
    #endregion
    #endregion
  }
}
