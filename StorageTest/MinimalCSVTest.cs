using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class MinimalCSVTest {


    CsvConfig? csvConfig;
    List<string> expectedMinimal = new List<string>();
    List<string> expectedMinimalRef= new List<string>();



    [TestMethod]
    public void TestMinimal() {
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

        var minimal1 = addMinimal(1, 2);
        addMinimalRef(1, minimal1);

        var minimal2 = addMinimal(2, 99);
        addMinimalRef(2, minimal2);
        addMinimalRef(3, minimal2);

      } finally {
        DL.DisposeData();
      }
    }


    private void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private void initDL() {
      DL.Init(csvConfig);
    }


    private void assertDL() {
      Assert.AreEqual(expectedMinimal.Count, DL.Data.Minimals.Count);
      foreach (var minimal in DL.Data.Minimals) {
        Assert.AreEqual(expectedMinimal[minimal.Key], minimal.ToString());
      }

      Assert.AreEqual(expectedMinimalRef.Count, DL.Data.MinimalRefs.Count);
      foreach (var minimalRef in DL.Data.MinimalRefs) {
        Assert.AreEqual(expectedMinimalRef[minimalRef.Key], minimalRef.ToString());
      }
    }


    private Minimal addMinimal(int someNumber, int anotherNumber) {
      var newMinimal = new Minimal(someNumber, anotherNumber, isStoring: true);
      expectedMinimal.Add(newMinimal.ToString());
      assertData();
      return newMinimal;
    }


    private void addMinimalRef(int number, Minimal minimalLookup) {
      var newMinimalRef = new MinimalRef(number, minimalLookup, isStoring: true);
      expectedMinimalRef.Add(newMinimalRef.ToString());
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