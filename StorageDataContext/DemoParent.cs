using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Demo parent
    /// </summary>
    /// 
  public partial class DemoParent: IStorageItemGeneric<DemoParent> {


    #region Properties
    //      ----------

    #endregion


    #region Events
    //      ------

    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Called once the constructor has filled all the properties
    /// </summary>
    partial void onConstruct() {
    }


    /// <summary>
    /// Called once the cloning constructor has filled all the properties. Clones have no children data.
    /// </summary>
    partial void onCloned(DemoParent clone) {
    }


    /// <summary>
    /// Called once the CSV-constructor who reads the data from a CSV file has filled all the properties
    /// </summary>
    partial void onCsvConstruct() {
    }


    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Called before {ClassName}.Store() gets executed
    /// </summary>
    partial void onStoring(ref bool isCancelled) {
    }


    /// <summary>
    /// Called after DemoParent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before DemoParent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of DemoParent is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string demoParentData, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of DemoParent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(DemoParent old) {
    }


    /// <summary>
    /// Called after an update for DemoParent is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after DemoParent.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new DemoParent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after DemoParent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after DemoParent.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(DemoParent oldDemoParent) {
    }


    /// <summary>
    /// Called after DemoParent.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a demoChild gets added to DemoChildren.
    /// </summary>
    partial void onAddedToDemoChildren(DemoChild demoChild){
    }


    /// <summary>
    /// Called after a demoChild gets removed from DemoChildren.
    /// </summary>
    partial void onRemovedFromDemoChildren(DemoChild demoChild){
    }


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    partial void onToShortString(ref string returnString) {
    }


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    partial void onToString(ref string returnString) {
    }
    #endregion
  }
}
