using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Some comment for SampleMaster.
    /// With an additional line.
    /// </summary>
  public partial class SampleMaster: IStorage<SampleMaster> {


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
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    //partial void onUpdating(string text, int numberWithDefault, ref bool isCancelled){
   //}


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    //partial void onUpdated() {
    //}


    /// <summary>
    /// Called after an update is read from a CSV file
    /// </summary>
    //partial void onCsvUpdate() {
    //}


    /// <summary>
    /// Called after a sample gets added to SampleX.
    /// </summary>
    //partial void onAddedToSampleX(Sample sample){
    //}


    /// <summary>
    /// Called after a sample gets removed from SampleX.
    /// </summary>
    //partial void onRemovedFromSampleX(Sample sample){
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
