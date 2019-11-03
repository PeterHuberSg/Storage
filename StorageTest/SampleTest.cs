using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class SampleTest {

    [TestMethod]
    public void TestSample() {

      DLSet? dLSet = null;
      try {
        dLSet = new DLSet();

        var master0 = newSampleMaster(dLSet, 0, out Sample sample00, out Sample sample01);
        var master1 = newSampleMaster(dLSet, 1, out Sample sample10, out Sample sample11);

      } finally {
        if (dLSet!=null) dLSet.Dispose();

      }
    }


    private object newSampleMaster(DLSet dLSet, int MasterId, out Sample sample0, out Sample sample1) {
      var master = new SampleMaster(MasterId, "Master" + MasterId);
      dLSet.SampleMasters.Add(master);
      sample0 = newSample(dLSet, master, 0);
      sample1 = newSample(dLSet, master, 1);
      return master;
    }


    private Sample newSample(DLSet dLSet, SampleMaster sampleMaster, int sampleId) {
      var newId = 10*sampleMaster.Id + sampleId;
      var sample = new Sample(0, "Sample" + sampleMaster.Id + sampleId, newId, newId, new DateTime(2019, 12, 31).AddDays(-newId));
      sampleMaster.Samples.Add(sample);
      var item0 = new SampleItem(0, "item" + sampleMaster.Id + sampleId + "0");
      sample.Items.Add(item0);
      var item1 = new SampleItem(1, "item" + sampleMaster.Id + sampleId + "1");
      sample.Items.Add(item1);
      return sample;
    }
  }
}
