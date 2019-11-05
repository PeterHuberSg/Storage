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
    private static int lastId = -1;


    public void ResetLastId() {
      lastId = -1;
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
    #endregion


    #region Events
    //      ------

    public event Action<Sample>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    public Sample(int id, string text, int number, decimal amount, DateTime date, string? optional = null) {
      Key = IStorage<Sample>.HandleId(this, ref lastId, id);
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


    public bool CanDelete() { return true; }


    public string ToCsvString(char delimiter) {
      //return "" +
      //  Key + delimiter +
      //  Info + delimiter;
      throw new NotImplementedException();
    }


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
