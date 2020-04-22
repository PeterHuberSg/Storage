using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {

  [TestClass]
  public class UpdatableChildTest {

    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


    [TestMethod]
    public void TestUpdatableChild() {
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
        var parent1Key = addReadOnlyParent2("1", isStoring: true).Key;
        var parent1NullableKey = addReadOnlyParent2Nullable("1N", isStoring: true).Key;

        var child1Key = addUpdatableChild(parent1Key, null, "11", isStoring: true).Key;

        var parent2Key = addReadOnlyParent2("2", isStoring: true).Key;
        var parent2NullableKey = addReadOnlyParent2Nullable("2N", isStoring: true).Key;
        var child2Key = addUpdatableChild(parent2Key, parent2NullableKey, "21", isStoring: true).Key;
        var child3Key = addUpdatableChild(parent2Key, parent2NullableKey, "22", isStoring: true).Key;

        //not stored
        var parent3 = addReadOnlyParent2("3", isStoring: false);
        var parent3Nullable = addReadOnlyParent2("3N", isStoring: false);

        var child4 = addUpdatableChild(parent3, null, "11", isStoring: false);

        var parent4 = addReadOnlyParent2("4", isStoring: false);
        var parent4Nullable = addReadOnlyParent2Nullable("4N", isStoring: false);
        var child5 = addUpdatableChild(parent4, parent4Nullable, "21", isStoring: false);
        var child6 = addUpdatableChild(parent4, parent4Nullable, "22", isStoring: false);

        store(parent3);
        store(parent3Nullable);
        store(child4);
        assertData();


        store(parent4);
        store(parent4Nullable);
        store(child5);
        store(child6);
        assertData();

        updateChild(child1Key, parent2Key, parent2NullableKey, now, "12");
        updateChild(child1Key, parent2Key, parent2NullableKey, now.AddDays(-1), "13");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "14");
        updateChild(child1Key, parent1Key, null, now.AddDays(-1), "15");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "16");


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
      Assert.AreEqual(expectedParents.Count, DL.Data.ReadOnlyParent2s.Count);
      foreach (var parent in DL.Data.ReadOnlyParent2s) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DL.Data.ReadOnlyParent2Nullables.Count);
      foreach (var parentNullable in DL.Data.ReadOnlyParent2Nullables) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DL.Data.UpdatableChildren.Count);
      foreach (var child in DL.Data.UpdatableChildren) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private ReadOnlyParent2 addReadOnlyParent2(string text, bool isStoring) {
      var newParent = new ReadOnlyParent2(text, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(ReadOnlyParent2 newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private ReadOnlyParent2Nullable addReadOnlyParent2Nullable(string text, bool isStoring) {
      var newParentNullable = new ReadOnlyParent2Nullable(text, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(ReadOnlyParent2Nullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private UpdatableChild addUpdatableChild(int parentKey, int? parentNullableKey, string text, bool isStoring) {
      var parent = DL.Data.ReadOnlyParent2s[parentKey];
      ReadOnlyParent2Nullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DL.Data.ReadOnlyParent2Nullables[parentNullableKey.Value];
      }
      var newChild = new UpdatableChild(text, parent, parentNullable, isStoring);
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


    private UpdatableChild addUpdatableChild(ReadOnlyParent2 parent, ReadOnlyParent2Nullable? parentNullable,
      string text, bool isStoring) 
    {
      var newChild = new UpdatableChild(text, parent, parentNullable, isStoring);
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


    private void store(UpdatableChild newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[newChild.ReadOnlyParent2.Key] = newChild.ReadOnlyParent2.ToString();
      var parentNullable = newChild.ReadOnlyParent2Nullable;
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var child = DL.Data.UpdatableChildren[childKey];
      var newParent = DL.Data.ReadOnlyParent2s[parentKey];
      var oldParent = child.ReadOnlyParent2;
      ReadOnlyParent2Nullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DL.Data.ReadOnlyParent2Nullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.ReadOnlyParent2Nullable;
      child.Update(text, newParent, newParentNullable);

      expectedChildren[child.Key] = child.ToString();
      update(newParent, newParentNullable);
      update(oldParent, oldParentNullable);
      assertData();
    }


    private void update(ReadOnlyParent2 newParent, ReadOnlyParent2Nullable? newParentNullable) {
      expectedParents[newParent.Key] = newParent.ToString();
      foreach (var child in newParent.UpdatableChildren) {
        expectedChildren[child.Key] = child.ToString();
      }
      if (newParentNullable!=null) {
        expectedParentsNullable[newParentNullable.Key] = newParentNullable.ToString();
        foreach (var child in newParentNullable.UpdatableChildren) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
    }


    private void assertData() {
      assertDL();
      DL.DisposeData();

      initDL();
      assertDL();
    }
  }
}
