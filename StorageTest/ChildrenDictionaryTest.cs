using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class ChildrenDictionaryTest {

    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParentDictionary = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentNullableDictionary = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedDictionaryChild= new Dictionary<int, string>();


    [TestMethod]
    public void TestChildrenDictionary() {
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

        //////////////////////////////////////////////////////////////////////////////////////////
        // Todo: Update DictionaryTest like SortedDirectory
        //////////////////////////////////////////////////////////////////////////////////////////

        //removeParentDictionary(0);

        //removeDictionaryChild(1);

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
      Assert.AreEqual(expectedParentDictionary.Count, DC.Data.ChildrenDictionary_Parents.Count);
      foreach (var parentDictionary in DC.Data.ChildrenDictionary_Parents) {
        Assert.AreEqual(expectedParentDictionary[parentDictionary.Key], parentDictionary.ToString());
      }

      Assert.AreEqual(expectedParentNullableDictionary.Count, DC.Data.ChildrenDictionary_ParentNullables.Count);
      foreach (var parentDictionaryNullable in DC.Data.ChildrenDictionary_ParentNullables) {
        Assert.AreEqual(expectedParentDictionary[parentDictionaryNullable.Key], parentDictionaryNullable.ToString());
      }

      Assert.AreEqual(expectedDictionaryChild.Count, DC.Data.ChildrenDictionary_Children.Count);
      foreach (var dictionaryChild in DC.Data.ChildrenDictionary_Children) {
        Assert.AreEqual(expectedDictionaryChild[dictionaryChild.Key], dictionaryChild.ToString());
      }
    }


    private void addParentDictionary(string someText) {
      var newParentDictionary = new ChildrenDictionary_Parent(someText, isStoring: true);
      expectedParentDictionary.Add(newParentDictionary.Key, newParentDictionary.ToString());
      var newParentDictionaryNullable = new ChildrenDictionary_ParentNullable(someText, isStoring: true);
      expectedParentNullableDictionary.Add(newParentDictionaryNullable.Key, newParentDictionaryNullable.ToString());
      assertData();
    }


    private void addDictionaryChild(int parentDictionaryKey, DateTime date, string text) {
      var parentDictionary = DC.Data.ChildrenDictionary_Parents[parentDictionaryKey];
      var parentDictionaryNullable = DC.Data.ChildrenDictionary_ParentNullables[parentDictionaryKey];
      var newDictionaryChild = new ChildrenDictionary_Child(date, text, parentDictionary, parentDictionaryNullable, isStoring: true);
      expectedDictionaryChild.Add(newDictionaryChild.Key, newDictionaryChild.ToString());
      expectedParentDictionary[parentDictionary.Key] = parentDictionary.ToString();
      expectedParentNullableDictionary[parentDictionaryNullable.Key] = parentDictionaryNullable.ToString();
      assertData();
    }


    private void removeParentDictionary(int parentDictionaryKey) {
      var parent = DC.Data.ChildrenDictionary_Parents[parentDictionaryKey];
      foreach (var child in parent.ChildrenDictionary_Children.Values) {
        expectedDictionaryChild.Remove(child.Key);
      }
      expectedParentDictionary.Remove(parentDictionaryKey);
      parent.Remove();
      assertData();
    }


    private void removeDictionaryChild(int dictionaryChildKey) {
      var child = DC.Data.ChildrenDictionary_Children[dictionaryChildKey];
      expectedDictionaryChild.Remove(child.Key);
      var parentDictionary = child.ParentWithDictionary;
      var parentDictionaryNullable = child.ParentWithDictionaryNullable;
      child.Remove();
      expectedParentDictionary[parentDictionary.Key] = parentDictionary.ToString();
      expectedParentNullableDictionary[parentDictionaryNullable!.Key] = parentDictionaryNullable.ToString();
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
