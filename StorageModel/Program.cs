using System;


namespace Storage {


  class Program {


    public static void Main(string[] _) {
      new StorageClassGenerator(
        sourceDirectoryString: @"C:\Users\peter\Source\Repos\Storage\StorageModel", //directory from where the .cs files get read.
        targetDirectoryString: @"C:\Users\peter\Source\Repos\Storage\StorageClasses", //directory where the new .cs files get written.
        context: "DL"); //>Name of Context class, which gives static access to all data stored.
    }
  }
}
