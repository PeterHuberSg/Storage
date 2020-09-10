using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a "CreateOnly" Parent, i.e. this parent's properties will not change and this parent will never get
    /// deleted, but it is still possible to add and remove children. The parent property in the child 
    /// is nullable.
    /// </summary>
  public partial class CreateOnlyParentChangeableChild_ParentNullable: IStorageItemGeneric<CreateOnlyParentChangeableChild_ParentNullable> {


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
    partial void onCloned(CreateOnlyParentChangeableChild_ParentNullable clone) {
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
    /// Called after CreateOnlyParentChangeableChild_ParentNullable.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before CreateOnlyParentChangeableChild_ParentNullable gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_ParentNullable.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new CreateOnlyParentChangeableChild_ParentNullable()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_ParentNullable.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_ParentNullable.Release() transaction is rolled back
    /// </summary>
    partial void onRollbackItemRelease() {
    }


    /// <summary>
    /// Called after a createOnlyParentChangeableChild_Child gets added to CreateOnlyParentChangeableChild_Children.
    /// </summary>
    partial void onAddedToCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child){
    }


    /// <summary>
    /// Called after a createOnlyParentChangeableChild_Child gets removed from CreateOnlyParentChangeableChild_Children.
    /// </summary>
    partial void onRemovedFromCreateOnlyParentChangeableChild_Children(CreateOnlyParentChangeableChild_Child createOnlyParentChangeableChild_Child){
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
