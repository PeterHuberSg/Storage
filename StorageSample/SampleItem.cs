
using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public class SampleItem: IStorage<SampleItem> {
    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Key { get; protected set; }
    private static int lastId = -1;


    public void ResetLastId() {
      lastId = -1;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Text { get; }
    #endregion


    #region Events
    //      ------

    public event Action<SampleItem>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    public SampleItem(int id, string text) {
      Key = id;
      Text = text;
    }

    #endregion


    #region Methods
    //      -------

    public override string ToString() {
      return
        $"Key: {Key};" +
        $" Text: {Text},";
    }
    #endregion
  }
}
