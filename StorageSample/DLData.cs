using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Storage {
 
  
  public partial class DLData: IDisposable {

    #region Properties
    //      ----------

    public StorageDictionary<SampleMaster, DLData> SampleMasters { get; private set; }
    public StorageDictionary<Sample, DLData> Samples { get; private set; }
    public StorageDictionary<SampleDetail, DLData> SampleDetails { get; private set; }
    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    public DLData(CsvConfig? csvConfig, bool isCompactDuringDispose = true) {
      if (csvConfig==null) {
        SampleMasters = new StorageDictionary<SampleMaster, DLData>(
          this,
          SampleMaster.SetKey,
          SampleMaster.Disconnect, 
          areItemsUpdatable: true, 
          areItemsDeletable: true);
        Samples = new StorageDictionary<Sample, DLData>(
          this,
          Sample.SetKey,
          Sample.Disconnect,
          areItemsUpdatable: true, 
          areItemsDeletable: true);
        SampleDetails = new StorageDictionary<SampleDetail, DLData>(
          this,
          SampleDetail.SetKey,
          SampleDetail.Disconnect,
          areItemsUpdatable: true, 
          areItemsDeletable: true);
      } else {
        SampleMasters = new StorageDictionaryCSV<SampleMaster, DLData>(
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
          isCompactDuringDispose: isCompactDuringDispose);
        Samples = new StorageDictionaryCSV<Sample, DLData>(
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
          areItemsUpdatable: true,
          areItemsDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
        SampleDetails = new StorageDictionaryCSV<SampleDetail, DLData>(
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
          isCompactDuringDispose: isCompactDuringDispose);
      }
    }
    #endregion


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Is DLData already disposed ?
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

      // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
      // TODO: set large fields to null.

    }


    // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
    // ~DLSet()
    // {
    //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
    //   Dispose(false);
    // }


    // This code added to correctly implement the disposable pattern.
    public void Dispose() {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
      // TODO: uncomment the following line if the finalizer is overridden above.
      // GC.SuppressFinalize(this);
    }
    #endregion


    #region Methods
    //      -------

    #endregion
  }
}
