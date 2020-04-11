//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into DL.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Threading;
using Storage;


namespace StorageModel  {

  /// <summary>
  /// A part of DL is static, which gives easy access to all stored data (=context) through DL.Data. But most functionality is in the
  /// instantiatable part of DL. Since it is instantiatable, is possible to use different contexts over the lifetime of a program. This 
  /// is helpful for unit testing. Use DL.Init() to create a new context and dispose it with DisposeData() before creating a new one.
  /// </summary>
  public partial class DL: IDisposable {

    #region static Part
    //      -----------

    /// <summary>
    /// Provides static root access to the data context
    /// </summary>
    public static DL Data {
      get { return data!; }
    }
    private static DL? data; //data is needed for Interlocked.Exchange(ref data, null) in DisposeData()


    /// <summary>
    /// Flushes all data to permanent storage location if permanent data storage is active. Compacts data storage
    /// by applying all updates and removing all instances marked as deleted.
    /// </summary>
    public static void DisposeData() {
      var dataLocal = Interlocked.Exchange(ref data, null);
      dataLocal?.Dispose();
    }
    #endregion


    #region Properties
    //      ----------

    /// <summary>
    /// Configuration parameters if data gets stored in .csv files
    /// </summary>
    public CsvConfig? CsvConfig { get; }

    /// <summary>
    /// Directory of all DictionaryChildren
    /// </summary>
    public StorageDictionary<DictionaryChild, DL> DictionaryChildren { get; private set; }

    /// <summary>
    /// Directory of all LookupChildren
    /// </summary>
    public StorageDictionary<LookupChild, DL> LookupChildren { get; private set; }

    /// <summary>
    /// Directory of all LookupParents
    /// </summary>
    public StorageDictionary<LookupParent, DL> LookupParents { get; private set; }

    /// <summary>
    /// Directory of all ParentsWithDictionary
    /// </summary>
    public StorageDictionary<ParentWithDictionary, DL> ParentsWithDictionary { get; private set; }

    /// <summary>
    /// Directory of all ParentsWithSortedList
    /// </summary>
    public StorageDictionary<ParentWithSortedList, DL> ParentsWithSortedList { get; private set; }

    /// <summary>
    /// Directory of all ReadOnlyChildren
    /// </summary>
    public StorageDictionary<ReadOnlyChild, DL> ReadOnlyChildren { get; private set; }

    /// <summary>
    /// Directory of all ReadOnlyParents
    /// </summary>
    public StorageDictionary<ReadOnlyParent, DL> ReadOnlyParents { get; private set; }

    /// <summary>
    /// Directory of all SampleX
    /// </summary>
    public StorageDictionary<Sample, DL> SampleX { get; private set; }

    /// <summary>
    /// Directory of all SampleDetails
    /// </summary>
    public StorageDictionary<SampleDetail, DL> SampleDetails { get; private set; }

    /// <summary>
    /// Directory of all SampleMasters
    /// </summary>
    public StorageDictionary<SampleMaster, DL> SampleMasters { get; private set; }

    /// <summary>
    /// Directory of all SortedListChildren
    /// </summary>
    public StorageDictionary<SortedListChild, DL> SortedListChildren { get; private set; }
    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Creates a new data context. If csvConfig is null, the data is only stored in RAM, but gets lost once 
    /// program terminates. With csvConfig defined, existing data gets read at startup, changes immediately
    /// when written and Dispose() ensures by flushing that all data is permanently stored.
    /// </summary>
    public DL(CsvConfig? csvConfig) {
      if (!IsDisposed) {
        throw new Exception("Dispose old DL before creating a new one.");
      }
      isDisposed = 0;
      data = this;
      CsvConfig = csvConfig;
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
          ParentWithDictionary.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        DictionaryChildren = new StorageDictionary<DictionaryChild, DL>(
          this,
          DictionaryChild.SetKey,
          DictionaryChild.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
        ParentsWithSortedList = new StorageDictionary<ParentWithSortedList, DL>(
          this,
          ParentWithSortedList.SetKey,
          ParentWithSortedList.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
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
          areInstancesDeletable: false);
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
          areInstancesDeletable: true);
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
          areInstancesDeletable: true);
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
          areInstancesDeletable: false);
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
          areInstancesDeletable: false);
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
          ParentWithDictionary.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
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
          areInstancesDeletable: true);
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
          ParentWithSortedList.Disconnect,
          areInstancesUpdatable: true,
          areInstancesDeletable: true);
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
          areInstancesDeletable: true);
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
          areInstancesDeletable: false);
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
          areInstancesUpdatable: false,
          areInstancesDeletable: false);
      }
      onConstruct();
    }

    /// <summary>}
    /// Called at end of constructor
    /// </summary>}
    partial void onConstruct();
    #endregion


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Is DL.Data already disposed ?
    /// </summary>
    public bool IsDisposed {
      get { return isDisposed==1; }
    }
    int isDisposed = 1;


    protected virtual void Dispose(bool disposing) {
      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
      if (wasDisposed==1) return; // already disposed

      if (disposing) {
        onDispose();
        ReadOnlyChildren.Dispose();
        ReadOnlyParents.Dispose();
        SortedListChildren.Dispose();
        ParentsWithSortedList.Dispose();
        DictionaryChildren.Dispose();
        ParentsWithDictionary.Dispose();
        LookupChildren.Dispose();
        LookupParents.Dispose();
        SampleDetails.Dispose();
        SampleX.Dispose();
        SampleMasters.Dispose();
        data = null;
      }
    }

    /// <summary>}
    /// Called before storageDirectories get disposed.
    /// </summary>}
    partial void onDispose();


    public void Dispose() {
      Dispose(true);
    }
    #endregion


    #region Methods
    //      -------

    #endregion

  }
}

