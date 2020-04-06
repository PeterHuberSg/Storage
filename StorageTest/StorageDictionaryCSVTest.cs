using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class StorageDictionaryCSVTest {

    CsvConfig? csvConfig;
    StorageDictionary<TestItemCsv, object>? dictionary;

    bool wasAdded = false;
    bool wasChanged = false;
    bool wasDeleted = false;

    const bool cont = true;
    const bool notC = false;


    #region Readonly
    //      --------

    [TestMethod]
    public void TestStorageDictionaryReadonlyCSV() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      try {
        dictionary?.Dispose();
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        dictionary = createDictionaryReadonly();
        var expectedList = new List<string>();
        assertRewriteReadonly(expectedList, cont, ref dictionary);

        addReadonly(dictionary, expectedList, 0, "0");
        addReadonly(dictionary, expectedList, 1, "1");
        addReadonly(dictionary, expectedList, 2, "2");
        addReadonly(dictionary, expectedList, 3, "3");
      } finally {
        dictionary?.Dispose();
      }
    }


    private StorageDictionary<TestItemCsv, object> createDictionaryReadonly() {
      dictionary = new StorageDictionaryCSV<TestItemCsv, object>(
        null, //no context needed
        csvConfig!,
        TestItemCsv.MaxLineLength,
        TestItemCsv.Headers,
        TestItemCsv.SetKey,
        TestItemCsv.Create,
        null,
        TestItemCsv.Write);
      Assert.IsTrue(dictionary.IsReadOnly);
      dictionary.Added += dictionary_Added;
      dictionary.Changed += illegalActivity;
      dictionary.Removed += illegalActivity;
      return dictionary;
    }


    private void illegalActivity(TestItemCsv _) {
      throw new Exception();


    }
    private void assertRewriteReadonly(List<string> expectedList, bool areKeysContinous, ref StorageDictionary<TestItemCsv, object> dictionary) {
      assert(expectedList, areKeysContinous, ref dictionary);
      dictionary.Dispose();

      dictionary = createDictionaryReadonly();
      assert(expectedList, areKeysContinous, ref dictionary);
    }


    private void addReadonly(StorageDictionary<TestItemCsv, object> dictionary, List<string> expectedList, int key, string text) {
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var testItemCsv = new TestItemCsv(text);
      dictionary.Add(testItemCsv);
      Assert.IsTrue(wasAdded);
      wasAdded = false;
      assertRewriteReadonly(expectedList, cont, ref dictionary);
    }
    #endregion


    #region Updatable
    //      ---------
    [TestMethod]
    public void TestStorageDictionaryCSV() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      for (int configurationIndex = 0; configurationIndex < 2; configurationIndex++) {
        try {
          directoryInfo.Refresh();
          if (directoryInfo.Exists) {
            directoryInfo.Delete(recursive: true);
            directoryInfo.Refresh();
          }

          directoryInfo.Create();
          directoryInfo.Refresh();

          csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
          dictionary = createDictionary();
          var expectedList = new List<string>();
          assertRewrite(expectedList, cont, ref dictionary);

          add(dictionary, expectedList, 0, "0", cont);
          add(dictionary, expectedList, 1, "1", cont);
          add(dictionary, expectedList, 2, "2", cont);
          add(dictionary, expectedList, 3, "3", cont);
          add(dictionary, expectedList, 4, "4", cont);

          remove(dictionary, expectedList, 3, notC);
          add(dictionary, expectedList, 5, "5", notC);
          remove(dictionary, expectedList, 4, notC);
          remove(dictionary, expectedList, 5, cont);

          add(dictionary, expectedList, 3, "3a", cont);
          update(dictionary, expectedList, 2, "2a", cont);

          remove(dictionary, expectedList, 2, notC);
          remove(dictionary, expectedList, 3, cont);
          remove(dictionary, expectedList, 0, cont);
          remove(dictionary, expectedList, 1, cont);

          add(dictionary, expectedList, 0, "0a", cont);
          add(dictionary, expectedList, 1, "1a", cont);

        } finally {
          dictionary?.Dispose();
          directoryInfo.Delete(recursive: true);
        }
      }
    }


    private StorageDictionary<TestItemCsv, object> createDictionary() {
      dictionary = new StorageDictionaryCSV<TestItemCsv, object>(
        null, //no context needed
        csvConfig!,
        TestItemCsv.MaxLineLength,
        TestItemCsv.Headers,
        TestItemCsv.SetKey,
        TestItemCsv.Create,
        null,
        TestItemCsv.Update,
        TestItemCsv.Write,
        TestItemCsv.Disconnect,
        areInstancesUpdatable: true,
        areInstancesDeletable: true);
      Assert.IsFalse(dictionary.IsReadOnly);
      dictionary.Added += dictionary_Added;
      dictionary.Changed += dictionary_Changed;
      dictionary.Removed += dictionary_Deleted;
      return dictionary;
    }


    private void reportException(Exception ex) {
      Assert.Fail();
    }


    private void dictionary_Added(TestItemCsv item) {
      Assert.IsFalse(wasAdded);
      wasAdded = true;
      Assert.IsTrue(dictionary![item.Key].Text==item.Text);
    }


    private void dictionary_Changed(TestItemCsv item) {
      Assert.IsFalse(wasChanged);
      wasChanged = true;
      Assert.IsTrue(dictionary![item.Key].Text==item.Text);
    }


    private void dictionary_Deleted(TestItemCsv item) {
      Assert.IsFalse(wasDeleted);
      wasDeleted = true;
      Assert.IsFalse(dictionary!.ContainsKey(item.Key));
    }


    private void add(StorageDictionary<TestItemCsv, object> dictionary, List<string> expectedList, int key, string text, bool isCont) {
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var testItemCsv = new TestItemCsv(text);
      Assert.AreEqual(StorageExtensions.NoKey, testItemCsv.Key);
      dictionary.Add(testItemCsv);
      Assert.IsTrue(wasAdded);
      wasAdded = false;
      assertRewrite(expectedList, isCont, ref dictionary);
    }


    private void update(StorageDictionary<TestItemCsv, object> dictionary, List<string> expectedList, int key, string text, bool isCont) {
      removeExpected(expectedList, key);
      var dataString = $"{key}|{text}";
      expectedList.Add(dataString);
      var item = dictionary[key];
      item.Update(text); //fires HasChanged event
      Assert.IsTrue(wasChanged);
      wasChanged = false;
      assertRewrite(expectedList, isCont, ref dictionary);
    }


    private void remove(StorageDictionary<TestItemCsv, object> dictionary, List<string> expectedList, int key, bool isCont) {
      removeExpected(expectedList, key);
      dictionary.Remove(key);
      Assert.IsTrue(wasDeleted);
      wasDeleted = false;
      assertRewrite(expectedList, isCont, ref dictionary);
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


    private void assertRewrite(List<string> expectedList, bool areKeysContinous, ref StorageDictionary<TestItemCsv, object> dictionary) {
      assert(expectedList, areKeysContinous, ref dictionary);
      dictionary.Dispose();

      dictionary = createDictionary();
      assert(expectedList, areKeysContinous, ref dictionary);
    }


    private void assert(List<string> expectedList, bool areKeysContinous, ref StorageDictionary<TestItemCsv, object> dictionary) {
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
    #endregion
  }
}
