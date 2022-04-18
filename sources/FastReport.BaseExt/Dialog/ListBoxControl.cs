using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace FastReport.Dialog
{
  /// <summary>
  /// Represents a Windows control to display a list of items.
  /// Wraps the <see cref="System.Windows.Forms.ListBox"/> control.
  /// </summary>
  public class ListBoxControl : ListBoxBaseControl
  {
    private ListBox listBox;

    #region Properties
    /// <summary>
    /// Gets an internal <b>ListBox</b>.
    /// </summary>
    [Browsable(false)]
    public ListBox ListBox
    {
      get { return listBox; }
    }       

    #endregion

    #region Protected Methods
    /// <inheritdoc/>
    protected override object GetValue()
    {
      List<string> list = new List<string>();
      foreach (object item in SelectedItems)
      {
        list.Add((string)item);
      }
      return list.ToArray();
    }
    #endregion

    #region Public Methods
    /// <inheritdoc/>
    public override void OnSelectedIndexChanged(EventArgs e)
    {
      OnFilterChanged();
      base.OnSelectedIndexChanged(e);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>ListBoxControl</b> class with default settings. 
    /// </summary>
    public ListBoxControl()
    {
      listBox = new ListBox();
      Control = listBox;
      ListBox.IntegralHeight = false;
      BindableProperty = this.GetType().GetProperty("SelectedItem");
#if !FRCORE
            DrawControl = new ListBox();
            (DrawControl as ListBox).IntegralHeight = false;
            DrawControl.Font = DrawFont;
            DrawControl.Size = new Size((int)(Control.Width * DpiScale), (int)(Control.Height * DpiScale));
#endif
    }
  }
}
