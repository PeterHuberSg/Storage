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


    [TestMethod]
    public void TestGetNearestExtensions() {
      var sortedList = new SortedList<DateTime, itemClass>();
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
      Assert.IsNull(sortedList.GetEqualGreater(now));

      add(sortedList, now, 0);
      assert(sortedList, now, 0);
      assert(sortedList, now1, 0);
      assert(sortedList, now_1, 0);

      add(sortedList, now1, 1);
      assert(sortedList, now, 0);
      assert(sortedList, now1, 1);
      assert(sortedList, now2, 1);
      assert(sortedList, now_1, 0);

      add(sortedList, now_2, -2);
      assert(sortedList, now, 0);
      assert(sortedList, now1, 1);
      assert(sortedList, now2, 1);
      assert(sortedList, now_1, 0);
      assert(sortedList, now_2, -2);
      assert(sortedList, now_3, -2);

      add(sortedList, now4, 4);
      assert(sortedList, now, 0);
      assert(sortedList, now1, 1);
      assert(sortedList, now2, 4);
      assert(sortedList, now3, 4);
      assert(sortedList, now4, 4);
      assert(sortedList, now5, 4);
      assert(sortedList, now_1, 0);
      assert(sortedList, now_2, -2);
      assert(sortedList, now_3, -2);

      add(sortedList, now_6, -6);
      assert(sortedList, now, 0);
      assert(sortedList, now1, 1);
      assert(sortedList, now2, 4);
      assert(sortedList, now3, 4);
      assert(sortedList, now4, 4);
      assert(sortedList, now5, 4);
      assert(sortedList, now_1, 0);
      assert(sortedList, now_2, -2);
      assert(sortedList, now_3, -2);
      assert(sortedList, now_4, -2);
      assert(sortedList, now_5, -2);
      assert(sortedList, now_6, -6);
      assert(sortedList, now_7, -6);
    }


    private void add(SortedList<DateTime, itemClass> sortedList, DateTime key, int value) {
      sortedList.Add(key, new itemClass { Key = key, Value = value });
    }


    private void assert(SortedList<DateTime, itemClass> sortedList, DateTime key, int value) {
      Assert.AreEqual(value, sortedList.GetEqualGreater(key)!.Value);
    }
  }
}
