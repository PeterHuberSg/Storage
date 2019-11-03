using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Storage {


  public static class DL {

    #region Properties
    //      ----------


    public static DLSet? Set {
      get { return set; }
      set { set = value; }
    }
    private static DLSet? set;

    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    public static void Init() {
      if (Set!=null) throw new Exception();

      Set = new DLSet();
    }


    public static void DisposeSet() {
      var setLocal = Interlocked.Exchange(ref set, null);
      setLocal?.Dispose();
    }
    #endregion


    #region Methods
    //      -------

    #endregion
  }
}
