using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Dialog
{
  /// <summary>
  /// Displays a hierarchical collection of labeled items, each represented by a TreeNode.
  /// Wraps the <see cref="System.Windows.Forms.TreeView"/> control.
  /// </summary>
  public partial class TreeViewControl : DialogControl
  {
    private TreeView treeView;
    private string afterSelectEvent;
    
    #region Properties
    /// <summary>
    /// Occurs after the tree node is selected.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.AfterSelect"/> event.
    /// </summary>
    public event TreeViewEventHandler AfterSelect;

    /// <summary>
    /// Gets an internal <b>TreeView</b>.
    /// </summary>
    [Browsable(false)]
    public TreeView TreeView
    {
      get { return treeView; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether check boxes are displayed next to the tree nodes in the tree view control.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.CheckBoxes"/> property.
    /// </summary>
    [DefaultValue(false)]
    [Category("Appearance")]
    public bool CheckBoxes
    {
      get { return TreeView.CheckBoxes; }
      set { TreeView.CheckBoxes = value; (DrawControl as TreeView).CheckBoxes = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether lines are drawn between tree nodes in the tree view control.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.ShowLines"/> property.
    /// </summary>
    [DefaultValue(true)]
    [Category("Behavior")]
    public bool ShowLines
    {
      get { return TreeView.ShowLines; }
      set { TreeView.ShowLines = value; (DrawControl as TreeView).ShowLines = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether lines are drawn between the tree nodes that are at the root of the tree view.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.ShowRootLines"/> property.
    /// </summary>
    [DefaultValue(true)]
    [Category("Behavior")]
    public bool ShowRootLines
    {
      get { return TreeView.ShowRootLines; }
      set { TreeView.ShowRootLines = value; (DrawControl as TreeView).ShowRootLines = value; }
    }

    /// <summary>
    /// Gets or sets the <b>ImageList</b> that contains the <b>Image</b> objects used by the tree nodes.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.ImageList"/> property.
    /// </summary>
    [Browsable(false)]
    public ImageList ImageList
    {
      get { return TreeView.ImageList; }
      set { TreeView.ImageList = value; (DrawControl as TreeView).ImageList = value; }
    }

    /// <summary>
    /// Gets the collection of tree nodes that are assigned to the tree view control.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.Nodes"/> property.
    /// </summary>
    [Browsable(false)]
    public TreeNodeCollection Nodes 
    {
      get { return TreeView.Nodes; }
    }

    /// <summary>
    /// Gets or sets the tree node that is currently selected in the tree view control.
    /// Wraps the <see cref="System.Windows.Forms.TreeView.SelectedNode"/> property.
    /// </summary>
    [Browsable(false)]
    public TreeNode SelectedNode 
    {
      get { return TreeView.SelectedNode; }
      set { TreeView.SelectedNode = value; }
    }

    /// <summary>
    /// Gets or sets a script method name that will be used to handle the 
    /// <see cref="AfterSelect"/> event.
    /// </summary>
    [Category("Events")]
    public string AfterSelectEvent
    {
      get { return afterSelectEvent; }
      set { afterSelectEvent = value; }
    }

    #endregion

    #region Private Methods
    private void TreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
      OnAfterSelect(e);
    }
        #endregion

        #region Protected Methods

    /// <inheritdoc/>
    protected override void AttachEvents()
    {
      base.AttachEvents();
      TreeView.AfterSelect += new TreeViewEventHandler(TreeView_AfterSelect);
    }

    /// <inheritdoc/>
    protected override void DetachEvents()
    {
      base.DetachEvents();
      TreeView.AfterSelect -= new TreeViewEventHandler(TreeView_AfterSelect);
    }
    #endregion
    
    #region Public Methods

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer)
    {
      TreeViewControl c = writer.DiffObject as TreeViewControl;
      base.Serialize(writer);

      if (CheckBoxes != c.CheckBoxes)
        writer.WriteBool("CheckBoxes", CheckBoxes);
      if (ShowLines != c.ShowLines)
        writer.WriteBool("ShowLines", ShowLines);
      if (ShowRootLines != c.ShowRootLines)
        writer.WriteBool("ShowRootLines", ShowRootLines);
    }

    /// <summary>
    /// This method fires the <b>AfterSelect</b> event and the script code connected to the <b>AfterSelectEvent</b>.
    /// </summary>
    /// <param name="e">Event data.</param>
    public virtual void OnAfterSelect(TreeViewEventArgs e)
    {
      if (AfterSelect != null)
        AfterSelect(this, e);
      InvokeEvent(AfterSelectEvent, e);
    }
        #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TreeViewControl"/> class with default settings. 
    /// </summary>
    public TreeViewControl()
    {
      treeView = new TreeView();
      Control = treeView;
      TreeView.HideSelection = false;

            DrawControl = new TreeView();
            (DrawControl as TreeView).HideSelection = false;
            DrawControl.Size = new Size((int)(DpiScale * Control.Width), (int)(DpiScale * Control.Height));
    }
  }
}
