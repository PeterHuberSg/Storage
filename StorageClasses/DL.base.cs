using System;
using System.Threading;
using Storage;


namespace StorageModel  {

  /// <summary>
  /// A part of DL is static, which gives easy access to all stored data (=context) through DL.Data. But most functionality is in the
  /// instantiatable part of DL. Since it is instantiatable, is is possible to use different contexts over the lifetime of a program. This 
  /// is helpful for unti testing. Use DL.Init() to create a new context and dispose it with DisposeData() before creating a new one.
  /// </summary>
  public partial class DL: IDisposable {

    #region static Part
    //      -----------

    /// <summary>
    /// Provides static root access to the data context
    /// </summary>
    public static DL? Data {
      get { return data; }
    }
    private static DL? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()


    /// <summary>
    /// Constructs the StorageDirectories for all auto generated classes
    /// </summary>
    /// <param name="csvConfig">null: no permanent data storage, not null: info where to store the data</param>
    public static void Init(CsvConfig? csvConfig) {
      if (data!=null) throw new Exception();

      data = new DL(csvConfig);
    }


    /// <summary>
    /// Flushes all data to permanent storage location if permanent data storage is active. Compacts data storage
    /// by applying all updates and removing all instances marked as deleted if isCompactDuringDispose==true.
    /// </summary>
    public static void DisposeData() {
      var dataLocal = Interlocked.Exchange(ref data, null);
      dataLocal?.Dispose();
    }
    #endregion


    #region Properties
    //      ----------

    /// <summary>
    /// Directory of all Samples
    /// </summary>
    public StorageDictionary<Sample, DL> Samples { get; private set; }

    /// <summary>
    /// Directory of all SampleDetails
    /// </summary>
    public StorageDictionary<SampleDetail, DL> SampleDetails { get; private set; }

    /// <summary>
    /// Directory of all SampleMasters
    /// </summary>
    public StorageDictionary<SampleMaster, DL> SampleMasters { get; private set; }
    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Creates a new data context. If csvConfig is null, the data is only stored in RAM, but gets lost once 
    /// program terminates. With csvConfig defined, existing data gets read at startup, changes immediately
    /// when written and Dispose() ensures by flushing that all data is permanently stored.
    /// </summary>
    public DL(CsvConfig? csvConfig) {
      if (csvConfig==null) {
        SampleMasters = new StorageDictionary<SampleMaster, DL>(
          this,
          SampleMaster.SetKey,
          SampleMaster.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true);
        Samples = new StorageDictionary<Sample, DL>(
          this,
          Sample.SetKey,
          Sample.Disconnect,
          areItemsUpdatable: false,
          areItemsDeletable: true);
        SampleDetails = new StorageDictionary<SampleDetail, DL>(
          this,
          SampleDetail.SetKey,
          SampleDetail.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true);
      } else {
        SampleMasters = new StorageDictionaryCSV<SampleMaster, DL>(
          this,
          csvConfig!,
          SampleMaster.MaxLineLength,
          SampleMaster.Headers,
          SampleMaster.SetKey,
          SampleMaster.Create,
          null,
          SampleMaster.Update,
          SampleMaster.Write,
          SampleMaster.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true,
          isCompactDuringDispose: true);
        Samples = new StorageDictionaryCSV<Sample, DL>(
          this,
          csvConfig!,
          Sample.MaxLineLength,
          Sample.Headers,
          Sample.SetKey,
          Sample.Create,
          Sample.Verify,
          Sample.Update,
          Sample.Write,
          Sample.Disconnect,
          areItemsUpdatable: false,
          areItemsDeletable: true,
          isCompactDuringDispose: false);
        SampleDetails = new StorageDictionaryCSV<SampleDetail, DL>(
          this,
          csvConfig!,
          SampleDetail.MaxLineLength,
          SampleDetail.Headers,
          SampleDetail.SetKey,
          SampleDetail.Create,
          SampleDetail.Verify,
          SampleDetail.Update,
          SampleDetail.Write,
          SampleDetail.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true,
          isCompactDuringDispose: true);
      }
    }
    #endregion


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Is DL.Data already disposed ?
    /// </summary>
    public bool IsDisposed {
      get { return isDisposed==1; }
    }
    int isDisposed = 0;


    protected virtual void Dispose(bool disposing) {
      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
      if (wasDisposed==1) return; // already disposed

      if (disposing) {
        SampleDetails.Dispose();
        Samples.Dispose();
        SampleMasters.Dispose();
      }
    }


    public void Dispose() {
      Dispose(true);
    }
    #endregion


    #region Methods
    //      -------

    #endregion

  }
}
