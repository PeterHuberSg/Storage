using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  public interface IStorageCSV<TItem>: IStorage<TItem> {

    public string Write(CsvWriter csvWriter);
  }
}
