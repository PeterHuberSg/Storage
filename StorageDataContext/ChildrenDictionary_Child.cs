using System;
using System.Collections.Generic;
using Storage;


namespace StorageDataContext  {


    /// <summary>
    /// DictionaryChild has a member providing the key value needed to add DictionaryChild to  
    /// ParentWithDictionary and ParentWithDictionaryNullable
    /// </summary>
  public partial class ChildrenDictionary_Child: IStorageItemGeneric<ChildrenDictionary_Child> {


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
    partial void onCloned(ChildrenDictionary_Child clone) {
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
    /// Called after ChildrenDictionary_Child.Store() is executed
    /// </summary>
    partial void onStored() {
    }


    /// <summary>
    /// Called before ChildrenDictionary_Child gets written to a CSV file
    /// </summary>
    partial void onCsvWrite() {
    }


    /// <summary>
    /// Called before any property of ChildrenDictionary_Child is updated and before the HasChanged event gets raised
    /// </summary>
    partial void onUpdating(
      DateTime dateKey, 
      string text, 
      ChildrenDictionary_Parent parentWithDictionary, 
      ChildrenDictionary_ParentNullable? parentWithDictionaryNullable, 
      ref bool isCancelled)
   {
   }


    /// <summary>
    /// Called after all properties of ChildrenDictionary_Child are updated, but before the HasChanged event gets raised
    /// </summary>
    partial void onUpdated(ChildrenDictionary_Child old) {
    }


    /// <summary>
    /// Called after an update for ChildrenDictionary_Child is read from a CSV file
    /// </summary>
    partial void onCsvUpdate() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_Child.Release() got executed
    /// </summary>
    partial void onReleased() {
    }


    /// <summary>
    /// Called after 'new ChildrenDictionary_Child()' transaction is rolled back
    /// </summary>
    partial void onRollbackItemNew() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_Child.Store() transaction is rolled back
    /// </summary>
    partial void onRollbackItemStored() {
    }


    /// <summary>
    /// Called after ChildrenDictionary_Child.Update() transaction is rolled back
    /// </summary>
    partial void onRollbackItemUpdated(ChildrenDictionary_Child oldChildrenDictionary_Child) {
    }


    /// <summary>
    /// Called after ChildrenDictionary_Child.Release() transaction is rolled back
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
