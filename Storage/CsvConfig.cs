﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  public class CsvConfig {

    #region Properties
    //      ----------

    /// <summary>
    /// Format to be used for date conversion
    /// </summary>
    public static string DateFormat = "dd.MM.yyyy";

    /// <summary>
    /// Directory where the CSV files get stored
    /// </summary>
    public readonly string DirectoryPath;


    /// <summary>
    /// Delemiter character used in CSV file to seperate fields
    /// </summary>
    public readonly char Delimiter;


    /// <summary>
    /// Encoding used to read and write CSV Files
    /// </summary>
    public readonly Encoding Encoding = Encoding.UTF8;


    /// <summary>
    /// BufferSize of FileStream. Default is 32k Bytes, any smaller size is slower.
    /// </summary>
    public readonly int BufferSize;


    /// <summary>
    /// The timer throws exception on a ThreadPool thread. reportException() needs to pass the exception to the main thread of the application.
    /// </summary>
    public readonly Action<Exception>? ReportException;


    public readonly char LineCharAdd;


    public readonly char LineCharUpdate;


    public readonly char LineCharDelete;
    #endregion


    #region Constructor
    //      -----------

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="directoryPath">Directory where the CSV files get stored</param>
    /// <param name="delimiter">Delemiter character used in CSV file to seperate fields</param>
    /// <param name="encoding">Encoding used to read and write CSV Files</param>
    /// <param name="bufferSize">BufferSize of FileStream. Default is 32k Bytes, any smaller Buffer is slower.</param>
    ///// <param name="writingIntervall">How often new content of RecordCollection gets writen to the CSV file, in milliseconds</param>
    ///// <param name="maxWaitIntervalls">How often new content of RecordCollection gets writen to the CSV file, in milliseconds</param>
    /// <param name="reportException">The timer throws exception on a ThreadPool thread. reportException() needs to pass the exception to the main thread of the application.</param>
    public CsvConfig(
      string directoryPath,
      char delimiter = '\t',
      Encoding? encoding = null,
      int bufferSize = 1 << 15, //32k
      Action<Exception>? reportException = null,
      char lineCharAdd = '+',
      char lineCharUpdate = '*',
      char lineCharDelete = '-') 
    {
      DirectoryPath = directoryPath;
      Delimiter = delimiter;
      if (encoding!=null) {
        Encoding = encoding;
      }
      var _4k = 1<<12;
      if (bufferSize<_4k) throw new ArgumentOutOfRangeException("bufferSize " + bufferSize + " cannot be smaller 4k (4096).");
      if (bufferSize%_4k!=0) throw new ArgumentOutOfRangeException("bufferSize " + bufferSize + " should be a multiple of 4k (4096).");
      BufferSize = bufferSize;
      //WritingIntervall = writingIntervall;
      //MaxWaitIntervalls = maxWaitIntervalls;
      ReportException = reportException;
      LineCharAdd = lineCharAdd;
      LineCharUpdate = lineCharUpdate;
      LineCharDelete = lineCharDelete;
    }
    #endregion


    #region Methods
    //      -------

    public override string ToString() {
      return
        "DirectoryPath: " + DirectoryPath +
        "; Delimiter: " + Delimiter +
        "; Encoding: " + Encoding +
        "; BufferSize: " + BufferSize +
        "; ReportException: " + ReportException +
        ";";
    }
    #endregion
  }
}
