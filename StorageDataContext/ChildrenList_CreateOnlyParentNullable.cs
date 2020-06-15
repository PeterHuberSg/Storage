using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Example of none deletable parent using a List for its children. The child can be deletable or none deletable. The 
    /// child might have a parent (the Parent property is nullable).
    /// </summary>
  public partial class ChildrenList_CreateOnlyParentNullable: IStorage<ChildrenList_CreateOnlyParentNullable> {


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
    //partial void onConstruct() {
    //}


    /// <summary>
    /// Called once the CSV-constructor who reads the data from a CSV file has filled all the properties
    /// </summary>
    //partial void onCsvConstruct(DC context) {
    //}


    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Called before storing gets executed
    /// </summary>
    //partial void onStoring(ref bool isCancelled) {
    //}


    /// <summary>
    /// Called after storing is executed
    /// </summary>
    //partial void onStored() {
    //}


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    //partial void onCsvWrite() {
    //}


    /// <summary>
    /// Called after a childrenList_Child gets added to ChildrenList_Children.
    /// </summary>
    //partial void onAddedToChildrenList_Children(ChildrenList_Child childrenList_Child){
    //}


    /// <summary>
    /// Called after a childrenList_Child gets removed from ChildrenList_Children.
    /// </summary>
    //partial void onRemovedFromChildrenList_Children(ChildrenList_Child childrenList_Child){
    //}


    /// <summary>
    /// Called after a childrenList_CreateOnlyChild gets added to ChildrenList_CreateOnlyChildren.
    /// </summary>
    //partial void onAddedToChildrenList_CreateOnlyChildren(ChildrenList_CreateOnlyChild childrenList_CreateOnlyChild){
    //}


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    //partial void onToShortString(ref string returnString) {
    //}


    /// <summary>
    /// Updates returnString with additional info for a short description.
    /// </summary>
    //partial void onToString(ref string returnString) {
    //}
    #endregion
  }
}
