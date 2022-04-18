using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Forms;
using FastReport.Utils;

namespace FastReport.Wizards
{
  /// <summary>
  /// Represents the "Label" wizard.
  /// </summary>
  public class LabelWizard : WizardBase
  {
    private string selectedManufacturer;
    private string selectedLabelName;
    private XmlItem selectedLabelParameters;

    /// <summary>
    /// Gets a selected label manufacturer.
    /// </summary>
    public string SelectedManufacturer
    {
      get { return selectedManufacturer; }
    }

    /// <summary>
    /// Gets a selected label name.
    /// </summary>
    public string SelectedLabelName
    {
      get { return selectedLabelName; }
    }

    /// <summary>
    /// Gets the XML item containing a selected label parameters.
    /// </summary>
    public XmlItem SelectedLabelParameters
    {
      get { return selectedLabelParameters; }
    }

    /// <inheritdoc/>
    public override bool Run(Designer designer)
    {
      if (!designer.CreateEmptyReport())
        return false;

      using (LabelWizardForm form = new LabelWizardForm())
      {
        form.InitWizard(designer.ActiveReport);
        bool result = form.ShowDialog() == DialogResult.OK;
        if (result)
        {
          selectedManufacturer = form.SelectedManufacturer;
          selectedLabelName = form.SelectedLabelName;
          selectedLabelParameters = form.SelectedLabelParameters;
        }
        return result;
      }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LabelWizard"/> class with the default settings.
    /// </summary>
    public LabelWizard()
    {
      selectedManufacturer = "";
      selectedLabelName = "";
    }
  }
}
