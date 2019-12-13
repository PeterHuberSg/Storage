//using System;
//using System.Threading;
//using Storage;


//namespace StorageSample {

//  /// <summary>
//  /// Dl has a static part, which gives easy access to all stored data (=context) through DL.Data. But most functionality is in the
//  /// constructable part of DL, which allows to with different data sets over the lifetime of a program. This is helpful for
//  /// unti testing. Use DL.Init() to create a new context and dispose it with DisposeData() before creating a new one.
//  /// </summary>
//  public partial class DL: IDisposable {

//    #region static Part
//    //      -----------

//    /// <summary>
//    /// Provides static root access to the data context
//    /// </summary>
//    public static DL? Data {
//      get { return data; }
//    }
//    private static DL? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()


//    /// <summary>
//    /// Constructs the StorageDirectories for all auto generated classes
//    /// </summary>
//    /// <param name="csvConfig">null: no permanent data storage, not null: info where to store the data</param>
//    public static void Init(CsvConfig? csvConfig) {
//      if (data!=null) throw new Exception();

//      data = new DL(csvConfig);
//    }


//    /// <summary>
//    /// Writes all data to permanent storage location if permanent data storage is active
//    /// </summary>
//    public static void DisposeData() {
//      var dataLocal = Interlocked.Exchange(ref data, null);
//      dataLocal?.Dispose();
//    }
//    #endregion


//    #region Properties
//    //      ----------

//    /// <summary>
//    /// Directory of all SampleMasters
//    /// </summary>
//    public StorageDictionary<SampleMaster, DL> SampleMasters { get; private set; }

//    /// <summary>
//    /// Directory of all SampleMasters
//    /// </summary>
//    public StorageDictionary<Sample, DL> Samples { get; private set; }

//    /// <summary>
//    /// Directory of all SampleMasters
//    /// </summary>
//    public StorageDictionary<SampleDetail, DL> SampleDetails { get; private set; }
//    #endregion


//    #region Events
//    //      ------

//    #endregion


//    #region Constructors
//    //      ------------

//    /// <summary>
//    /// Creates a new data context. If csvConfig is null, the data is only stored in Ram, but gets lost once 
//    /// program terminates. With csvConfig defined, existing data gets read at startup, changes immediately written
//    /// and on dispose flushes guarantees that all data is permanently stored.
//    /// </summary>
//    public DL(CsvConfig? csvConfig) {
//      if (csvConfig==null) {
//        SampleMasters = new StorageDictionary<SampleMaster, DL>(
//          this,
//          SampleMaster.SetKey,
//          SampleMaster.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true);
//        Samples = new StorageDictionary<Sample, DL>(
//          this,
//          Sample.SetKey,
//          Sample.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true);
//        SampleDetails = new StorageDictionary<SampleDetail, DL>(
//          this,
//          SampleDetail.SetKey,
//          SampleDetail.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true);
//      } else {
//        SampleMasters = new StorageDictionaryCSV<SampleMaster, DL>(
//          this,
//          csvConfig!,
//          SampleMaster.MaxLineLength,
//          SampleMaster.Headers,
//          SampleMaster.SetKey,
//          SampleMaster.Create,
//          null,
//          SampleMaster.Update,
//          SampleMaster.Write,
//          SampleMaster.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true,
//          isCompactDuringDispose: true);
//        Samples = new StorageDictionaryCSV<Sample, DL>(
//          this,
//          csvConfig!,
//          Sample.MaxLineLength,
//          Sample.Headers,
//          Sample.SetKey,
//          Sample.Create,
//          Sample.Verify,
//          Sample.Update,
//          Sample.Write,
//          Sample.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true,
//          isCompactDuringDispose: true);
//        SampleDetails = new StorageDictionaryCSV<SampleDetail, DL>(
//          this,
//          csvConfig!,
//          SampleDetail.MaxLineLength,
//          SampleDetail.Headers,
//          SampleDetail.SetKey,
//          SampleDetail.Create,
//          SampleDetail.Verify,
//          SampleDetail.Update,
//          SampleDetail.Write,
//          SampleDetail.Disconnect,
//          areItemsUpdatable: true,
//          areItemsDeletable: true,
//          isCompactDuringDispose: true);
//      }
//    }
//    #endregion


//    #region IDisposable Support
//    //      -------------------

//    /// <summary>
//    /// Is DLData already disposed ?
//    /// </summary>
//    public bool IsDisposed {
//      get { return isDisposed==1; }
//    }
//    int isDisposed = 0;


//    protected virtual void Dispose(bool disposing) {
//      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
//      if (wasDisposed==1) return; // already disposed

//      if (disposing) {
//        SampleDetails.Dispose();
//        Samples.Dispose();
//        SampleMasters.Dispose();
//      }

//      // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
//      // TODO: set large fields to null.

//    }


//    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
//    // ~DLSet()
//    // {
//    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
//    //   Dispose(false);
//    // }


//    // This code added to correctly implement the disposable pattern.
//    public void Dispose() {
//      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
//      Dispose(true);
//      // TODO: uncomment the following line if the finalizer is overridden above.
//      // GC.SuppressFinalize(this);
//    }
//    #endregion


//    #region Methods
//    //      -------

//    #endregion

//  }
//}
