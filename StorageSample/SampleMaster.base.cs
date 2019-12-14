using System;
using System.Collections.Generic;
using Storage;


namespace StorageSample {


  /// <summary>
  /// Some comment for SampleMaster.
  /// With an additional line.
  /// </summary>
  public partial class SampleMaster: IStorage<SampleMaster> {

    #region Properties
    //      ----------

    /// <summary>
    /// Unique identifier for SampleMaster. Gets set once SampleMaster gets added to DL.Data.
    /// </summary>
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
    /// Headers written to first line in CSV file
    /// </summary>
    internal static readonly string[] Headers = {"Key", "Text"};


    /// <summary>
    /// None existing SampleMaster
    /// </summary>
    internal static SampleMaster NoSampleMaster = new SampleMaster("NoText", isStoring: false);
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
    /// SampleMaster Constructor. If isStoring is true, adds SampleMaster to DL.Data.SampleMasters.
    /// </summary>
    public SampleMaster(string text, bool isStoring = true) {
      Key = Storage.Storage.NoKey;
      Text = text;
      samples = new List<Sample>();
      if (isStoring) {
        Store();
      }
    }


    /// <summary>
    /// Constructor for SampleMaster read from CSV file
    /// </summary>
    private SampleMaster(int key, CsvReader csvReader, DL _) {
      Key = key;
      Text = csvReader.ReadString()!;
      samples = new List<Sample>();
    }


    /// <summary>
    /// New SampleMaster read from CSV file
    /// </summary>
    internal static SampleMaster Create(int key, CsvReader csvReader, DL context) {
      return new SampleMaster(key, csvReader, context);
    }
    #endregion


    #region Methods
    //      -------

    /// <summary>
    /// Adds SampleMaster to DL.Data.SampleMasters. 
    /// </summary>
    public void Store() {
      if (Key>=0) {
        throw new Exception($"SampleMaster 'Class SampleMaster' can not be stored in DL.Data, key is {Key} greater equal 0.");
      }
      DL.Data!.SampleMasters.Add(this);
    }


    /// <summary>
    /// Maximal number of UTF8 characters needed to write SampleMaster to CSV file
    /// </summary>
    internal const int MaxLineLength = 50;


    /// <summary>
    /// Write SampleMaster to CSV file
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


    /// <summary>
    /// Updates this SampleMaster with values from CSV file
    /// </summary>
    internal static void Update(SampleMaster sampleMaster, CsvReader csvReader, DL _) {
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
    /// Removes SampleMaster from DL.Data.SampleMasters and disconnects all Samples.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleMaster.Remove(): SampleMaster 'Class SampleMaster' is not stored in DL.Data, key is {Key}.");
      }
      DL.Data!.SampleMasters.Remove(Key);
    }


    /// <summary>
    /// Disconnects all Samples.
    /// </summary>
    internal static void Disconnect(SampleMaster sampleMaster) {
      foreach (var sample in sampleMaster.Samples) {
        sample.RemoveSampleMaster(sampleMaster);
      }
    }


    /// <summary>
    /// Returns property values
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
        $"Key: {Key}," +
        $" {Text}," +
        $" Samples: {Samples.Count};";
    }
    #endregion
  }
}
