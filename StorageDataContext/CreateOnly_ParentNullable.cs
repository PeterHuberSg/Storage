using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a "CreateOnly" Parent, i.e. the parent's properties will not change and the parent will never get
    /// deleted, but it is still possible to add children, but not to remove them. The parent property in the child 
    /// is nullable.
    /// </summary>
  public partial class CreateOnly_ParentNullable: IStorageItemGeneric<CreateOnly_ParentNullable> {


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
    partial void onCloned(CreateOnly_ParentNullable clone) {
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
    /// Called before storing gets executed
    /// </summary>
    partial void onStoring(ref bool isCancelled) {
    }


    /// <summary>
    /// Called after storing is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }




    /// <summary>
    /// Called after item.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }
    /// <summary>
    /// Called after a createOnly_Child gets added to CreateOnly_Children.
    /// </summary>
    partial void onAddedToCreateOnly_Children(CreateOnly_Child createOnly_Child){
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
