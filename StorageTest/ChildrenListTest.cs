using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class ChildrenListTest {


    CsvConfig? csvConfig;
    readonly Dictionary<int, string> parents = new Dictionary<int, string>();
    readonly Dictionary<int, string> parentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> coParents = new Dictionary<int, string>();
    readonly Dictionary<int, string> coParentsNullable = new Dictionary<int, string>();
    readonly Dictionary<int, string> children = new Dictionary<int, string>();
    readonly Dictionary<int, string> coChildren = new Dictionary<int, string>();

    readonly Dictionary<int, string> children1 = new Dictionary<int, string>();


    [TestMethod]
    public void TestChildrenList() {
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

        //create
        var p0 = new ChildrenList_Parent("P0");
        var pn0 = new ChildrenList_ParentNullable("PN0");
        var cp0 = new ChildrenList_CreateOnlyParent("CP0");
        var cpn0 = new ChildrenList_CreateOnlyParentNullable("CPN0");
        var c0 = new  ChildrenList_Child("C0", p0, pn0, cp0, cpn0);
        var cc0 = new  ChildrenList_CreateOnlyChild("CC0", cp0, cpn0);
        children[c0.Key] = c0.ToString();
        children1[c0.Key] = c0.ToTestString();
        coChildren[cc0.Key] = cc0.ToString();
        parents[p0.Key] = p0.ToString();
        parentsNullable[pn0.Key] = pn0.ToString();
        coParents[cp0.Key] = cp0.ToString();
        coParentsNullable[cpn0.Key] = cpn0.ToString();
        assertData();

        //create with nulls
        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        var c1 = new  ChildrenList_Child("C1", p0, null, cp0, null);
        var cc1 = new  ChildrenList_CreateOnlyChild("CC1", cp0, null);
        children[c1.Key] = c1.ToString();
        children1[c1.Key] = c1.ToTestString();
        coChildren[cc1.Key] = cc1.ToString();
        parents[p0.Key] = p0.ToString();
        coParents[cp0.Key] = cp0.ToString();
        assertData();

        //update child
        var p1 = new ChildrenList_Parent("P1");
        var pn1 = new ChildrenList_ParentNullable("PN1");
        var cp1 = new ChildrenList_CreateOnlyParent("CP1");
        var cpn1 = new ChildrenList_CreateOnlyParentNullable("CPN1");
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        c0.Update("C0U", p1, pn1, cp1, cpn1);
        children[c0.Key] = c0.ToString();
        children1[c0.Key] = c0.ToTestString();
        parents[p0.Key] = DC.Data.ChildrenList_Parents[p0.Key].ToString();
        parentsNullable[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents[cp0.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key].ToString();
        coParentsNullable[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        parents[p1.Key] = p1.ToString();
        parentsNullable[pn1.Key] = pn1.ToString();
        coParents[cp1.Key] = cp1.ToString();
        coParentsNullable[cpn1.Key] = cpn1.ToString();
        assertData();

        //update child with nulls
        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        c0.Update("C0U", p1, null, cp1, null);
        children[c0.Key] = c0.ToString();
        children1[c0.Key] = c0.ToTestString();
        parentsNullable[pn1.Key] = DC.Data.ChildrenList_ParentNullables[pn1.Key].ToString();
        coParentsNullable[cpn1.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn1.Key].ToString();
        assertData();

        //update child remove nulls
        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        c0.Update("C0U.1", p1, pn0, cp1, cpn0);
        children[c0.Key] = c0.ToString();
        children1[c0.Key] = c0.ToTestString();
        parentsNullable[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParentsNullable[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        assertData();

        //test reader and writer
        DC.DisposeData();
        var fromFilePath = csvConfig.DirectoryPath + "\\ChildrenList_Child.csv";
        var toFilePath = csvConfig.DirectoryPath + "\\ChildrenList_Child.new";
        using (var reader = new ChildrenList_ChildReader(fromFilePath, csvConfig)) {
          using (var writer = new ChildrenList_ChildWriter(toFilePath, csvConfig)) {
            var childIndex = 0;
            while (reader.ReadLine(out var childRaw)) {
              Assert.AreEqual(children1[childIndex++], childRaw.ToTestString());
              writer.Write(childRaw);
            }
            var newChildRaw = new ChildrenList_ChildRaw{
              Key = 2,
              Text = "C2",
              ParentKey = 0,
              ParentNullableKey = 0,
              CreateOnlyParentKey = 0,
              CreateOnlyParentNullableKey = 0
            };
            writer.Write(newChildRaw);
            children1[newChildRaw.Key] = newChildRaw.ToTestString();
          }
        }
        File.Delete(fromFilePath);
        File.Move(toFilePath, fromFilePath);
        initDL();
        children[2] = DC.Data.ChildrenList_Children[2].ToString();
        parents[p0.Key] = p0.ToString();
        parentsNullable[pn0.Key] = pn0.ToString();
        coParents[cp0.Key] = cp0.ToString();
        coParentsNullable[cpn0.Key] = cpn0.ToString();

        //delete child
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        c0.Remove();
        children.Remove(c0.Key);
        children1.Remove(c0.Key);
        parents[p1.Key] = DC.Data.ChildrenList_Parents[p1.Key].ToString();
        parentsNullable[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents[cp1.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key].ToString();
        coParentsNullable[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        assertData();

        //delete parent
        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        c1 = DC.Data.ChildrenList_Children[c1.Key];
        p0.Remove();
        parents.Remove(p0.Key);
        //removing p0 causes the removal of c1, which was a child of cp0 => cp0 has now 1 less child
        parentsNullable[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents[cp0.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key].ToString();
        coParentsNullable[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        children.Remove(c1.Key);
        children1.Remove(c1.Key);
        children.Remove(2);
        children1.Remove(2);
        assertData();

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


    private void assertData() {
      assertDL();
      DC.DisposeData();

      initDL();
      assertDL();
    }


    private void assertDL() {
      Assert.AreEqual(parents.Count, DC.Data.ChildrenList_Parents.Count);
      foreach (var parent in DC.Data.ChildrenList_Parents) {
        Assert.AreEqual(parents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(parentsNullable.Count, DC.Data.ChildrenList_ParentNullables.Count);
      foreach (var parent in DC.Data.ChildrenList_ParentNullables) {
        Assert.AreEqual(parentsNullable[parent.Key], parent.ToString());
      }

      Assert.AreEqual(coParents.Count, DC.Data.ChildrenList_CreateOnlyParents.Count);
      foreach (var parent in DC.Data.ChildrenList_CreateOnlyParents) {
        Assert.AreEqual(coParents[parent.Key], parent.ToString());
      }

      Assert.AreEqual(coParentsNullable.Count, DC.Data.ChildrenList_CreateOnlyParentNullables.Count);
      foreach (var parent in DC.Data.ChildrenList_CreateOnlyParentNullables) {
        Assert.AreEqual(coParentsNullable[parent.Key], parent.ToString());
      }

      Assert.AreEqual(children.Count, DC.Data.ChildrenList_Children.Count);
      foreach (var child in DC.Data.ChildrenList_Children) {
        Assert.AreEqual(children[child.Key], child.ToString());
      }

      Assert.AreEqual(children1.Count, DC.Data.ChildrenList_Children.Count);
      foreach (var child in DC.Data.ChildrenList_Children) {
        Assert.AreEqual(children1[child.Key], child.ToTestString());
      }

      Assert.AreEqual(coChildren.Count, DC.Data.ChildrenList_CreateOnlyChildren.Count);
      foreach (var child in DC.Data.ChildrenList_CreateOnlyChildren) {
        Assert.AreEqual(coChildren[child.Key], child.ToString());
      }
    }
  }


  public static class ChildrenListTestExtensions {

    public static string ToTestString(this ChildrenList_Child child) {
      return $"{child.Key}, {child.Text}, {child.Parent.Key}, {child.ParentNullable?.Key}, {child.CreateOnlyParent.Key}, " +
        $"{child.CreateOnlyParentNullable?.Key}";
    }

    public static string ToTestString(this ChildrenList_ChildRaw child) {
      return $"{child.Key}, {child.Text}, {child.ParentKey}, {child.ParentNullableKey}, {child.CreateOnlyParentKey}, " +
        $"{child.CreateOnlyParentNullableKey}";
    }
  }
}