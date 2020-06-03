using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {

  [TestClass]
  public class ParentOneChildTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> expectedParentStrings= new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedParentNullableStrings= new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedChildStrings= new Dictionary<int, string>();
    readonly Dictionary<int, string> expectedReadonlyChildStrings= new Dictionary<int, string>();


    [TestMethod]
    public void TestMultipleChildrenDictionary() {
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

        var parent = new ParentOneChild_Parent("P0");
        expectedParentStrings[parent.Key] = parent.ToString();
        assertData();

        //create children
        parent = DC.Data.ParentOneChild_Parents[parent.Key];
        var child = new ParentOneChild_Child("C0", parent, null);
        expectedChildStrings[child.Key] = child.ToString();
        var childReadonly = new ParentOneChild_ReadonlyChild("C0R", parent, null);
        expectedReadonlyChildStrings[childReadonly.Key] = childReadonly.ToString();
        expectedParentStrings[parent.Key] = parent.ToString();
        assertData();

        //update children
        parent = DC.Data.ParentOneChild_Parents[parent.Key];
        var parent1 = new ParentOneChild_Parent("P1");
        var parentNullable = new ParentOneChild_ParentNullable("P0N");
        child = DC.Data.ParentOneChild_Children[child.Key];
        child.Update("C0U", parent1, parentNullable);
        expectedChildStrings[child.Key] = child.ToString();
        var childReadonly1 = new ParentOneChild_ReadonlyChild("C1R", parent1, parentNullable);
        expectedReadonlyChildStrings[childReadonly1.Key] = childReadonly1.ToString();
        expectedParentStrings[parent.Key] = parent.ToString();
        expectedParentStrings[parent1.Key] = parent1.ToString();
        expectedParentNullableStrings[parentNullable.Key] = parentNullable.ToString();
        assertData();

        //update children
        parent = DC.Data.ParentOneChild_Parents[parent.Key];
        parent1 = DC.Data.ParentOneChild_Parents[parent1.Key];
        parentNullable = DC.Data.ParentOneChild_ParentNullables[parentNullable.Key];
        var parentNullable1 = new ParentOneChild_ParentNullable("P1N");
        child = DC.Data.ParentOneChild_Children[child.Key];
        child.Update("C0U1", parent, parentNullable1);
        expectedChildStrings[child.Key] = child.ToString();
        expectedParentStrings[parent.Key] = parent.ToString();
        expectedParentStrings[parent1.Key] = parent1.ToString();
        expectedParentNullableStrings[parentNullable.Key] = parentNullable.ToString();
        expectedParentNullableStrings[parentNullable1.Key] = parentNullable1.ToString();
        assertData();

        //delete children
        child = DC.Data.ParentOneChild_Children[child.Key];
        child.Remove();
        expectedChildStrings.Remove(child.Key);
        childReadonly = DC.Data.ParentOneChild_ReadonlyChildren[childReadonly.Key];
        childReadonly.Remove();
        expectedChildStrings.Remove(childReadonly.Key);
        expectedParentStrings[parent.Key] = parent.ToString();
        expectedParentStrings[parent1.Key] = parent1.ToString();
        expectedParentNullableStrings[parentNullable1.Key] = parentNullable1.ToString();

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
      Assert.AreEqual(expectedParentStrings.Count, DC.Data.ParentOneChild_Parents.Count);
      foreach (var parent in DC.Data.ParentOneChild_Parents) {
        Assert.AreEqual(expectedParentStrings[parent.Key], parent.ToString());
      }
      Assert.AreEqual(expectedParentNullableStrings.Count, DC.Data.ParentOneChild_ParentNullables.Count);
      foreach (var parent in DC.Data.ParentOneChild_ParentNullables) {
        Assert.AreEqual(expectedParentNullableStrings[parent.Key], parent.ToString());
      }
      Assert.AreEqual(expectedChildStrings.Count, DC.Data.ParentOneChild_Children.Count);
      foreach (var child in DC.Data.ParentOneChild_Children) {
        Assert.AreEqual(expectedChildStrings[child.Key], child.ToString());
      }
      Assert.AreEqual(expectedReadonlyChildStrings.Count, DC.Data.ParentOneChild_ReadonlyChildren.Count);
      foreach (var child in DC.Data.ParentOneChild_ReadonlyChildren) {
        Assert.AreEqual(expectedReadonlyChildStrings[child.Key], child.ToString());
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