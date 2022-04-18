using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport
{
  partial class PageBase
  {
    #region Properties
    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public new Restrictions Restrictions
    {
      get { return base.Restrictions; }
      set { base.Restrictions = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override AnchorStyles Anchor
    {
      get { return base.Anchor; }
      set { base.Anchor = value; }
    }

    /// <summary>
    /// This property is not relevant to this class.
    /// </summary>
    [Browsable(false)]
    public override DockStyle Dock
    {
      get { return base.Dock; }
      set { base.Dock = value; }
    }

    /// <summary>
    /// Gets the snap size for this page.
    /// </summary>
    [Browsable(false)]
    public virtual SizeF SnapSize
    {
      get { return new SizeF(0, 0); }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Gets a page designer for this page type.
    /// </summary>
    /// <returns>The page designer.</returns>
    public abstract Type GetPageDesignerType();

    /// <summary>
    /// This method is called by the designer when you create a new page. 
    /// </summary>
    /// <remarks>
    /// You may create the default page layout (add default bands, set default page size, etc).
    /// </remarks>
    public virtual void SetDefaults()
    {
      Config.DesignerSettings.OnPageAdded(this, EventArgs.Empty);
    }
    
    #endregion
  }
}