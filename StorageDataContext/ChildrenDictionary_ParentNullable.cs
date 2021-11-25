using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a parent child relationship using a Dictionary where the child's parent property is nullable.
    /// </summary>
  public partial class ChildrenDictionary_ParentNullable: IStorageItemGeneric<ChildrenDictionary_ParentNullable> {


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
    partial void onCloned(ChildrenDictionary_ParentNullable clone) {
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
    /// Called after ChildrenDictionary_ParentNullable.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenDictionary_ParentNullable gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of ChildrenDictionary_ParentNullable is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string text, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ChildrenDictionary_ParentNullable are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenDictionary_ParentNullable old) {
    }


    /// <summary>
    /// Called after an update for ChildrenDictionary_ParentNullable is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_ParentNullable.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenDictionary_ParentNullable()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_ParentNullable.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_ParentNullable.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenDictionary_ParentNullable oldChildrenDictionary_ParentNullable) {
    }


    /// <summary>
    /// Called after ChildrenDictionary_ParentNullable.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a childrenDictionary_Child gets added to ChildrenDictionary_Children.
    /// </summary>
    partial void onAddedToChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child){
    }


    /// <summary>
    /// Called after a childrenDictionary_Child gets removed from ChildrenDictionary_Children.
    /// </summary>
    partial void onRemovedFromChildrenDictionary_Children(ChildrenDictionary_Child childrenDictionary_Child){
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
