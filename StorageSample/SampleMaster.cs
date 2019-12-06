using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  /// <summary>
  /// Some comment for SampleMaster
  /// </summary>
  public partial class SampleMaster: IStorage<SampleMaster> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleMaster. Gets set once SampleMaster gets added to DL.Data.    /// </summary>
    public int Key { get; private set; }
    internal static void SetKey(SampleMaster sampleMaster, int key) { sampleMaster.Key = key; }


    /// <summary>
    /// Some Text comment
    /// </summary>
    public string Text { get; private set; }


    /// <summary>
    /// Some Samples comment
    /// </summary>
    public IReadOnlyList<Sample> Samples { get { return samples; } }
    readonly List<Sample> samples;


    /// <summary>
    /// Headers written to first line in csv file
    /// </summary>
    public static readonly string[] Headers = { 
      "Key", 
      "Text" 
    };


    /// <summary>
    /// None existing SampleMaster
    /// </summary>
    internal static SampleMaster NoSampleMaster = new SampleMaster("NoSampleMaster", isStoring: false);
    #endregion


    #region Events
    //      ------

    /// <summary>
    /// Content of SampleMaster has changed. Gets only raised for changes occuring after loading DL.Data with previously stored data.
    /// </summary>
    public event Action<SampleMaster>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    /// <summary>
    /// Sample SampleMaster
    /// </summary>
    public SampleMaster(string text, bool isStoring = true) {
      Key = Storage.NoKey;
      Text = text;
      samples = new List<Sample>();
      if (isStoring) {
        Store();
      }
    }


    /// <summary>
    /// Constructor for SampleMaster read from csv file
    /// </summary>
    private SampleMaster(int key, CsvReader csvReader) {
      Key = key;
      Text = csvReader.ReadString()!;
      samples = new List<Sample>();
    }


    /// <summary>
    /// New SampleMaster read from csv file
    /// </summary>
    internal static SampleMaster Create(int key, CsvReader csvReader, DLData dlData) {
      return new SampleMaster(key, csvReader);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds SampleMaster to DL.Data.SampleMasters. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleMaster '{this}' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      DL.Data!.SampleMasters.Add(this);
    }


    /// <summary>
    /// Maximal number of UTF8 characters needed to write Sample to csv file
    /// </summary>
    internal const int MaxLineLength = 30;


    /// <summary>
    /// Write Sample to csv file
    /// </summary>
    internal static void Write(SampleMaster sampleMaster, CsvWriter csvWriter) {
      csvWriter.Write(sampleMaster.Text);
    }


    /// <summary>
    /// Updates SampleMaster with the provided values
    /// </summary>
    public void Update(string text) {
      var isChangeDetected = false;
      if (Text!=text) {
        Text = text;
        isChangeDetected = true;
      }
      if (isChangeDetected) HasChanged?.Invoke(this);
    }


    ///// <summary>
    ///// Copies the values from sampleMasterChanged to this SampleMaster.
    ///// </summary>
    //public void Update(SampleMaster sampleMasterChanged) {
    //  var isChangeDetected = false;
    //  if (Text!=sampleMasterChanged.Text) {
    //    Text = sampleMasterChanged.Text;
    //    isChangeDetected = true;
    //  }
    //  if (isChangeDetected) HasChanged?.Invoke(this);
    //}


    /// <summary>
    /// Updates this SampleMaster with values from csvReader
    /// </summary>
    internal static void Update(SampleMaster sampleMaster, CsvReader csvReader, DLData dlData) {
      sampleMaster.Text = csvReader.ReadString()!;
    }


    /// <summary>
    /// Add sample to Samples.
    /// </summary>
    internal void AddToSamples(Sample sample) {
      samples.Add(sample);
    }


    /// <summary>
    /// Removes sample from Samples.
    /// </summary>
    internal void RemoveFromSamples(Sample sample) {
#if DEBUG
      if (!samples.Remove(sample)) throw new Exception();
#else
        samples.Remove(sample));
#endif
    }


    /// <summary>
    /// Removes SampleMaster from DL.Data.SampleMasters. 
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleMaster.Remove(): SampleMaster '{this}' is not stored in DL.Data, key is {Key}.");
      }
      DL.Data!.SampleMasters.Remove(Key);
    }


    /// <summary>
    /// Removes SampleMaster from all Samples.
    /// </summary>
    internal static void Disconnect(SampleMaster sampleMaster) {
      foreach (var sample in sampleMaster.Samples) {
        sample.RemoveSampleMaster(sampleMaster);
      }
    }


    /// <summary>
    /// Returns some property values
    /// </summary>
    public string ToShortString() {
      return
        $"{Key.ToKeyString()}," +
        $" {Text}";
    }


    /// <summary>
    /// Returns all property names and values
    /// </summary>
    public override string ToString() {
      return
        $"Key: {Key};" +
        $" Text: {Text};" +
        $" Samples: {Samples.Count};";
    }
#endregion
  }
}
