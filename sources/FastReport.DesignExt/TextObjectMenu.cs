using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport
{
  /// <summary>
  /// The class introduces some menu items specific to the <b>TextObject</b>.
  /// </summary>
  public class TextObjectMenu : TextObjectBaseMenu
  {
    #region Fields
    /// <summary>
    /// The "Clear" menu item.
    /// </summary>
    public ContextMenuItem miClear;

    /// <summary>
    /// The "Auto Width" menu item.
    /// </summary>
    public ContextMenuItem miAutoWidth;

    /// <summary>
    /// The "Word Wrap" menu item.
    /// </summary>
    public ContextMenuItem miWordWrap;

    private SelectedTextObjects textObjects;
    #endregion

    #region Private Methods
    private void miClear_Click(object sender, EventArgs e)
    {
      textObjects.ClearText();
    }

    private void miAutoWidth_Click(object sender, EventArgs e)
    {
      textObjects.SetAutoWidth(miAutoWidth.Checked);
    }

    private void miWordWrap_Click(object sender, EventArgs e)
    {
      textObjects.SetWordWrap(miWordWrap.Checked);
    }
    #endregion

    /// <summary>
    /// Initializes a new instance of the <b>TextObjectMenu</b> 
    /// class with default settings. 
    /// </summary>
    /// <param name="designer">The reference to a report designer.</param>
    public TextObjectMenu(Designer designer) : base(designer)
    {
      textObjects = new SelectedTextObjects(designer);
      textObjects.Update();

      MyRes res = new MyRes("ComponentMenu,TextObject");
      miClear = CreateMenuItem(res.Get("Clear"), new EventHandler(miClear_Click));
      miAutoWidth = CreateMenuItem(res.Get("AutoWidth"), new EventHandler(miAutoWidth_Click));
      miAutoWidth.BeginGroup = true;
      miAutoWidth.CheckOnClick = true;
      miWordWrap = CreateMenuItem(res.Get("WordWrap"), new EventHandler(miWordWrap_Click));
      miWordWrap.CheckOnClick = true;

      miAllowExpressions.BeginGroup = false;

      int insertPos = Items.IndexOf(miFormat) + 1;
      Items.Insert(insertPos, miClear);
      insertPos = Items.IndexOf(miAllowExpressions);
      Items.Insert(insertPos, miAutoWidth);
      Items.Insert(insertPos + 1, miWordWrap);

      bool enabled = textObjects.Enabled;
      miAutoWidth.Enabled = enabled;
      miWordWrap.Enabled = enabled;

      TextObject obj = textObjects.First;
      miAutoWidth.Checked = obj.AutoWidth;
      miWordWrap.Checked = obj.WordWrap;
    }
  }
}
