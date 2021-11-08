using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using System;
using System.Collections.Generic;


namespace StorageTest {


  [TestClass]
  public class SortedBucketCollectionTest {


    public record TestRecord(DateTime Key1Date, int Key2, string Data) {
      public override string ToString() {
        return $"{Key1Date:dd:MM:yy}, {Key2}, {Data}";
      }
    }


    [TestMethod]
    public void TestSortedBucketCollection() {
      var sortedBucketCollection = new SortedBucketCollection<DateTime, int, TestRecord>(tr => tr.Key1Date, tr => tr.Key2);
      var expectedKeys = new SortedDictionary<DateTime, List<int>>();

      var today = DateTime.Now.Date;
      add(sortedBucketCollection, expectedKeys, today, 1);
      add(sortedBucketCollection, expectedKeys, today, 3);
      add(sortedBucketCollection, expectedKeys, today, 2);
      add(sortedBucketCollection, expectedKeys, today, 4);
      add(sortedBucketCollection, expectedKeys, today, 0);

      var tomorrow = today.AddDays(1);
      add(sortedBucketCollection, expectedKeys, tomorrow, 1);
      add(sortedBucketCollection, expectedKeys, tomorrow, 0);

      remove(sortedBucketCollection, expectedKeys, today, 2);
      remove(sortedBucketCollection, expectedKeys, today, 0);
      remove(sortedBucketCollection, expectedKeys, today, 4);
      remove(sortedBucketCollection, expectedKeys, today, 1);
      remove(sortedBucketCollection, expectedKeys, today, 3);

      add(sortedBucketCollection, expectedKeys, today, 1);
      add(sortedBucketCollection, expectedKeys, today, 3);
      add(sortedBucketCollection, expectedKeys, today, 2);

      clear(sortedBucketCollection, expectedKeys);
    }


    private static void add(
      SortedBucketCollection<DateTime, int, TestRecord> sortedBucketCollection,
      SortedDictionary<DateTime, List<int>> expectedKeys,
      DateTime key1,
      int key2) 
    {
      sortedBucketCollection.Add(new TestRecord(key1, key2, key1.ToString("dd.MM.yyyy") + " " + key2));

      if (!expectedKeys.TryGetValue(key1, out var key1Items)) {
        expectedKeys.Add(key1, key1Items = new List<int>());
      } else {
        Assert.IsFalse(key1Items.Contains(key2));
      }
      key1Items.Add(key2);
      key1Items.Sort();

      assert(sortedBucketCollection, expectedKeys);
    }


    private static void remove(
      SortedBucketCollection<DateTime, int, TestRecord> sortedBucketCollection,
      SortedDictionary<DateTime, List<int>> expectedKeys,
      DateTime key1,
      int key2) 
    {
      sortedBucketCollection.Remove(sortedBucketCollection[key1, key2]!);

      var key1Items = expectedKeys[key1];
      key1Items.Remove(key2);
      if (key1Items.Count==0) {
        expectedKeys.Remove(key1);
      }

      assert(sortedBucketCollection, expectedKeys);
    }


    private static void clear(
      SortedBucketCollection<DateTime, int, TestRecord> sortedBucketCollection,
      SortedDictionary<DateTime, List<int>> expectedKeys) 
    {
      sortedBucketCollection.Clear();
      expectedKeys.Clear();

      assert(sortedBucketCollection, expectedKeys);
    }


    private static void assert(SortedBucketCollection<DateTime, int, TestRecord> sortedBucketCollection, 
      SortedDictionary<DateTime, List<int>> expectedKeys) 
    {
      Assert.AreEqual(expectedKeys.Count, sortedBucketCollection.Key1Count);
      var expectedItemsCount = 0;
      var expectedAllKeys = "";
      foreach (var keyValuePairExpectedKey in expectedKeys) {
        var date = keyValuePairExpectedKey.Key;
        Assert.IsTrue(sortedBucketCollection.Contains(date));
        var expectedKey2s = "";
        foreach (var key2 in keyValuePairExpectedKey.Value) {
          Assert.IsTrue(sortedBucketCollection.Contains(date, key2));
          TestRecord testRecord;
          Assert.IsTrue(sortedBucketCollection.TryGetValue(date, key2, out testRecord!));
          Assert.AreEqual(date, testRecord.Key1Date);
          Assert.AreEqual(key2, testRecord.Key2);
          testRecord = sortedBucketCollection[date, key2]!;
          Assert.AreEqual(date, testRecord.Key1Date);
          Assert.AreEqual(key2, testRecord.Key2);
          Assert.IsTrue(sortedBucketCollection.Contains(testRecord));

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

      Assert.AreEqual(expectedItemsCount, sortedBucketCollection.Count);

      var actualAllKeys = "";
      foreach (var testRecord in sortedBucketCollection) {
        actualAllKeys +=  testRecord.Key1Date.ToString("dd.MM.yyyy") + ", " + testRecord.Key2 + "|";
      }
      Assert.AreEqual(expectedAllKeys, actualAllKeys);
    }
  }
}
