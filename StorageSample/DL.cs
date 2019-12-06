using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Storage {


  public static class DL {

    #region Properties
    //      ----------


    public static DLData? Data {
      get { return data; }
      set { data = value; }
    }
    private static DLData? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()

    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    public static void Init(CsvConfig? csvConfig, bool isCompactDuringDispose = true) {
      if (Data!=null) throw new Exception();

      Data = new DLData(csvConfig, isCompactDuringDispose);
    }


    public static void DisposeData() {
      var dataLocal = Interlocked.Exchange(ref data, null);
      dataLocal?.Dispose();
    }
    #endregion


    #region Methods
    //      -------

    #endregion
  }
}
