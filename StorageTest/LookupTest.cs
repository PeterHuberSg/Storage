using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageDataContext;


namespace StorageTest {


  [TestClass]
  public class LookupTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


    [TestMethod]
    public void TestLookup() {
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
        var parent1Key = addParent(now, 1, isStoring: true).Key;
        var parent1NullableKey = addParentNullable(now, 1, isStoring: true).Key;
        var child1Key = addChild("1", parent1Key, parent1NullableKey, isStoring: true).Key;

        var parent2Key = addParent(now.AddDays(dayIndex), 2, isStoring: true).Key;
        var parent2NullableKey = addParentNullable(now.AddDays(dayIndex++), 2, isStoring: true).Key;
        addChild("2", parent2Key, parent2NullableKey, isStoring: true);
        addChild("3", parent2Key, parent2NullableKey, isStoring: true);
        DC.Data.CommitTransaction();

        //not stored
        var parent3 = addParent(now.AddDays(dayIndex++), 3, isStoring: false);
        var child4 = addChild("4", parent3, null, isStoring: false);
        DC.Data.StartTransaction();
        parent3.Store();
        child4.Store();
        DC.Data.RollbackTransaction();
        DC.Data.StartTransaction();
        store(parent3);
        store(child4);
        DC.Data.CommitTransaction();
        assertData();


        var parent4 = addParent(now.AddDays(dayIndex), 4, isStoring: false);
        var parent4Nullable = addParentNullable(now.AddDays(dayIndex++), 4, isStoring: false);
        var child5 = addChild("5", parent4, parent4Nullable, isStoring: false);
        var child6 = addChild("6", parent4, parent4Nullable, isStoring: false);
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
        updateParent(parent2Key, 2.1m);
        updateParentNullable(parent2NullableKey, 2.1m);
        updateChild(child1Key, parent2Key, parent2NullableKey, "11.U1");
        updateChild(child1Key, parent2Key, parent2NullableKey, "11.U2");
        updateChild(child1Key, parent1Key, parent1NullableKey, "11.U3");
        updateChild(child1Key, parent1Key, null, "11.U4");
        updateChild(child1Key, parent1Key, parent1NullableKey, "11.U5");

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
      Assert.AreEqual(expectedParents.Count, DC.Data.Lookup_Parents.Count);
      foreach (var parent in DC.Data.Lookup_Parents) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DC.Data.Lookup_ParentNullables.Count);
      foreach (var parentNullable in DC.Data.Lookup_ParentNullables) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.Lookup_Children.Count);
      foreach (var child in DC.Data.Lookup_Children) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private Lookup_Parent addParent(DateTime date, decimal someValue, bool isStoring) {
      var newParent = new Lookup_Parent(date, someValue, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(Lookup_Parent newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private Lookup_ParentNullable addParentNullable(DateTime date, decimal someValue, bool isStoring) {
      var newParentNullable = new Lookup_ParentNullable(date, someValue, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(Lookup_ParentNullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private Lookup_Child addChild(string info, int parentKey, int? parentNullableKey, bool isStoring) {
      var parent = DC.Data.Lookup_Parents[parentKey];
      Lookup_ParentNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DC.Data.Lookup_ParentNullables[parentNullableKey.Value];
      }
      var newChild = new Lookup_Child(info, parent, parentNullable, isStoring);
      if (isStoring) {
        expectedChildren.Add(newChild.Key, newChild.ToString());
        assertData();
      }
      return newChild;
    }


    private Lookup_Child addChild(string info, Lookup_Parent parent, Lookup_ParentNullable? parentNullable, bool isStoring){
      var newChild = new Lookup_Child(info, parent, parentNullable, isStoring);
      if (isStoring) {
        expectedChildren.Add(newChild.Key, newChild.ToString());
        assertData();
      }
      return newChild;
    }


    private void store(Lookup_Child newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
    }


    private void updateParent(int parentKey, decimal newValue) {
      var parent = DC.Data.Lookup_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(parent.Date, newValue);
      DC.Data.RollbackTransaction();
      assertData();
      parent = DC.Data.Lookup_Parents[parentKey];
      DC.Data.StartTransaction();
      parent.Update(parent.Date, newValue);
      DC.Data.CommitTransaction();
      expectedParents[parent.Key] = parent.ToString();
      foreach (var child in DC.Data.Lookup_Children) {
        if (child.LookupParent==parent) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
      assertData();
    }


    private void updateParentNullable(int parentNullableKey, decimal newValue) {
      var parentNullable = DC.Data.Lookup_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(parentNullable.Date, newValue);
      DC.Data.RollbackTransaction();
      assertData();
      parentNullable = DC.Data.Lookup_ParentNullables[parentNullableKey];
      DC.Data.StartTransaction();
      parentNullable.Update(parentNullable.Date, newValue);
      DC.Data.CommitTransaction();
      expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      foreach (var child in DC.Data.Lookup_Children) {
        if (child.LookupParentNullable==parentNullable) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
      assertData();
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, string text) {
      var child = DC.Data.Lookup_Children[childKey];
      var newParent = DC.Data.Lookup_Parents[parentKey];
      Lookup_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.Lookup_ParentNullables[parentNullableKey.Value];
      }
      DC.Data.StartTransaction();
      child.Update(text, newParent, newParentNullable);
      DC.Data.RollbackTransaction();
      assertData();

      child = DC.Data.Lookup_Children[childKey];
      newParent = DC.Data.Lookup_Parents[parentKey];
      var oldParent = child.LookupParent;
      newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.Lookup_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.LookupParentNullable;
      DC.Data.StartTransaction();
      child.Update(text, newParent, newParentNullable);
      DC.Data.CommitTransaction();
      expectedChildren[child.Key] = child.ToString();
      //if (oldParent!=newParent) {
      //  Assert.AreNotEqual(expectedParents[oldParent.Key], oldParent.ToString());
      //  Assert.AreNotEqual(expectedParents[newParent.Key], newParent.ToString());
      //}
      //if (oldParentNullable!=newParentNullable) {
      //  if (oldParentNullable!=null) {
      //    Assert.AreNotEqual(expectedParents[oldParentNullable.Key], oldParentNullable.ToString());
      //  }
      //  if (newParentNullable!=null) {
      //    Assert.AreNotEqual(expectedParents[newParentNullable.Key], newParentNullable.ToString());
      //  }
      //}
      //updateExpected(newParent, newParentNullable);
      //updateExpected(oldParent, oldParentNullable);
      assertData();
    }


    //private void updateExpected(Lookup_Parent parent, Lookup_ParentNullable? parentNullable) {
    //  expectedParents[parent.Key] = parent.ToString();
    //  foreach (var child in DC.Data.Lookup_Children) {
    //    if (child.LookupParent==parent) {
    //      expectedChildren[child.Key] = child.ToString();
    //    }
    //  }
    //  if (parentNullable!=null) {
    //    expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
    //    foreach (var child in DC.Data.Lookup_Children) {
    //      if (child.LookupParentNullable==parentNullable) {
    //        expectedChildren[child.Key] = child.ToString();
    //      }
    //    }
    //  }
    //}


    private void removeChild(int childKey) {
      var child = DC.Data.Lookup_Children[childKey];
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.RollbackTransaction();
      assertData();
      child = DC.Data.Lookup_Children[childKey];
      expectedChildren.Remove(child.Key);
      DC.Data.StartTransaction();
      child.Remove();
      DC.Data.CommitTransaction();
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