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

    public const int MaxLineLength = 30;

    public event Action<TestItemCsv>? HasChanged;


    public TestItemCsv(int key, string text) {
      Key = key;
      this.text = text;
    }


    public bool CanDelete() { return true; }


    public void Write(CsvWriter csvWriter) {
      csvWriter.Write(Key);
      csvWriter.Write(Text);
    }



    public static TestItemCsv? ReadCsvLine(CsvReader csvReader) {
      int key = csvReader.ReadInt();
      string text = csvReader.ReadString()!;
      var item = new TestItemCsv(key, text);
      return item;
    }


    public void UpdateFromCsvLine(CsvReader csvReader) 
    {
      Text = csvReader.ReadString()!;
      HasChanged?.Invoke(this);
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
