using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of a "readonly" Child, i.e. the child's properties will not change and once it is added to its parent
    /// and therefore it also cannot be removed from parent, because the Parent property of the child cannot be changed
    /// either.
    /// </summary>
  public partial class ReadOnlyParentUpdatableChild_Child: IStorage<ReadOnlyParentUpdatableChild_Child> {


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
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(string text, ReadOnlyParentUpdatableChild_Parent parent, ReadOnlyParentUpdatableChild_ParentNullable? parentNullable, ref bool isCancelled){
   }


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated() {
    }


    /// <summary>
    /// Called after an update is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
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
