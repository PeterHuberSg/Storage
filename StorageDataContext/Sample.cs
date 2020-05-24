using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Some comment for Sample
    /// </summary>
  public partial class Sample: IStorage<Sample> {


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
    //partial void onStore() {
    //}


    /// <summary>
    /// Called before the data gets written to a CSV file
    /// </summary>
    //partial void onCsvWrite() {
    //}


    /// <summary>
    /// Called after all properties are updated, but before the HasChanged event gets raised
    /// </summary>
    //partial void onUpdating(
      //string text, 
      //bool flag, 
      //int number, 
      //decimal amount, 
      //decimal amount4, 
      //decimal? amount5, 
      //decimal preciseDecimal, 
      //SampleStateEnum sampleState, 
      //DateTime dateOnly, 
      //TimeSpan timeOnly, 
      //DateTime dateTimeTicks, 
      //DateTime dateTimeMinute, 
      //DateTime dateTimeSecond, 
      //SampleMaster? oneMaster, 
      //SampleMaster? otherMaster, 
      //string? optional, 
      //ref bool isCancelled)
   //{
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
    /// Called before removal gets executed
    /// </summary>
    //partial void onRemove() {
    //}


    /// <summary>
    /// Called after a sampleDetail gets added to SampleDetails.
    /// </summary>
    //partial void onAddedToSampleDetails(SampleDetail sampleDetail){
    //}


    /// <summary>
    /// Called after a sampleDetail gets removed from SampleDetails.
    /// </summary>
    //partial void onRemovedFromSampleDetails(SampleDetail sampleDetail){
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
