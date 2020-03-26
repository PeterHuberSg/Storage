using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class SortedListTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParentSortedList = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedSortedListChild= new Dictionary<int, string>();


    [TestMethod]
    public void TestSortedList() {
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
        addParentSortedList("1");
        addSortedListChild(0, now, "11");

        addParentSortedList("2");
        addSortedListChild(1, now, "21");
        addSortedListChild(1, now.AddDays(1), "22");

        removeParentSortedList(0);

        removeSortedListChild(1);

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
      Assert.AreEqual(expectedParentSortedList.Count, DL.Data.ParentsWithSortedList.Count);
      foreach (var parentSortedList in DL.Data.ParentsWithSortedList) {
        Assert.AreEqual(expectedParentSortedList[parentSortedList.Key], parentSortedList.ToString());
      }

      Assert.AreEqual(expectedSortedListChild.Count, DL.Data.SortedListChildren.Count);
      foreach (var sortedListChild in DL.Data.SortedListyChildren) {
        Assert.AreEqual(expectedSortedListChild[sortedListChild.Key], sortedListChild.ToString());
      }
    }


    private void addParentSortedList(string someText) {
      var newParentSortedList = new ParentWithSortedList(someText, isStoring: true);
      expectedParentSortedList.Add(newParentSortedList.Key, newParentSortedList.ToString());
      assertData();
    }


    private void addSortedListChild(int parentSortedListKey, DateTime date, string text) {
      var parentSortedList = DL.Data.ParentsWithSortedList[parentSortedListKey];
      var newSortedListChild = new SortedListChild(parentSortedList, date, text, isStoring: true);
      expectedSortedListChild.Add(newSortedListChild.Key, newSortedListChild.ToString());
      expectedParentSortedList[parentSortedList.Key] = parentSortedList.ToString();
      assertData();
    }


    private void removeParentSortedList(int parentSortedListKey) {
      var parent = DL.Data.ParentsWithSortedList[parentSortedListKey];
      foreach (var child in parent.SortedListChildren.Values) {
        expectedSortedListChild.Remove(child.Key);
      }
      expectedParentSortedList.Remove(parentSortedListKey);
      parent.Remove();
      assertData();
    }


    private void removeSortedListChild(int sortedListChildKey) {
      var child = DL.Data.SortedListChildren[sortedListChildKey];
      expectedSortedListChild.Remove(child.Key);
      var parentSortedList = child.ParentWithSortedList;
      child.Remove();
      expectedParentSortedList[parentSortedList.Key] = parentSortedList.ToString();
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
