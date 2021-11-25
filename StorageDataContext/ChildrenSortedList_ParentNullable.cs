using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a parent child relationship using a SortedList where the child's parent property is nullable.
    /// </summary>
  public partial class ChildrenSortedList_ParentNullable: IStorageItemGeneric<ChildrenSortedList_ParentNullable> {


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
    partial void onCloned(ChildrenSortedList_ParentNullable clone) {
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
    /// Called after ChildrenSortedList_ParentNullable.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenSortedList_ParentNullable gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of ChildrenSortedList_ParentNullable is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string textUpdateable, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ChildrenSortedList_ParentNullable are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenSortedList_ParentNullable old) {
    }


    /// <summary>
    /// Called after an update for ChildrenSortedList_ParentNullable is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_ParentNullable.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenSortedList_ParentNullable()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_ParentNullable.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_ParentNullable.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenSortedList_ParentNullable oldChildrenSortedList_ParentNullable) {
    }


    /// <summary>
    /// Called after ChildrenSortedList_ParentNullable.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a childrenSortedList_Child gets added to ChildrenSortedList_Children.
    /// </summary>
    partial void onAddedToChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child){
    }


    /// <summary>
    /// Called after a childrenSortedList_Child gets removed from ChildrenSortedList_Children.
    /// </summary>
    partial void onRemovedFromChildrenSortedList_Children(ChildrenSortedList_Child childrenSortedList_Child){
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
