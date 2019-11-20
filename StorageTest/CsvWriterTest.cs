using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass()]
  public class CsvWriterTest {
    [TestMethod()]
    public void TestCsvReader() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      try {
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        var csvConfig = new CsvConfig(directoryInfo.FullName, reportException: reportException);
        var fileName = csvConfig.DirectoryPath + @"\TestCsvWriterInt.csv";
        using (var csvWriter = new CsvWriter(fileName, csvConfig, 100, isAsciiOnly: true)) {
          csvWriter.Write(int.MaxValue);
          csvWriter.Write(1);
          csvWriter.Write(0);
          csvWriter.Write(-1);
          csvWriter.Write(int.MinValue);
          csvWriter.WriteEndOfLine();

          csvWriter.Write('a');// 
          csvWriter.Write('Ä');// 
          csvWriter.Write('☹');// Smiley with white frowning face
          csvWriter.WriteEndOfLine();

          for (int i = -csvConfig.BufferSize; i < csvConfig.BufferSize; i++) {
            csvWriter.Write(i);
            csvWriter.WriteEndOfLine();
          }
        }

        using var fileStream = new FileStream(fileName, FileMode.Open);
        using var streamReader = new StreamReader(fileStream);
        var line = streamReader.ReadLine();
        var fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual(int.MaxValue, int.Parse(fieldStrings[0]));
        Assert.AreEqual(1, int.Parse(fieldStrings[1]));
        Assert.AreEqual(0, int.Parse(fieldStrings[2]));
        Assert.AreEqual(-1, int.Parse(fieldStrings[3]));
        Assert.AreEqual(int.MinValue, int.Parse(fieldStrings[4]));

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual("a", fieldStrings[0]);
        Assert.AreEqual("Ä", fieldStrings[1]);
        Assert.AreEqual("☹", fieldStrings[2]);

        for (int i = -csvConfig.BufferSize; i < csvConfig.BufferSize; i++) {
          line = streamReader.ReadLine();
          Assert.AreEqual(i.ToString() + '\t', line);
        }
        Assert.IsTrue(fileStream.ReadByte()<0);
      } finally {
        directoryInfo.Delete(recursive: true);
      }
    }


    private void reportException(Exception obj) {
      throw new Exception();
    }
  }
}
