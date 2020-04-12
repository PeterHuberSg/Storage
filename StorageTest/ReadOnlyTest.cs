using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class ReadOnlyTest {
    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


    [TestMethod]
    public void TestReadOnly() {
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

        addParent("1");
        addChild(0, "11");

        addParent("2");
        addChild(1, "21");
        addChild(1, "22");
      } finally {
        DL.DisposeData();
      }
    }


    private void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private void initDL() {
      new DL(csvConfig);
    }


    private void assertDL() {
      Assert.AreEqual(expectedParents.Count, DL.Data.ReadOnlyParents.Count);
      foreach (var parent in DL.Data.ReadOnlyParents) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DL.Data.ReadOnlyChildren.Count);
      foreach (var child in DL.Data.ReadOnlyChildren) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private void addParent(string someText) {
      var newReadOnlyParent = new ReadOnlyParent(someText, isStoring: true);
      expectedParents.Add(newReadOnlyParent.Key, newReadOnlyParent.ToString());
      var newReadOnlyParenNullablet = new ReadOnlyParentNullable(someText, isStoring: true);
      expectedParentsNullable.Add(newReadOnlyParenNullablet.Key, newReadOnlyParenNullablet.ToString());
      assertData();
    }


    private void addChild(int parentKey, string text) {
      var parentDictionary = DL.Data.ReadOnlyParents[parentKey];
      var parentDictionaryNullable = DL.Data.ReadOnlyParentNullables[parentKey];
      var newChild = new ReadOnlyChild(text, parentDictionary, parentDictionaryNullable, isStoring: true);
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[parentDictionary.Key] = parentDictionary.ToString();
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
