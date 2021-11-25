using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// SortedListChild has a member providing the key value needed to add SortedListChild to  
    /// ParentWithSortedList and ParentWithSortedListNullable
    /// </summary>
  public partial class ChildrenSortedList_Child: IStorageItemGeneric<ChildrenSortedList_Child> {


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
    partial void onCloned(ChildrenSortedList_Child clone) {
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
    /// Called after ChildrenSortedList_Child.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenSortedList_Child gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of ChildrenSortedList_Child is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(
      DateTime dateKey, 
      string text, 
      ChildrenSortedList_Parent parentWithSortedList, 
      ChildrenSortedList_ParentNullable? parentWithSortedListNullable, 
      ref bool isCancelled)
   {
   }


    /// <summary>
    /// Called after all properties of ChildrenSortedList_Child are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenSortedList_Child old) {
    }


    /// <summary>
    /// Called after an update for ChildrenSortedList_Child is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Child.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenSortedList_Child()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Child.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Child.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenSortedList_Child oldChildrenSortedList_Child) {
    }


    /// <summary>
    /// Called after ChildrenSortedList_Child.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
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
