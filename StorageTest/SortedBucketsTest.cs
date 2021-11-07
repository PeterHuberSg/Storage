using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using System;
using System.Collections.Generic;


namespace StorageTest {


  [TestClass]
  public class SortedBucketsTest {


    public record TestRecord(DateTime Key1Date, int Key2, string Data) {
      public override string ToString() {
        return $"{Key1Date:dd:MM:yy}, {Key2}, {Data}";
      }
    }


    [TestMethod]
    public void TestSortedBuckets() {
      var sortedBuckets = new SortedBuckets<DateTime, int, TestRecord>(tr => tr.Key1Date, tr => tr.Key2);
      var expectedKeys = new SortedDictionary<DateTime, List<int>>();

      var today = DateTime.Now.Date;
      add(sortedBuckets, expectedKeys, today, 1);
      add(sortedBuckets, expectedKeys, today, 3);
      add(sortedBuckets, expectedKeys, today, 2);
      add(sortedBuckets, expectedKeys, today, 4);
      add(sortedBuckets, expectedKeys, today, 0);

      var tomorrow = today.AddDays(1);
      add(sortedBuckets, expectedKeys, tomorrow, 1);
      add(sortedBuckets, expectedKeys, tomorrow, 0);

      remove(sortedBuckets, expectedKeys, today, 2);
      remove(sortedBuckets, expectedKeys, today, 0);
      remove(sortedBuckets, expectedKeys, today, 4);
      remove(sortedBuckets, expectedKeys, today, 1);
      remove(sortedBuckets, expectedKeys, today, 3);

      add(sortedBuckets, expectedKeys, today, 1);
      add(sortedBuckets, expectedKeys, today, 3);
      add(sortedBuckets, expectedKeys, today, 2);

      clear(sortedBuckets, expectedKeys);
    }


    private static void add(
      SortedBuckets<DateTime, int, TestRecord> sortedBuckets,
      SortedDictionary<DateTime, List<int>> expectedKeys,
      DateTime key1,
      int key2) 
    {
      sortedBuckets.Add(new TestRecord(key1, key2, key1.ToString("dd.MM.yyyy") + " " + key2));

      if (!expectedKeys.TryGetValue(key1, out var key1Items)) {
        expectedKeys.Add(key1, key1Items = new List<int>());
      } else {
        Assert.IsFalse(key1Items.Contains(key2));
      }
      key1Items.Add(key2);
      key1Items.Sort();

      assert(sortedBuckets, expectedKeys);
    }


    private static void remove(
      SortedBuckets<DateTime, int, TestRecord> sortedBuckets,
      SortedDictionary<DateTime, List<int>> expectedKeys,
      DateTime key1,
      int key2) 
    {
      sortedBuckets.Remove(sortedBuckets[key1, key2]!);

      var key1Items = expectedKeys[key1];
      key1Items.Remove(key2);
      if (key1Items.Count==0) {
        expectedKeys.Remove(key1);
      }

      assert(sortedBuckets, expectedKeys);
    }


    private static void clear(
      SortedBuckets<DateTime, int, TestRecord> sortedBuckets,
      SortedDictionary<DateTime, List<int>> expectedKeys) 
    {
      sortedBuckets.Clear();
      expectedKeys.Clear();

      assert(sortedBuckets, expectedKeys);
    }


    private static void assert(SortedBuckets<DateTime, int, TestRecord> sortedBuckets, 
      SortedDictionary<DateTime, List<int>> expectedKeys) 
    {
      Assert.AreEqual(expectedKeys.Count, sortedBuckets.Key1Count);
      var expectedItemsCount = 0;
      var expectedAllKeys = "";
      foreach (var keyValuePairExpectedKey in expectedKeys) {
        var date = keyValuePairExpectedKey.Key;
        Assert.IsTrue(sortedBuckets.Contains(date));
        var expectedKey2s = "";
        foreach (var key2 in keyValuePairExpectedKey.Value) {
          Assert.IsTrue(sortedBuckets.Contains(date, key2));
          TestRecord testRecord;
          Assert.IsTrue(sortedBuckets.TryGetValue(date, key2, out testRecord!));
          Assert.AreEqual(date, testRecord.Key1Date);
          Assert.AreEqual(key2, testRecord.Key2);
          testRecord = sortedBuckets[date, key2]!;
          Assert.AreEqual(date, testRecord.Key1Date);
          Assert.AreEqual(key2, testRecord.Key2);
          Assert.IsTrue(sortedBuckets.Contains(testRecord));

          expectedAllKeys +=  date.ToString("dd.MM.yyyy") + ", " + key2 + "|";
          expectedKey2s += key2 + "|";
          expectedItemsCount++;
        }
        var actualKey2s = "";
        foreach (var key2 in keyValuePairExpectedKey.Value) {
          actualKey2s += key2 + "|";
        }

        Assert.AreEqual(expectedKey2s, actualKey2s);
      }

      Assert.AreEqual(expectedItemsCount, sortedBuckets.Count);

      var actualAllKeys = "";
      foreach (var testRecord in sortedBuckets) {
        actualAllKeys +=  testRecord.Key1Date.ToString("dd.MM.yyyy") + ", " + testRecord.Key2 + "|";
      }
      Assert.AreEqual(expectedAllKeys, actualAllKeys);
    }
  }
}
