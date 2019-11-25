using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass()]
  public class CsvReaderTest {


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
        var fileName = csvConfig.DirectoryPath + @"\TestCsvReaderInt.csv";

        var expectedInts = new List<int>();
        var expectedNullInts = new List<int?>();
        var expectedLongs = new List<long>();
        var expectedDecimals = new List<decimal>();
        using (var fileStream = new FileStream(fileName, FileMode.Create)) {
          using (var streamWriter = new StreamWriter(fileStream)) {
            writeInt(streamWriter, int.MaxValue, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, 1, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, 0, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, -1, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, -10, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, -111, expectedInts, csvConfig.Delimiter);
            writeInt(streamWriter, int.MinValue, expectedInts, csvConfig.Delimiter);
            streamWriter.WriteLine();

            writeIntNull(streamWriter, null, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, int.MaxValue, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, 1, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, 0, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, -1, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, -10, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, -111, expectedNullInts, csvConfig.Delimiter);
            writeIntNull(streamWriter, int.MinValue, expectedNullInts, csvConfig.Delimiter);
            streamWriter.WriteLine();

            writeLong(streamWriter, long.MaxValue, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, 1, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, 0, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, -1, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, -10, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, -111, expectedLongs, csvConfig.Delimiter);
            writeLong(streamWriter, long.MinValue, expectedLongs, csvConfig.Delimiter);
            streamWriter.WriteLine();

            writeDecimal(streamWriter, decimal.MaxValue, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, 1234567890.1234567890m, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, 1234567890.12m, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, decimal.One , expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, decimal.Zero, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, decimal.MinusOne, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, -1234567890.12m, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, -1234567890.1234567890m, expectedDecimals, csvConfig.Delimiter);
            writeDecimal(streamWriter, decimal.MinValue, expectedDecimals, csvConfig.Delimiter);
            streamWriter.WriteLine();

            streamWriter.Write("a" + csvConfig.Delimiter);
            streamWriter.Write("Ä" + csvConfig.Delimiter);
            streamWriter.Write("☹" + csvConfig.Delimiter);
            streamWriter.WriteLine();

            streamWriter.Write(csvConfig.Delimiter);
            streamWriter.Write("a" + csvConfig.Delimiter);
            streamWriter.Write("abc" + csvConfig.Delimiter);
            streamWriter.Write("Ä" + csvConfig.Delimiter);
            streamWriter.Write("aÄ" + csvConfig.Delimiter);
            streamWriter.Write("abcÄ ☹de" + csvConfig.Delimiter);
            streamWriter.WriteLine();

            streamWriter.Write("31.12.9999" + csvConfig.Delimiter);
            streamWriter.Write("1.1.0001" + csvConfig.Delimiter);
            streamWriter.WriteLine();

            for (int i = -csvConfig.BufferSize; i < csvConfig.BufferSize; i++) {
              streamWriter.WriteLine(i.ToString() + csvConfig.Delimiter);
            }
          }
        }

        string s;
        using (var fileStream = new FileStream(fileName, FileMode.Open)) {
          using var streamReader = new StreamReader(fileStream);
          s = streamReader.ReadToEnd();
        }

        for (int maxLineLenghtIndex = 0; maxLineLenghtIndex < 2; maxLineLenghtIndex++) {
          int maxLineLenght = maxLineLenghtIndex==0 ? int.MaxValue : 150;
          using (var csvReader = new CsvReader(fileName, csvConfig, maxLineLenght)) {
            //int
            foreach (var expectedInt in expectedInts) {
              Assert.AreEqual(expectedInt, csvReader.ReadInt());
              Assert.IsFalse(csvReader.IsEndOfFileReached());
              Assert.IsFalse(csvReader.IsEof);
            }
            csvReader.ReadEndOfLine();
            Assert.IsFalse(csvReader.IsEof);

            //int?
            foreach (var expectedInt in expectedNullInts) {
              var actualInt = csvReader.ReadIntNull();
              if (expectedInt.HasValue) {
                Assert.AreEqual(expectedInt, actualInt);
              } else {
                Assert.IsNull(actualInt);
              }
              Assert.IsFalse(csvReader.IsEndOfFileReached());
              Assert.IsFalse(csvReader.IsEof);
            }
            csvReader.ReadEndOfLine();
            Assert.IsFalse(csvReader.IsEof);

            //long
            foreach (var expectedLong in expectedLongs) {
              Assert.AreEqual(expectedLong, csvReader.ReadLong());
              Assert.IsFalse(csvReader.IsEndOfFileReached());
              Assert.IsFalse(csvReader.IsEof);
            }
            csvReader.ReadEndOfLine();
            Assert.IsFalse(csvReader.IsEof);

            //decimal
            foreach (var expectedDecimal in expectedDecimals) {
              Assert.AreEqual(expectedDecimal, csvReader.ReadDecimal());
              Assert.IsFalse(csvReader.IsEndOfFileReached());
              Assert.IsFalse(csvReader.IsEof);
            }
            csvReader.ReadEndOfLine();
            Assert.IsFalse(csvReader.IsEof);

            //char
            Assert.AreEqual('a', csvReader.ReadChar());
            Assert.AreEqual('Ä', csvReader.ReadChar());
            Assert.AreEqual('☹', csvReader.ReadChar());
            csvReader.ReadEndOfLine();
            Assert.IsFalse(csvReader.IsEof);

            //string?
            Assert.IsNull(csvReader.ReadString());
            Assert.AreEqual("a", csvReader.ReadString());
            Assert.AreEqual("abc", csvReader.ReadString());
            Assert.AreEqual("Ä", csvReader.ReadString());
            Assert.AreEqual("aÄ", csvReader.ReadString());
            Assert.AreEqual("abcÄ ☹de", csvReader.ReadString());
            csvReader.ReadEndOfLine();

            Assert.AreEqual(DateTime.MaxValue.Date, csvReader.ReadDate());
            Assert.AreEqual(DateTime.MinValue.Date, csvReader.ReadDate());
            csvReader.ReadEndOfLine();

            //more than 1 buffer data
            for (int i = -csvConfig.BufferSize; i < csvConfig.BufferSize; i++) {
              Assert.AreEqual(i, csvReader.ReadInt());
              Assert.IsFalse(csvReader.IsEndOfFileReached());
              Assert.IsFalse(csvReader.IsEof);
              csvReader.ReadEndOfLine();
              if (i < csvConfig.BufferSize-1) {
                Assert.IsFalse(csvReader.IsEndOfFileReached());
                Assert.IsFalse(csvReader.IsEof);
              } else {
                Assert.IsTrue(csvReader.IsEndOfFileReached());
                Assert.IsTrue(csvReader.IsEof);
              }
            }
          }
        }
      } finally {
        directoryInfo.Delete(recursive: true);
      }
    }


    private void reportException(Exception obj) {
      throw new NotImplementedException();
    }


    private void writeInt(StreamWriter streamWriter, int i, List<int> expectedInts, char delimiter) {
      streamWriter.Write(i.ToString());
      streamWriter.Write(delimiter);
      expectedInts.Add(i);
    }


    private void writeIntNull(StreamWriter streamWriter, int? i, List<int?> expectedInts, char delimiter) {
      if (i.HasValue) {
        streamWriter.Write(i.ToString());
      }
      streamWriter.Write(delimiter);
      expectedInts.Add(i);
    }


    private void writeLong(StreamWriter streamWriter, long l, List<long> expectedLongs, char delimiter) {
      streamWriter.Write(l.ToString());
      streamWriter.Write(delimiter);
      expectedLongs.Add(l);
    }


    private void writeDecimal(StreamWriter streamWriter, decimal d, List<decimal> expectedDecimalss, char delimiter) {
      streamWriter.Write(d.ToString());
      streamWriter.Write(delimiter);
      expectedDecimalss.Add(d);
    }


  }
}