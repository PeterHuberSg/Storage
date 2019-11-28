using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  public interface IStorageCSV<TItem>: IStorage<TItem> {

    public void Write(CsvWriter csvWriter);

    public void UpdateFromCsvLine(CsvReader csvReader);
  }
}
