using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of none deletable parent using a List for its children. It can have deletable and none
    /// deletable children. The child must have a parent (the Parent property is not nullable).
    /// </summary>
  public partial class ChildrenList_CreateOnlyParent: IStorageItemGeneric<ChildrenList_CreateOnlyParent> {


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
    partial void onCloned(ChildrenList_CreateOnlyParent clone) {
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
    /// Called after ChildrenList_CreateOnlyParent.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenList_CreateOnlyParent gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after 'new ChildrenList_CreateOnlyParent()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenList_CreateOnlyParent.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
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
    /// Called after a childrenList_CreateOnlyChild gets added to ChildrenList_CreateOnlyChildren.
    /// </summary>
    partial void onAddedToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild){
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
