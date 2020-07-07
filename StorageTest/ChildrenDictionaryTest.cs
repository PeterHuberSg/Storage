using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageDataContext;


namespace StorageTest {


  [TestClass]
  public class ChildrenDictionaryTest {

    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


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

        //stored immediately
        DC.Data.StartTransaction();
        var now = DateTime.Now.Date;
        var dayIndex = 1;
        var parent1Key = addParent("1", isStoring: true).Key;
        var parent1NullableKey = addParentNullable("1N", isStoring: true).Key;

        var child1Key = addChild(parent1Key, null, now, "11", isStoring: true).Key;

        var parent2Key = addParent("2", isStoring: true).Key;
        var parent2NullableKey = addParentNullable("2N", isStoring: true).Key;
        var child2Key = addChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "21", isStoring: true).Key;
        var child3Key = addChild(parent2Key, parent2NullableKey, now.AddDays(dayIndex++), "22", isStoring: true).Key;
        DC.Data.CommitTransaction();

        //not stored
        var parent3 = addParent("3", isStoring: false);
        var parent3Nullable = addParentNullable("3N", isStoring: false);
        var child4 = addChild(parent3, null, now.AddDays(dayIndex++), "31", isStoring: false);
        DC.Data.StartTransaction();
        parent3.Store();
        parent3Nullable.Store();
        child4.Store();
        DC.Data.RollbackTransaction();
        DC.Data.StartTransaction();
        store(parent3);
        store(parent3Nullable);
        store(child4);
        DC.Data.CommitTransaction();
        assertData();


        var parent4 = addParent("4", isStoring: false);
        var parent4Nullable = addParentNullable("4N", isStoring: false);
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

        updateParent(parent2Key, "2.1");
        updateParentNullable(parent2NullableKey, "2N.1");
        updateChild(child1Key, parent2Key, parent2NullableKey, now, "11.U1");
        updateChild(child1Key, parent2Key, parent2NullableKey, now.AddDays(-1), "11.U2");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "11.U3");
        updateChild(child1Key, parent1Key, null, now.AddDays(-1), "11.U4");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "11.U5");

        //updating a not stored child should not lead to any change in the parents
        parent3 = DC.Data.ChildrenDictionary_Parents[parent3.Key];
        parent4 = DC.Data.ChildrenDictionary_Parents[parent4.Key];
        parent4Nullable = DC.Data.ChildrenDictionary_ParentNullables[parent4Nullable.Key];
        var parent3Expected = parent3.ToString();
        var parent4Expected = parent4.ToString();
        var parent4NullableExpected = parent4Nullable.ToString();
        var child7 = addChild(parent4, parent4Nullable, now.AddDays(dayIndex++), "43", isStoring: false);
        child7.Update(now.AddDays(dayIndex++), "33U", parent3, null);
        Assert.AreEqual(parent3Expected, parent3.ToString());
        Assert.AreEqual(parent4Expected, parent4.ToString());
        Assert.AreEqual(parent4NullableExpected, parent4Nullable.ToString());

        //removeParentDictionary(0);

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
      Assert.AreEqual(expectedParents.Count, DC.Data.ChildrenDictionary_Parents.Count);
      foreach (var parentDictionary in DC.Data.ChildrenDictionary_Parents) {
        Assert.AreEqual(expectedParents[parentDictionary.Key], parentDictionary.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DC.Data.ChildrenDictionary_ParentNullables.Count);
      foreach (var parentDictionaryNullable in DC.Data.ChildrenDictionary_ParentNullables) {
        Assert.AreEqual(expectedParentsNullable[parentDictionaryNullable.Key], parentDictionaryNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.ChildrenDictionary_Children.Count);
      foreach (var dictionaryChild in DC.Data.ChildrenDictionary_Children) {
        Assert.AreEqual(expectedChildren[dictionaryChild.Key], dictionaryChild.ToString());
      }
    }


    private ChildrenDictionary_Parent addParent(string someText, bool isStoring) {
      var newParent = new ChildrenDictionary_Parent(someText, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(ChildrenDictionary_Parent newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private ChildrenDictionary_ParentNullable addParentNullable(string someText, bool isStoring) {
      var newParentNullable = new ChildrenDictionary_ParentNullable(someText, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(ChildrenDictionary_ParentNullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private ChildrenDictionary_Child addChild(int parentKey, int? parentNullableKey, DateTime date, string text, bool isStoring) {
      var parent = DC.Data.ChildrenDictionary_Parents[parentKey];
      ChildrenDictionary_ParentNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DC.Data.ChildrenDictionary_ParentNullables[parentNullableKey.Value];
      }
      var newChild = new ChildrenDictionary_Child(date, text, parent, parentNullable, isStoring: true);
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


    private ChildrenDictionary_Child addChild(ChildrenDictionary_Parent parent, ChildrenDictionary_ParentNullable? parentNullable,
      DateTime date, string text, bool isStoring) 
    {
      var newChild = new ChildrenDictionary_Child(date, text, parent, parentNullable, isStoring);
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


    private void store(ChildrenDictionary_Child newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[newChild.ParentWithDictionary.Key] = newChild.ParentWithDictionary.ToString();
      var parentNullable = newChild.ParentWithDictionaryNullable;
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
    }


    private void updateParent(int parentKey, string textUpdateable) {
      var parent = DC.Data.ChildrenDictionary_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(textUpdateable);
      DC.Data.RollbackTransaction();
      assertData();
      parent = DC.Data.ChildrenDictionary_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(textUpdateable);
      DC.Data.CommitTransaction();
      expectedParents[parent.Key] = parent.ToString();
      foreach (var child in parent.ChildrenDictionary_Children.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      assertData();
    }


    private void updateParentNullable(int parentNullableKey, string textUpdateable) {
      var parentNullable = DC.Data.ChildrenDictionary_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(textUpdateable);
      DC.Data.RollbackTransaction();
      assertData();
      parentNullable = DC.Data.ChildrenDictionary_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(textUpdateable);
      DC.Data.CommitTransaction();
      expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      foreach (var Child in parentNullable.ChildrenDictionary_Children.Values) {
        expectedChildren[Child.Key] = Child.ToString();
      }
      assertData();
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var child = DC.Data.ChildrenDictionary_Children[childKey];
      var newParent = DC.Data.ChildrenDictionary_Parents[parentKey];
      ChildrenDictionary_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.ChildrenDictionary_ParentNullables[parentNullableKey.Value];
      }
      DC.Data.StartTransaction();
      child.Update(date, text, newParent, newParentNullable);
      DC.Data.RollbackTransaction();
      assertData();

      child = DC.Data.ChildrenDictionary_Children[childKey];
      newParent = DC.Data.ChildrenDictionary_Parents[parentKey];
      var oldParent = child.ParentWithDictionary;
      newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.ChildrenDictionary_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.ParentWithDictionaryNullable;
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


    private void updateExpected(ChildrenDictionary_Parent parent, ChildrenDictionary_ParentNullable? parentNullable) {
      expectedParents[parent.Key] = parent.ToString();
      foreach (var child in parent.ChildrenDictionary_Children.Values) {
        expectedChildren[child.Key] = child.ToString();
      }
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
        foreach (var child in parentNullable.ChildrenDictionary_Children.Values) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
    }


    //private void removeParentDictionary(int parentDictionaryKey) {
    //  var parent = DC.Data.ChildrenDictionary_Parents[parentDictionaryKey];
    //  foreach (var child in parent.ChildrenDictionary_Children.Values) {
    //    expectedDictionaryChild.Remove(child.Key);
    //  }
    //  expectedParentDictionary.Remove(parentDictionaryKey);
    //  parent.Remove();
    //  assertData();
    //}


    private void removeChild(int childKey) {
      var child = DC.Data.ChildrenDictionary_Children[childKey];
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.RollbackTransaction();
      assertData();
      child = DC.Data.ChildrenDictionary_Children[childKey];
      expectedChildren.Remove(child.Key);
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.CommitTransaction();
      var parent = child.ParentWithDictionary;
      var parentNullable = child.ParentWithDictionaryNullable;
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
