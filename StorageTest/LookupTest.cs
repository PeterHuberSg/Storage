using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class LookupTest {


    CsvConfig? csvConfig;
    readonly List<string> expectedLookupParent = new List<string>();
    readonly List<string> expectedLookupParentNullable = new List<string>();
    readonly List<string> expectedLookupChild= new List<string>();


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

        var now = DateTime.Now.Date;
        var lookupParent1 = addLookupParent(now, 2);
        var lookupParent1Nullable = addLookupParentNullable(now, 2);
        addLookupChild(1, lookupParent1, lookupParent1Nullable);

        var lookupParent2 = addLookupParent(now.AddDays(1), 99);
        var lookupParent2Nullable = addLookupParentNullable(now.AddDays(1), 99);
        addLookupChild(2, lookupParent2, lookupParent2Nullable);
        addLookupChild(3, lookupParent2, lookupParent2Nullable);

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
      Assert.AreEqual(expectedLookupParent.Count, DL.Data.LookupParents.Count);
      foreach (var lookupParent in DL.Data.LookupParents) {
        Assert.AreEqual(expectedLookupParent[lookupParent.Key], lookupParent.ToString());
      }

      Assert.AreEqual(expectedLookupParentNullable.Count, DL.Data.LookupParentNullables.Count);
      foreach (var lookupParentNullable in DL.Data.LookupParentNullables) {
        Assert.AreEqual(expectedLookupParent[lookupParentNullable.Key], lookupParentNullable.ToString());
      }

      Assert.AreEqual(expectedLookupChild.Count, DL.Data.LookupChildren.Count);
      foreach (var lookupChild in DL.Data.LookupChildren) {
        Assert.AreEqual(expectedLookupChild[lookupChild.Key], lookupChild.ToString());
      }
    }


    private LookupParent addLookupParent(DateTime date, decimal someValue) {
      var newLookupParent = new LookupParent(date, someValue, isStoring: true);
      expectedLookupParent.Add(newLookupParent.ToString());
      assertData();
      return newLookupParent;
    }


    private LookupParentNullable addLookupParentNullable(DateTime date, decimal someValue) {
      var newLookupParentNullable = new LookupParentNullable(date, someValue, isStoring: true);
      expectedLookupParentNullable.Add(newLookupParentNullable.ToString());
      assertData();
      return newLookupParentNullable;
    }


    private void addLookupChild(int number, LookupParent lookupParentLookup, LookupParentNullable lookupParentLookupNullable) {
      var newLookupChild = new LookupChild(number, lookupParentLookup, lookupParentLookupNullable, isStoring: true);
      expectedLookupChild.Add(newLookupChild.ToString());
      assertData();
    }


    private void assertData() {
      assertDL();
      DL.DisposeData();

      initDL();
      assertDL();
    }
  }
}