﻿using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {
 
  
  public class DLData: IDisposable {

    #region Properties
    //      ----------

    public StorageDictionary<SampleMaster> SampleMasters { get; private set; }
    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    public DLData(CsvConfig? csvConfig) {
      if (csvConfig==null) {
        SampleMasters = new StorageDictionary<SampleMaster>(areItemsUpdatable: true, areItemsDeletable: true);
      } else {
        //SampleMasters = new StorageDictionaryCSV<SampleMaster>(
        //  csvConfig,
        //  SampleMaster.Headers,);
      }
    }
    #endregion


    #region IDisposable Support
    //      -------------------

    private bool disposedValue = false; // To detect redundant calls


    protected virtual void Dispose(bool disposing) {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects).
        }

        // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
        // TODO: set large fields to null.

        disposedValue = true;
      }
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
