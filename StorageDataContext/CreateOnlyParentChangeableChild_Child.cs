using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of an updatable and deletable Child, i.e. the child's properties can change.Therefore it 
    /// can be removed from parent and assigned to another parent.
    /// </summary>
  public partial class CreateOnlyParentChangeableChild_Child: IStorageItemGeneric<CreateOnlyParentChangeableChild_Child> {


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
    partial void onCloned(CreateOnlyParentChangeableChild_Child clone) {
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
    /// Called after CreateOnlyParentChangeableChild_Child.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before CreateOnlyParentChangeableChild_Child gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after all properties of CreateOnlyParentChangeableChild_Child are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string updatableText, CreateOnlyParentChangeableChild_Parent parent, CreateOnlyParentChangeableChild_ParentNullable? parentNullable, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties of CreateOnlyParentChangeableChild_Child are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(CreateOnlyParentChangeableChild_Child old) {
    }


    /// <summary>
    /// Called after an update for CreateOnlyParentChangeableChild_Child is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_Child.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new CreateOnlyParentChangeableChild_Child()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_Child.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_Child.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(CreateOnlyParentChangeableChild_Child oldCreateOnlyParentChangeableChild_Child) {
    }


    /// <summary>
    /// Called after CreateOnlyParentChangeableChild_Child.Release() transaction is rolled back
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
