using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageTest {


  public class TestItem: IStorageItemGeneric<TestItem> {
    public int Key { get; private set; }
    public static void SetKey(IStorageItem testItem, int key) { ((TestItem)testItem).Key = key; }

    public string Text { get; private set; }


    public event Action<TestItem, TestItem>? HasChanged;


    public TestItem(string text) {
      Key = StorageExtensions.NoKey;
      Text = text;
    }


    public void Store() {
      throw new NotSupportedException();
    }


    public void Update(string text) {
      if (Text!=text) {
        var old = new TestItem(Text);
        Text = text;
        HasChanged?.Invoke(old, this);
      }
    }


    public void Remove() {
      throw new NotImplementedException();
    }


    public void Remove(DataStore<TestItem> dataStore) {
      if (Key<0) {
        throw new Exception($"TestItem.Remove(): TestItem '{this}' is not stored in storageDictionary, key is {Key}.");
      }
      dataStore.Remove(Key);
    }


    internal static void Disconnect(TestItem _) {
      //nothing to do
    }


    internal static void RollbackItemStore(IStorageItem _) {
    }


    internal static void RollbackItemUpdate(IStorageItem oldItem, IStorageItem newItem) {
    }


    internal static void RollbackItemRemove(IStorageItem _) {
    }


    public string ToShortString() {
      return $"{Key.ToKeyString()}, {Text};";
    }


    public override string ToString() {
      return $"Key: {Key}; Text: {Text};";
    }
  }
}
