using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class SampleTest {


    class expectedDataClass {
      public readonly Dictionary<int, string> Masters = new Dictionary<int, string>();
      public readonly Dictionary<int, string> Samples = new Dictionary<int, string>();
      public readonly Dictionary<int, string> Details = new Dictionary<int, string>();
    }
    expectedDataClass? expectedData;


    CsvConfig? csvConfig;
    bool isCompactDuringDispose;


    [TestMethod]
    public void TestSample() {
      var directoryInfo = new DirectoryInfo("TestCsv");

      for (int configurationIndex = 0; configurationIndex < 3; configurationIndex++) {
        switch (configurationIndex) {
        case 0: csvConfig = null; isCompactDuringDispose = false; break;
        case 1: csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException); break;
        case 2: isCompactDuringDispose = true; break;
        }
        try {
          directoryInfo.Refresh();
          if (directoryInfo.Exists) {
            directoryInfo.Delete(recursive: true);
            directoryInfo.Refresh();
          }

          directoryInfo.Create();

          expectedData = new expectedDataClass();
          initDL();
          assertDL();

          addMaster(0);
          addMaster(1);
          addMaster(2);
          addSample(0, null);
          addSample(1, 1);
          addSample(2, 2);
          addDetail(0, 1);
          addDetail(1, 1);
          addDetail(2, 2);

          updateMaster(1, "a");
          updateSample(1, "b", DL.Data!.SampleMasters[0]);
          updateSample(1, "c", null);
          updateSample(1, "d", DL.Data!.SampleMasters[1]);
          updateDetail(1, "a");

          removeDetail(2);
          removeSample(2);
          removeMaster(2);
          removeMaster(1);
        } finally {
          DL.DisposeData();
        }
      }
    }


    private void initDL() {
      DL.Init(csvConfig, isCompactDuringDispose);
    }


    private void reportException(Exception obj) {
      Console.WriteLine(obj);
      Assert.Fail();
    }


    private void addMaster(int key) {
      var masterText = "Master" + key;
      expectedData!.Masters.Add(key, masterText);
      var master = new SampleMaster(masterText, isStoring: false);
      master.Store();
      assertData();
    }


    private void updateMaster(int masterKey, string text) {
      var masterText = "Master" + masterKey + text;
      expectedData!.Masters.Remove(masterKey);
      expectedData!.Masters.Add(masterKey, masterText);
      var master = DL.Data!.SampleMasters[masterKey];
      master.Update(masterText);
      foreach (var sample in master.Samples) {
        expectedData!.Samples.Remove(sample.Key);
        expectedData!.Samples.Add(sample.Key, sample.ToString());
      }
      assertData();
    }


    private void removeMaster(int masterKey) {
      expectedData!.Masters.Remove(masterKey);
      var master = DL.Data!.SampleMasters[masterKey];
      master.Remove();
      foreach (var sample in master.Samples) {
        expectedData!.Samples.Remove(sample.Key);
        expectedData!.Samples.Add(sample.Key, sample.ToString());
      }
      assertData();
    }


    private void addSample(int sampleKey, int? masterKey) {
      var sampleText = "Sample" + sampleKey;
      Sample sample;
      if (masterKey is null) {
        sample = new Sample(sampleText, sampleKey, sampleKey, DateTime.Now.Date.AddDays(-sampleKey), isStoring: false);
      } else {
        var master =
        sample = new Sample(
          sampleText, 
          sampleKey, 
          sampleKey, 
          DateTime.Now.Date.AddDays(-sampleKey), 
          DL.Data!.SampleMasters[masterKey.Value], 
          "option" + sampleKey,
          isStoring: false);
      }
      sample.Store();
      expectedData!.Samples.Add(sampleKey, sample.ToString());
      assertData();
    }


    private void updateSample(int sampleKey, string text, SampleMaster? newSampleMaster) {
      var sampleText = "Sample" + sampleKey + text;
      Sample sample = DL.Data!.Samples[sampleKey];
      sample.Update(sampleText, sample.Number, sample.Amount, sample.Date, newSampleMaster, sample.Optional);
      expectedData!.Samples.Remove(sampleKey);
      expectedData!.Samples.Add(sampleKey, sample.ToString());
      assertData();
    }


    private void removeSample(int sampleKey) {
      expectedData!.Samples.Remove(sampleKey);
      var sample = DL.Data!.Samples[sampleKey];
      sample.Remove();
      assertData();
    }


    private void addDetail(int detailKey, int sampleKey) {
      var detailText = "Detail" + sampleKey + '.' + detailKey;
      expectedData!.Details.Add(detailKey, detailText);
      var sample = DL.Data!.Samples[sampleKey];
      var detail = new SampleDetail(detailText, sample, false);
      detail.Store();
      expectedData!.Samples.Remove(sampleKey);
      expectedData!.Samples.Add(sampleKey, sample.ToString());
      assertData();
    }


    private void updateDetail(int key, string text) {
      var detailText = "Detail" + key + text;
      expectedData!.Details.Remove(key);
      expectedData!.Details.Add(key, detailText);
      var detail = DL.Data!.SampleDetails[key];
      detail.Update(detailText);
      assertData();
    }


    private void removeDetail(int detailKey) {
      expectedData!.Details.Remove(detailKey);
      var detail = DL.Data!.SampleDetails[detailKey];
      detail.Remove();
      var sampleKey = detail.Sample.Key;
      expectedData!.Samples.Remove(sampleKey);
      expectedData!.Samples.Add(sampleKey, detail.Sample.ToString());
      assertData();
    }


    private void assertData() {
      assertDL();

      if (csvConfig is null) return;

      DL.DisposeData();
      initDL();
      assertDL();
    }


    private void assertDL() {
      Assert.AreEqual(expectedData!.Masters.Count, DL.Data!.SampleMasters.Count);
      foreach (var master in expectedData!.Masters) {
        var dlMaster = DL.Data!.SampleMasters[master.Key];
        Assert.AreEqual(master.Value, dlMaster.Text);
      }

      Assert.AreEqual(expectedData!.Samples.Count, DL.Data!.Samples.Count);
      foreach (var sample in expectedData!.Samples) {
        var dlSample = DL.Data!.Samples[sample.Key];
        Assert.AreEqual(sample.Value, dlSample.ToString());
        if (dlSample.SampleMaster is null) {
          var isFound = false;
          foreach (var master1 in DL.Data!.SampleMasters) {
            foreach (var sample1 in master1.Samples) {
              if (sample1.Key==dlSample.Key) {
                isFound = true;
                break;
              }
            }
            if (isFound) {
              break;
            }
          }
          Assert.IsFalse(isFound);
        } else {
          var isFound = false;
          foreach (var sample1 in dlSample.SampleMaster.Samples) {
            if (sample1.Key==dlSample.Key) {
              isFound = true;
              break;
            }
          }
          Assert.IsTrue(isFound);
        }
      }

      Assert.AreEqual(expectedData!.Details.Count, DL.Data!.SampleDetails.Count);
      foreach (var detail in expectedData!.Details) {
        var dlDetail = DL.Data!.SampleDetails[detail.Key];
        Assert.AreEqual(detail.Value, dlDetail.Text);
        var isFound = false;
        foreach (var sampleDetail in dlDetail.Sample.SampleDetails) {
          if (sampleDetail.Key==dlDetail.Key) {
            isFound = true;
            break;
          }
        }
        Assert.IsTrue(isFound);
      }
    }
  }
}
