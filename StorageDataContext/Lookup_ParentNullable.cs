using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Parent of children who use lookup, i.e. parent has no children collection,  where the child's 
    /// parent property is nullable. areInstancesDeletable must be false.
    /// </summary>
  public partial class Lookup_ParentNullable: IStorageItemGeneric<Lookup_ParentNullable> {


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
    partial void onCloned(Lookup_ParentNullable clone) {
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
    /// Called before storing gets executed
    /// </summary>
    partial void onStoring(ref bool isCancelled) {
    }


    /// <summary>
    /// Called after storing is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(DateTime date, decimal someValue, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(Lookup_ParentNullable old) {
    }


    /// <summary>
    /// Called after an update is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }




    /// <summary>
    /// Called after item.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after item.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(Lookup_ParentNullable oldLookup_ParentNullable) {
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
