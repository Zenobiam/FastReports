using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace FastReport.Utils
{
    /// <summary>
    /// The style of FastReport user interface.
    /// </summary>
    public enum UIStyle
    {
        /// <summary>
        /// Specifies the Microsoft Visual Studio 2005 style.
        /// </summary>
        VisualStudio2005,

        /// <summary>
        /// Specifies the Microsoft Office 2003 style (blue).
        /// </summary>
        Office2003,

        /// <summary>
        /// Specifies the Microsoft Office 2007 style (blue).
        /// </summary>
        Office2007Blue,

        /// <summary>
        /// Specifies the Microsoft Office 2007 style (silver).
        /// </summary>
        Office2007Silver,

        /// <summary>
        /// Specifies the Microsoft Office 2007 style (black).
        /// </summary>
        Office2007Black,

        /// <summary>
        /// Specifies the Microsoft Vista style (black).
        /// </summary>
        VistaGlass
    }

    internal class FRColorTable : ProfessionalColorTable
    {
        public Color ControlBackColor;
        public Color WorkspaceBackColor;
        public Color ToolWindowCaptionColor;
        public Color TabStripGradientBegin;
        public Color TabStripGradientEnd;
        public Color TabStripBorder;

        public FRColorTable()
        {
        }
    }

    internal class FRVS2005ColorTable : FRColorTable
    {
        private static Color _toolStripBorder = Color.FromArgb(111, 157, 217);

        public override Color ToolStripBorder
        {
            get { return SeparatorDark; }
        }

        public FRVS2005ColorTable()
        {
            UseSystemColors = true;
            ControlBackColor = SystemColors.Control;
            WorkspaceBackColor = SystemColors.AppWorkspace;
            ToolWindowCaptionColor = Color.FromArgb(204, 199, 186);
            TabStripGradientBegin = ToolStripGradientBegin;
            TabStripGradientEnd = ToolStripGradientEnd;
            TabStripBorder = SystemColors.ControlDark;
        }
    }

    internal class FROffice2003ColorTable : FRColorTable
    {
        private static Color _toolStripDropDownBackground = Color.FromArgb(250, 250, 250);
        private static Color _buttonPressedGradientBegin = Color.FromArgb(248, 181, 106);
        private static Color _buttonPressedGradientEnd = Color.FromArgb(255, 208, 134);
        private static Color _buttonPressedGradientMiddle = Color.FromArgb(251, 140, 60);
        private static Color _buttonSelectedGradientBegin = Color.FromArgb(255, 255, 222);
        private static Color _buttonSelectedGradientEnd = Color.FromArgb(255, 203, 136);
        private static Color _buttonSelectedGradientMiddle = Color.FromArgb(255, 225, 172);
        private static Color _menuItemSelectedGradientBegin = Color.FromArgb(255, 213, 103);
        private static Color _menuItemSelectedGradientEnd = Color.FromArgb(255, 228, 145);
        private static Color _checkBackground = Color.FromArgb(255, 227, 149);
        private static Color _gripDark = Color.FromArgb(111, 157, 217);
        private static Color _gripLight = Color.FromArgb(255, 255, 255);
        private static Color _imageMarginGradientBegin = Color.FromArgb(233, 238, 238);
        private static Color _menuBorder = Color.FromArgb(134, 134, 134);
        private static Color _overflowButtonGradientBegin = Color.FromArgb(167, 204, 251);
        private static Color _overflowButtonGradientEnd = Color.FromArgb(101, 147, 207);
        private static Color _overflowButtonGradientMiddle = Color.FromArgb(167, 204, 251);
        private static Color _menuToolBack = Color.FromArgb(191, 219, 255);
        private static Color _separatorDark = Color.FromArgb(154, 198, 255);
        private static Color _separatorLight = Color.FromArgb(255, 255, 255);
        private static Color _statusStripGradientBegin = Color.FromArgb(215, 229, 247);
        private static Color _statusStripGradientEnd = Color.FromArgb(172, 201, 238);
        private static Color _toolStripBorder = Color.FromArgb(111, 157, 217);
        private static Color _toolStripContentPanelGradientBegin = Color.FromArgb(164, 195, 235);
        private static Color _toolStripGradientBegin = Color.FromArgb(227, 239, 255);
        private static Color _toolStripGradientEnd = Color.FromArgb(152, 186, 230);
        private static Color _toolStripGradientMiddle = Color.FromArgb(222, 236, 255);
        private static Color _buttonSelectedHighlightBorder = Color.FromArgb(121, 153, 194);

        public override Color ButtonCheckedGradientBegin
        {
            get { return _buttonPressedGradientBegin; }
        }

        public override Color ButtonCheckedGradientEnd
        {
            get { return _buttonPressedGradientEnd; }
        }

        public override Color ButtonCheckedGradientMiddle
        {
            get { return _buttonPressedGradientMiddle; }
        }

        public override Color ButtonPressedGradientBegin
        {
            get { return _buttonPressedGradientBegin; }
        }

        public override Color ButtonPressedGradientEnd
        {
            get { return _buttonPressedGradientEnd; }
        }

        public override Color ButtonPressedGradientMiddle
        {
            get { return _buttonPressedGradientMiddle; }
        }

        public override Color ButtonSelectedGradientBegin
        {
            get { return _buttonSelectedGradientBegin; }
        }

        public override Color ButtonSelectedGradientEnd
        {
            get { return _buttonSelectedGradientEnd; }
        }

        public override Color ButtonSelectedGradientMiddle
        {
            get { return _buttonSelectedGradientMiddle; }
        }

        public override Color ButtonSelectedHighlightBorder
        {
            get { return _buttonSelectedHighlightBorder; }
        }

        public override Color CheckBackground
        {
            get { return _checkBackground; }
        }

        public override Color GripDark
        {
            get { return _gripDark; }
        }

        public override Color GripLight
        {
            get { return _gripLight; }
        }

        public override Color ImageMarginGradientBegin
        {
            get { return _imageMarginGradientBegin; }
        }

        public override Color MenuBorder
        {
            get { return _menuBorder; }
        }

        public override Color MenuItemPressedGradientBegin
        {
            get { return _toolStripGradientBegin; }
        }

        public override Color MenuItemPressedGradientEnd
        {
            get { return _toolStripGradientEnd; }
        }

        public override Color MenuItemPressedGradientMiddle
        {
            get { return _toolStripGradientMiddle; }
        }

        public override Color MenuItemSelectedGradientBegin
        {
            get { return _menuItemSelectedGradientBegin; }
        }

        public override Color MenuItemSelectedGradientEnd
        {
            get { return _menuItemSelectedGradientEnd; }
        }

        public override Color MenuStripGradientBegin
        {
            get { return _menuToolBack; }
        }

        public override Color MenuStripGradientEnd
        {
            get { return _menuToolBack; }
        }

        public override Color OverflowButtonGradientBegin
        {
            get { return _overflowButtonGradientBegin; }
        }

        public override Color OverflowButtonGradientEnd
        {
            get { return _overflowButtonGradientEnd; }
        }

        public override Color OverflowButtonGradientMiddle
        {
            get { return _overflowButtonGradientMiddle; }
        }

        public override Color RaftingContainerGradientBegin
        {
            get { return _menuToolBack; }
        }

        public override Color RaftingContainerGradientEnd
        {
            get { return _menuToolBack; }
        }

        public override Color SeparatorDark
        {
            get { return _separatorDark; }
        }

        public override Color SeparatorLight
        {
            get { return _separatorLight; }
        }

        public override Color StatusStripGradientBegin
        {
            get { return _statusStripGradientBegin; }
        }

        public override Color StatusStripGradientEnd
        {
            get { return _statusStripGradientEnd; }
        }

        public override Color ToolStripBorder
        {
            get { return _toolStripBorder; }
        }

        public override Color ToolStripContentPanelGradientBegin
        {
            get { return _toolStripContentPanelGradientBegin; }
        }

        public override Color ToolStripContentPanelGradientEnd
        {
            get { return _menuToolBack; }
        }

        public override Color ToolStripDropDownBackground
        {
            get { return _toolStripDropDownBackground; }
        }

        public override Color ToolStripGradientBegin
        {
            get { return _toolStripGradientBegin; }
        }

        public override Color ToolStripGradientEnd
        {
            get { return _toolStripGradientEnd; }
        }

        public override Color ToolStripGradientMiddle
        {
            get { return _toolStripGradientMiddle; }
        }

        public override Color ToolStripPanelGradientBegin
        {
            get { return _menuToolBack; }
        }

        public override Color ToolStripPanelGradientEnd
        {
            get { return _menuToolBack; }
        }

        public FROffice2003ColorTable()
        {
            ControlBackColor = Color.FromArgb(182, 208, 248);
            WorkspaceBackColor = Color.FromArgb(144, 153, 174);
            ToolWindowCaptionColor = Color.FromArgb(154, 188, 234);
            TabStripGradientBegin = ToolStripGradientBegin;
            TabStripGradientEnd = ToolStripGradientEnd;
            TabStripBorder = ToolStripBorder;
        }
    }

    /// <summary>
    /// Contains conversion methods between FastReport's UIStyle to various enums.
    /// </summary>
    public static class UIStyleUtils
    {
        private static ToolStripProfessionalRenderer FVS2005ToolStripRenderer;
        private static ToolStripProfessionalRenderer FOffice2003ToolStripRenderer;
        private static FRColorTable FVS2005ColorTable;
        private static FRColorTable FOffice2003ColorTable;

        /// <summary>
        /// Gets UI style names.
        /// </summary>
        public static readonly string[] UIStyleNames = new string[] {
        "VisualStudio 2005",  
        "Office 2003",
        "Office 2007 Blue",
        "Office 2007 Silver",
        "Office 2007 Black",
        "VistaGlass"
      };

        /// <summary>
        /// Returns toolstrip renderer for the given style.
        /// </summary>
        /// <param name="style">UI style.</param>
        /// <returns>The renderer.</returns>
        public static ToolStripProfessionalRenderer GetToolStripRenderer(UIStyle style)
        {
            if (FVS2005ToolStripRenderer == null)
            {
                FVS2005ToolStripRenderer = new ToolStripProfessionalRenderer(GetColorTable(UIStyle.VisualStudio2005));
                //        FVS2005ToolStripRenderer.RoundedEdges = false;
            }

            if (FOffice2003ToolStripRenderer == null)
            {
                FOffice2003ToolStripRenderer = new ToolStripProfessionalRenderer(GetColorTable(UIStyle.Office2003));
                //        FOffice2003ToolStripRenderer.RoundedEdges = false;
            }

            switch (style)
            {
                case UIStyle.Office2003:
                    return FOffice2003ToolStripRenderer;
                default:
                    return FVS2005ToolStripRenderer;
            }
        }

        internal static FRColorTable GetColorTable(UIStyle style)
        {
            if (FVS2005ColorTable == null)
                FVS2005ColorTable = new FRVS2005ColorTable();

            if (FOffice2003ColorTable == null)
                FOffice2003ColorTable = new FROffice2003ColorTable();

            switch (style)
            {
                case UIStyle.Office2003:
                    return FOffice2003ColorTable;
                default:
                    return FVS2005ColorTable;
            }
        }
    }
}
