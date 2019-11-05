
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

    public void Update(SampleItem itemChanged) {
      HasChanged?.Invoke(this);
      throw new NotImplementedException();
    }


    public bool CanDelete() { return true; }


    public override string ToString() {
      return
        $"Key: {Key};" +
        $" Text: {Text},";
    }
    #endregion
  }
}
