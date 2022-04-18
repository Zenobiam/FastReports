using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Data.Common;
using FastReport.Utils;

namespace FastReport.Forms
{
  /// <summary>
  /// The "Advanced Connection Properties" form.
  /// </summary>
  public partial class AdvancedConnectionPropertiesForm : BaseDialogForm
  {
    private DbConnectionStringBuilder advancedProperties;
    
    /// <summary>
    /// Gets or sets the connection string builder which contains the connection properties.
    /// </summary>
    public DbConnectionStringBuilder AdvancedProperties
    {
      get { return advancedProperties; }
      set
      {
        advancedProperties = value;
        frPropertyGrid1.SelectedObject = value;
      }
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="AdvancedConnectionPropertiesForm"/> class.
    /// </summary>
    public AdvancedConnectionPropertiesForm()
    {
      InitializeComponent();
      Localize();
      Text = Res.Get("Forms,AdvancedConnectionProperties");
            Scale();
    }
        /// <inheritdoc/>
        protected override void Scale()
        {
            base.Scale();
            ScaleFormSize();
        }
    }
}

