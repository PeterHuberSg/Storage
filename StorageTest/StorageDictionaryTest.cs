using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class StorageDictionaryTest {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    StorageDictionary<TestItem, object> dictionary;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    bool wasAdded = false;
    bool wasChanged = false;
    bool wasDeleted = false;

    const bool cont = true;
    const bool notC = false;


    [TestMethod]
    public void TestStorageDictionary() {
      dictionary = new StorageDictionary<TestItem, object>(
        null,
        TestItem.SetKey,
        TestItem.Disconnect, 
        areItemsUpdatable: true, 
        areItemsDeletable: true);
      dictionary.Added += dictionary_Added;
      dictionary.Changed += dictionary_Changed;
      dictionary.Removed += dictionary_Deleted;
      var expectedList = new List<string>();
      assert(expectedList, cont, dictionary);

      add(dictionary, expectedList, 0, "0", cont);
      add(dictionary, expectedList, 1, "1", cont);
      add(dictionary, expectedList, 2, "2", cont);
      add(dictionary, expectedList, 3, "3", cont);
      add(dictionary, expectedList, 4, "4", cont);

      remove(dictionary, expectedList, 3, notC);
      add(dictionary, expectedList, 5, "5", notC);
      remove(dictionary, expectedList, 5, notC);
      remove(dictionary, expectedList, 4, cont);

      add(dictionary, expectedList, 3, "3a", cont);

      update(dictionary, expectedList, 2, "2a", cont);

      remove(dictionary, expectedList, 1, notC);
      remove(dictionary, expectedList, 0, cont);
      remove(dictionary, expectedList, 2, cont);
      remove(dictionary, expectedList, 3, cont);

      add(dictionary, expectedList, 0, "0b", cont);
      add(dictionary, expectedList, 1, "1b", cont);
      add(dictionary, expectedList, 2, "2b", cont);
      add(dictionary, expectedList, 3, "3b", cont);
      remove(dictionary, expectedList, 0, cont);
      add(dictionary, expectedList, 4, "4b", cont);
      remove(dictionary, expectedList, 2, notC);
      add(dictionary, expectedList, 5, "5b", notC);
      remove(dictionary, expectedList, 3, notC);
      remove(dictionary, expectedList, 1, cont);
      remove(dictionary, expectedList, 5, cont);
    }


    private void dictionary_Added(TestItem item) {
      Assert.IsFalse(wasAdded);
      wasAdded = true;
      Assert.IsTrue(dictionary[item.Key].Text==item.Text);
    }


    private void dictionary_Changed(TestItem item) {
      Assert.IsFalse(wasChanged);
      wasChanged = true;
      Assert.IsTrue(dictionary[item.Key].Text==item.Text);
    }


    private void dictionary_Deleted(TestItem item) {
      Assert.IsFalse(wasDeleted);
      wasDeleted = true;
      Assert.IsFalse(dictionary.ContainsKey(item.Key));
    }


    private void add(StorageDictionary<TestItem, object> dictionary, List<string> expectedList, int key, string text, bool cont) {
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var testItem = new TestItem(text);
      Assert.AreEqual(StorageExtensions.NoKey, testItem.Key);
      dictionary.Add(testItem);
      Assert.IsTrue(wasAdded);
      wasAdded = false;
      assert(expectedList, cont, dictionary);
    }


    private void update(StorageDictionary<TestItem, object> dictionary, List<string> expectedList, int key, string text, bool cont) {
      removeExpected(expectedList, key);
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var item = dictionary[key];
      item.Update(text); //fires HasChanged event
      Assert.IsTrue(wasChanged);
      wasChanged = false;
      assert(expectedList, cont, dictionary);
    }


    private void remove(StorageDictionary<TestItem, object> dictionary, List<string> expectedList, int key, bool cont) {
      removeExpected(expectedList, key);
      dictionary.Remove(key);
      Assert.IsTrue(wasDeleted);
      wasDeleted = false;
      assert(expectedList, cont, dictionary);
    }


    private void removeExpected(List<string> expectedList, int key) {
      var keyString = key.ToString();
      var hasFound = false;
      for (int index = 0; index < expectedList.Count; index++) {
        if (expectedList[index].Split("|")[0]==keyString) {
          expectedList.RemoveAt(index);
          hasFound = true;
        }
      }
      Assert.IsTrue(hasFound);
    }


    private void assert(List<string> expectedList, bool areKeysContinous, StorageDictionary<TestItem, object> dictionary) {
      int count = expectedList.Count;
      Assert.AreEqual(count, dictionary.Count);
      Assert.AreEqual(count, dictionary.Keys.Count);
      Assert.AreEqual(count, dictionary.Values.Count);
      Assert.AreEqual(areKeysContinous, dictionary.AreKeysContinous);
      for (int index = 0; index < count; index++) {
        var fields = expectedList[index].Split("|");
        var key = int.Parse(fields[0]);
        var data = dictionary[key];
        Assert.AreEqual(fields[1], data.Text);
        Assert.IsTrue(dictionary.Keys.Contains(key));
        Assert.IsTrue(dictionary.Values.Contains(data));
      }
      var countedItems = 0;
      foreach (var data in dictionary) {
        countedItems++;
        var dataString = $"{data.Key}|{data.Text}";
        Assert.IsTrue(expectedList.Contains(dataString));
      }
      Assert.AreEqual(count, countedItems);
    }
  }
}
