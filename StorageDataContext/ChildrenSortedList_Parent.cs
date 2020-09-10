using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a parent child relationship using a SortedList.
    /// </summary>
  public partial class ChildrenSortedList_Parent: IStorageItemGeneric<ChildrenSortedList_Parent> {


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
    partial void onCloned(ChildrenSortedList_Parent clone) {
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
    /// Called after ChildrenSortedList_Parent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenSortedList_Parent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties of ChildrenSortedList_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string textUpdateable, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ChildrenSortedList_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenSortedList_Parent old) {
    }


    /// <summary>
    /// Called after an update for ChildrenSortedList_Parent is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Parent.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenSortedList_Parent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Parent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Parent.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenSortedList_Parent oldChildrenSortedList_Parent) {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Parent.Release() transaction is rolled back
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
