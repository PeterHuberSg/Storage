using System;
using System.Collections.Generic;
using Storage;


namespace StorageModel  {


    ////      ---------------------------------------------------------------
    ////shows also in Sample most data types supported
    ///// <summary>
    ///// Some SampleStateEnum comment
    ///// </summary>
    //  /// <summary>
    //  /// Recommendation while creating your own enums: use value 0 as undefined
    //  /// </summary>
    ///// <summary>
    ///// Some comment for SampleMaster.
    ///// With an additional line.
    ///// </summary>
    //  /// <summary>
    //  /// Some Text comment
    //  /// </summary>
    //  /// <summary>
    //  /// List representing parent child relationship
    //  /// </summary>
    //  /// <summary>
    //  /// Integer property with int.MinValue as default
    //  /// </summary>
    ///// <summary>
    ///// Some comment for Sample
    ///// </summary>
    //  /// <summary>
    //  /// Some Text comment
    //  /// </summary>
    //  /// <summary>
    //  /// Some Flag comment
    //  /// </summary>
    //  /// <summary>
    //  /// Some Amount comment
    //  /// </summary>
    //  /// <summary>
    //  /// Amount with 2 digits after comma comment
    //  /// </summary>
    //  /// <summary>
    //  /// Amount with 4 digits after comma comment
    //  /// </summary>
    //  /// <summary>
    //  /// Nullable amount with 5 digits after comma comment
    //  /// </summary>
    //  /// <summary>
    //  /// PreciseDecimal with about 20 digits precision, takes a lot of storage space
    //  /// </summary>
    //  /// <summary>
    //  /// Some SampleState comment
    //  /// </summary>
    //  /// <summary>
    //  /// Stores dates but not times
    //  /// </summary>
    //  /// <summary>
    //  /// Stores times (24 hour timespan) but not date
    //  /// </summary>
    //  /// <summary>
    //  /// Stores date and time precisely to a tick
    //  /// </summary>
    //  /// <summary>
    //  /// Stores date and time precisely to a minute
    //  /// </summary>
    //  /// <summary>
    //  /// Stores date and time precisely to a second
    //  /// </summary>
    //  /// <summary>
    //  /// Some OneMaster comment
    //  /// </summary>
    //  /// <summary>
    //  /// Some OtherMaster comment
    //  /// </summary>
    //  /// <summary>
    //  /// Some Optional comment
    //  /// </summary>
    //  /// <summary>
    //  /// Some SampleDetails comment
    //  /// </summary>
    ///// <summary>
    ///// Some comment for SampleDetail
    ///// </summary>
    //  /// <summary>
    //  /// Some Text comment
    //  /// </summary>
    //  /// <summary>
    //  /// Link to parent Sample
    //  /// </summary>
    ////      ------------------------------------------------------------------
    ////an example for lookup, only the child linking to parent but the parent having no child collection
    ////this can be useful for example if parent holds exchange rates for every day. The child links to
    ////one particular exchange rate, but the exchange rate does not know which child links to it. In this
    ////scenario, the parent can never be deleted.
    //  /// <summary>
    //  /// Parent of children who uses lookup, i.e. parent has no children collection
    //  /// </summary>
    ////      ----------------------------------------------------------------------
    ////Example where parent has a Dictionary instead a List for its children. The child needs an additional field which
    ////can be used as Key for the Dictionary. 
    ///// <summary>
    ///// Example of a Parent child relationship using a Dictionary.
    ///// </summary>
    //  /// <summary>
    //  /// Some Text
    //  /// </summary>
    //  /// <summary>
    //  /// Dictionary used instead of List. Comment is required and indicates which property of the DictionaryChild to 
    //  /// use as key
    //  /// </summary>
    ///// <summary>
    ///// DictionaryChild has a member providing the key value needed to add DictionaryChild to the ParentWithDictionary.DictionaryChildren
    ///// </summary>
    //  /// <summary>
    //  /// Parent
    //  /// </summary>
    //  /// <summary>
    //  /// Key field used in ParentWithDictionary.DictionaryChildren Dictionary
    //  /// </summary>
    //  /// <summary>
    //  /// Some info
    //  /// </summary>
    ////      ----------------------------------------------------------------------
    ////Example where parent has a SortedList instead a List for its children. The child needs an additional field which
    ////can be used as Key for the SortedList.
    ////It's better to use a SortedList than a SortedDictionary, because in a SortedList, an item can be accessed
    ////by its place in the SortedList like the last item:
    ////key = sortedList.Keys[sortedList.Lenght];
    ////item = sortedList[key];
    ///// <summary>
    ///// Example of a Parent child relationship using a SortedList.
    ///// </summary>
    //  /// <summary>
    //  /// Some Text
    //  /// </summary>
    //  /// <summary>
    //  /// SortedList used instead of List. Comment is required and indicates which property of the SortedListChild to 
    //  /// use as key
    //  /// </summary>
    ///// <summary>
    ///// SortedListChild has a member providing the key value needed to add SortedListChild to the ParentWithSortedList.SortedListChildren
    ///// </summary>
    //  /// <summary>
    //  /// Parent
    //  /// </summary>
    //  /// <summary>
    //  /// Key field used in ParentWithSortedList.SortedListChildren SortedList
    //  /// </summary>
    //  /// <summary>
    //  /// Some info
    //  /// </summary>
    /// <summary>
    /// Example of a "readonly" Parent, i.e. the parent's properties will not change and the parent will never get
    /// deleted, but it is still possible to add children, but not to remove them.
    /// </summary>
  public partial class ReadOnlyParent: IStorage<ReadOnlyParent> {


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
    //partial void onCsvConstruct(DL context) {
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
    /// Called after a readOnlyChild gets added to ReadOnlyChildren.
    /// </summary>
    //partial void onAddedToReadOnlyChildren(ReadOnlyChild readOnlyChild){
    //}


    /// <summary>
    /// Called after a readOnlyChild gets removed from ReadOnlyChildren.
    /// </summary>
    //partial void onRemovedFromReadOnlyChildren(ReadOnlyChild readOnlyChild){
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
