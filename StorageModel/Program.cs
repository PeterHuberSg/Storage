﻿/**************************************************************************************

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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Storage {

  /// <summary>
  /// starts and configures data compiler by just creating a new StorageClassGenerator
  /// </summary>
  class Program {


    public static void Main(string[] _) {
      var storageSolutionDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent;
      var sourceDirectoryPath = storageSolutionDirectory.FullName + @"\StorageModel";
      var targetDirectoryPath = storageSolutionDirectory.FullName + @"\StorageDataContext";

      #region normally not needed code ---------------------------------------------------------------
      //normally, do not delete all files in targetDirectory, because manual changes in Xxx.CS files would get lost.
      //We can do it here, because there are no changes in Xxx.CS files, but the model might have changed and some
      //Xxx classes are no longer needed. To get rid of those, we just delete here all files.
      var targetDirectory = new DirectoryInfo(targetDirectoryPath);
      foreach (FileInfo file in targetDirectory.GetFiles()) {
        if (file.Extension.ToLowerInvariant()==".cs" && !file.Name.StartsWith('_')) {
          file.Delete();
        }
      }
      #endregion -------------------------------------------------------------------------------------

      new StorageClassGenerator(
        sourceDirectoryString: sourceDirectoryPath, //directory from where the .cs files get read.
        targetDirectoryString: targetDirectoryPath, //directory where the new .cs files get written.
        context: "DC", //class name of data context, which gives static access to all data stored.
        //isTracing: TracingEnum.noTracing, //defines if tracing instructions should get added to the code
        isTracing: TracingEnum.debugOnlyTracing, //defines if tracing instructions should get added to the code
        isFullyCommented: false); //If true (default), the created .cs files (not .base.cs files) have all code lines 
                                  //commented out. False is only used here for testing.
    }
  }
}
