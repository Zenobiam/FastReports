using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Design;
using FastReport.Utils;

namespace FastReport
{
    /// <summary>
    /// The base class for the context menu of the report component.
    /// </summary>
    /// <remarks>
    /// This class represents a context menu of the report component that is displayed when the object 
    /// is right-clicked in the designer. This class implements the following actions: Edit, Cut, Copy, 
    /// Paste, Delete, Bring to Front, Send to Back.
    /// </remarks>
    [ToolboxItem(false)]
    public class ComponentBaseMenu : ContextMenuBase
    {
        #region Fields
        /// <summary>
        /// The "Name" menu item.
        /// </summary>
        public ContextMenuItem miName;
        /// <summary>
        /// The "Edit" menu item.
        /// </summary>
        public ContextMenuItem miEdit;
        /// <summary>
        /// The "Cut" menu item.
        /// </summary>
        public ContextMenuItem miCut;
        /// <summary>
        /// The "Copy" menu item.
        /// </summary>
        public ContextMenuItem miCopy;
        /// <summary>
        /// The "Paste" menu item.
        /// </summary>
        public ContextMenuItem miPaste;
        /// <summary>
        /// The "Delete" menu item.
        /// </summary>
        public ContextMenuItem miDelete;
        /// <summary>
        /// The "BringToFront" menu item.
        /// </summary>
        public ContextMenuItem miBringToFront;
        /// <summary>
        /// The "SendToBack" menu item.
        /// </summary>
        public ContextMenuItem miSendToBack;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <b>ComponentBaseMenu</b> class with default settings. 
        /// </summary>
        /// <param name="designer">The reference to a report designer.</param>
        public ComponentBaseMenu(Designer designer)
            : base(designer)
        {
            miName = CreateMenuItem("");
            miEdit = CreateMenuItem(Res.Get("ComponentMenu,Component,Edit"), Designer.cmdEdit.Invoke);
            miEdit.BeginGroup = true;
            miCut = CreateMenuItem(Res.GetImage(5), Res.Get("Designer,Menu,Edit,Cut"), Designer.cmdCut.Invoke);
            miCut.BeginGroup = true;
            miCopy = CreateMenuItem(Res.GetImage(6), Res.Get("Designer,Menu,Edit,Copy"), Designer.cmdCopy.Invoke);
            miPaste = CreateMenuItem(Res.GetImage(7), Res.Get("Designer,Menu,Edit,Paste"), Designer.cmdPaste.Invoke);
            miDelete = CreateMenuItem(Res.GetImage(51), Res.Get("Designer,Menu,Edit,Delete"), Designer.cmdDelete.Invoke);
            miBringToFront = CreateMenuItem(Res.GetImage(14), Res.Get("Designer,Toolbar,Layout,BringToFront"), Designer.cmdBringToFront.Invoke);
            miBringToFront.BeginGroup = true;
            miSendToBack = CreateMenuItem(Res.GetImage(15), Res.Get("Designer,Toolbar,Layout,SendToBack"), Designer.cmdSendToBack.Invoke);

            miEdit.Visible = Designer.cmdEdit.Enabled;
            miCut.Enabled = Designer.cmdCut.Enabled;
            miCopy.Enabled = Designer.cmdCopy.Enabled;
            miPaste.Enabled = Designer.cmdPaste.Enabled;
            miDelete.Enabled = Designer.cmdDelete.Enabled;
            miBringToFront.Enabled = Designer.cmdBringToFront.Enabled;
            miSendToBack.Enabled = Designer.cmdSendToBack.Enabled;

            SelectedObjectCollection selection = Designer.SelectedObjects;
            miName.Text = (selection.Count == 1 ?
              selection[0].Name :
              String.Format(Res.Get("Designer,ToolWindow,Properties,NObjectsSelected"), selection.Count)) + ":";
            miName.SetFontBold();

            Items.AddRange(new ContextMenuItem[] {
                miName, miEdit, 
                miCut, miCopy, miPaste, miDelete,
                miBringToFront, miSendToBack });
        }
    }
}
