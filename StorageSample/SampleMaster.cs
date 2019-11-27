using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public class SampleMaster: IStorageCSV<SampleMaster> {

    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Key { get; }


    /// <summary>
    /// 
    /// </summary>
    public string Text {
      get { return text; }
      set {
        if (text!=value) {
          text = value;
          HasChanged?.Invoke(this);
        }
      }
    }
    private string text;



    /// <summary>
    /// 
    /// </summary>
    public List<Sample> Samples { get; }


    /// <summary>
    /// 
    /// </summary>
    public static readonly string[] Headers = { "Key", "Text" };
    #endregion


    #region Events
    //      ------

    public event Action<SampleMaster>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    public SampleMaster(int key, string text) {
      Key = key;
      this.text = text;
      Samples = new List<Sample>();
    }
    #endregion


    #region Methods
    //      -------

    public void Update(SampleMaster itemChanged) {
      Text = itemChanged.Text;
      HasChanged?.Invoke(this);
    }


    public static void UpdateFromCsvLine(
      string line,
      char delimiter,
      StorageDictionary<SampleMaster> storageDirectoryCSV,
      StringBuilder errorStringBuilder) 
    {
      var itemChanged = ReadCsvLine(line, delimiter, errorStringBuilder);
      if (itemChanged==null) return;

      var item = storageDirectoryCSV[itemChanged.Key];
      item.Update(itemChanged);
    }


    public bool CanDelete() {
      return Samples.Count==0;
    }


    public string Write(CsvWriter csvWriter) {
      throw new NotImplementedException();
    }


    public string ToCsvString(char delimiter) {
      return "" +
        Key + delimiter +
        Text + delimiter;
    }


    public static SampleMaster? ReadCsvLine(string line, char delimiter, StringBuilder errorStringBuilder) {
      var fields = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
      if (fields.Length!=Headers.Length) {
        errorStringBuilder.AppendLine($"TestItem should have {Headers.Length} fields, but had {fields.Length}: '{line}'.");
        return null;
      }

      var fieldIndex = 0;
      var key = Csv.ParseInt("TestRecord.Id", fields[fieldIndex++], line, errorStringBuilder);
      var text = fields[fieldIndex++];
      var sampleMaster = new SampleMaster(key, text);
      return sampleMaster;
    }


    public override string ToString() {
      return
        $"Id: {Key};" +
        $" Text: {Text};";
    }
    #endregion
  }
}
