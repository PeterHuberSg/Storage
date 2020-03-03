/**************************************************************************************

Storage.IStorage
================

Interface IStorage defines the class members needed that a class can be written to and read from a CSV file.

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/
using System;


namespace Storage {


  /// <summary>
  /// Inheriting classes can be written to and read from a CSV file,
  /// </summary>
  public interface IStorage<TItem> where TItem : class, IStorage<TItem> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique key, used to establish Master detail relationships
    /// </summary>
    public int Key { get;  }
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Called when some properties of the class have been changed, which usually requires that the
    /// class gets written to the CSV file.
    /// </summary>
    public event Action<TItem>? HasChanged;
    #endregion


    #region Methods
    //      -------

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
