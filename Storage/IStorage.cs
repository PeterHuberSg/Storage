using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public interface IStorage<TItem> where TItem : class, IStorage<TItem> {

    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Key { get;  }
    #endregion


    #region Events
    //      ------

    public event Action<TItem>? HasChanged;
    #endregion


    #region Methods
    //      -------

    ///// <summary>
    ///// Copies all values from itemChanged to this item.
    ///// </summary>
    //public void Update(TItem itemChanged);


    /// <summary>
    /// Removes item from StorageDictionary and parent collections, deletes all children.
    /// </summary>
    public void Remove();


    /// <summary>
    /// Returns a shorter string than ToString()
    /// </summary>
    public string ToShortString();
    #endregion
  }

}
