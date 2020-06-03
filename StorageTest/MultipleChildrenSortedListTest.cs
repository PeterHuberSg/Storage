using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class MultipleChildrenSortedListTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


    [TestMethod]
    public void TestMultipleChildrenSortedList() {
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

        //stored immediately
        var now = DateTime.Now.Date;
        var dayIndex = 1;
        var parent1Key = addParentSortedList("1", "1.0", isStoring: true).Key;
        var parent1NullableKey = addParentSortedListNullable("1", "1N.0", isStoring: true).Key;

        var child1Key = addSortedListChild(parent1Key, null, now, "11", isStoring: true).Key;

        var parent2Key = addParentSortedList("2", "2.0", isStoring: true).Key;
        var parent2NullableKey = addParentSortedListNullable("1", "2N.0", isStoring: true).Key;
        var child2Key = addSortedListChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "21", isStoring: true).Key;
        var child3Key = addSortedListChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "22", isStoring: true).Key;

        //not stored
        var parent3 = addParentSortedList("1", "1.0", isStoring: false);
        var parent3Nullable = addParentSortedListNullable("1", "1N.0", isStoring: false);

        var child4 = addSortedListChild(parent3, null, now.AddDays(dayIndex++), "11", isStoring: false);

        var parent4 = addParentSortedList("2", "2.0", isStoring: false);
        var parent4Nullable = addParentSortedListNullable("1", "2N.0", isStoring: false);
        var child5 = addSortedListChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "21", isStoring: false);
        var child6 = addSortedListChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "22", isStoring: false);

        store(parent3);
        store(parent3Nullable);
        store(child4);
        assertData();


        store(parent4);
        store(parent4Nullable);
        store(child5);
        store(child6);
        assertData();



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
      Assert.AreEqual(expectedParents.Count, DC.Data.MultipleChildrenSortedList_Parents.Count);
      foreach (var parentSortedList in DC.Data.MultipleChildrenSortedList_Parents) {
        Assert.AreEqual(expectedParents[parentSortedList.Key], parentSortedList.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DC.Data.MultipleChildrenSortedList_ParentNullables.Count);
      foreach (var parentNullable in DC.Data.MultipleChildrenSortedList_ParentNullables) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.MultipleChildrenSortedList_Children.Count);
      foreach (var sortedListChild in DC.Data.MultipleChildrenSortedList_Children) {
        Assert.AreEqual(expectedChildren[sortedListChild.Key], sortedListChild.ToString());
      }
    }


    private MultipleChildrenSortedList_Parent addParentSortedList(string readOnlyText, string updateableText, bool isStoring) {
      var newParent = new MultipleChildrenSortedList_Parent(readOnlyText, updateableText, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(MultipleChildrenSortedList_Parent newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private MultipleChildrenSortedList_ParentNullable addParentSortedListNullable(string readOnlyText, string updateableText, bool isStoring) {
      var newParentNullable = new MultipleChildrenSortedList_ParentNullable(readOnlyText, updateableText, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(MultipleChildrenSortedList_ParentNullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private MultipleChildrenSortedList_Child addSortedListChild(int parentKey, int? parentNullableKey, DateTime date, string text, bool isStoring) {
      var parent = DC.Data.MultipleChildrenSortedList_Parents[parentKey];
      MultipleChildrenSortedList_ParentNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DC.Data.MultipleChildrenSortedList_ParentNullables[parentNullableKey.Value];
      }
      var newChild = new MultipleChildrenSortedList_Child(date, text, parent, parentNullable, isStoring);
      if (isStoring) {
        expectedChildren.Add(newChild.Key, newChild.ToString());
        expectedParents[parent.Key] = parent.ToString();
        if (parentNullable!=null) {
          expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
        }
        assertData();
      }
      return newChild;
    }


    private MultipleChildrenSortedList_Child addSortedListChild(MultipleChildrenSortedList_Parent parent, MultipleChildrenSortedList_ParentNullable? parentNullable, 
      DateTime date, string text, bool isStoring) 
    {
      var newChild = new MultipleChildrenSortedList_Child(date, text, parent, parentNullable, isStoring);
      if (isStoring) {
        expectedChildren.Add(newChild.Key, newChild.ToString());
        expectedParents[parent.Key] = parent.ToString();
        if (parentNullable!=null) {
          expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
        }
        assertData();
      }
      return newChild;
    }


    private void store(MultipleChildrenSortedList_Child newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[newChild.ParentWithSortedList.Key] = newChild.ParentWithSortedList.ToString();
      var parentNullable = newChild.ParentWithSortedListNullable;
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
    }


    private void updateParent(int parentSortedListKey, string textUpdateable) {
      var parentSortedList = DC.Data.MultipleChildrenSortedList_Parents[parentSortedListKey];
      parentSortedList.Update(textUpdateable);
      expectedParents[parentSortedList.Key] = parentSortedList.ToString();
      foreach (var sortedListChild in parentSortedList.MultipleChildrenSortedList_Children.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
      assertData();
    }


    private void updateParentNullable(int parentSortedListNullableKey, string textUpdateable) {
      var parentSortedListNullable = DC.Data.MultipleChildrenSortedList_ParentNullables[parentSortedListNullableKey];
      parentSortedListNullable.Update(textUpdateable);
      expectedParentsNullable[parentSortedListNullable.Key] = parentSortedListNullable.ToString();
      foreach (var sortedListChild in parentSortedListNullable.MultipleChildrenSortedList_Children.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
      assertData();
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var childSortedList = DC.Data.MultipleChildrenSortedList_Children[childKey];
      var newParent = DC.Data.MultipleChildrenSortedList_Parents[parentKey];
      var oldParent = childSortedList.ParentWithSortedList;
      MultipleChildrenSortedList_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.MultipleChildrenSortedList_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = childSortedList.ParentWithSortedListNullable;
      childSortedList.Update(date, text, newParent, newParentNullable);

      expectedChildren[childSortedList.Key] = childSortedList.ToString();
      update(newParent, newParentNullable);
      update(oldParent, oldParentNullable);
      assertData();
    }


  private void update(MultipleChildrenSortedList_Parent newParent, MultipleChildrenSortedList_ParentNullable? newParentNullable) {
    expectedParents[newParent.Key] = newParent.ToString();
    foreach (var sortedListChild in newParent.MultipleChildrenSortedList_Children.Values) {
      expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
    }
    if (newParentNullable!=null) {
      expectedParentsNullable[newParentNullable.Key] = newParentNullable.ToString();
      foreach (var sortedListChild in newParentNullable.MultipleChildrenSortedList_Children.Values) {
        expectedChildren[sortedListChild.Key] = sortedListChild.ToString();
      }
    }
  }


    private void removeParent(int parentKey) {
      var parent = DC.Data.MultipleChildrenSortedList_Parents[parentKey];
      foreach (var child in parent.MultipleChildrenSortedList_Children.Values) {
        expectedChildren.Remove(child.Key);
      }
      expectedParents.Remove(parentKey);
      parent.Remove();
      assertData();
    }


    private void removeParentNullable(int parentNullableKey) {
      var parentNullable = DC.Data.MultipleChildrenSortedList_ParentNullables[parentNullableKey];
      foreach (var child in parentNullable.MultipleChildrenSortedList_Children.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      expectedParents.Remove(parentNullableKey);
      parentNullable.Remove();
      assertData();
    }


    private void removeChild(int ChildKey) {
      var child = DC.Data.MultipleChildrenSortedList_Children[ChildKey];
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
      DC.DisposeData();

      initDL();
      assertDL();
    }
  }
}
