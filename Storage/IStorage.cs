using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public interface IStorage<TItem> {

    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Key { get; }


    #endregion


    #region Events
    //      ------

    public event Action<TItem>? HasChanged;
    #endregion


    #region Methods
    //      -------

    public void Update(TItem itemChanged);


    public bool CanDelete();


    public static int HandleId(object lockObject, ref int lastId, int id) {
      lock (lockObject) {
        if (id>=0) {
          if (lastId<id) {
            lastId = id;
          }
        } else {
          id = lastId++;
        }
      }
      return id;
    }
    #endregion
  }

}
