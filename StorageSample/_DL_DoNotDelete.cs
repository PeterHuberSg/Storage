﻿using System;
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
          SampleMaster.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true);
        SampleX = new StorageDictionary<Sample, DL>(
          this,
          Sample.SetKey,
          Sample.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true);
        SampleDetails = new StorageDictionary<SampleDetail, DL>(
          this,
          SampleDetail.SetKey,
          SampleDetail.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true);
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
          SampleMaster.Disconnect,
          areItemsUpdatable: true,
          areItemsDeletable: true,
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
          areItemsUpdatable: true,
          areItemsDeletable: true,
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
          areItemsUpdatable: true,
          areItemsDeletable: true,
          isCompactDuringDispose: isCompactDuringDispose);
      }
    }
  }
}