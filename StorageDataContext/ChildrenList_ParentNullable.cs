using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of deletable parent using a List for its children. It can have only deletable children. The child might have a 
    /// parent (the child.Parent property is nullable).
    /// </summary>
  public partial class ChildrenList_ParentNullable: IStorageItemGeneric<ChildrenList_ParentNullable> {


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
    partial void onCloned(ChildrenList_ParentNullable clone) {
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
    /// Called after ChildrenList_ParentNullable.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenList_ParentNullable gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties of ChildrenList_ParentNullable are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string text, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ChildrenList_ParentNullable are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenList_ParentNullable old) {
    }


    /// <summary>
    /// Called after an update for ChildrenList_ParentNullable is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenList_ParentNullable.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenList_ParentNullable()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenList_ParentNullable.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenList_ParentNullable.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenList_ParentNullable oldChildrenList_ParentNullable) {
    }


    /// <summary>
    /// Called after ChildrenList_ParentNullable.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a childrenList_Child gets added to ChildrenList_Children.
    /// </summary>
    partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child){
    }


    /// <summary>
    /// Called after a childrenList_Child gets removed from ChildrenList_Children.
    /// </summary>
    partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child){
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
