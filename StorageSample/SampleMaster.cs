using System;
using System.Collections.Generic;
using System.Text;


namespace Storage {


  public class SampleMaster {

    #region Properties
    //      ----------

    /// <summary>
    /// 
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// 
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// 
    /// </summary>
    public List<Sample> Samples { get; }
    #endregion


    #region Events
    //      ------

    public event Action<Sample>? HasChanged;
    #endregion


    #region Constructors
    //      ------------

    public SampleMaster(int id, string text) {
      Id = id;
      Text = text;
      Samples = new List<Sample>();
    }

    #endregion


    #region Methods
    //      -------

    public override string ToString() {
      return
        $"Id: {Id};" +
        $" Text: {Text};";
    }
    #endregion
  }
}
