using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel {


  /// <summary>
  /// Some comment for SampleDetail
  /// </summary>
  public partial class SampleDetail: IStorage<SampleDetail> {


    #region Properties
    //      ----------

    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Called once the constructor has filled all the properties
    /// </summary>
    //partial void onConstruct() {
    //}


    /// <summary>
    /// Called once the CSV-constructor who reads the data from a CSV file has filled all the properties
    /// </summary>
    //partial void onCsvConstruct(DL context) {
    //}


    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Called before storing gets executed
    /// </summary>
    //partial void onStore() {
    //}


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    //partial void onCsvWrite() {
    //}


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    //partial void onUpdate() {
    //}


    /// <summary>
    /// Called after an update is read from a CSV file
    /// </summary>
    //partial void onCsvUpdate() {
    //}


    /// <summary>
    /// Called before removal gets executed
    /// </summary>
    //partial void onRemove() {
    //}


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    //partial void onToShortString(ref string returnString) {
    //}


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    //partial void onToString(ref string returnString) {
    //}
    #endregion
  }


  #region SampleDetailWriter
  //      ------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as SampleDetail. Note that the keys of linked objects
  /// need to be provided, since the data context will not be involved.
  /// </summary>
  public class SampleDetailWriter: IDisposable {


    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;


    /// <summary>
    /// Constructor, will write the SampleDetail header line into the CSV file. Dispose SampleDetailWriter once done.
    /// </summary>
    public SampleDetailWriter(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(SampleDetail.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one SampleDetail to the CSV file
    /// </summary>
    public void Write(int key, string text, int sampleKey) {
      //if (IsReadOnly) {
      //  csvWriter.StartNewLine();
      //} else {
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(key);
      csvWriter.Write(text);
      csvWriter.Write(sampleKey);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of SampleDetailWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is SampleDetailWriter already exposed ?
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

      csvWriter.Dispose();
    }
    #endregion
  }
  #endregion
}
