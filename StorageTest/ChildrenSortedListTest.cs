using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageDataContext;


namespace StorageTest {
  [TestClass]
  public class ChildrenSortedListTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren = new Dictionary<int, string>();


    [TestMethod]
    public void TestChildrenSortedList() {
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
        DC.Data.StartTransaction();
        var now = DateTime.Now.Date;
        var dayIndex = 1;
        var parent1Key = addParent("1", "1.0", isStoring: true).Key;
        var parent1NullableKey = addParentNullable("1N", "1N.0", isStoring: true).Key;
        var child1Key = addChild(parent1Key, null, now, "11", isStoring: true).Key;

        var parent2Key = addParent("2", "2.0", isStoring: true).Key;
        var parent2NullableKey = addParentNullable("2N", "2N.0", isStoring: true).Key;
        var child2Key = addChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "21", isStoring: true).Key;
        var child3Key = addChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "22", isStoring: true).Key;
        DC.Data.CommitTransaction();

        //not stored
        var parent3 = addParent("3", "3.0", isStoring: false);
        var child4 = addChild(parent3, null, now.AddDays(dayIndex++), "31", isStoring: false);
        DC.Data.StartTransaction();
        parent3.Store();
        child4.Store();
        DC.Data.RollbackTransaction();
        DC.Data.StartTransaction();
        store(parent3);
        store(child4);
        DC.Data.CommitTransaction();
        assertData();

        var parent4 = addParent("4", "4.0", isStoring: false);
        var parent4Nullable = addParentNullable("4N", "4N.0", isStoring: false);
        var child5 = addChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "41", isStoring: false);
        var child6 = addChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "42", isStoring: false);
        DC.Data.StartTransaction();
        parent4.Store();
        parent4Nullable.Store();
        child5.Store();
        child6.Store();
        DC.Data.RollbackTransaction();
        DC.Data.StartTransaction();
        store(parent4);
        store(parent4Nullable);
        store(child5);
        store(child6);
        DC.Data.CommitTransaction();
        assertData();

        //test update()
        updateParent(parent2Key, "2.1");
        updateParentNullable(parent2NullableKey, "2N.1");
        updateChild(child1Key, parent2Key, parent2NullableKey, now, "11.U1");
        updateChild(child1Key, parent2Key, parent2NullableKey, now.AddDays(-1), "11.U2");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "11.U3");
        updateChild(child1Key, parent1Key, null, now.AddDays(-1), "11.U4");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "11.U5");

        //updating a not stored child should not lead to any change in the parents
        parent3 = DC.Data.ChildrenSortedList_Parents[parent3.Key];
        parent4 = DC.Data.ChildrenSortedList_Parents[parent4.Key];
        parent4Nullable = DC.Data.ChildrenSortedList_ParentNullables[parent4Nullable.Key];
        var parent3Expected = parent3.ToString();
        var parent4Expected = parent4.ToString();
        var parent4NullableExpected = parent4Nullable.ToString();
        var child7 = addChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "43", isStoring: false);
        child7.Update(now.AddDays(dayIndex++), "33U", parent3, null);
        Assert.AreEqual(parent3Expected, parent3.ToString());
        Assert.AreEqual(parent4Expected, parent4.ToString());
        Assert.AreEqual(parent4NullableExpected, parent4Nullable.ToString());

        //////////////////////////////////////////////////////////////
        // Todo: Improve Remove for List, SortedList, Dictionary
        //
        // if child.Parent is nullable and the parent gets removed, set child.Parent to null
        // if child.Parent is not nullable and the parent should get removed, throw an exception. Just
        // deleting all children and their children and removing them from other parents is too complicated and
        // far reaching
        //
        // Lookups should only be allowed for undeletable parents. Otherwise it's too complicated what to
        // do when the parent gets deleted. Idea: Lookup could copy the actual values into the child, so when the
        // lookup parent gets deleted, the value is still valid in the child
        //////////////////////////////////////////////////////////////

        //removeParent(parent1Key);
        //removeParentNullable(parent1NullableKey);

        removeChild(child1Key);

      } finally {
        DC.DisposeData();
      }
    }


    private void reportException(Exception obj) {
      System.Diagnostics.Debug.WriteLine(obj);
      System.Diagnostics.Debugger.Break();
      Assert.Fail();
    }


    private void initDL() {
      new DC(csvConfig);
    }


    private void assertDL() {
      Assert.AreEqual(expectedParents.Count, DC.Data.ChildrenSortedList_Parents.Count);
      foreach (var parent in DC.Data.ChildrenSortedList_Parents) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DC.Data.ChildrenSortedList_ParentNullables.Count);
      foreach (var parentNullable in DC.Data.ChildrenSortedList_ParentNullables) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.ChildrenSortedList_Children.Count);
      foreach (var child in DC.Data.ChildrenSortedList_Children) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private ChildrenSortedList_Parent addParent(string readOnlyText, string updateableText, bool isStoring) {
      var newParent = new ChildrenSortedList_Parent(readOnlyText, updateableText, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(ChildrenSortedList_Parent newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private ChildrenSortedList_ParentNullable addParentNullable(string readOnlyText, string updateableText, bool isStoring) {
      var newParentNullable = new ChildrenSortedList_ParentNullable(readOnlyText, updateableText, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(ChildrenSortedList_ParentNullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private ChildrenSortedList_Child addChild(int parentKey, int? parentNullableKey, DateTime date, string text, bool isStoring) {
      var parent = DC.Data.ChildrenSortedList_Parents[parentKey];
      ChildrenSortedList_ParentNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey.Value];
      }
      var newChild = new ChildrenSortedList_Child(date, text, parent, parentNullable, isStoring);
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


    private ChildrenSortedList_Child addChild(ChildrenSortedList_Parent parent, ChildrenSortedList_ParentNullable? parentNullable, 
      DateTime date, string text, bool isStoring) 
    {
      var newChild = new ChildrenSortedList_Child(date, text, parent, parentNullable, isStoring);
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


    private void store(ChildrenSortedList_Child newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[newChild.ParentWithSortedList.Key] = newChild.ParentWithSortedList.ToString();
      var parentNullable = newChild.ParentWithSortedListNullable;
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
    }


    private void updateParent(int parentKey, string textUpdateable) {
      var parent = DC.Data.ChildrenSortedList_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(textUpdateable);
      DC.Data.RollbackTransaction();
      assertData();
      parent = DC.Data.ChildrenSortedList_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(textUpdateable);
      DC.Data.CommitTransaction();
      expectedParents[parent.Key] = parent.ToString();
      foreach (var child in parent.ChildrenSortedList_Children.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      assertData();
    }


    private void updateParentNullable(int parentNullableKey, string textUpdateable) {
      var parentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(textUpdateable);
      DC.Data.RollbackTransaction();
      assertData();
      parentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(textUpdateable);
      DC.Data.CommitTransaction();
      expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      foreach (var Child in parentNullable.ChildrenSortedList_Children.Values) {
        expectedChildren[Child.Key] = Child.ToString();
      }
      assertData();
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var child = DC.Data.ChildrenSortedList_Children[childKey];
      var newParent = DC.Data.ChildrenSortedList_Parents[parentKey];
      ChildrenSortedList_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey.Value];
      }
      DC.Data.StartTransaction();
      child.Update(date, text, newParent, newParentNullable);
      DC.Data.RollbackTransaction();
      assertData();

      child = DC.Data.ChildrenSortedList_Children[childKey];
      newParent = DC.Data.ChildrenSortedList_Parents[parentKey];
      var oldParent = child.ParentWithSortedList;
      newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.ParentWithSortedListNullable;
      DC.Data.StartTransaction();
      child.Update(date, text, newParent, newParentNullable);
      DC.Data.CommitTransaction();
      expectedChildren[child.Key] = child.ToString();
      if (oldParent!=newParent) {
        Assert.AreNotEqual(expectedParents[oldParent.Key], oldParent.ToString());
        Assert.AreNotEqual(expectedParents[newParent.Key], newParent.ToString());
      }
      if (oldParentNullable!=newParentNullable) {
        if (oldParentNullable!=null) {
          Assert.AreNotEqual(expectedParents[oldParentNullable.Key], oldParentNullable.ToString());
        }
        if (newParentNullable!=null) {
          Assert.AreNotEqual(expectedParents[newParentNullable.Key], newParentNullable.ToString());
        }
      }
      updateExpected(newParent, newParentNullable);
      updateExpected(oldParent, oldParentNullable);
      assertData();
    }


    private void updateExpected(ChildrenSortedList_Parent parent, ChildrenSortedList_ParentNullable? parentNullable) {
      expectedParents[parent.Key] = parent.ToString();
      foreach (var child in parent.ChildrenSortedList_Children.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
        foreach (var child in parentNullable.ChildrenSortedList_Children.Values) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
    }


    //private void removeParent(int parentKey) {
    //  var parent = DC.Data.ChildrenSortedList_Parents[parentKey];
    //  foreach (var child in parent.ChildrenSortedList_Children.Values) {
    //    expectedChildren.Remove(child.Key);
    //  }
    //  expectedParents.Remove(parentKey);
    //  parent.Remove();
    //  assertData();
    //}


    //private void removeParentNullable(int parentNullableKey) {
    //  var parentNullable = DC.Data.ChildrenSortedList_ParentNullables[parentNullableKey];
    //  foreach (var child in parentNullable.ChildrenSortedList_Children.Values) {
    //    expectedChildren[child.Key] = child.ToString();
    //  }
    //  expectedParents.Remove(parentNullableKey);
    //  parentNullable.Remove();
    //  assertData();
    //}


    private void removeChild(int childKey) {
      var child = DC.Data.ChildrenSortedList_Children[childKey];
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.RollbackTransaction();
      assertData();
      child = DC.Data.ChildrenSortedList_Children[childKey];
      expectedChildren.Remove(child.Key);
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.CommitTransaction();
      var parent = child.ParentWithSortedList;
      var parentNullable = child.ParentWithSortedListNullable;
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
