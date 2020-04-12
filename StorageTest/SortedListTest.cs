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
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


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
        var parent1Key = addParentSortedList("1", "1.0");
        var parent1NullableKey = addParentSortedListNullable("1", "1N.0");

        var child1 = addSortedListChild(parent1Key, null, now, "11");

        var parent2Key = addParentSortedList("2", "2.0");
        var parent2NullableKey = addParentSortedListNullable("1", "2N.0");
        var child2 = addSortedListChild(parent2Key, parent2NullableKey, now, "21");
        var child3 = addSortedListChild(parent2Key, parent2NullableKey, now.AddDays(1), "22");

        updateParent(parent2Key, "2.1");
        updateParentNullable(parent2NullableKey, "2N.1");

        //////////////////////////////////////////////////////////////
        // Todo: Improve Update for List, SortedList, Dictionary, Update
        //
        // SortedList & Dictionary: Not just changing the parent, but also changing the value of 
        // child.ParentDictionaryKey needs to create an removal and adding of the child in that Dictionary
        //
        // if child.Parent is nullable and the parent gets removed, set child.Parent to null
        // if child.Parent is not nullable and the parent should get removed, throw an exception. Just
        // deleting all children and their children and removing them from other parents is too complicated and
        // far reaching
        //
        // Lookups should only be allowed for undeletable parents. Otherwise it's too complicated what to
        // do when the parent gets deleted.
        //////////////////////////////////////////////////////////////

        //updateChild(child1, parent2Key, parent2NullableKey, now, "12");
        //updateChild(child1, parent2Key, parent2NullableKey, now.AddDays(-1), "13");
        //updateChild(child1, parent1Key, parent1NullableKey, now.AddDays(-1), "14");
        //updateChild(child1, parent1Key, null, now.AddDays(-1), "15");
        //updateChild(child1, parent1Key, parent1NullableKey, now.AddDays(-1), "16");

        //removeParent(parent1Key);
        //removeParentNullable(parent1NullableKey);

        removeChild(1);

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
      Assert.AreEqual(expectedParents.Count, DL.Data.ParentsWithSortedList.Count);
      foreach (var parentSortedList in DL.Data.ParentsWithSortedList) {
        Assert.AreEqual(expectedParents[parentSortedList.Key], parentSortedList.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DL.Data.ParentsWithSortedListNullable.Count);
      foreach (var parentNullable in DL.Data.ParentsWithSortedListNullable) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DL.Data.SortedListChildren.Count);
      foreach (var sortedListChild in DL.Data.SortedListChildren) {
        Assert.AreEqual(expectedChildren[sortedListChild.Key], sortedListChild.ToString());
      }
    }


    private int addParentSortedList(string readOnlyText, string updateableText) {
      var newParent = new ParentWithSortedList(readOnlyText, updateableText, isStoring: true);
      expectedParents.Add(newParent.Key, newParent.ToString());
      assertData();
      return newParent.Key;
    }


    private int addParentSortedListNullable(string readOnlyText, string updateableText) {
      var newParentNullable = new ParentWithSortedListNullable(readOnlyText, updateableText, isStoring: true);
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
      assertData();
      return newParentNullable.Key;
    }


    private int addSortedListChild(int parentKey, int? parentNullableKey, DateTime date, string text) {
      var parent = DL.Data.ParentsWithSortedList[parentKey];
      ParentWithSortedListNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DL.Data.ParentsWithSortedListNullable[parentNullableKey.Value];
      }
      var newChild = new SortedListChild(date, text, parent, parentNullable, isStoring: true);
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[parent.Key] = parent.ToString();
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
      assertData();
      return newChild.Key;
    }


    private void updateParent(int parentSortedListKey, string textUpdateable) {
      var parentSortedList = DL.Data.ParentsWithSortedList[parentSortedListKey];
      parentSortedList.Update(textUpdateable);
      expectedParents[parentSortedList.Key] = parentSortedList.ToString();
      foreach (var sortedListChild in parentSortedList.SortedListChildren.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
      assertData();
    }


    private void updateParentNullable(int parentSortedListNullableKey, string textUpdateable) {
      var parentSortedListNullable = DL.Data.ParentsWithSortedListNullable[parentSortedListNullableKey];
      parentSortedListNullable.Update(textUpdateable);
      expectedParentsNullable[parentSortedListNullable.Key] = parentSortedListNullable.ToString();
      foreach (var sortedListChild in parentSortedListNullable.SortedListChildren.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
      assertData();
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var childSortedList = DL.Data.SortedListChildren[childKey];
      var newParent = DL.Data.ParentsWithSortedList[parentKey];
      var oldParent = childSortedList.ParentWithSortedList;
      ParentWithSortedListNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DL.Data.ParentsWithSortedListNullable[parentNullableKey.Value];
      }
      var oldParentNullable = childSortedList.ParentWithSortedListNullable;
      childSortedList.Update(date, text, newParent, newParentNullable);

      expectedChildren[childSortedList.Key] = childSortedList.ToString();
      update(newParent, newParentNullable);
      update(oldParent, oldParentNullable);
      assertData();
    }


  private void update(ParentWithSortedList newParent, ParentWithSortedListNullable? newParentNullable) {
    expectedParents[newParent.Key] = newParent.ToString();
    foreach (var sortedListChild in newParent.SortedListChildren.Values) {
      expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
    }
    if (newParentNullable!=null) {
      expectedParentsNullable[newParentNullable.Key] = newParentNullable.ToString();
      foreach (var sortedListChild in newParentNullable.SortedListChildren.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
    }
  }


    private void removeParent(int parentKey) {
      var parent = DL.Data.ParentsWithSortedList[parentKey];
      foreach (var child in parent.SortedListChildren.Values) {
        expectedChildren.Remove(child.Key);
      }
      expectedParents.Remove(parentKey);
      parent.Remove();
      assertData();
    }


    private void removeParentNullable(int parentNullableKey) {
      var parentNullable = DL.Data.ParentsWithSortedListNullable[parentNullableKey];
      foreach (var child in parentNullable.SortedListChildren.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      expectedParents.Remove(parentNullableKey);
      parentNullable.Remove();
      assertData();
    }


    private void removeChild(int ChildKey) {
      var child = DL.Data.SortedListChildren[ChildKey];
      expectedChildren.Remove(child.Key);
      var parent = child.ParentWithSortedList;
      var parentNullable = child.ParentWithSortedListNullable;
      child.Remove();
      expectedParents[parent.Key] = parent.ToString();
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable!.Key] = parentNullable.ToString();
      }
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
