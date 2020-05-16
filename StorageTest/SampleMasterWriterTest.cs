using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass()]
  public class SampleMasterWriterTest {


    [TestMethod()]
    public void TestSampleMasterWriter() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      try {
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        var csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        var fileName = csvConfig.DirectoryPath + @"\TestSampleMasterWriter.csv";

        using (var sampleMasterWriter = new SampleMasterWriter(fileName, csvConfig, 100)) {
          //will just write a header line
        }
        var headerLine = "Key\tText\tNumberWithDefault\t";
        using (var streamReader = new StreamReader(fileName)) {
          var fileContent = streamReader.ReadToEnd();
          Assert.AreEqual(headerLine + Environment.NewLine, fileContent);
        }

        using (var sampleMasterWriter = new SampleMasterWriter(fileName, csvConfig, 100)) {
          sampleMasterWriter.Write(0, "Master1", 1);
          sampleMasterWriter.Write(1, "Master2");
        }
        using (var streamReader = new StreamReader(fileName)) {
          var fileContent = streamReader.ReadToEnd();
          var lines = fileContent!.Split(Environment.NewLine);
          Assert.AreEqual(4, lines.Length);
          Assert.AreEqual(headerLine, lines[0]);
          Assert.AreEqual("+0\tMaster1\t1\t", lines[1]);
          Assert.AreEqual("+1\tMaster2\t-2147483648\t", lines[2]);
        }

        using (var sampleMasterWriter = new SampleMasterWriter(fileName, csvConfig, 100)) {
          Assert.ThrowsException<Exception>(() => sampleMasterWriter.Write(-1, "Master1", 1));
          sampleMasterWriter.Write(0, "Master1", 1);
          Assert.ThrowsException<Exception>(() => sampleMasterWriter.Write(0, "Master1", 1));
        }

      } finally {
        directoryInfo.Delete(recursive: true);
      }
    }


    private void reportException(Exception obj) {
      throw new Exception();
    }
  }
}
