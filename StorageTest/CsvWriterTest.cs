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
        using (var csvWriter = new CsvWriter(fileName, csvConfig, 250, isAsciiOnly: true)) {
          csvWriter.Write(int.MaxValue);
          csvWriter.Write(1);
          csvWriter.Write(0);
          csvWriter.Write(-1);
          csvWriter.Write(int.MinValue);
          csvWriter.WriteEndOfLine();

          csvWriter.Write((int?)null);
          csvWriter.Write((int?)int.MaxValue);
          csvWriter.Write((int?)1);
          csvWriter.Write((int?)0);
          csvWriter.Write((int?)-1);
          csvWriter.Write((int?)int.MinValue);
          csvWriter.WriteEndOfLine();

          csvWriter.Write(long.MaxValue);
          csvWriter.Write(1L);
          csvWriter.Write(0L);
          csvWriter.Write(-1L);
          csvWriter.Write(long.MinValue);
          csvWriter.WriteEndOfLine();

          csvWriter.Write(decimal.MaxValue, 0);
          csvWriter.Write(decimal.MaxValue);
          csvWriter.Write(1234567890.12345678m, 8);
          csvWriter.Write(1.1m, 1);
          csvWriter.Write(1.1m, 0);
          csvWriter.Write(1m, 1);
          csvWriter.Write(decimal.One);
          csvWriter.Write(0.9m, 1);
          csvWriter.Write(0.9m, 0);
          csvWriter.Write(0.4m, 0);
          csvWriter.Write(decimal.Zero);
          csvWriter.Write(-0.4m, 0);
          csvWriter.Write(-0.9m, 0);
          csvWriter.Write(-0.9m, 3);
          csvWriter.Write(decimal.MinusOne);
          csvWriter.Write(-1m, 1);
          csvWriter.Write(-1.1m, 0);
          csvWriter.Write(-1.1m, 1);
          csvWriter.Write(-1234567890.12345678m, 8);
          csvWriter.Write(decimal.MinValue);
          csvWriter.Write(decimal.MinValue, 0);
          csvWriter.WriteEndOfLine();

          csvWriter.Write('a');// 
          csvWriter.Write('Ä');// 
          csvWriter.Write('☹');// Smiley with white frowning face
          csvWriter.WriteEndOfLine();

          csvWriter.Write((string?)null);
          csvWriter.Write("");
          csvWriter.Write("abc");
          csvWriter.Write("Ä");
          csvWriter.Write("aÄ☹");
          csvWriter.WriteEndOfLine();

          csvWriter.WriteDate(DateTime.MaxValue.Date);
          csvWriter.WriteDate(DateTime.MinValue.Date);
          csvWriter.WriteDate(new DateTime(2000, 1, 1));
          csvWriter.WriteDate(new DateTime(2009, 12, 31));
          csvWriter.WriteDate(new DateTime(2010, 1, 1));
          csvWriter.WriteDate(new DateTime(2019, 12, 31));
          csvWriter.WriteDate(new DateTime(2020, 1, 1));
          csvWriter.WriteDate(new DateTime(2029, 12, 31));
          csvWriter.WriteDate(new DateTime(2020, 1, 1));
          csvWriter.WriteDate(new DateTime(2120, 1, 1));
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
        Assert.AreEqual("", fieldStrings[0]);
        Assert.AreEqual(int.MaxValue, int.Parse(fieldStrings[1]));
        Assert.AreEqual(1, int.Parse(fieldStrings[2]));
        Assert.AreEqual(0, int.Parse(fieldStrings[3]));
        Assert.AreEqual(-1, int.Parse(fieldStrings[4]));
        Assert.AreEqual(int.MinValue, int.Parse(fieldStrings[5]));

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual(long.MaxValue, long.Parse(fieldStrings[0]));
        Assert.AreEqual(1L, long.Parse(fieldStrings[1]));
        Assert.AreEqual(0L, long.Parse(fieldStrings[2]));
        Assert.AreEqual(-1L, long.Parse(fieldStrings[3]));
        Assert.AreEqual(long.MinValue, long.Parse(fieldStrings[4]));

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        var fieldIndex = 0;
        //csvWriter.Write(decimal.MaxValue, 0);
        Assert.AreEqual(decimal.MaxValue, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.MaxValue);
        Assert.AreEqual(decimal.MaxValue, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(1234567890.12345678m, 8);
        Assert.AreEqual(1234567890.12345678m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(1.1m, 1);
        Assert.AreEqual(1.1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(1.1m, 0);
        Assert.AreEqual(1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(1m, 1);
        Assert.AreEqual(1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.One);
        Assert.AreEqual(1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(0.9m, 1);
        Assert.AreEqual(0.9m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(0.9m, 0);
        Assert.AreEqual(1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(0.4m, 0);
        Assert.AreEqual(0m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.Zero);
        Assert.AreEqual(0m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-0.4m, 0);
        Assert.AreEqual(0m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-0.9m, 0);
        Assert.AreEqual(-1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-0.9m, 3);
        Assert.AreEqual(-0.9m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.MinusOne);
        Assert.AreEqual(-1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-1m, 1);
        Assert.AreEqual(-1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-1.1m, 0);
        Assert.AreEqual(-1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-1.1m, 1);
        Assert.AreEqual(-1.1m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(-1234567890.12345678m, 8);
        Assert.AreEqual(-1234567890.12345678m, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.MinValue);
        Assert.AreEqual(decimal.MinValue, decimal.Parse(fieldStrings[fieldIndex++]));
        //csvWriter.Write(decimal.MinValue, 1);
        Assert.AreEqual(decimal.MinValue, decimal.Parse(fieldStrings[fieldIndex++]));

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual("a", fieldStrings[0]);
        Assert.AreEqual("Ä", fieldStrings[1]);
        Assert.AreEqual("☹", fieldStrings[2]);

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual("", fieldStrings[0]);
        Assert.AreEqual("", fieldStrings[1]);
        Assert.AreEqual("abc", fieldStrings[2]);
        Assert.AreEqual("Ä", fieldStrings[3]);
        Assert.AreEqual("aÄ☹", fieldStrings[4]);

        line = streamReader.ReadLine();
        fieldStrings = line!.Split(csvConfig.Delimiter);
        Assert.AreEqual("31.12.9999", fieldStrings[0]);
        Assert.AreEqual("1.1.0001", fieldStrings[1]);
        Assert.AreEqual("1.1.2000", fieldStrings[2]);
        Assert.AreEqual("31.12.2009", fieldStrings[3]);
        Assert.AreEqual("1.1.2010", fieldStrings[4]);
        Assert.AreEqual("31.12.2019", fieldStrings[5]);
        Assert.AreEqual("1.1.2020", fieldStrings[6]);
        Assert.AreEqual("31.12.2029", fieldStrings[7]);
        Assert.AreEqual("1.1.2020", fieldStrings[8]);
        Assert.AreEqual("1.1.2120", fieldStrings[9]);

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
