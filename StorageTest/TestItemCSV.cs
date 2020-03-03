using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageTest {


  public class TestItemCsv: IStorage<TestItemCsv> {

    public int Key { get; private set; }
    internal static void SetKey(TestItemCsv testItemCsv, int key) { testItemCsv.Key = key; }

    public string Text { get; private set; }


    public static readonly string[] Headers = { "Key", "Text" };

    public const int MaxLineLength = 30;

    public event Action<TestItemCsv>? HasChanged;


    public TestItemCsv(string text) {
      Key = StorageExtensions.NoKey;
      Text = text;
    }


    public TestItemCsv(int key, CsvReader csvReader) {
      Key = key;
      Text = csvReader.ReadString()!;
    }


    public static TestItemCsv Create(int key, CsvReader csvReader, object _) {
      return new TestItemCsv(key, csvReader);
    }


    internal static void Write(TestItemCsv testItemCsv, CsvWriter csvWriter) {
      csvWriter.Write(testItemCsv.Text);
    }


    internal static void Update(TestItemCsv testItemCsv, CsvReader csvReader, object _) 
    {
      testItemCsv.Text = csvReader.ReadString()!;
    }


    public void Update(string text) {
      if (Text!=text) {
        Text = text;
        HasChanged?.Invoke(this);
      }
    }


    public void Remove() {
      throw new NotImplementedException();
    }


    public void Remove(StorageDictionary<TestItemCsv, object> storageDictionary) {
      if (Key<0) {
        throw new Exception($"TestItemCsv.Remove(): TestItemCsv is not in storageDictionary, key is {Key}.");
      }
      storageDictionary.Remove(Key);
    }


    internal static void Disconnect(TestItemCsv _) {
      //nothing to do
    }


    public string ToShortString() {
      return $"{Key.ToKeyString()}, {Text};";
    }


    public override string ToString() {
      return $"Key: {Key.ToKeyString()}; Text: {Text};";
    }
  }
}
