using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example for parent which can have at most 1 child and the parent property in the child is not nullable.
    /// </summary>
  public partial class ParentOneChild_Parent: IStorageItemGeneric<ParentOneChild_Parent> {


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
    partial void onCloned(ParentOneChild_Parent clone) {
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
    /// Called after ParentOneChild_Parent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ParentOneChild_Parent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties of ParentOneChild_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string text, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ParentOneChild_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ParentOneChild_Parent old) {
    }


    /// <summary>
    /// Called after an update for ParentOneChild_Parent is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ParentOneChild_Parent.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ParentOneChild_Parent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ParentOneChild_Parent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ParentOneChild_Parent.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ParentOneChild_Parent oldParentOneChild_Parent) {
    }


    /// <summary>
    /// Called after ParentOneChild_Parent.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a parentOneChild_Child gets added to ParentOneChild_Children.
    /// </summary>
    partial void onAddedToChild(ParentOneChild_Child parentOneChild_Child){
    }


    /// <summary>
    /// Called after a parentOneChild_Child gets removed from ParentOneChild_Children.
    /// </summary>
    partial void onRemovedFromChild(ParentOneChild_Child parentOneChild_Child){
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
