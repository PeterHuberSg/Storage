//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by StorageClassGenerator
//
//     Do not change code in this file, it will get lost when the file gets 
//     auto generated again. Write your code into SampleWithDictionary.cs.
// </auto-generated>
//------------------------------------------------------------------------------
#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using Storage;


namespace StorageModel  {


    /// <summary>
    /// Some comment for SampleDetail
    /// </summary>
  public partial class SampleWithDictionary: IStorage<SampleWithDictionary> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleWithDictionary. Gets set once SampleWithDictionary gets added to DC.Data.
    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(SampleWithDictionary sampleWithDictionary, int key) { sampleWithDictionary.Key = key; }


    /// <summary>
    /// Used as key into dictionary SampleWithDictionaryByIdInt
    /// </summary>
    public int IdInt { get; private set; }


    /// <summary>
    /// Used as key into dictionary SampleWithDictionaryByIdString
    /// </summary>
    public string? IdString { get; private set; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "IdInt", "IdString", "Text"};


    /// <summary>
    /// None existing SampleWithDictionary
    /// </summary>
    internal static SampleWithDictionary NoSampleWithDictionary = new SampleWithDictionary(int.MinValue, null, "NoText", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of SampleWithDictionary has changed. Gets only raised for changes occurring after loading DC.Data with previously stored data.
    /// </summary>
    public event Action<SampleWithDictionary>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// SampleWithDictionary Constructor. If isStoring is true, adds SampleWithDictionary to DC.Data.SampleWithDictionaries.
    /// </summary>
    public SampleWithDictionary(int idInt, string? idString, string text, bool isStoring = true) {
      Key = StorageExtensions.NoKey;
      IdInt = idInt;
      IdString = idString;
      Text = text;
      onConstruct();

      if (isStoring) {
        Store();
      }
    }
    partial void onConstruct();


    /// <summary>
    /// Constructor for SampleWithDictionary read from CSV file
    /// </summary>
    private SampleWithDictionary(int key, CsvReader csvReader, DC context) {
      Key = key;
      IdInt = csvReader.ReadInt();
      DC.Data.SampleWithDictionariesByIdInt.Add(IdInt, this);
      IdString = csvReader.ReadStringNull();
      if (IdString!=null) {
        DC.Data.SampleWithDictionariesByIdString.Add(IdString, this);
      }
      Text = csvReader.ReadString();
      onCsvConstruct(context);
    }
    partial void onCsvConstruct(DC context);


    /// <summary>
    /// New SampleWithDictionary read from CSV file
    /// </summary>
    internal static SampleWithDictionary Create(int key, CsvReader csvReader, DC context) {
      return new SampleWithDictionary(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds SampleWithDictionary to DC.Data.SampleWithDictionaries. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleWithDictionary can not be stored again in DC.Data, key is {Key} greater equal 0." + Environment.NewLine + ToString());
      }
      onStore();
      DC.Data.SampleWithDictionaries.Add(this);
      DC.Data.SampleWithDictionariesByIdInt.Add(IdInt, this);
      if (IdString!=null) {
        DC.Data.SampleWithDictionariesByIdString.Add(IdString, this);
      }
    }
    partial void onStore();


    /// <summary>
    /// Maximal number of UTF8 characters needed to write SampleWithDictionary to CSV file
    /// </summary>
    public const int MaxLineLength = 311;


    /// <summary>
    /// Write SampleWithDictionary to CSV file
    /// </summary>
    internal static void Write(SampleWithDictionary sampleWithDictionary, CsvWriter csvWriter) {
      sampleWithDictionary.onCsvWrite();
      csvWriter.Write(sampleWithDictionary.IdInt);
      csvWriter.Write(sampleWithDictionary.IdString);
      csvWriter.Write(sampleWithDictionary.Text);
    }
    partial void onCsvWrite();


    /// <summary>
    /// Updates SampleWithDictionary with the provided values
    /// </summary>
    public void Update(int idInt, string? idString, string text) {
      var isChangeDetected = false;
      if (IdInt!=idInt) {
        DC.Data.SampleWithDictionariesByIdInt.Remove(IdInt);
        IdInt = idInt;
        DC.Data.SampleWithDictionariesByIdInt.Add(IdInt, this);
        isChangeDetected = true;
      }
      if (IdString!=idString) {
        if (IdString!=null) {
          DC.Data.SampleWithDictionariesByIdString.Remove(IdString);
        }
        IdString = idString;
        if (IdString!=null) {
          DC.Data.SampleWithDictionariesByIdString.Add(IdString, this);
        }
        isChangeDetected = true;
      }
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) {
        onUpdate();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdate();


    /// <summary>
    /// Updates this SampleWithDictionary with values from CSV file
    /// </summary>
    internal static void Update(SampleWithDictionary sampleWithDictionary, CsvReader csvReader, DC _) {
      DC.Data.SampleWithDictionariesByIdInt.Remove(sampleWithDictionary.IdInt);
      sampleWithDictionary.IdInt = csvReader.ReadInt();
      DC.Data.SampleWithDictionariesByIdInt.Add(sampleWithDictionary.IdInt, sampleWithDictionary);
      if (sampleWithDictionary.IdString!=null) {
        DC.Data.SampleWithDictionariesByIdString.Remove(sampleWithDictionary.IdString);
      }
      sampleWithDictionary.IdString = csvReader.ReadStringNull();
      if (sampleWithDictionary.IdString!=null) {
        DC.Data.SampleWithDictionariesByIdString.Add(sampleWithDictionary.IdString, sampleWithDictionary);
      }
      sampleWithDictionary.Text = csvReader.ReadString();
      sampleWithDictionary.onCsvUpdate();
    }
    partial void onCsvUpdate();


    /// <summary>
    /// Removes SampleWithDictionary from DC.Data.SampleWithDictionaries, 
    /// removes SampleWithDictionary from DC.Data.SampleWithDictionariesByIdInt and 
    /// removes SampleWithDictionary from DC.Data.SampleWithDictionariesByIdString.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleWithDictionary.Remove(): SampleWithDictionary 'Class SampleWithDictionary' is not stored in DC.Data, key is {Key}.");
      }
      onRemove();
      DC.Data.SampleWithDictionaries.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Removes SampleWithDictionary from DC.Data.SampleWithDictionariesByIdInt and 
    /// removes SampleWithDictionary from DC.Data.SampleWithDictionariesByIdString.
    /// </summary>
    internal static void Disconnect(SampleWithDictionary sampleWithDictionary) {
      DC.Data.SampleWithDictionariesByIdInt.Remove(sampleWithDictionary.IdInt);
      if (sampleWithDictionary.IdString!=null) {
        DC.Data.SampleWithDictionariesByIdString.Remove(sampleWithDictionary.IdString);
      }
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
        $" {IdInt}," +
        $" {IdString}," +
        $" {Text}";
      onToShortString(ref returnString);
      return returnString;
    }
    partial void onToShortString(ref string returnString);


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      var returnString =
        $"Key: {Key}," +
        $" IdInt: {IdInt}," +
        $" IdString: {IdString}," +
        $" Text: {Text};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }


  #region SampleWithDictionaryWriter
  //      --------------------------

  /// <summary>
  /// Writes a CSV file containing records which can be read back as SampleWithDictionary. Note that the keys of linked objects
  /// need to be provided in Write(), since the data context will not be involved.
  /// </summary>
  public class SampleWithDictionaryWriter: IDisposable {

    readonly CsvConfig csvConfig;
    readonly CsvWriter csvWriter;
    int lastKey = int.MinValue;


    /// <summary>
    /// Constructor, will write the SampleWithDictionary header line into the CSV file. Dispose SampleWithDictionaryWriter once done.
    /// </summary>
    public SampleWithDictionaryWriter(string? fileNamePath, CsvConfig csvConfig, int maxLineCharLenght) {
      this.csvConfig = csvConfig;
      csvWriter = new CsvWriter(fileNamePath, csvConfig, maxLineCharLenght, null, 0);
      var csvHeaderString = Csv.ToCsvHeaderString(SampleWithDictionary.Headers, csvConfig.Delimiter);
      csvWriter.WriteLine(csvHeaderString);
    }


    /// <summary>
    /// Writes the details of one SampleWithDictionary to the CSV file
    /// </summary>
    public void Write(int key, int idInt, string? idString, string text) {
      if (key<0) {
        throw new Exception($"SampleWithDictionary's key {key} needs to be greater equal 0.");
      }
      if (key<=lastKey) {
        throw new Exception($"SampleWithDictionary's key {key} must be greater than the last written SampleWithDictionary's key {lastKey}.");
      }
      lastKey = key;
      csvWriter.WriteFirstLineChar(csvConfig.LineCharAdd);
      csvWriter.Write(key);
      csvWriter.Write(idInt);
      csvWriter.Write(idString);
      csvWriter.Write(text);
      csvWriter.WriteEndOfLine();
    }


    #region IDisposable Support
    //      -------------------

    /// <summary>
    /// Executes disposal of SampleWithDictionaryWriter exactly once.
    /// </summary>
    public void Dispose() {
      Dispose(true);

      GC.SuppressFinalize(this);
    }


    /// <summary>
    /// Is SampleWithDictionaryWriter already exposed ?
    /// </summary>
    protected bool IsDisposed {
      get { return isDisposed==1; }
    }


    int isDisposed = 0;


    /// <summary>
    /// Inheritors should call Dispose(false) from their destructor
    /// </summary>
    protected void Dispose(bool disposing) {
      var wasDisposed = Interlocked.Exchange(ref isDisposed, 1);//prevents that 2 threads dispose simultaneously
      if (wasDisposed==1) return; // already disposed

      csvWriter.Dispose();
    }
    #endregion
  }
  #endregion
}
