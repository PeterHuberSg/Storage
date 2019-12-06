using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  public static class Storage {

    public const int NoKey = -1;


    //public static int HandleKey(this int key, ref int lastKey, object lockObject) {
    //  lock (lockObject) {
    //    if (key>=0) {
    //      if (lastKey<key) {
    //        lastKey = key;
    //      } else {
    //        throw new Exception();
    //      }
    //    } else {
    //      key = ++lastKey;
    //    }
    //  }
    //  return key;
    //}


    //public static int VerifyKey(this int key, ref int lastKey) {
    //  if (lastKey<key) {
    //    lastKey = key;
    //    return key;
    //  } else {
    //    throw new Exception();
    //  }
    //}


    public static string ToKeyString(this int key) {
      if (key==NoKey) return "no";
      return key.ToString();
    }

  }
}
