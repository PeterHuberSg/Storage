using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using StorageModel;


namespace StorageTest {


  [TestClass]
  public class GetNearestExtensionsTest {

    class itemClass {
      public DateTime Key;
      public int Value;
    }

    readonly SortedList<DateTime, itemClass> sortedListClass = new SortedList<DateTime, itemClass>();
    readonly SortedList<DateTime, int> sortedListStruct = new SortedList<DateTime, int>();


    [TestMethod]
    public void TestGetNearestExtensions() {
      var now = DateTime.Now.Date;
      var now1 = DateTime.Now.Date.AddDays(1);
      var now2 = DateTime.Now.Date.AddDays(2);
      var now3 = DateTime.Now.Date.AddDays(3);
      var now4 = DateTime.Now.Date.AddDays(4);
      var now5 = DateTime.Now.Date.AddDays(5);
      var now_1 = DateTime.Now.Date.AddDays(-1);
      var now_2 = DateTime.Now.Date.AddDays(-2);
      var now_3 = DateTime.Now.Date.AddDays(-3);
      var now_4 = DateTime.Now.Date.AddDays(-4);
      var now_5 = DateTime.Now.Date.AddDays(-5);
      var now_6 = DateTime.Now.Date.AddDays(-6);
      var now_7 = DateTime.Now.Date.AddDays(-7);

      Assert.IsNull(sortedListClass.GetEqualGreater(now));
      Assert.ThrowsException<Exception>(() => sortedListStruct.GetEqualGreater(now));

      add(now, 0);
      assert(now, 0);
      assert(now1, 0);
      assert(now_1, 0);

      add(now1, 1);
      assert(now, 0);
      assert(now1, 1);
      assert(now2, 1);
      assert(now_1, 0);

      add(now_2, -2);
      assert(now, 0);
      assert(now1, 1);
      assert(now2, 1);
      assert(now_1, 0);
      assert(now_2, -2);
      assert(now_3, -2);

      add(now4, 4);
      assert(now, 0);
      assert(now1, 1);
      assert(now2, 4);
      assert(now3, 4);
      assert(now4, 4);
      assert(now5, 4);
      assert(now_1, 0);
      assert(now_2, -2);
      assert(now_3, -2);

      add(now_6, -6);
      assert(now, 0);
      assert(now1, 1);
      assert(now2, 4);
      assert(now3, 4);
      assert(now4, 4);
      assert(now5, 4);
      assert(now_1, 0);
      assert(now_2, -2);
      assert(now_3, -2);
      assert(now_4, -2);
      assert(now_5, -2);
      assert(now_6, -6);
      assert(now_7, -6);
    }


    private void add(DateTime key, int value) {
      sortedListClass.Add(key, new itemClass { Key = key, Value = value });
      sortedListStruct.Add(key, value);
    }


    private void assert(DateTime key, int value) {
      Assert.AreEqual(value, sortedListClass.GetEqualGreater(key)!.Value);
      Assert.AreEqual(value, sortedListStruct.GetEqualGreater(key));
    }
  }
}
