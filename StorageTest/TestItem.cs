using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageTest {


  public class TestItem: IStorage<TestItem> {
    public int Key { get; private set; }
    public static void SetKey(TestItem testItem, int key) { testItem.Key = key; }

    public string Text { get; private set; }


    public event Action<TestItem>? HasChanged;


    public TestItem(string text) {
      Key = Storage.Storage.NoKey;
      Text = text;
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


    public void Remove(StorageDictionary<TestItem, object> storageDictionary) {
      if (Key<0) {
        throw new Exception($"TestItem.Remove(): TestItem '{this}' is not stored in storageDictionary, key is {Key}.");
      }
      storageDictionary.Remove(Key);
    }


    internal static void Disconnect(TestItem item) {
      //nothing to do
    }


    public string ToShortString() {
      return $"{Key.ToKeyString()}, {Text};";
    }


    public override string ToString() {
      return $"Key: {Key}; Text: {Text};";
    }
  }
}
