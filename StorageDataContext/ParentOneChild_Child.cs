using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Child class with one parent property which is not nullable and one property to a different parent 
    /// which is nullable
    /// </summary>
  public partial class ParentOneChild_Child: IStorageItemGeneric<ParentOneChild_Child> {


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
    partial void onCloned(ParentOneChild_Child clone) {
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
    partial void onUpdating(string text, ParentOneChild_Parent parent, ParentOneChild_ParentNullable? parentNullable, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ParentOneChild_Child old) {
    }


    /// <summary>
    /// Called after an update is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called before removal gets executed
    /// </summary>
    partial void onRemove() {
    }




    /// <summary>
    /// Called after item.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after item.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ParentOneChild_Child oldParentOneChild_Child) {
    }


    /// <summary>
    /// Called after item.Remove() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRemoved() {
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
