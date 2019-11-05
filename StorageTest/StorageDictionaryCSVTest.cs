using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class StorageDictionaryCSVTest {

#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    StorageDictionary<TestItemCsv> dictionary;
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
    bool wasAdded = false;
    bool wasChanged = false;
    bool wasDeleted = false;

    const bool cont = true;
    const bool notC = false;
    CsvConfig? csvConfig;


    [TestMethod]
    public void TestStorageDictionaryCSV() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      try {
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        dictionary = createDictionary();
        var expectedList = new List<string>();
        assert(expectedList, cont, ref dictionary);

        add(dictionary, expectedList, 2, "2", cont);
        add(dictionary, expectedList, 3, "3", cont);
        add(dictionary, expectedList, 4, "4", cont);
        add(dictionary, expectedList, 6, "6", notC);
        add(dictionary, expectedList, 7, "7", notC);

        remove(dictionary, expectedList, 7, notC);
        add(dictionary, expectedList, 7, "7a", notC);
        remove(dictionary, expectedList, 7, notC);

        remove(dictionary, expectedList, 6, cont);
        add(dictionary, expectedList, 6, "6a", notC);
        remove(dictionary, expectedList, 6, cont);

        add(dictionary, expectedList, 5, "5", cont);
        add(dictionary, expectedList, 6, "6", cont);

        update(dictionary, expectedList, 4, "4a", cont);

        remove(dictionary, expectedList, 2, cont);
        remove(dictionary, expectedList, 6, cont);
        remove(dictionary, expectedList, 4, notC);
        remove(dictionary, expectedList, 5, cont);
        remove(dictionary, expectedList, 3, cont);

        add(dictionary, expectedList, 1, "1a", cont);
        add(dictionary, expectedList, 6, "6b", notC);

      } finally {
        dictionary?.Dispose();
        directoryInfo.Delete(recursive: true);
      }


    }


    private StorageDictionary<TestItemCsv> createDictionary() {
      dictionary = new StorageDictionaryCSV<TestItemCsv>(
        csvConfig!,
        TestItemCsv.Headers, 
        TestItemCsv.ReadCsvLine, 
        true,
        true,
        true,
        TestItemCsv.UpdateFromCsvLine);
      dictionary.Added += dictionary_Added;
      dictionary.Changed += dictionary_Changed;
      dictionary.Removed += dictionary_Deleted;
      return dictionary;
    }


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "ex is required by CsvConfig")]
    private void reportException(Exception ex) {
      Assert.Fail();
    }


    private void dictionary_Added(TestItemCsv item) {
      Assert.IsFalse(wasAdded);
      wasAdded = true;
      Assert.IsTrue(dictionary[item.Key].Text==item.Text);
    }


    private void dictionary_Changed(TestItemCsv item) {
      Assert.IsFalse(wasChanged);
      wasChanged = true;
      Assert.IsTrue(dictionary[item.Key].Text==item.Text);
    }


    private void dictionary_Deleted(TestItemCsv item) {
      Assert.IsFalse(wasDeleted);
      wasDeleted = true;
      Assert.IsFalse(dictionary.ContainsKey(item.Key));
    }


    private void add(StorageDictionary<TestItemCsv> dictionary, List<string> expectedList, int key, string text, bool cont) {
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var data = new TestItemCsv(key, text);
      dictionary.Add(data);
      Assert.IsTrue(wasAdded);
      wasAdded = false;
      assert(expectedList, cont, ref dictionary);
    }


    private void update(StorageDictionary<TestItemCsv> dictionary, List<string> expectedList, int key, string text, bool cont) {
      removeExpected(expectedList, key);
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var item = dictionary[key];
      item.Text = text; //fires HasChanged event
      Assert.IsTrue(wasChanged);
      wasChanged = false;
      assert(expectedList, cont, ref dictionary);
    }


    private void remove(StorageDictionary<TestItemCsv> dictionary, List<string> expectedList, int key, bool cont) {
      removeExpected(expectedList, key);
      dictionary.Remove(key);
      Assert.IsTrue(wasDeleted);
      wasDeleted = false;
      assert(expectedList, cont, ref dictionary);
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


    private void assert(List<string> expectedList, bool areKeysContinous, ref StorageDictionary<TestItemCsv> dictionary) {
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

      dictionary.Dispose();

      dictionary = createDictionary();
    }
  }
}
