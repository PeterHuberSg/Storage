using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageDataContext;


namespace StorageTest {

  [TestClass]
  public class CreateOnlyParentChangeableChildTest {

    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildren= new Dictionary<int, string>();


    [TestMethod]
    public void TestCreateOnlyParentChangeableChild() {
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
        var parent1Key = addParent("1", isStoring: true).Key;
        var parent1NullableKey = addParentNullable("1N", isStoring: true).Key;

        var child1Key = addChild(parent1Key, null, "11", "11U", isStoring: true).Key;

        var parent2Key = addParent("2", isStoring: true).Key;
        var parent2NullableKey = addParentNullable("2N", isStoring: true).Key;
        var child2Key = addChild(parent2Key, parent2NullableKey, "21", "21U", isStoring: true).Key;
        var child3Key = addChild(parent2Key, parent2NullableKey, "22", "22U", isStoring: true).Key;
        DC.Data.CommitTransaction();

        //not stored
        var parent3 = addParent("3", isStoring: false);
        var parent3Nullable = addParent("3N", isStoring: false);
        var child4 = addChild(parent3, null, "31", "31U", isStoring: false);
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
        var child5 = addChild(parent4, parent4Nullable, "41", "41U", isStoring: false);
        var child6 = addChild(parent4, parent4Nullable, "42", "42U", isStoring: false);
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

        updateChild(child1Key, parent2Key, parent2NullableKey, now, "12");
        updateChild(child1Key, parent2Key, parent2NullableKey, now.AddDays(-1), "13");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "14");
        updateChild(child1Key, parent1Key, null, now.AddDays(-1), "15");
        updateChild(child1Key, parent1Key, parent1NullableKey, now.AddDays(-1), "16");


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
      Assert.AreEqual(expectedParents.Count, DC.Data.CreateOnlyParentChangeableChild_Parents.Count);
      foreach (var parent in DC.Data.CreateOnlyParentChangeableChild_Parents) {
        Assert.AreEqual(expectedParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(expectedParentsNullable.Count, DC.Data.CreateOnlyParentChangeableChild_ParentNullables.Count);
      foreach (var parentNullable in DC.Data.CreateOnlyParentChangeableChild_ParentNullables) {
        Assert.AreEqual(expectedParentsNullable[parentNullable.Key], parentNullable.ToString());
      }

      Assert.AreEqual(expectedChildren.Count, DC.Data.CreateOnlyParentChangeableChild_Children.Count);
      foreach (var child in DC.Data.CreateOnlyParentChangeableChild_Children) {
        Assert.AreEqual(expectedChildren[child.Key], child.ToString());
      }
    }


    private CreateOnlyParentChangeableChild_Parent addParent(string text, bool isStoring) {
      var newParent = new CreateOnlyParentChangeableChild_Parent(text, isStoring);
      if (isStoring) {
        expectedParents.Add(newParent.Key, newParent.ToString());
        assertData();
      }
      return newParent;
    }


    private void store(CreateOnlyParentChangeableChild_Parent newParent) {
      newParent.Store();
      expectedParents.Add(newParent.Key, newParent.ToString());
    }


    private CreateOnlyParentChangeableChild_ParentNullable addParentNullable(string text, bool isStoring) {
      var newParentNullable = new CreateOnlyParentChangeableChild_ParentNullable(text, isStoring);
      if (isStoring) {
        expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
        assertData();
      }
      return newParentNullable;
    }


    private void store(CreateOnlyParentChangeableChild_ParentNullable newParentNullable) {
      newParentNullable.Store();
      expectedParentsNullable.Add(newParentNullable.Key, newParentNullable.ToString());
    }


    private CreateOnlyParentChangeableChild_Child addChild(int parentKey, int? parentNullableKey, 
      string readonlyText, string updatableText, bool isStoring) 
    {
      var parent = DC.Data.CreateOnlyParentChangeableChild_Parents[parentKey];
      CreateOnlyParentChangeableChild_ParentNullable? parentNullable = null;
      if (parentNullableKey.HasValue) {
        parentNullable = DC.Data.CreateOnlyParentChangeableChild_ParentNullables[parentNullableKey.Value];
      }
      var newChild = new CreateOnlyParentChangeableChild_Child(readonlyText, updatableText, parent, parentNullable, isStoring);
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


    private CreateOnlyParentChangeableChild_Child addChild(CreateOnlyParentChangeableChild_Parent parent, 
      CreateOnlyParentChangeableChild_ParentNullable? parentNullable, string readonlyText, string updatableText, 
      bool isStoring) 
    {
      var newChild = new CreateOnlyParentChangeableChild_Child(readonlyText, updatableText, parent, parentNullable, isStoring);
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


    private void store(CreateOnlyParentChangeableChild_Child newChild) {
      newChild.Store();
      expectedChildren.Add(newChild.Key, newChild.ToString());
      expectedParents[newChild.Parent.Key] = newChild.Parent.ToString();
      var parentNullable = newChild.ParentNullable;
      if (parentNullable!=null) {
        expectedParentsNullable[parentNullable.Key] = parentNullable.ToString();
      }
    }


    private void updateChild(int childKey, int parentKey, int? parentNullableKey, DateTime date, string text) {
      var child = DC.Data.CreateOnlyParentChangeableChild_Children[childKey];
      var newParent = DC.Data.CreateOnlyParentChangeableChild_Parents[parentKey];
      CreateOnlyParentChangeableChild_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.CreateOnlyParentChangeableChild_ParentNullables[parentNullableKey.Value];
      }
      DC.Data.StartTransaction();
      child.Update(text, newParent, newParentNullable);
      DC.Data.RollbackTransaction();
      assertData();

      child = DC.Data.CreateOnlyParentChangeableChild_Children[childKey];
      newParent = DC.Data.CreateOnlyParentChangeableChild_Parents[parentKey];
      var oldParent = child.Parent;
      newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.CreateOnlyParentChangeableChild_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.ParentNullable;
      DC.Data.StartTransaction();
      child.Update(text, newParent, newParentNullable);
      DC.Data.CommitTransaction();
      expectedChildren[child.Key] = child.ToString();
      updateExpected(newParent, newParentNullable);
      updateExpected(oldParent, oldParentNullable);
      assertData();
    }


    private void updateExpected(CreateOnlyParentChangeableChild_Parent newParent, 
      CreateOnlyParentChangeableChild_ParentNullable? newParentNullable) 
    {
      expectedParents[newParent.Key] = newParent.ToString();
      foreach (var child in newParent.CreateOnlyParentChangeableChild_Children) {
        expectedChildren[child.Key] = child.ToString();
      }
      if (newParentNullable!=null) {
        expectedParentsNullable[newParentNullable.Key] = newParentNullable.ToString();
        foreach (var child in newParentNullable.CreateOnlyParentChangeableChild_Children) {
          expectedChildren[child.Key] = child.ToString();
        }
      }
    }


    private void assertData() {
      assertDL();
      DC.DisposeData();

      initDL();
      assertDL();
    }
  }
}
