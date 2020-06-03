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
        DC.DisposeData();
      }
    }


    private void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private void initDL() {
      new DC(csvConfig);
    }


    private void assertDL() {
      Assert.AreEqual(expectedParents.Count, DC.Data.ReadOnly_Parents.Count);
      foreach (var parent in DC.Data.ReadOnly_Parents) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.ReadOnly_Children.Count);
      foreach (var child in DC.Data.ReadOnly_Children) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private void addParent(string someText) {
      var newReadOnlyParent = new ReadOnly_Parent(someText, isStoring: true);
      expectedParents.Add(newReadOnlyParent.Key, newReadOnlyParent.ToString());
      var newReadOnlyParenNullablet = new ReadOnly_ParentNullable(someText, isStoring: true);
      expectedParentsNullable.Add(newReadOnlyParenNullablet.Key, newReadOnlyParenNullablet.ToString());
      assertData();
    }


    private void addChild(int parentKey, string text) {
      var parentDictionary = DC.Data.ReadOnly_Parents[parentKey];
      var parentDictionaryNullable = DC.Data.ReadOnly_ParentNullables[parentKey];
      var newChild = new ReadOnly_Child(text, parentDictionary, parentDictionaryNullable, isStoring: true);
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[parentDictionary.Key] = parentDictionary.ToString();
      assertData();
    }


    private void assertData() {
      assertDL();
      DC.DisposeData();

      initDL();
      assertDL();
    }
  }
}
