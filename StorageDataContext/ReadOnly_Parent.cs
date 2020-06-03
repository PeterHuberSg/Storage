using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
    /// deleted, but it is still possible to add children, but not to remove them.
    /// </summary>
  public partial class ReadOnly_Parent: IStorage<ReadOnly_Parent> {


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
    /// Called once the CSV-constructor who reads the data from a CSV file has filled all the properties
    /// </summary>
    partial void onCsvConstruct(DC context) {
    }


    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Called before storing gets executed
    /// </summary>
    partial void onStore() {
    }


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called after a readOnly_Child gets added to ReadOnly_Children.
    /// </summary>
    partial void onAddedToReadOnly_Children(ReadOnly_Child readOnly_Child){
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
