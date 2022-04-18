using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Controls
{
    /// <summary>
    /// Split Container Control
    /// </summary>
    public class FRSplitContainer : System.Windows.Forms.SplitContainer
#if !DOTNET_4
        , System.ComponentModel.ISupportInitialize
#endif
    {
        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public FRSplitContainer() : base() { }

        #endregion Constructor

        #region ISupportInitialize Methods

#if !DOTNET_4

        public void BeginInit() { }

        public void EndInit() { }

#endif

        #endregion ISupportInitialize Methods
    }
}
