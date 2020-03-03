/**************************************************************************************

Storage.StorageExtensions
=========================

Extension used to display key as string.

Written in 2020 by Jürgpeter Huber 
Contact: PeterCode at Peterbox dot com

To the extent possible under law, the author(s) have dedicated all copyright and 
related and neighboring rights to this software to the public domain worldwide under
the Creative Commons 0 license (details see COPYING.txt file, see also
<http://creativecommons.org/publicdomain/zero/1.0/>). 

This software is distributed without any warranty. 
**************************************************************************************/


namespace Storage {


  /// <summary>
  /// Extension used to display key as string.
  /// </summary>
  public static class StorageExtensions {

    public const int NoKey = -1;


    /// <summary>
    /// Display key as string
    /// </summary>
    public static string ToKeyString(this int key) {
      if (key==NoKey) return "no";
      return key.ToString();
    }

  }
}
