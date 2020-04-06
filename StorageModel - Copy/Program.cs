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

using System;
using System.IO;
using System.Text;

namespace Storage {


  /// <summary>
  /// starts and configures data compiler by just creating a new StorageClassGenerator
  /// </summary>
  class Program {


    public static void Main(string[] _) {
      Console.WriteLine("Press 'y' 'enter' to copy the files from {} to {}.");
      var r = Console.ReadLine();
      if (Console.ReadLine().Length==0) {

      }
      var storageSolutionDirectory = new DirectoryInfo(Environment.CurrentDirectory).Parent.Parent.Parent.Parent;
      var sourceDirectoryPath = storageSolutionDirectory.FullName + @"\StorageModel";
      var targetDirectoryPath = storageSolutionDirectory.FullName + @"\StorageClasses";

      #region normally not needed code ---------------------------------------------------------------
      //normally, do not delete all files in targetDirectory, because manual changes in Xxx.CS files would get lost.
      //We can do it here, because there are no changes in Xxx.CS files, but the model might have changed and some
      //Xxx classes are no longer needed. To get rid of those, we just delete here all.
      var sourceDirectory = new DirectoryInfo(targetDirectoryPath);
      foreach (FileInfo file in sourceDirectory.GetFiles()) {
        file.Delete();
      }
      #endregion -------------------------------------------------------------------------------------

      new StorageClassGenerator(
        sourceDirectoryString: @"C:\Users\peter\Source\Repos\Storage\StorageModel", //directory from where the .cs files get read.
        targetDirectoryString: targetDirectoryPath, //directory where the new .cs files get written.
        context: "DL"); //>Name of Context class, which gives static access to all data stored.

      #region normally not needed code ----------------------------------------------------------------
      //normally, there is no sampleDirectory. We have it here for the unit tests, so they still work, 
      //while the files in targetDirectory have already been changed, but might not be correct yet.
      var sampleDirectoryPath = storageSolutionDirectory.FullName + @"\StorageSample";
      Console.WriteLine("Do you want to copy the files from ");
      #endregion -------------------------------------------------------------------------------------
    }
  }
}
