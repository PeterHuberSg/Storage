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
      var d = new Dictionary<int, int>();
      d.Add(1, 1);
      d.Add(2, 2);
      d.Add(3, 3);
      foreach (var kv in d) {
        d.Remove(kv.Key);
      }

      var l = new List<int>();
      l.Add(1);
      l.Add(2);
      l.Add(3);
      //foreach (var item in l) {
      //  l.Remove(item);
      //}
      for (int i = l.Count-1; i >= 0; i--) {
        var item = l[i];
        l.Remove(item);
      }

      var sd = new SortedDictionary<int, int>();
      sd.Add(1, 1);
      sd.Add(2, 2);
      sd.Add(3, 3);
      int[] keys = new int[sd.Count];
      sd.Keys.CopyTo(keys, 0);
      foreach (var key in keys) {
        sd.Remove(key);
      }

      var sl = new SortedList<int, (int, int)>();
      sl.Add(1, (1, 1));
      sl.Add(2, (2, 2));
      sl.Add(3, (3, 3));
      var items = new (int, int)[sl.Count];
      sl.Values.CopyTo(items, 0);
      foreach (var item in items) {
        sl.Remove(item.Item1);
      }
      //foreach (var item in sl.Values) {
      //  sl.Remove(item.Item1);
      //}



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
        addParentSortedList("1", "1.0");
        addSortedListChild(0, now, "11");

        addParentSortedList("2", "2.0");
        addSortedListChild(1, now, "21");
        addSortedListChild(1, now.AddDays(1), "22");

        update(1, "2.1");

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
      foreach (var sortedListChild in DL.Data.SortedListChildren) {
        Assert.AreEqual(expectedSortedListChild[sortedListChild.Key], sortedListChild.ToString());
      }
    }


    private void addParentSortedList(string readOnlyText, string updateableText) {
      var newParentSortedList = new ParentWithSortedList(readOnlyText, updateableText, isStoring: true);
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


    private void update(int parentSortedListKey, string textUpdateable) {
      var parentSortedList = DL.Data.ParentsWithSortedList[parentSortedListKey];
      parentSortedList.Update(textUpdateable);
      expectedParentSortedList[parentSortedList.Key] = parentSortedList.ToString();
      foreach (var sortedListChild in parentSortedList.SortedListChildren.Values) {
        expectedSortedListChild[sortedListChild.Key] = sortedListChild.ToString();
      }
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
