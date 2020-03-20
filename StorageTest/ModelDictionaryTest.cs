using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class ModelDictionaryTest {


    CsvConfig? csvConfig;
    List<string> expectedParentDictionary = new List<string>();
    List<string> expectedDictionaryChild= new List<string>();


    [TestMethod]
    public void TestModelDictionary() {
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
      Assert.AreEqual(expectedParentDictionary.Count, DL.Data.ParentDictionarys.Count);
      foreach (var parentDictionary in DL.Data.ParentDictionarys) {
        Assert.AreEqual(expectedParentDictionary[parentDictionary.Key], parentDictionary.ToString());
      }

      Assert.AreEqual(expectedDictionaryChild.Count, DL.Data.DictionaryChildren.Count);
      foreach (var dictionaryChild in DL.Data.DictionaryChildren) {
        Assert.AreEqual(expectedDictionaryChild[dictionaryChild.Key], dictionaryChild.ToString());
      }
    }


    private void addParentDictionary(string someText) {
      var newParentDictionary = new ParentDictionary(someText, isStoring: true);
      expectedParentDictionary.Add(newParentDictionary.ToString());
      assertData();
    }


    private void addDictionaryChild(int parentDictionaryKey, DateTime date, string text) {
      var parentDictionary = DL.Data.ParentDictionarys[parentDictionaryKey];
      var newDictionaryChild = new DictionaryChild(parentDictionary, date, text, isStoring: true);
      expectedDictionaryChild.Add(newDictionaryChild.ToString());
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
