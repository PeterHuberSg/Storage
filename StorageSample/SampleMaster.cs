using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Some comment for SampleMaster.
    /// With an additional line.
    /// </summary>
  public partial class SampleMaster: IStorage<SampleMaster> {


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
    /// Called after a sample gets added to SampleX.
    /// </summary>
    //partial void onAddedToSampleX(Sample sample){
    //}


    /// <summary>
    /// Called after a sample gets removed from SampleX.
    /// </summary>
    //partial void onRemovedFromSampleX(Sample sample){
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


  #region SampleMasterWriter
  //      ------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as SampleMaster. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class SampleMasterWriter: IDisposable {


    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int lastKey = int.MinValue;


    /// <summary>
    /// Constructor, will write the SampleMaster header line into the CSV file. Dispose SampleMasterWriter once done.
    /// </summary>
    public SampleMasterWriter(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(SampleMaster.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one SampleMaster to the CSV file
    /// </summary>
    public void Write(int key, string text, int numberWithDefault = int.MinValue) {
      //if (IsReadOnly) {
      //  csvWriter.StartNewLine();
      //} else {
      if (key<0) {
        throw new Exception($"SampleMaster's key {key} needs to be greater equal 0.");
      }
      if (key<=lastKey) {
        throw new Exception($"SampleMaster's key {key} must be greater than the last written SampleMaster's key {lastKey}.");
      }
      lastKey = key;
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(key);
      csvWriter.Write(text);
      csvWriter.Write(numberWithDefault);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of SampleMasterWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is SampleMasterWriter already exposed ?
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
