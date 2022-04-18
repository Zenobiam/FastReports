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
  /// The class introduces some menu items specific to the <b>TextObjectBase</b>.
  /// </summary>
  public class TextObjectBaseMenu : BreakableComponentMenu
  {
    #region Fields
    /// <summary>
    /// The "Format" menu item.
    /// </summary>
    public ContextMenuItem miFormat;

    /// <summary>
    /// The "Allow Expressions" menu item.
    /// </summary>
    public ContextMenuItem miAllowExpressions;

    /// <summary>
    /// The "Hide Zeros" menu item.
    /// </summary>
    public ContextMenuItem miHideZeros;

    private SelectedTextBaseObjects textObjects;
    #endregion

    #region Private Methods

    private void miFormat_Click(object sender, EventArgs e)
    {
      using (FormatEditorForm form = new FormatEditorForm())
      {
        form.TextObject = textObjects.First;
        if (form.ShowDialog() == DialogResult.OK)
        {
          textObjects.SetFormat(form.Formats);
          Change();
        }
      }
    }

    private void miAllowExpressions_Click(object sender, EventArgs e)
    {
      textObjects.SetAllowExpressions(miAllowExpressions.Checked);
      Change();
    }

    private void miHideZeros_Click(object sender, EventArgs e)
    {
        textObjects.SetHideZeros(miHideZeros.Checked);
        Change();
    }

    #endregion Private Methods

    /// <summary>
    /// Initializes a new instance of the <b>TextObjectBaseMenu</b> 
    /// class with default settings. 
    /// </summary>
    /// <param name="designer">The reference to a report designer.</param>
    public TextObjectBaseMenu(Designer designer) : base(designer)
    {
      textObjects = new SelectedTextBaseObjects(designer);
      textObjects.Update();

      MyRes res = new MyRes("ComponentMenu,TextObject");
      miFormat = CreateMenuItem(Res.GetImage(168), res.Get("Format"), new EventHandler(miFormat_Click));
      miAllowExpressions = CreateMenuItem(res.Get("AllowExpressions"), new EventHandler(miAllowExpressions_Click));
      miHideZeros = CreateMenuItem(res.Get("HideZeros"), new EventHandler(miHideZeros_Click));
      miAllowExpressions.BeginGroup = true;
      miAllowExpressions.CheckOnClick = true;
      miHideZeros.CheckOnClick = true;

      int insertPos = Items.IndexOf(miEdit) + 1;
      Items.Insert(insertPos, miFormat);
      insertPos = Items.IndexOf(miCut);
      Items.Insert(insertPos, miAllowExpressions);
      insertPos = Items.IndexOf(miGrowToBottom) + 1;
      Items.Insert(insertPos, miHideZeros);

      TextObjectBase obj = textObjects.First;
      bool enabled = !obj.HasRestriction(Restrictions.DontModify);
      miAllowExpressions.Enabled = enabled;
      miAllowExpressions.Checked = obj.AllowExpressions && enabled;
      miHideZeros.Enabled = enabled;
      miHideZeros.Checked = obj.HideZeros && enabled;
    }
  }
}
