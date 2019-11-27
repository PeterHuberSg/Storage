using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageTest {


  public class TestItemCsv: IStorageCSV<TestItemCsv> {

    public int Key { get; set; }

    public string Text {
      get { return text; }
      set {
        if (text!=value) {
          text = value;
          HasChanged?.Invoke(this);
        }
      }
    }
    private string text;


    public static readonly string[] Headers = { "Key", "Text" };


    public event Action<TestItemCsv>? HasChanged;


    public TestItemCsv(int key, string text) {
      Key = key;
      this.text = text;
    }


    public bool CanDelete() { return true; }


    public string Write(CsvWriter csvWriter) {
      throw new NotImplementedException();
    }


    public string ToCsvString(char delimiter) {
      return "" +
        Key + delimiter +
        Text + delimiter;
    }


    public static TestItemCsv? ReadCsvLine(string line, char delimiter, StringBuilder errorStringBuilder) {
      var fields = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
      if (fields.Length!=Headers.Length) {
        errorStringBuilder.AppendLine($"TestItem should have {Headers.Length} fields, but had {fields.Length}: '{line}'.");
        return null;
      }

      int fieldIndex = 0;
      int key = Csv.ParseInt("TestRecord.Id", fields[fieldIndex++], line, errorStringBuilder);
      string text = fields[fieldIndex++];
      var item = new TestItemCsv(key, text);
      return item;
    }


    public static void UpdateFromCsvLine(
      string line, 
      char delimiter, 
      StorageDictionary<TestItemCsv> storageDirectoryCSV, 
      StringBuilder errorStringBuilder) 
    {
      var itemChanged = ReadCsvLine(line, delimiter, errorStringBuilder);
      if (itemChanged==null) return;

      var item = storageDirectoryCSV[itemChanged.Key];
      item.Update(itemChanged);
    }


    public void Update(TestItemCsv itemChanged) {
      Text = itemChanged.Text;
      HasChanged?.Invoke(this);
    }


    public override string ToString() {
      return $"Key: {Key}; Text: {Text};";
    }
  }
}
