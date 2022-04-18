using System.Windows.Forms;

namespace FastReport
{
  partial class HeaderFooterBandBase
  {
    #region Public Methods
    /// <inheritdoc/>
    public override ContextMenuBase GetContextMenu()
    {
      return new HeaderFooterBandBaseMenu(Report.Designer);
    }
    #endregion
  }
}
