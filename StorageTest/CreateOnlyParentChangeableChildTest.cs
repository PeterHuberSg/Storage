using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


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
        var now = DateTime.Now.Date;
        var parent1Key = addCreateOnlyParent2("1", isStoring: true).Key;
        var parent1NullableKey = addCreateOnlyParent2Nullable("1N", isStoring: true).Key;

        var child1Key = addChangeableChild(parent1Key, null, "11", "11U", isStoring: true).Key;

        var parent2Key = addCreateOnlyParent2("2", isStoring: true).Key;
        var parent2NullableKey = addCreateOnlyParent2Nullable("2N", isStoring: true).Key;
        var child2Key = addChangeableChild(parent2Key, parent2NullableKey, "21", "21U", isStoring: true).Key;
        var child3Key = addChangeableChild(parent2Key, parent2NullableKey, "22", "22U", isStoring: true).Key;

        //not stored
        var parent3 = addCreateOnlyParent2("3", isStoring: false);
        var parent3Nullable = addCreateOnlyParent2("3N", isStoring: false);

        var child4 = addChangeableChild(parent3, null, "31", "31U", isStoring: false);

        var parent4 = addCreateOnlyParent2("4", isStoring: false);
        var parent4Nullable = addCreateOnlyParent2Nullable("4N", isStoring: false);
        var child5 = addChangeableChild(parent4, parent4Nullable, "41", "41U", isStoring: false);
        var child6 = addChangeableChild(parent4, parent4Nullable, "42", "42U", isStoring: false);

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


    private CreateOnlyParentChangeableChild_Parent addCreateOnlyParent2(string text, bool isStoring) {
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


    private CreateOnlyParentChangeableChild_ParentNullable addCreateOnlyParent2Nullable(string text, bool isStoring) {
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


    private CreateOnlyParentChangeableChild_Child addChangeableChild(int parentKey, int? parentNullableKey, 
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


    private CreateOnlyParentChangeableChild_Child addChangeableChild(CreateOnlyParentChangeableChild_Parent parent, 
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
      var oldParent = child.Parent;
      CreateOnlyParentChangeableChild_ParentNullable? newParentNullable = null;
      if (parentNullableKey!=null) {
        newParentNullable = DC.Data.CreateOnlyParentChangeableChild_ParentNullables[parentNullableKey.Value];
      }
      var oldParentNullable = child.ParentNullable;
      child.Update(text, newParent, newParentNullable);

      expectedChildren[child.Key] = child.ToString();
      update(newParent, newParentNullable);
      update(oldParent, oldParentNullable);
      assertData();
    }


    private void update(CreateOnlyParentChangeableChild_Parent newParent, CreateOnlyParentChangeableChild_ParentNullable? newParentNullable) {
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
