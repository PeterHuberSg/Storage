using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class FileStreamExtensionsTest {


    [TestMethod]
    public void TestInt() {
      var directoryInfo = new DirectoryInfo("TestCsv");
      try {
        if (directoryInfo.Exists) {
          directoryInfo.Delete(recursive: true);
          directoryInfo.Refresh();
        }

        directoryInfo.Create();
        directoryInfo.Refresh();

        var expectedInts = new List<int>();
        var fileName = directoryInfo.FullName + @"\TestInt.csv";
        var delimiter = (int)';';
        using (var fileStream = new FileStream(fileName, FileMode.Create)) {
          write(fileStream, int.MaxValue, expectedInts, delimiter);
          write(fileStream, 1, expectedInts, delimiter);
          write(fileStream, 0, expectedInts, delimiter);
          write(fileStream, -1, expectedInts, delimiter);
          write(fileStream, -10, expectedInts, delimiter);
          write(fileStream, -111, expectedInts, delimiter);
          write(fileStream, int.MinValue, expectedInts, delimiter);
        }

        using (var fileStream = new FileStream(fileName, FileMode.Open)) {
          using var streamReader = new StreamReader(fileStream);
          var s = streamReader.ReadToEnd();
        }

        var esb = new StringBuilder();
        using (var fileStream = new FileStream(fileName, FileMode.Open)) {
          foreach (var expectedInt in expectedInts) {
            var (IsEof, I)= fileStream.ReadInt(delimiter, "Test", esb);
            Assert.IsFalse(IsEof);
            Assert.AreEqual(0, esb.Length);
            Assert.AreEqual(expectedInt, I);
          }
          Assert.AreEqual(fileStream.Length, fileStream.Position);
          var result1 = fileStream.ReadInt(delimiter, "Test", esb);
          Assert.IsTrue(result1.IsEof);
          Assert.AreEqual(0, esb.Length);
          Assert.AreEqual(result1.I, int.MinValue);
        }

        File.Delete(fileName);
        using (var fileStream = new FileStream(fileName, FileMode.CreateNew)) {
          using var streamWriter = new StreamWriter(fileStream);
          streamWriter.Write("1a2;34");
        }

        using (var fileStream = new FileStream(fileName, FileMode.Open)) {
          var result = fileStream.ReadInt(delimiter, "Test", esb);
          Assert.IsFalse(result.IsEof);
          Assert.AreEqual(int.MinValue, result.I);
          Assert.AreNotEqual(0, esb.Length);
          esb.Length = 0;

          result = fileStream.ReadInt(delimiter, "Test", esb);
          Assert.IsTrue(result.IsEof);
          Assert.AreEqual(int.MinValue, result.I);
          Assert.AreNotEqual(0, esb.Length);
          Assert.AreEqual(fileStream.Length, fileStream.Position);
          esb.Length = 0;

          result = fileStream.ReadInt(delimiter, "Test", esb);
          Assert.IsTrue(result.IsEof);
          Assert.AreEqual(0, esb.Length);
          Assert.AreEqual(result.I, int.MinValue);
        }

        //var charArray = new char[10];
        //var errorSB = new StringBuilder();
        //charArray[0] = '1';
        //var indexRead = 3;
        ////index>length
        //var newI = charArray.ReadInt(ref indexRead, 1, 0, 1, "test", errorSB);
        //Assert.AreEqual(int.MinValue, newI);
        //Assert.IsTrue(errorSB.Length>0);
        //errorSB.Length = 0;
        ////illegal char
        //charArray[0] = ';';
        //charArray[1] = '1';
        //charArray[2] = 'a';
        //charArray[3] = ';';
        //indexRead = 1;
        //newI = charArray.ReadInt(ref indexRead, 3, 0, 4, "test", errorSB);
        //Assert.AreEqual(int.MinValue, newI);
        //Assert.IsTrue(errorSB.Length>0);
        //errorSB.Length = 0;
        //charArray = new char[0];
        //Assert.ThrowsException<ArgumentOutOfRangeException>(() => charArray.ReadInt(ref indexRead, 0, 0, 4, "test", errorSB));

        //var indexWrite = 10;
        //Assert.ThrowsException<IndexOutOfRangeException>(() => charArray.Write(100, ref indexWrite));
      } finally {
        directoryInfo.Delete(recursive: true);
      }
    }


    private void write(FileStream fileStream, int i, List<int> expectedInts, int delimiter) {
      fileStream.Write(i, delimiter);
      expectedInts.Add(i);
    }


    private void assertInt(int i) {
      var charArray = new char[100];
      var indexWrite = 0;
      charArray.Write(i, ref indexWrite);
      var indexRead = 0;
      var errorSB = new StringBuilder();
      var newI = charArray.ReadInt(ref indexRead, indexWrite, 0, indexWrite, "test", errorSB);
      Assert.AreEqual(i, newI);
      Assert.AreEqual(indexWrite, indexRead);
      Assert.AreEqual(0, errorSB.Length);
    }
  }
}
