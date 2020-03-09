/**************************************************************************************

StorageModel
============

Shows how to start and configure data compiler by just creating a new StorageClassGenerator
in a console application

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
  /// starts and configures data compiler by just creating a new StorageClassGenerator
  /// </summary>
  class Program {


    public static void Main(string[] _) {
      new StorageClassGenerator(
        sourceDirectoryString: @"C:\Users\peter\Source\Repos\Storage\StorageModel", //directory from where the .cs files get read.
        targetDirectoryString: @"C:\Users\peter\Source\Repos\Storage\StorageClasses", //directory where the new .cs files get written.
        context: "DL"); //>Name of Context class, which gives static access to all data stored.
    }
  }
}
