using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;

namespace StorageTest {


  [TestClass]
  public class PropertyNeedsDictionaryTest {

    static readonly DirectoryInfo directoryInfo =  new DirectoryInfo("TestCsv");
    static readonly CsvConfig csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
    static readonly Dictionary<int, string> expectedIdInts = new Dictionary<int, string>();
    static readonly Dictionary<string, string> expectedIdStrings = new Dictionary<string, string>();


    [TestMethod]
    public void TestPropertyNeedsDictionary() {
      try {
        directoryInfo.Refresh();
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        new DC(csvConfig);
        assertData();
        var key0 = addData(1, "one", "first");
        var key1 = addData(2, null, "second null");

        update(key0, 1, "one", "first");
        update(key0, 11, "one.one", "changed");
        update(key0, 10, null, "one.null changed");
        update(key1, 20, null, "two.null");
        update(key1, 21, null, "two.one");

        delete(key0);
        delete(key1);

      } finally {
        DC.Data.Dispose();
        directoryInfo.Delete(recursive: true);
      }

    }


    private static void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private int addData(int idInt, string? idString, string text) {
      var sample = new PropertyNeedsDictionaryClass(idInt, idString, text);
      var sampleString = sample.ToString();
      expectedIdInts.Add(idInt, sampleString);
      if (idString!=null) {
        expectedIdStrings.Add(idString, sampleString);
      }
      assertData();
      return sample.Key;
    }


    private void update(int key, int idInt, string? idString, string text) {
      var sample = DC.Data.PropertyNeedsDictionaryClasses[key];
      expectedIdInts.Remove(sample.IdInt);
      if (sample.IdString!=null) {
        expectedIdStrings.Remove(sample.IdString);
      }
      sample.Update(idInt, idString, text);
      var sampleString = sample.ToString();
      expectedIdInts.Add(idInt, sampleString);
      if (idString!=null) {
        expectedIdStrings.Add(idString, sampleString);
      }
    }


    private void delete(int key) {
      var sample = DC.Data.PropertyNeedsDictionaryClasses[key];
      expectedIdInts.Remove(sample.IdInt);
      if (sample.IdString!=null) {
        expectedIdStrings.Remove(sample.IdString);
      }
      sample.Remove();
    }


    private void assertData() {
      assertDictionaries();
      DC.Data.Dispose();
      new DC(csvConfig);
      assertDictionaries();
    }


    private void assertDictionaries() {
      Assert.AreEqual(expectedIdInts.Count, DC.Data.PropertyNeedsDictionaryClassesByIdInt.Count);
      if (expectedIdInts.Count>0) {
        foreach (var expectedIntKVP in expectedIdInts) {
          Assert.AreEqual(expectedIntKVP.Value, 
            DC.Data.PropertyNeedsDictionaryClassesByIdInt[expectedIntKVP.Key].ToString());
        }
      }
      Assert.AreEqual(expectedIdStrings.Count, DC.Data.PropertyNeedsDictionaryClassesByIdString.Count);
      if (expectedIdStrings.Count>0) {
        foreach (var expectedIntKVP in expectedIdStrings) {
          Assert.AreEqual(expectedIntKVP.Value,
            DC.Data.PropertyNeedsDictionaryClassesByIdString[expectedIntKVP.Key].ToString());
        }
      }
    }
  }
}
