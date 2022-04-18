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
  public class HtmlObjectMenu : TextObjectBaseMenu
  {
    #region Fields
    private SelectedTextBaseObjects htmlObjects;
    #endregion


    /// <summary>
    /// Initializes a new instance of the <b>TextObjectMenu</b> 
    /// class with default settings. 
    /// </summary>
    /// <param name="designer">The reference to a report designer.</param>
    public HtmlObjectMenu(Designer designer) : base(designer)
    {
      htmlObjects = new SelectedTextBaseObjects(designer);
      htmlObjects.Update();

      MyRes res = new MyRes("ComponentMenu,HtmlObject");

      miAllowExpressions.BeginGroup = false;

      int insertPos = Items.IndexOf(miFormat) + 1;
      insertPos = Items.IndexOf(miAllowExpressions);

      bool enabled = htmlObjects.Enabled;

      HtmlObject obj = htmlObjects.First as HtmlObject;
    }
  }
}
