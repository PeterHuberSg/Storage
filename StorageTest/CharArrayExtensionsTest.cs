using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;


namespace StorageTest {


  [TestClass]
  public class CharArrayExtensionsTest {


    [TestMethod]
    public void TestInt() {
      var a = new char[10];
      a[0]='a';
      var b = a[0..1];
      var s = a.ToString() + b.ToString();
      assertInt(int.MaxValue);
      assertInt(1);
      assertInt(0);
      assertInt(-1);
      assertInt(-12);
      assertInt(-123);
      assertInt(int.MinValue+1);

      var charArray = new char[10];
      var errorSB = new StringBuilder();
      charArray[0] = '1';
      var indexRead = 3;
      //index>length
      var newI = charArray.ReadInt(ref indexRead, 1, 0, 1, "test", errorSB);
      Assert.AreEqual(int.MinValue, newI);
      Assert.IsTrue(errorSB.Length>0);
      errorSB.Length = 0;
      //illegal char
      charArray[0] = ';';
      charArray[1] = '1';
      charArray[2] = 'a';
      charArray[3] = ';';
      indexRead = 1;
      newI = charArray.ReadInt(ref indexRead, 3, 0, 4, "test", errorSB);
      Assert.AreEqual(int.MinValue, newI);
      Assert.IsTrue(errorSB.Length>0);
      errorSB.Length = 0;
      charArray = new char[0];
      Assert.ThrowsException<ArgumentOutOfRangeException>(() => charArray.ReadInt(ref indexRead, 0, 0, 4, "test", errorSB));

      var indexWrite = 10;
      Assert.ThrowsException<IndexOutOfRangeException>(() => charArray.Write(100, ref indexWrite));
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
