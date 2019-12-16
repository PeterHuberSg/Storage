using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;
using System.Linq;


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
    bool isCompactDuringDispose = false;


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
          addMaster(3);
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
          //showStructure()
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
        sample = new Sample(
          text: sampleText,
          flag: false,
          number: sampleKey,
          amount: sampleKey,
          preciseDecimal: sampleKey/1000000m,
          sampleState: SampleStateEnum.None,
          dateOnly: DateTime.Now.Date.AddDays(-sampleKey),
          timeOnly: TimeSpan.FromHours(sampleKey),
          dateAndTime: DateTime.Now.Date.AddDays(-sampleKey) + TimeSpan.FromHours(sampleKey),
          oneMaster: null,
          otherMaster: null,
          optional: null,
          isStoring: false); ;
      } else {
        var master =
        sample = new Sample(
          text: sampleText,
          flag: true,
          number: sampleKey,
          amount: sampleKey,
          preciseDecimal: sampleKey/1000000m,
          sampleState: SampleStateEnum.Some,
          dateOnly: DateTime.Now.Date.AddDays(-sampleKey),
          timeOnly: TimeSpan.FromHours(sampleKey),
          dateAndTime: DateTime.Now.Date.AddDays(-sampleKey) + TimeSpan.FromHours(sampleKey),
          oneMaster: DL.Data!.SampleMasters[masterKey.Value], 
          otherMaster: DL.Data!.SampleMasters[masterKey.Value + 1],
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
      sample.Update(
        text: sampleText,
        flag: sample.Flag,
        number: sample.Number,
        amount: sample.Amount,
        preciseDecimal: sample.PreciseDecimal,
        sampleState: sample.SampleState,
        dateOnly: sample.DateOnly,
        timeOnly: sample.TimeOnly,
        dateAndTime: sample.DateAndTime,
        oneMaster: newSampleMaster,
        otherMaster: newSampleMaster,
        optional: sample.Optional);
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
      detail.Update(detailText, detail.Sample);
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
        //assertMaster(dlSample.OneMaster, dlSample);
        //assertMaster(dlSample.OtherMaster, dlSample);
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

      var mastersFromSamples = new Dictionary<int, HashSet<Sample>>();
      foreach (var sample in DL.Data!.Samples.Values) {
        addSampleToMaster(sample, sample.OneMaster, mastersFromSamples);
        addSampleToMaster(sample, sample.OtherMaster, mastersFromSamples);
      }
      foreach (var master in DL.Data!.SampleMasters.Values) {
        if (mastersFromSamples.TryGetValue(master.Key, out var samplesHashSet)) {
          Assert.AreEqual(samplesHashSet.Count, master.Samples.Count);
          foreach (var sample in samplesHashSet) {
            Assert.IsTrue(master.Samples.Contains(sample));
          }
        } else {
          Assert.AreEqual(0, master.Samples.Count);
          //showStructure();
        }

      }
    }


    private string showStructure() {
      var sb = new StringBuilder();
      sb.AppendLine("Samples");
      foreach (var sample in DL.Data!.Samples.Values) {
        sb.AppendLine($"{sample.Key}: {sample.OneMaster?.Key} {sample.OtherMaster?.Key}");
      }
      sb.AppendLine();
      sb.AppendLine("Masters");
      foreach (var master in DL.Data!.SampleMasters.Values) {
        sb.Append(master.Key + ":");
        foreach (var sample in master.Samples) {
          sb.Append(" " + sample.Key);
        }
        sb.AppendLine();
      }
      return sb.ToString();
    }


    private void addSampleToMaster(Sample sample, SampleMaster? master, Dictionary<int, HashSet<Sample>> mastersFromSamples) {
      if (master is null) return;

      if (!mastersFromSamples.TryGetValue(master.Key, out var samplesHashSet)) {
        samplesHashSet = new HashSet<Sample>();
        mastersFromSamples.Add(master.Key, samplesHashSet);
      }
      samplesHashSet.Add(sample);
    }


    //private void assertMaster(SampleMaster? master, Sample dlSample) {
    //  if (master is null) {
    //    var isFound = false;
    //    foreach (var master1 in DL.Data!.SampleMasters) {
    //      foreach (var sample1 in master1.Samples) {
    //        if (sample1.Key==dlSample.Key) {
    //          isFound = true;
    //          break;
    //        }
    //      }
    //      if (isFound) {
    //        break;
    //      }
    //    }
    //    Assert.IsFalse(isFound);
    //  } else {
    //    var isFound = false;
    //    foreach (var sample1 in master.Samples) {
    //      if (sample1.Key==dlSample.Key) {
    //        isFound = true;
    //        break;
    //      }
    //    }
    //    Assert.IsTrue(isFound);
    //  }
    //}
  }
}
