using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class DictionaryTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParentDictionary = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedDictionaryChild= new Dictionary<int, string>();


    [TestMethod]
    public void TestDictionary() {
      try {
        var directoryInfo = new DirectoryInfo("TestCsv");
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();

        csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        initDL();
        assertDL();

        var now = DateTime.Now.Date;
        addParentDictionary("1");
        addDictionaryChild(0, now, "11");

        addParentDictionary("2");
        addDictionaryChild(1, now, "21");
        addDictionaryChild(1, now.AddDays(1), "22");

        removeParentDictionary(0);

        removeDictionaryChild(1);

      } finally {
        DL.DisposeData();
      }
    }


    private void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private void initDL() {
      DL.Init(csvConfig);
    }


    private void assertDL() {
      Assert.AreEqual(expectedParentDictionary.Count, DL.Data.ParentsWithDictionary.Count);
      foreach (var parentDictionary in DL.Data.ParentsWithDictionary) {
        Assert.AreEqual(expectedParentDictionary[parentDictionary.Key], parentDictionary.ToString());
      }

      Assert.AreEqual(expectedDictionaryChild.Count, DL.Data.SortedListyChildren.Count);
      foreach (var dictionaryChild in DL.Data.SortedListyChildren) {
        Assert.AreEqual(expectedDictionaryChild[dictionaryChild.Key], dictionaryChild.ToString());
      }
    }


    private void addParentDictionary(string someText) {
      var newParentDictionary = new ParentWithDictionary(someText, isStoring: true);
      expectedParentDictionary.Add(newParentDictionary.Key, newParentDictionary.ToString());
      assertData();
    }


    private void addDictionaryChild(int parentDictionaryKey, DateTime date, string text) {
      var parentDictionary = DL.Data.ParentsWithDictionary[parentDictionaryKey];
      var newDictionaryChild = new DictionaryChild(parentDictionary, date, text, isStoring: true);
      expectedDictionaryChild.Add(newDictionaryChild.Key, newDictionaryChild.ToString());
      expectedParentDictionary[parentDictionary.Key] = parentDictionary.ToString();
      assertData();
    }


    private void removeParentDictionary(int parentDictionaryKey) {
      var parent = DL.Data.ParentsWithDictionary[parentDictionaryKey];
      foreach (var child in parent.DictionaryChildren.Values) {
        expectedDictionaryChild.Remove(child.Key);
      }
      expectedParentDictionary.Remove(parentDictionaryKey);
      parent.Remove();
      assertData();
    }


    private void removeDictionaryChild(int dictionaryChildKey) {
      var child = DL.Data.SortedListyChildren[dictionaryChildKey];
      expectedDictionaryChild.Remove(child.Key);
      var parentDictionary = child.ParentWithDictionary;
      child.Remove();
      expectedParentDictionary[parentDictionary.Key] = parentDictionary.ToString();
      assertData();
    }


    private void assertData() {
      assertDL();
      DL.DisposeData();

      initDL();
      assertDL();
    }
  }
}
