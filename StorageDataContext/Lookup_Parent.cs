using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Parent of children who use lookup, i.e. parent has no children collection. areInstancesDeletable
    /// must be false.
    /// </summary>
  public partial class Lookup_Parent: IStorageItemGeneric<Lookup_Parent> {


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
    partial void onCloned(Lookup_Parent clone) {
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
    /// Called after Lookup_Parent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before Lookup_Parent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties of Lookup_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(DateTime date, decimal someValue, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of Lookup_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(Lookup_Parent old) {
    }


    /// <summary>
    /// Called after an update for Lookup_Parent is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after 'new Lookup_Parent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after Lookup_Parent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after Lookup_Parent.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(Lookup_Parent oldLookup_Parent) {
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
