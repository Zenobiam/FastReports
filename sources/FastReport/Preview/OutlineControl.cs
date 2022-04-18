using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using FastReport.Utils;
using FastReport.DevComponents.AdvTree;
using FastReport.DevComponents.DotNetBar;

namespace FastReport.Preview
{
  internal class OutlineControl : UserControl
  {
    private AdvTree tree;
    private NodeConnector nodeConnector1;
    private ElementStyle elementStyle1;
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

        eScrollBarAppearance appearance = eScrollBarAppearance.Default;
        if (tree.ColorSchemeStyle == eColorSchemeStyle.Office2007)
          appearance = eScrollBarAppearance.ApplicationScroll;
        if (tree.HScrollBar != null)
          tree.HScrollBar.Appearance = appearance;
        if (tree.VScrollBar != null)
          tree.VScrollBar.Appearance = appearance;

        tree.Refresh();
      }
    }

    private void EnumNodes(XmlItem rootItem, NodeCollection rootNode)
    {
      // skip root xml item
      if (rootItem.Parent != null)
      {
        string text = rootItem.GetProp("Text");
        Node node = new Node();
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

    private void FTree_AfterNodeSelect(object sender, AdvTreeNodeEventArgs e)
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
      nodeConnector1 = new NodeConnector();
      nodeConnector1.LineColor = SystemColors.ControlText;
      elementStyle1 = new ElementStyle();
      elementStyle1.TextColor = SystemColors.ControlText;

      tree = new AdvTree();
      tree.LicenseKey = "F962CEC7-CD8F-4911-A9E9-CAB39962FC1F";
      tree.AntiAlias = false;
      tree.Font = DrawUtils.DefaultFont;
      tree.BackColor = SystemColors.Window;
      tree.Dock = DockStyle.Fill;
      tree.HideSelection = false;
      tree.HotTracking = true;
      tree.NodesConnector = nodeConnector1;
      tree.NodeStyle = elementStyle1;
      tree.Styles.Add(elementStyle1);
      tree.AfterNodeSelect += new AdvTreeNodeEventHandler(FTree_AfterNodeSelect);

      Controls.Add(tree);
    }
  }
}
