/**************************************************************************************

Storage.CsvConfig
=================

Defines parameters used for CSV file generation.

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;
using System.Text;

namespace Storage {


  /// <summary>
  /// Defines parameters used for CSV file generation.
  /// </summary>
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
    /// Delimiter character used in CSV file to separate fields
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
    /// The timer throws exception on a ThreadPool thread. ReportException() needs to pass the exception to the main thread of the application.
    /// </summary>
    public readonly Action<Exception>? ReportException;


    /// <summary>
    /// Character used at the start of a line in a CSV file to mark adding a new item.
    /// </summary>
    public readonly char LineCharAdd;


    /// <summary>
    /// Character used at the start of a line in a CSV file to mark an updated item.
    /// </summary>
    public readonly char LineCharUpdate;


    /// <summary>
    /// Character used at the start of a line in a CSV file to mark a deleted item.
    /// </summary>
    public readonly char LineCharDelete;
    #endregion


    #region Constructor
    //      -----------

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="directoryPath">Directory where the CSV files get stored</param>
    /// <param name="delimiter">Delimiter character used in CSV file to separate fields</param>
    /// <param name="encoding">Encoding used to read and write CSV Files</param>
    /// <param name="bufferSize">BufferSize of FileStream. Default is 32k Bytes, any smaller Buffer is slower.</param>
    /// <param name="reportException">The timer throws exception on a ThreadPool thread. reportException() needs to 
    /// pass the exception to the main thread of the application.</param>
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

    /// <summary>
    /// Write CsvConfig parameters into a string
    /// </summary>
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
