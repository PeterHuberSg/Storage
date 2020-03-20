using System;
using System.Collections.Generic;
using System.Text;
using Storage;


namespace StorageModel {


  public partial class DL {
    /// <summary>
    /// Special static "constructor" for testing purpose. All or no StorageDirectory are set to isCompactDuringDispose. All are udateable and deletable.
    /// </summary>
    public static void Init(CsvConfig? csvConfig, bool isCompactDuringDispose) {
      if (data!=null) throw new Exception();

      data = new DL(csvConfig, isCompactDuringDispose);
    }


    /// <summary>
    /// Special "constructor" for testing purpose. All or no StorageDirectory are set to isCompactDuringDispose. All are udateable and deletable.
    /// </summary>
    public DL(CsvConfig? csvConfig, bool isCompactDuringDispose) {
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
        Minimals = new StorageDictionary<Minimal, DL>(
          this,
          Minimal.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
        MinimalRefs = new StorageDictionary<MinimalRef, DL>(
          this,
          MinimalRef.SetKey,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
        ParentDictionarys = new StorageDictionary<ParentDictionary, DL>(
          this,
          ParentDictionary.SetKey,
          null,
          areInstancesUpdatable: true,
          areInstancesDeletable: false);
        DictionaryChildren = new StorageDictionary<DictionaryChild, DL>(
          this,
          DictionaryChild.SetKey,
          DictionaryChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
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
        Minimals = new StorageDictionaryCSV<Minimal, DL>(
          this,
          csvConfig!,
          Minimal.MaxLineLength,
          Minimal.Headers,
          Minimal.SetKey,
          Minimal.Create,
          null,
          null,
          Minimal.Write,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        MinimalRefs = new StorageDictionaryCSV<MinimalRef, DL>(
          this,
          csvConfig!,
          MinimalRef.MaxLineLength,
          MinimalRef.Headers,
          MinimalRef.SetKey,
          MinimalRef.Create,
          null,
          null,
          MinimalRef.Write,
          null,
          areInstancesUpdatable: false,
          areInstancesDeletable: false,
          isCompactDuringDispose: isCompactDuringDispose);
        ParentDictionarys = new StorageDictionaryCSV<ParentDictionary, DL>(
          this,
          csvConfig!,
          ParentDictionary.MaxLineLength,
          ParentDictionary.Headers,
          ParentDictionary.SetKey,
          ParentDictionary.Create,
          null,
          ParentDictionary.Update,
          ParentDictionary.Write,
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
      }
    }
  }
}
