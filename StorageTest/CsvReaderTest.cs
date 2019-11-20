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

            streamWriter.Write("a" + csvConfig.Delimiter);
            streamWriter.Write("Ä" + csvConfig.Delimiter);
            streamWriter.Write("☹" + csvConfig.Delimiter);
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
          int maxLineLenght = maxLineLenghtIndex==0 ? int.MaxValue : 100;
          using (var csvReader = new CsvReader(fileName, csvConfig, maxLineLenght)) {
            //int
            foreach (var expectedInt in expectedInts) {
              var actualInt = csvReader.ReadInt();
              Assert.AreEqual(expectedInt, actualInt);
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

            //char
            Assert.AreEqual('a', csvReader.ReadChar());
            Assert.AreEqual('Ä', csvReader.ReadChar());
            Assert.AreEqual('☹', csvReader.ReadChar());

            //more than 1 buffer data
            for (int i = -csvConfig.BufferSize; i < csvConfig.BufferSize; i++) {
              var actualInt = csvReader.ReadInt();
              Assert.AreEqual(i, actualInt);
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


  }
}