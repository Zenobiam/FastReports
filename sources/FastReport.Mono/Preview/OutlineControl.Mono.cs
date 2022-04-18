using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;

namespace FastReport.Preview
{
  internal class OutlineControl : UserControl
  {
    private TreeView tree;
    private PreviewControl preview;
    private PreparedPages preparedPages;
    private UIStyle style;
    
    internal PreparedPages PreparedPages
    {
      get { return preparedPages; }
      set
      {
        preparedPages = value;
        UpdateContent();
      }
    }
    
    internal UIStyle Style
    {
      get { return style; }
      set
      {
        style = value;
        tree.Refresh();
      }
    }

    private void EnumNodes(XmlItem rootItem, TreeNodeCollection rootNode)
    {
      // skip root xml item
      if (rootItem.Parent != null)
      {
        string text = Converter.FromXml(rootItem.GetProp("Text"));
        TreeNode node = new TreeNode();
        node.Text = text;
        node.Tag = rootItem;
        rootNode.Add(node);
        rootNode = node.Nodes;
      }
      
      for (int i = 0; i < rootItem.Count; i++)
      {
        EnumNodes(rootItem[i], rootNode);
      }
    }
    
    private void UpdateContent()
    {
      tree.Nodes.Clear();
      if (PreparedPages == null)
        return;
      
      Outline outline = PreparedPages.Outline;
      tree.BeginUpdate();
      EnumNodes(outline.Xml, tree.Nodes);
      if (tree.Nodes.Count == 1)
        tree.Nodes[0].Expand();
      tree.EndUpdate();
      
      // to update tree's scrollbars
      Style = Style;
    }

    private void FTree_AfterSelect(object sender, TreeViewEventArgs e)
    {
      // avoid bug when closing the preview
      if (!Visible)
        return;
      if (e.Node == null)
        return;
        
      XmlItem item = e.Node.Tag as XmlItem;
      string s = item.GetProp("Page");
      if (s != "")
      {
        int pageNo = int.Parse(s);
        s = item.GetProp("Offset");
        if (s != "")
        {
          float offset = (float)Converter.FromString(typeof(float), s);
          preview.PositionTo(pageNo + 1, new PointF(0, offset));
        }
      }
    }

    internal void SetPreview(PreviewControl preview)
    {
      this.preview = preview;
    }
    
    /// <summary>
    /// Initializes a new instance of the <b>OutlineControl</b> class with default settings. 
    /// </summary>
    public OutlineControl()
    {
      tree = new TreeView();
      tree.Font = DrawUtils.DefaultFont;
      tree.BackColor = SystemColors.Window;
      tree.Dock = DockStyle.Fill;
      tree.HideSelection = false;
      tree.HotTracking = true;
      tree.AfterSelect += new TreeViewEventHandler(FTree_AfterSelect);

      Controls.Add(tree);
    }
  }
}
