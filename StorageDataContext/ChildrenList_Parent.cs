using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of deletable parent using a List for its children. It can have only deletable children. The child must have a 
    /// parent (the child.Parent property is not nullable).
    /// </summary>
  public partial class ChildrenList_Parent: IStorageItemGeneric<ChildrenList_Parent> {


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
    partial void onCloned(ChildrenList_Parent clone) {
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
    /// Called after ChildrenList_Parent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenList_Parent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of ChildrenList_Parent is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string text, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of ChildrenList_Parent are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenList_Parent old) {
    }


    /// <summary>
    /// Called after an update for ChildrenList_Parent is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenList_Parent.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenList_Parent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenList_Parent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenList_Parent.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenList_Parent oldChildrenList_Parent) {
    }


    /// <summary>
    /// Called after ChildrenList_Parent.Release() transaction is rolled back
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
