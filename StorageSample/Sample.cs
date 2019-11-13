using System;
using System.Collections.Generic;
using System.Text;

namespace Storage {


  public partial class Sample: IStorage<Sample> {

    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Key { get; private set; }
    private static int lastKey = -1;


    public void ResetLastKey() {
      lastKey = -1;
    }


    /// <summary>
    /// 
    /// </summary>
    public string Text { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public int Number { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public decimal Amount { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public DateTime Date { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public string? Optional { get; private set; }


    /// <summary>
    /// 
    /// </summary>
    public List<SampleItem> Items { get; }


    /// <summary>
    /// 
    /// </summary>
    public static readonly string[] Headers = { "Key", "Text", "Number", "Amount", "Date", "Optional" };
    #endregion


    #region Events
    //      ------

    public event Action<Sample>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    public Sample(int id, string text, int number, decimal amount, DateTime date, string? optional = null) {
      Key = IStorage<Sample>.HandleId(this, ref lastKey, id);
      Text = text;
      Number = number;
      Amount = amount;
      Date = date;
      Optional = optional;
      Items = new List<SampleItem>();
    }
    #endregion


    #region Methods
    //      -------

    public void Update(string text, int number, decimal amount, DateTime date, string? optional) {
      Text = text;
      Number = number;
      Amount = amount;
      Date = date;
      Optional = optional;
      HasChanged?.Invoke(this);
    }


    public void Update(Sample itemChanged) {
      Update(itemChanged.Text, itemChanged.Number, itemChanged.Amount, itemChanged.Date, itemChanged.Optional);
    }


    public bool CanDelete() {
      return Items.Count==0;
    }


    public string ToCsvString(char delimiter) {
      return "" +
        Key + delimiter +
        Text + delimiter +
        Number + delimiter +
        Amount + delimiter +
        Date + delimiter +
        Optional + delimiter;
    }


    //public static Sample? ReadCsvLine(string line, char delimiter, StringBuilder errorStringBuilder) {
    //  var fields = line.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
    //  if (fields.Length!=Headers.Length) {
    //    errorStringBuilder.AppendLine($"TestItem should have {Headers.Length} fields, but had {fields.Length}: '{line}'.");
    //    return null;
    //  }

    //  var fieldIndex = 0;
    //  var key = Csv.ParseInt("Sample.Key", fields[fieldIndex++], line, errorStringBuilder);
    //  var text = fields[fieldIndex++];
    //  var number = Csv.ParseInt("Sample.Mumber", fields[fieldIndex++], line, errorStringBuilder);
    //  var amount = Csv.ParseDecimalFast("Sample.Mumber", fields[fieldIndex++], line, errorStringBuilder);
    //  var sample = new Sample(key, text);
    //  return sample;
    //}


    public override string ToString() {
      return
        $"Key: {Key};" +
        $" Text: {Text};" +
        $" Number: {Number};" +
        $" Amount: {Amount}" +
        $" Date: {Date};" +
        $" Optional: {Optional};";
    }
    #endregion
  }
}
