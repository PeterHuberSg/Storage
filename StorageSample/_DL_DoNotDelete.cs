using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageModel {


  public partial class DL {
    /// <summary>
    /// Special static "constructor" for testing purpose. All or no StorageDirectory are set to isCompactDuringDispose. All 
    /// are updatable and deletable.
    /// </summary>
    public static void Init(CsvConfig? csvConfig, bool isCompactDuringDispose) {
      if (data!=null) throw new Exception();

      data = new DL(csvConfig, isCompactDuringDispose);
    }


    /// <summary>
    /// Special "constructor" for testing purpose. All or no StorageDirectory are set to isCompactDuringDispose. All are 
    /// updatable and deletable.
    /// </summary>
    public DL(CsvConfig? csvConfig, bool isCompactDuringDispose) {
      if (!IsDisposed) {
        throw new Exception("Dispose old DL before creating a new one.");
      }
      isDisposed = 0;
      if (csvConfig==null) {
        SampleMasters = new StorageDictionary<SampleMaster, DL>(
          this,
          SampleMaster.SetKey,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false);
        SampleX = new StorageDictionary<Sample, DL>(
          this,
          Sample.SetKey,
          Sample.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        SampleDetails = new StorageDictionary<SampleDetail, DL>(
          this,
          SampleDetail.SetKey,
          SampleDetail.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        LookupParents = new StorageDictionary<LookupParent, DL>(
          this,
          LookupParent.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
        LookupChildren = new StorageDictionary<LookupChild, DL>(
          this,
          LookupChild.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
        ParentsWithDictionary = new StorageDictionary<ParentWithDictionary, DL>(
          this,
          ParentWithDictionary.SetKey,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false);
        DictionaryChildren = new StorageDictionary<DictionaryChild, DL>(
          this,
          DictionaryChild.SetKey,
          DictionaryChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        ParentsWithSortedList = new StorageDictionary<ParentWithSortedList, DL>(
          this,
          ParentWithSortedList.SetKey,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false);
        SortedListChildren = new StorageDictionary<SortedListChild, DL>(
          this,
          SortedListChild.SetKey,
          SortedListChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        ReadOnlyParents = new StorageDictionary<ReadOnlyParent, DL>(
          this,
          ReadOnlyParent.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
        ReadOnlyChildren = new StorageDictionary<ReadOnlyChild, DL>(
          this,
          ReadOnlyChild.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
      } else {
        SampleMasters = new StorageDictionaryCSV<SampleMaster, DL>(
          this,
          csvConfig!,
          SampleMaster.MaxLineLength,
          SampleMaster.Headers,
          SampleMaster.SetKey,
          SampleMaster.Create,
          null,
          SampleMaster.Update,
          SampleMaster.Write,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        SampleX = new StorageDictionaryCSV<Sample, DL>(
          this,
          csvConfig!,
          Sample.MaxLineLength,
          Sample.Headers,
          Sample.SetKey,
          Sample.Create,
          Sample.Verify,
          Sample.Update,
          Sample.Write,
          Sample.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
        SampleDetails = new StorageDictionaryCSV<SampleDetail, DL>(
          this,
          csvConfig!,
          SampleDetail.MaxLineLength,
          SampleDetail.Headers,
          SampleDetail.SetKey,
          SampleDetail.Create,
          SampleDetail.Verify,
          SampleDetail.Update,
          SampleDetail.Write,
          SampleDetail.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
        LookupParents = new StorageDictionaryCSV<LookupParent, DL>(
          this,
          csvConfig!,
          LookupParent.MaxLineLength,
          LookupParent.Headers,
          LookupParent.SetKey,
          LookupParent.Create,
          null,
          null,
          LookupParent.Write,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        LookupChildren = new StorageDictionaryCSV<LookupChild, DL>(
          this,
          csvConfig!,
          LookupChild.MaxLineLength,
          LookupChild.Headers,
          LookupChild.SetKey,
          LookupChild.Create,
          null,
          null,
          LookupChild.Write,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        ParentsWithDictionary = new StorageDictionaryCSV<ParentWithDictionary, DL>(
          this,
          csvConfig!,
          ParentWithDictionary.MaxLineLength,
          ParentWithDictionary.Headers,
          ParentWithDictionary.SetKey,
          ParentWithDictionary.Create,
          null,
          ParentWithDictionary.Update,
          ParentWithDictionary.Write,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        DictionaryChildren = new StorageDictionaryCSV<DictionaryChild, DL>(
          this,
          csvConfig!,
          DictionaryChild.MaxLineLength,
          DictionaryChild.Headers,
          DictionaryChild.SetKey,
          DictionaryChild.Create,
          DictionaryChild.Verify,
          DictionaryChild.Update,
          DictionaryChild.Write,
          DictionaryChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
        ParentsWithSortedList = new StorageDictionaryCSV<ParentWithSortedList, DL>(
          this,
          csvConfig!,
          ParentWithSortedList.MaxLineLength,
          ParentWithSortedList.Headers,
          ParentWithSortedList.SetKey,
          ParentWithSortedList.Create,
          null,
          ParentWithSortedList.Update,
          ParentWithSortedList.Write,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        SortedListChildren = new StorageDictionaryCSV<SortedListChild, DL>(
          this,
          csvConfig!,
          SortedListChild.MaxLineLength,
          SortedListChild.Headers,
          SortedListChild.SetKey,
          SortedListChild.Create,
          SortedListChild.Verify,
          SortedListChild.Update,
          SortedListChild.Write,
          SortedListChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
        ReadOnlyParents = new StorageDictionaryCSV<ReadOnlyParent, DL>(
          this,
          csvConfig!,
          ReadOnlyParent.MaxLineLength,
          ReadOnlyParent.Headers,
          ReadOnlyParent.SetKey,
          ReadOnlyParent.Create,
          null,
          null,
          ReadOnlyParent.Write,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        ReadOnlyChildren = new StorageDictionaryCSV<ReadOnlyChild, DL>(
          this,
          csvConfig!,
          ReadOnlyChild.MaxLineLength,
          ReadOnlyChild.Headers,
          ReadOnlyChild.SetKey,
          ReadOnlyChild.Create,
          ReadOnlyChild.Verify,
          null,
          ReadOnlyChild.Write,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
      }
    }
  }
}
