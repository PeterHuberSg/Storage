/**************************************************************************************

Storage.IStorage
================

Interfaces for Storage

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;


/*
Storage uses 2 Interfaces:
--------------------------

IStorageItemGeneric<TItem>: IStorageItem
Implemented by classes who wants to store their data in DataStore

IStorageItem
None generic version of IStorageItemGeneric<TItem>, used for transaction processing mostly
*/

namespace Storage {


  #region StorageItem Interfaces
  //      ----------------------

  /// <summary>
  /// Gives none generic access to IStorageItemGeneric<TItem>, used only for transaction processing
  /// </summary>
  public interface IStorageItem {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique key, used to identify item
    /// </summary>
    public int Key { get; }
    #endregion


    #region Methods
    //      -------

    /*+
    /// <summary>
    /// Copies all data except any collection (children) to a new IStorageItem instance.
    /// </summary>
    public IStorageItem Clone();
    +*/

    /// <summary>
    /// Item.Store() adds the item to its DataStore items. It can also be added to the DataStore by setting the construction
    /// parameter isStoring to true.
    /// </summary>
    public void Store();


    /// <summary>
    /// Removes item from DataStore and parent collections, deletes all children.
    /// </summary>
    public void Remove();


    /// <summary>
    /// Returns a shorter string than ToString()
    /// </summary>
    public string ToShortString();
    #endregion
  }


  /// <summary>
  /// Inheriting classes can be written to and read from a CSV file,
  /// </summary>
  public interface IStorageItemGeneric<TItem>: IStorageItem where TItem : class {

    #region Events
    //      ------

    /// <summary>
    /// Called when some properties of the class have been changed, which usually requires that the
    /// class gets written to the CSV file.
    /// </summary>
    public event Action</*old*/TItem, /*new*/TItem>? HasChanged;
    #endregion
  }
  #endregion
}