using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// Example of a "CreateOnly" Child, i.e. the child's properties will not change. If it is added to a parent during its
    /// creation, it cannot be removed from the parent, because the Parent property of the child cannot be changed
    /// either.
    /// </summary>
  public partial class CreateOnly_Child: IStorageItemGeneric<CreateOnly_Child> {


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
    partial void onCloned(CreateOnly_Child clone) {
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
    /// Called after CreateOnly_Child.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before CreateOnly_Child gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after 'new CreateOnly_Child()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after CreateOnly_Child.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
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
