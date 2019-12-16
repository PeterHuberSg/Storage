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
    public IReadOnlyCollection<Sample> Samples { get { return samples; } }
    readonly HashSet<Sample> samples;


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
      samples = new HashSet<Sample>();
      onCreate();

      if (isStoring) {
        Store();
      }
    }
    partial void onCreate();


    /// <summary>
    /// Constructor for SampleMaster read from CSV file
    /// </summary>
    private SampleMaster(int key, CsvReader csvReader, DL _) {
      Key = key;
      Text = csvReader.ReadString()!;
      samples = new HashSet<Sample>();
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
      onStore();
      DL.Data!.SampleMasters.Add(this);
    }
    partial void onStore();


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
      if (isChangeDetected) {
        onUpdate();
        HasChanged?.Invoke(this);
      }
    }
    partial void onUpdate();


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
      var countLinks = 0;
      if (sample.OneMaster==this ) countLinks++;
      if (sample.OtherMaster==this) countLinks++;
      if (countLinks>1) return;
#if DEBUG
      if (countLinks==0) throw new Exception();
      if (!samples.Remove(sample)) throw new Exception();
#else
        samples.Remove(sample));
#endif
    }


    /// <summary>
    /// Removes SampleMaster from DL.Data.SampleMasters, disconnects Sample.OneMaster from Samples and disconnects Sample.OtherMaster from Samples.
    /// </summary>
    public void Remove() {
      if (Key<0) {
        throw new Exception($"SampleMaster.Remove(): SampleMaster 'Class SampleMaster' is not stored in DL.Data, key is {Key}.");
      }
      onRemove();
      DL.Data!.SampleMasters.Remove(Key);
    }
    partial void onRemove();


    /// <summary>
    /// Disconnects Sample.OneMaster from Samples and disconnects Sample.OtherMaster from Samples.
    /// </summary>
    internal static void Disconnect(SampleMaster sampleMaster) {
      foreach (var sample in sampleMaster.Samples) {
        sample.RemoveOneMaster(sampleMaster);
        sample.RemoveOtherMaster(sampleMaster);
      }
    }


    /// <summary>
    /// Returns property values
    /// </summary>
    public string ToShortString() {
      var returnString =
        $"{Key.ToKeyString()}," +
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
        $" Text: {Text}," +
        $" Samples: {Samples.Count};";
      onToString(ref returnString);
      return returnString;
    }
    partial void onToString(ref string returnString);
    #endregion
  }
}
