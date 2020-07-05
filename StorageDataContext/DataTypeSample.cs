using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Class having every possible data type used for a property
    /// </summary>
  public partial class DataTypeSample: IStorage<DataTypeSample> {


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
    //partial void onUpdating(
      //DateTime aDate, 
      //DateTime? aNullableDate, 
      //TimeSpan aTime, 
      //TimeSpan? aNullableTime, 
      //DateTime aDateMinutes, 
      //DateTime? aNullableDateMinutes, 
      //DateTime aDateSeconds, 
      //DateTime? aNullableDateSeconds, 
      //DateTime aDateTime, 
      //DateTime? aNullableDateTime, 
      //TimeSpan aTimeSpan, 
      //TimeSpan? aNullableTimeSpan, 
      //decimal aDecimal, 
      //decimal? aNullableDecimal, 
      //decimal aDecimal2, 
      //decimal? aNullableDecimal2, 
      //decimal aDecimal4, 
      //decimal? aNullableDecimal4, 
      //decimal aDecimal5, 
      //decimal? aNullableDecimal5, 
      //bool aBool, 
      //bool? aNullableBool, 
      //int aInt, 
      //int? aNullableInt, 
      //long aLong, 
      //long? aNullableLong, 
      //char aChar, 
      //char? aNullableChar, 
      //string aString, 
      //string? aNullableString, 
      //SampleStateEnum aEnum, 
      //SampleStateEnum? aNullableEnum, 
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
