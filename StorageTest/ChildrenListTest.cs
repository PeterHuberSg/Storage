using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageDataContext;


namespace StorageTest {


  [TestClass]
  public class ChildrenListTest {
    /*
    Tests in detail if DataStore internal data is correct after each basic operation (add, update, remove) including
    RollbackTransaction() and CommitTransaction. After both transaction variants, the DataStore gets tested first,
    then disposed and initialised a new, then tested again.
    Because the data gets read fresh from the file each time, always the newest version of parents and children
    need to be used.
    */


    #region TestChildrenList
    //      ----------------

    public class DataStoreStats {
      public bool IsContinuous;
      public bool IsUpdated;
      public bool IsDeleted;
      public int FirstIndex;
      public int LastIndex;
      public readonly Dictionary<int, string> Items;

      public DataStoreStats() {
        IsContinuous = true;
        FirstIndex = -1;
        LastIndex = -1;
        Items = new Dictionary<int, string>();
      }

      public void Set(bool isContinuous, bool isUpdated, bool isDeleted, int firstIndex, int lastIndex) {
        IsContinuous = isContinuous;
        IsUpdated = isUpdated;
        IsDeleted = isDeleted;
        FirstIndex = firstIndex;
        LastIndex = lastIndex;
      }

      public override string ToString() {
        return $"Continuous: {IsContinuous}, Updated: {IsUpdated}, Deleted: {IsDeleted}, First: {FirstIndex}, " +
          $"Last: {LastIndex}, Count: {Items.Count}";
      }
    }


    CsvConfig? csvConfig;
    #pragma warning disable CS8618 // Non-nullable field is uninitialized.
    DataStoreStats parents;
    DataStoreStats parentNullables;
    DataStoreStats coParents;
    DataStoreStats coParentNullables;
    DataStoreStats children;
    DataStoreStats coChildren;
    #pragma warning restore CS8618 // Non-nullable field is uninitialized.

    //uses ToTestString() instead of ToString()
    Dictionary<int, string> childrenTestString = new Dictionary<int, string>();


    [TestMethod]
    public void TestChildrenList() {
      try {
        parents = new DataStoreStats();
        parentNullables = new DataStoreStats();
        coParents = new DataStoreStats();
        coParentNullables = new DataStoreStats();
        children = new DataStoreStats();
        coChildren = new DataStoreStats();
        childrenTestString = new Dictionary<int, string>();


        var directoryInfo = new DirectoryInfo("TestCsv");
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();

        csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        initDC();
        assertData();

        //create
        var p0 = new ChildrenList_Parent("P0", isStoring: false);
        var pn0 = new ChildrenList_ParentNullable("PN0", isStoring: false);
        var cp0 = new ChildrenList_CreateOnlyParent("CP0", isStoring: false);
        var cpn0 = new ChildrenList_CreateOnlyParentNullable("CPN0", isStoring: false);
        var c0 = new  ChildrenList_Child("C0", p0, pn0, cp0, cpn0, isStoring: false);
        var cc0 = new  ChildrenList_CreateOnlyChild("CC0", cp0, cpn0, isStoring: false);
        DC.Data.StartTransaction();
        p0.Store();
        pn0.Store();
        cp0.Store();
        cpn0.Store();
        c0.Store();
        cc0.Store();
        DC.Data.RollbackTransaction();
        assertData();

        /*+
        childrenAddedCount = 1;
        parentsAddedCount = 1;
        +*/
        DC.Data.StartTransaction();
        p0.Store();
        pn0.Store();
        cp0.Store();
        cpn0.Store();
        c0.Store();
        cc0.Store();
        DC.Data.CommitTransaction();
        children.Items[c0.Key] = c0.ToString();
        childrenTestString[c0.Key] = c0.ToTestString();
        coChildren.Items[cc0.Key] = cc0.ToString();
        parents.Items[p0.Key] = p0.ToString();
        parentNullables.Items[pn0.Key] = pn0.ToString();
        coParents.Items[cp0.Key] = cp0.ToString();
        coParentNullables.Items[cpn0.Key] = cpn0.ToString();
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        coChildren.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        parents.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        parentNullables.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        coParents.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        coParentNullables.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        assertData();

        //create with nulls
        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        var c1 = new  ChildrenList_Child("C1", p0, null, cp0, null, isStoring: false);
        var cc1 = new  ChildrenList_CreateOnlyChild("CC1", cp0, null, isStoring: false);
        DC.Data.StartTransaction();
        c1.Store();
        cc1.Store();
        DC.Data.RollbackTransaction();
        assertData();

        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        c1 = new ChildrenList_Child("C1", p0, null, cp0, null, isStoring: false);
        cc1 = new ChildrenList_CreateOnlyChild("CC1", cp0, null, isStoring: false);
        /*+
        childrenAddedCount = 1;
        +*/
        DC.Data.StartTransaction();
        c1.Store();
        cc1.Store();
        DC.Data.CommitTransaction();
        children.Items[c1.Key] = c1.ToString();
        childrenTestString[c1.Key] = c1.ToTestString();
        coChildren.Items[cc1.Key] = cc1.ToString();
        parents.Items[p0.Key] = p0.ToString();
        coParents.Items[cp0.Key] = cp0.ToString();
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        coChildren.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        assertData();

        //update child
        var p1 = new ChildrenList_Parent("P1", isStoring: false);
        var pn1 = new ChildrenList_ParentNullable("PN1", isStoring: false);
        var cp1 = new ChildrenList_CreateOnlyParent("CP1", isStoring: false);
        var cpn1 = new ChildrenList_CreateOnlyParentNullable("CPN1", isStoring: false);
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        DC.Data.StartTransaction();
        p1.Store();
        pn1.Store();
        cp1.Store();
        cpn1.Store();
        c0.Update("C0U", p1, pn1, cp1, cpn1);
        DC.Data.RollbackTransaction();
        assertData();

        c0 = DC.Data.ChildrenList_Children[c0.Key];
        /*+
        childrenUpdatedCount = 1;
        parentsAddedCount = 1;
        +*/
        DC.Data.StartTransaction();
        p1.Store();
        pn1.Store();
        cp1.Store();
        cpn1.Store();
        c0.Update("C0U", p1, pn1, cp1, cpn1);
        DC.Data.CommitTransaction();
        children.Items[c0.Key] = c0.ToString();
        childrenTestString[c0.Key] = c0.ToTestString();
        parents.Items[p0.Key] = DC.Data.ChildrenList_Parents[p0.Key].ToString();
        parentNullables.Items[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents.Items[cp0.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key].ToString();
        coParentNullables.Items[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        parents.Items[p1.Key] = p1.ToString();
        parentNullables.Items[pn1.Key] = pn1.ToString();
        coParents.Items[cp1.Key] = cp1.ToString();
        coParentNullables.Items[cpn1.Key] = cpn1.ToString();
        children.Set(isContinuous: true, isUpdated: true, isDeleted: false, firstIndex: 0, lastIndex: 1);
        parents.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        parentNullables.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        coParents.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        coParentNullables.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        assertData();

        //update child with nulls
        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        DC.Data.StartTransaction();
        c0.Update("C0U", p1, null, cp1, null);
        DC.Data.RollbackTransaction();
        assertData();

        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        children.Set(isContinuous: true, isUpdated: true, isDeleted: false, firstIndex: 0, lastIndex: 1);
        /*+
        childrenUpdatedCount = 1;
        +*/
        DC.Data.StartTransaction();
        c0.Update("C0U", p1, null, cp1, null);
        DC.Data.CommitTransaction();
        children.Items[c0.Key] = c0.ToString();
        childrenTestString[c0.Key] = c0.ToTestString();
        parentNullables.Items[pn1.Key] = DC.Data.ChildrenList_ParentNullables[pn1.Key].ToString();
        coParentNullables.Items[cpn1.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn1.Key].ToString();
        assertData();

        //update child remove nulls
        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);
        DC.Data.StartTransaction();
        c0.Update("C0U.1", p1, pn0, cp1, cpn0);
        DC.Data.RollbackTransaction();
        assertData();

        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp1 = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        children.Set(isContinuous: true, isUpdated: true, isDeleted: false, firstIndex: 0, lastIndex: 1);
        /*+
        childrenUpdatedCount = 1;
        +*/
        DC.Data.StartTransaction();
        c0.Update("C0U.1", p1, pn0, cp1, cpn0);
        DC.Data.CommitTransaction();
        children.Items[c0.Key] = c0.ToString();
        childrenTestString[c0.Key] = c0.ToTestString();
        parentNullables.Items[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParentNullables.Items[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        assertData();

        //test reader and writer
        DC.DisposeData();
        var fromFilePath = csvConfig.DirectoryPath + "\\ChildrenList_Child.csv";
        var toFilePath = csvConfig.DirectoryPath + "\\ChildrenList_Child.new";
        using (var reader = new ChildrenList_ChildReader(fromFilePath, csvConfig)) {
          using var writer = new ChildrenList_ChildWriter(toFilePath, csvConfig);
          var childIndex = 0;
          while (reader.ReadLine(out var childRaw)) {
            Assert.AreEqual(childrenTestString[childIndex++], childRaw.ToTestString());
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
          childrenTestString[newChildRaw.Key] = newChildRaw.ToTestString();
        }
        File.Delete(fromFilePath);
        File.Move(toFilePath, fromFilePath);
        initDC();
        children.Items[2] = DC.Data.ChildrenList_Children[2].ToString();
        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        parents.Items[p0.Key] = p0.ToString();
        parentNullables.Items[pn0.Key] = pn0.ToString();
        coParents.Items[cp0.Key] = cp0.ToString();
        coParentNullables.Items[cpn0.Key] = cpn0.ToString();
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 2);

        //delete child
        c0 = DC.Data.ChildrenList_Children[c0.Key];
        DC.Data.StartTransaction();
        c0.Remove();
        DC.Data.RollbackTransaction();
        assertData();

        c0 = DC.Data.ChildrenList_Children[c0.Key];
        children.Items.Remove(c0.Key);
        childrenTestString.Remove(c0.Key);
        /*+
        childrenRemovedCount = 1;
        +*/
        DC.Data.StartTransaction();
        c0.Remove();
        DC.Data.CommitTransaction();
        parents.Items[p1.Key] = DC.Data.ChildrenList_Parents[p1.Key].ToString();
        parentNullables.Items[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents.Items[cp1.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp1.Key].ToString();
        coParentNullables.Items[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        children.Set(isContinuous: true, isUpdated: false, isDeleted: true, firstIndex: 1, lastIndex: 2);
        assertData();
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 1);

        //delete parent
        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        DC.Data.StartTransaction();
        p0.Remove();
        DC.Data.RollbackTransaction();
        assertData();

        p0 = DC.Data.ChildrenList_Parents[p0.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        c1 = DC.Data.ChildrenList_Children[c1.Key];
        parents.Items.Remove(p0.Key);
        children.Items.Remove(c1.Key);
        childrenTestString.Remove(c1.Key);
        children.Items.Remove(2);
        childrenTestString.Remove(2);
        /*+
        childrenRemovedCount = 2;//C1, C2 get removed
        parentsRemovedCount = 1;
        +*/
        DC.Data.StartTransaction();
        p0.Remove();
        DC.Data.CommitTransaction();
        //removing p0 causes the removal of c1, which was a child of cp0 => cp0 has now 1 less child
        parentNullables.Items[pn0.Key] = DC.Data.ChildrenList_ParentNullables[pn0.Key].ToString();
        coParents.Items[cp0.Key] = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key].ToString();
        coParentNullables.Items[cpn0.Key] = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key].ToString();
        parents.Set(isContinuous: true, isUpdated: false, isDeleted: true, firstIndex: 1, lastIndex: 1);
        children.Set(isContinuous: true, isUpdated: false, isDeleted: true, firstIndex: -1, lastIndex: -1);
        assertData();

        //some tests without disposing DC between tests
        //---------------------------------------------

        //test added, updated and removed events without transaction
        p1 = DC.Data.ChildrenList_Parents[p1.Key];
        pn0 = DC.Data.ChildrenList_ParentNullables[pn0.Key];
        cp0 = DC.Data.ChildrenList_CreateOnlyParents[cp0.Key];
        cpn0 = DC.Data.ChildrenList_CreateOnlyParentNullables[cpn0.Key];
        /*+
        childrenAddedCount = 1;
        +*/
        var c3 = new  ChildrenList_Child("C3", p1, pn0, cp0, cpn0);
        children.Items[c3.Key] = c3.ToString();
        childrenTestString[c3.Key] = c3.ToTestString();
        parents.Items[p1.Key] = p1.ToString();
        parentNullables.Items[pn0.Key] = pn0.ToString();
        coParents.Items[cp0.Key] = cp0.ToString();
        coParentNullables.Items[cpn0.Key] = cpn0.ToString();
        parents.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        children.Set(isContinuous: true, isUpdated: false, isDeleted: false, firstIndex: 0, lastIndex: 0);
        assertDC(isAfterDispose: false);

        /*+
        childrenUpdatedCount = 1;
        +*/
        c3.Update("C3U", p1, pn0, cp0, cpn0);
        children.Items[c3.Key] = c3.ToString();
        childrenTestString[c3.Key] = c3.ToTestString();
        children.Set(isContinuous: true, isUpdated: true, isDeleted: false, firstIndex: 0, lastIndex: 0);
        assertDC(isAfterDispose: false);

        /*+
        childrenAddedCount = 1;
        +*/
        var cDelete = new  ChildrenList_Child("cDelete", p1, pn0, cp0, cpn0);
        /*+
        childrenAddedCount = 1;
        +*/
        var c4 = new  ChildrenList_Child("C4", p1, pn0, cp0, cpn0);
        /*+
        childrenRemovedCount = 1;
        +*/
        cDelete.Remove();
        children.Items[c4.Key] = c4.ToString();
        childrenTestString[c4.Key] = c4.ToTestString();
        parents.Items[p1.Key] = p1.ToString();
        parentNullables.Items[pn0.Key] = pn0.ToString();
        coParents.Items[cp0.Key] = cp0.ToString();
        coParentNullables.Items[cpn0.Key] = cpn0.ToString();
        children.Set(isContinuous: false, isUpdated: true, isDeleted: true, firstIndex: 0, lastIndex: 2);
        assertDC(isAfterDispose: false);

      } finally {
        DC.DisposeData();
      }
    }


    /*+
    int childrenAddedCount = 0;
    int childrenUpdatedCount = 0;
    int childrenRemovedCount = 0;
    int parentsAddedCount = 0;
    int parentsUpdatedCount = 0;
    int parentsRemovedCount = 0;
    +*/

    /*+
    private void childrenList_Children_Added(ChildrenList_Child _) {
      Assert.AreNotEqual(0, childrenAddedCount);
      childrenAddedCount--;
    }


    private void childrenList_Children_Updated(ChildrenList_Child _, ChildrenList_Child dummy) {
      Assert.AreNotEqual(0, childrenUpdatedCount);
      childrenUpdatedCount--;
    }


    private void childrenList_Children_Removed(ChildrenList_Child _) {
      Assert.AreNotEqual(0, childrenRemovedCount);
      childrenRemovedCount--;
    }


    private void childrenList_Parents_Added(ChildrenList_Parent _) {
      Assert.AreNotEqual(0, parentsAddedCount);
      parentsAddedCount--;
    }


    private void childrenList_Parents_Updated(ChildrenList_Parent _, ChildrenList_Parent dummy) {
      Assert.AreNotEqual(0, parentsUpdatedCount);
      parentsUpdatedCount--;
    }

    private void childrenList_Parents_Removed(ChildrenList_Parent _) {
      Assert.AreNotEqual(0, parentsRemovedCount);
      parentsRemovedCount--;
    }
    +*/

    private void reportException(Exception obj) {
      System.Diagnostics.Debug.WriteLine(obj);
      System.Diagnostics.Debugger.Break();
      Assert.Fail();
    }


    private void initDC() {
      new DC(csvConfig);
    }


    private void assertData() {
      assertDC(isAfterDispose: false);
      DC.DisposeData();

      initDC();
      assertDC(isAfterDispose: true);
    }


    private void assertDC(bool isAfterDispose, bool skipChildren = false) {

      var store = DC.Data.ChildrenList_Parents;
      assertStore(parents, store, isAfterDispose);
      foreach (var parent in DC.Data.ChildrenList_Parents) {
        Assert.AreEqual(parents.Items[parent.Key], parent.ToString());
      }

      var store1 = DC.Data.ChildrenList_ParentNullables;
      assertStore(parentNullables, store1, isAfterDispose);
      foreach (var parent in DC.Data.ChildrenList_ParentNullables) {
        Assert.AreEqual(parentNullables.Items[parent.Key], parent.ToString());
      }

      var store2 = DC.Data.ChildrenList_CreateOnlyParents;
      assertStore(coParents, store2, isAfterDispose);
      foreach (var parent in DC.Data.ChildrenList_CreateOnlyParents) {
        Assert.AreEqual(coParents.Items[parent.Key], parent.ToString());
      }

      var store3 = DC.Data.ChildrenList_CreateOnlyParentNullables;
      assertStore(coParentNullables, store3, isAfterDispose);
      foreach (var parent in DC.Data.ChildrenList_CreateOnlyParentNullables) {
        Assert.AreEqual(coParentNullables.Items[parent.Key], parent.ToString());
      }

      var store4 = DC.Data.ChildrenList_Children;
      if (!skipChildren) {
        foreach (var child in DC.Data.ChildrenList_Children) {
          Assert.AreEqual(children.Items[child.Key], child.ToString());
        }

        Assert.AreEqual(childrenTestString.Count, DC.Data.ChildrenList_Children.Count);
        foreach (var child in DC.Data.ChildrenList_Children) {
          Assert.AreEqual(childrenTestString[child.Key], child.ToTestString());
        }
      }
      assertStore(children, store4, isAfterDispose, skipCount: skipChildren);

      var store6 = DC.Data.ChildrenList_CreateOnlyChildren;
      assertStore(coChildren, store6, isAfterDispose);
      foreach (var child in DC.Data.ChildrenList_CreateOnlyChildren) {
        Assert.AreEqual(coChildren.Items[child.Key], child.ToString());
      }

      /*+
      Assert.AreEqual(0, childrenAddedCount);
      Assert.AreEqual(0, childrenUpdatedCount);
      Assert.AreEqual(0, childrenRemovedCount);
      Assert.AreEqual(0, parentsAddedCount);
      Assert.AreEqual(0, parentsUpdatedCount);
      Assert.AreEqual(0, parentsRemovedCount);
      +*/
    }


    static readonly PropertyInfo firstItemIndexProperty =  typeof(DataStore).GetProperty("FirstItemIndex",
      BindingFlags.NonPublic | BindingFlags.Instance)!;
    static readonly PropertyInfo lastItemIndexProperty = typeof(DataStore).GetProperty("LastItemIndex",
      BindingFlags.NonPublic | BindingFlags.Instance)!;


    private void assertStore(DataStoreStats storeStats, DataStore store, bool isAfterDispose, bool skipCount = false) {
      Assert.AreEqual(storeStats.IsContinuous, store.AreKeysContinuous);
      if (isAfterDispose) {
        Assert.AreEqual(false, store.AreItemsDeleted);
        Assert.AreEqual(false, store.AreItemsUpdated);
        if (storeStats.FirstIndex==-1) {
          Assert.AreEqual(-1, firstItemIndexProperty.GetValue(store));
          Assert.AreEqual(-1, lastItemIndexProperty.GetValue(store));
        } else {
          Assert.AreEqual(0, firstItemIndexProperty.GetValue(store));
          Assert.AreEqual(storeStats.LastIndex-storeStats.FirstIndex, lastItemIndexProperty.GetValue(store));
        }
      } else {
        Assert.AreEqual(storeStats.IsDeleted, store.AreItemsDeleted);
        Assert.AreEqual(storeStats.IsUpdated, store.AreItemsUpdated);
        Assert.AreEqual(storeStats.FirstIndex, firstItemIndexProperty.GetValue(store));
        Assert.AreEqual(storeStats.LastIndex, lastItemIndexProperty.GetValue(store));
      }
      if (!skipCount) {
        Assert.AreEqual(storeStats.Items.Count, store.Count);
      }
    }
    #endregion


    #region TestChildrenListPerformance
    //      ---------------------------

    /*
30.7.2020
100000 activitiesCount
10 childrenPerParent
00:00:03.1164733 Create, commit
00:00:01.8728488 Update, commit
00:00:01.5257475 Remove, commit
00:00:03.7565031 Create, rollback
00:00:02.2642793 Update, rollback
00:00:01.6454205 Remove, rollback
00:00:04.6097188 Create, noTransaction
00:00:02.0508278 Update, noTransaction
00:00:01.7699474 Remove, noTransaction

26.7.2020: Changed event item_HasChanged to call of ItemHasChanged()
100000 activitiesCount
10 childrenPerParent
00:00:02.4873223 Create, noTransaction
00:00:02.1326261 Update, noTransaction
00:00:02.0027097 Remove, noTransaction
00:00:03.1469637 Create, rollback
00:00:02.7766883 Update, rollback
00:00:02.2212682 Remove, rollback
00:00:02.6006215 Create, commit
00:00:02.2364214 Update, commit
00:00:02.3673098 Remove, commit

1000 activitiesCount
10 childrenPerParent
00:00:00.0168216 Create, noTransaction
00:00:00.0154454 Update, noTransaction
00:00:00.0143725 Remove, noTransaction
00:00:00.0253890 Create, rollback
00:00:00.0225073 Update, rollback
00:00:00.0177456 Remove, rollback
00:00:00.0179672 Create, commit
00:00:00.0171397 Update, commit
00:00:00.0142170 Remove, commit


    25.7.2020
100000 activitiesCount
10 childrenPerParent
00:00:02.6508348 Create, noTransaction
00:00:02.2467978 Update, noTransaction
00:00:02.0047998 Remove, noTransaction
00:00:03.7706295 Create, rollback
00:00:03.1206089 Update, rollback
00:00:02.6421976 Remove, rollback
00:00:03.8709616 Create, commit
00:00:02.3105336 Update, commit
00:00:01.8777740 Remove, commit

10000 activitiesCount
10 childrenPerParent
00:00:00.2404470 Create, noTransaction
00:00:00.2120004 Update, noTransaction
00:00:00.1649583 Remove, noTransaction
00:00:00.3387088 Create, rollback
00:00:00.1864754 Update, rollback
00:00:00.2326274 Remove, rollback
00:00:00.3366866 Create, commit
00:00:00.1783481 Update, commit
00:00:00.3059561 Remove, commit

1000 activitiesCount
10 childrenPerParent
00:00:00.0180422 Create, noTransaction
00:00:00.0160481 Update, noTransaction
00:00:00.0180843 Remove, noTransaction
00:00:00.0301951 Create, rollback
00:00:00.0175754 Update, rollback
00:00:00.0204837 Remove, rollback
00:00:00.0243624 Create, commit
00:00:00.0130577 Update, commit
00:00:00.0238833 Remove, commit

    */

    [TestMethod]
    public void TestChildrenListPerformance() {
      try {
        var directoryInfo = new DirectoryInfo("TestCsv");
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();

        GC.Collect(); //in case garbage collector would run soon because of previous tests
        GC.WaitForPendingFinalizers();
        csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        var activitiesCount = 1000;
        var childrenPerParent = 10;
        var sw = new Stopwatch();
        Debug.WriteLine($"{activitiesCount} activitiesCount");
        Debug.WriteLine($"{childrenPerParent} childrenPerParent");

        //testNormal(activitiesCount, childrenPerParent, csvConfig, sw);
        testCommit(activitiesCount, childrenPerParent, csvConfig, sw);
        directoryInfo.Delete(recursive: true);
        directoryInfo.Create();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        testRollback(activitiesCount, childrenPerParent, csvConfig, sw);
        directoryInfo.Delete(recursive: true);
        directoryInfo.Create();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        //testCommit(activitiesCount, childrenPerParent, csvConfig, sw);
        testNormal(activitiesCount, childrenPerParent, csvConfig, sw);
      } finally {
        DC.DisposeData();
      }
    }


    private void testNormal(int activitiesCount, int childrenPerParent, CsvConfig csvConfig, Stopwatch sw) {
      new DC(csvConfig);
      //new DC(null);
      createParents(activitiesCount, out var parentP, out var parentPC);
      sw.Start();
      createChildren(activitiesCount, childrenPerParent, parentP, parentPC, out var childrenP, out var cIndex);
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Create, noTransaction");

      sw.Restart();
      for (int i = 0; i < cIndex; i++) {
        var child = childrenP[i];
        child.Update(child.Text + "U", child.Parent, null, child.CreateOnlyParent, null);
      }
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Update, noTransaction");

      sw.Restart();
      for (int i = 0; i < cIndex; i++) {
        childrenP[i].Remove();
      }
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Remove, noTransaction");
      DC.Data.Dispose();
    }


    private void testRollback(int activitiesCount, int childrenPerParent, CsvConfig csvConfig, Stopwatch sw) {
      new DC(csvConfig);

      createParents(activitiesCount, out var parentP, out var parentPC);
      sw.Restart();
      DC.Data.StartTransaction();
      createChildren(activitiesCount, childrenPerParent, parentP, parentPC, out _, out _);
      DC.Data.RollbackTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Create, rollback");
      createChildren(activitiesCount, childrenPerParent, parentP, parentPC, out var childrenP, out var cIndex);
      DC.Data.ChildrenList_Children.Flush();
      sw.Restart();
      DC.Data.StartTransaction();
      for (int i = 0; i < cIndex; i++) {
        var child = childrenP[i];
        child.Update(child.Text + "U", child.Parent, null, child.CreateOnlyParent, null);
      }
      DC.Data.RollbackTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Update, rollback");

      sw.Restart();
      DC.Data.StartTransaction();
      for (int i = 0; i < cIndex; i++) {
        childrenP[i].Remove();
      }
      DC.Data.RollbackTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Remove, rollback");
      DC.Data.Dispose();
    }


    private void testCommit(int activitiesCount, int childrenPerParent, CsvConfig csvConfig, Stopwatch sw) {
      new DC(csvConfig);

      createParents(activitiesCount, out var parentP, out var parentPC);
      sw.Restart();
      DC.Data.StartTransaction();
      createChildren(activitiesCount, childrenPerParent, parentP, parentPC, out var childrenP, out var cIndex);
      DC.Data.CommitTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Create, commit");

      sw.Restart();
      DC.Data.StartTransaction();
      for (int i = 0; i < cIndex; i++) {
        var child = childrenP[i];
        child.Update(child.Text + "U", child.Parent, null, child.CreateOnlyParent, null);
      }
      DC.Data.CommitTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Update, commit");

      sw.Restart();
      DC.Data.StartTransaction();
      for (int i = 0; i < cIndex; i++) {
        childrenP[i].Remove();
      }
      DC.Data.CommitTransaction();
      DC.Data.ChildrenList_Parents.Flush();
      DC.Data.ChildrenList_CreateOnlyParents.Flush();
      DC.Data.ChildrenList_Children.Flush();
      sw.Stop();
      Debug.WriteLine($"{sw.Elapsed} Remove, commit");
      DC.Data.Dispose();
    }


    private void createParents(int activitiesCount, out ChildrenList_Parent[] parentP, out ChildrenList_CreateOnlyParent[] parentPC) {
      parentP = new ChildrenList_Parent[activitiesCount];
      parentPC = new ChildrenList_CreateOnlyParent[activitiesCount];
      for (int i = 0; i < activitiesCount; i++) {
        var pP = new ChildrenList_Parent($"{i}");
        parentP[i] = pP;
        var pC = new ChildrenList_CreateOnlyParent($"{i}");
        parentPC[i] = pC;
      }
    }


    private void createChildren(
      int activitiesCount,
      int childrenPerParent,
      ChildrenList_Parent[] parentP,
      ChildrenList_CreateOnlyParent[] parentPC,
      out ChildrenList_Child[] childrenP,
      out int cIndex) 
    {
      cIndex = 0;
      childrenP = new ChildrenList_Child[childrenPerParent * activitiesCount];
      for (int i = 0; i < activitiesCount; i++) {
        var pP = parentP[i];
        var pC = parentPC[i];
        for (int j = 0; j < childrenPerParent; j++) {
          childrenP[cIndex++] = new ChildrenList_Child($"{cIndex}", pP, null, pC, null);
        }
      }
    }


    //private void createChildrenParents(
    //  int activitiesCount,
    //  int childrenPerParent,
    //  out ChildrenList_Parent[] parentP,
    //  out ChildrenList_CreateOnlyParent[] parentPC,
    //  out ChildrenList_Child[] childrenP,
    //  out int cIndex) {
    //  cIndex = 0;
    //  parentP = new ChildrenList_Parent[activitiesCount];
    //  parentPC = new ChildrenList_CreateOnlyParent[activitiesCount];
    //  childrenP = new ChildrenList_Child[childrenPerParent * activitiesCount];
    //  for (int i = 0; i < activitiesCount; i++) {
    //    var pP = new ChildrenList_Parent($"{i}");
    //    parentP[i] = pP;
    //    var pC = new ChildrenList_CreateOnlyParent($"{i}");
    //    parentPC[i] = pC;
    //    for (int j = 0; j < childrenPerParent; j++) {
    //      childrenP[cIndex++] = new ChildrenList_Child($"{cIndex}", pP, null, pC, null);
    //    }
    //  }
    //}
    #endregion
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