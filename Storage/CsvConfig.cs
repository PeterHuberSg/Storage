using System;
using System.Collections.Generic;
using System.Text;
using ACoreLib;

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
    /// Delemiter character used in CSV file to seperate fields as char array, used in string.Split()
    /// </summary>
    public readonly char[] Delimiters;


    /// <summary>
    /// Encoding used to read and write CSV Files
    /// </summary>
    public readonly Encoding Encoding = Encoding.UTF8;


    /// <summary>
    /// BufferSize of FileStream. Default is 4k Bytes, as is default in StreamReader class.
    /// </summary>
    public readonly int BufferSize;


    ///// <summary>
    ///// How often new content of RecordCollection gets writen to the CSV file, in milliseconds
    ///// </summary>
    //public readonly uint WritingIntervall = 1000;


    ///// <summary>
    ///// After how many calls of the timer without writing data the file gets flushed.
    ///// </summary>
    //public readonly uint MaxWaitIntervalls = 5;


    /// <summary>
    /// The timer throws exception on a ThreadPool thread. reportException() needs to pass the exception to the main thread of the application.
    /// </summary>
    public readonly Action<Exception>? ReportException;
    #endregion


    #region Constructor
    //      -----------

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="directoryPath">Directory where the CSV files get stored</param>
    /// <param name="delimiter">Delemiter character used in CSV file to seperate fields</param>
    /// <param name="encoding">Encoding used to read and write CSV Files</param>
    /// <param name="bufferSize">BufferSize of FileStream. Default is 4k Bytes, as is default in StreamReader class.</param>
    ///// <param name="writingIntervall">How often new content of RecordCollection gets writen to the CSV file, in milliseconds</param>
    ///// <param name="maxWaitIntervalls">How often new content of RecordCollection gets writen to the CSV file, in milliseconds</param>
    /// <param name="reportException">The timer throws exception on a ThreadPool thread. reportException() needs to pass the exception to the main thread of the application.</param>
    public CsvConfig(
      string directoryPath,
      char delimiter = '\t',
      Encoding? encoding = null,
      int bufferSize = 2 << 12,
      //uint writingIntervall = 1000,
      //uint maxWaitIntervalls = 5,
      Action<Exception>? reportException = null) {
      DirectoryPath = directoryPath;
      Delimiter = delimiter;
      Delimiters =  new char[] { Delimiter };
      if (encoding!=null) {
        Encoding = encoding;
      }

      if (bufferSize<0) throw (Tracer.Exception(new ArgumentOutOfRangeException("bufferSize " + bufferSize + " cannot be smaller 0.")));
      BufferSize = bufferSize;
      //WritingIntervall = writingIntervall;
      //MaxWaitIntervalls = maxWaitIntervalls;
      ReportException = reportException;
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
