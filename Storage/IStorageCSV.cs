//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Storage {


//  public interface IStorageCSV<TItem>: IStorage<TItem> where TItem : class, IStorage<TItem> {

//    /// <summary>
//    /// Should only be called from StorageDirectory. Writes the values of TItem to a CSV file.
//    /// </summary>
//    public void Write(CsvWriter csvWriter);


//    /// <summary>
//    /// Should only be called from StorageDirectory. Overrwrites the values of TItem with the
//    /// values read from csvReader. The caller has to read the Key first and find TItem.
//    /// </summary>
//    public void Update(CsvReader csvReader);
//  }
//}
