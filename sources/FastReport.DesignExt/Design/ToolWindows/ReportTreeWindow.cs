using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Controls;
#if !MONO
using FastReport.DevComponents.DotNetBar;
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Design.ToolWindows
{
    /// <summary>
    /// Represents the "Report Tree" window.
    /// </summary>
    public class ReportTreeWindow : ToolWindowBase
    {
        #region Fields
        private TreeView tree;
        private List<Base> components;
        private List<TreeNode> nodes;
        private List<TreeNode> nodesSelected;
        private TreeNode nodeTree;
        private int lastFromitem;
        private int lastToitem;
        private bool lastNeedReverse;
        private bool updating;
        private bool LastData;
        private bool passSelection = false;
        private bool cancelNextSelection = false;
        private bool ShiftNonSelect = false;
        private bool RewriteSelect = false;
        private bool mouseButtonRight = false;

        #endregion

        #region Private Methods
        private void UpdateTree()
        {
            // if there was no changes in the report structure, do nothing
            if (Designer.ActiveReport != null && tree.Nodes.Count > 0)
            {
                if (CheckChanges(Designer.ActiveReport, tree.Nodes[0]))
                    return;
            }

            updating = true;
            tree.BeginUpdate();
            tree.Nodes.Clear();
            components.Clear();
            nodes.Clear();
            if (Designer.ActiveReport != null)
                EnumComponents(Designer.ActiveReport, tree.Nodes);
            tree.ExpandAll();
            tree.EndUpdate();
            updating = false;
        }

        private void UpdateSelection()
        {
            if (updating)
                return;
            if (Designer.SelectedObjects == null || Designer.SelectedObjects.Count == 0)
                return;
            
            Base c = Designer.SelectedObjects[Designer.SelectedObjects.Count-1];
            int i = components.IndexOf(c);
            if (i != -1)
            {
                unpaintSelectedNodes();
                updating = true;
                tree.SelectedNode = nodes[i];
                nodesSelected.Clear();
                foreach (Base b in Designer.SelectedObjects)
                {
                    nodesSelected.Add(nodes[components.IndexOf(b)]);
                }
                updating = false;
                paintSelectedNodes();
            }
        }

#if !MONO
        private void Reinit(float ratio = 0)
        {
            if (ratio == 0)
                ratio = DpiHelper.Multiplier;
            base.ReinitDpiSize();
            updating = true;
            tree.BeginUpdate();
            tree.Nodes.Clear();
            components.Clear();
            tree.Margin = DpiHelper.ConvertUnits(new System.Windows.Forms.Padding(3));
            tree.ImageList = GetCloneImageList();
            tree.Indent = 3 + tree.ImageList.ImageSize.Width;
            tree.ItemHeight = tree.ImageList.ImageSize.Width;
            Image = DpiHelper.ConvertButton16(Res.GetImage(189));
            nodes.Clear();
            if (Designer.ActiveReport != null)
                EnumComponents(Designer.ActiveReport, tree.Nodes);
            tree.ExpandAll();
            tree.EndUpdate();
            updating = false;
            tree.Refresh();
        }

#endif

        void paintSelectedNodes()
        {
            Color backColor = SystemColors.Highlight;
            Color foreColor = SystemColors.HighlightText;

            foreach (TreeNode node in nodesSelected)
            {
                node.BackColor = backColor;
                node.ForeColor = foreColor;
            }
        }
        void unpaintSelectedNodes()
        {
            foreach (TreeNode node in nodesSelected)
            {
                node.BackColor = tree.BackColor;
                node.ForeColor = tree.ForeColor;
            }
        }

        private void EnumComponents(Base rootComponent, TreeNodeCollection rootNode)
        {
            string name = rootComponent is Report ?
              "Report - " + Designer.ActiveReportTab.ReportName : rootComponent.Name;
            TreeNode node = rootNode.Add(name);
            node.Tag = rootComponent;

            components.Add(rootComponent);
            nodes.Add(node);

            ObjectInfo objItem = RegisteredObjects.FindObject(rootComponent);
            if (objItem != null)
            {
                int imageIndex = objItem.ImageIndex;
                node.ImageIndex = imageIndex;
                node.SelectedImageIndex = imageIndex;
            }

            if (rootComponent.HasFlag(Flags.CanShowChildrenInReportTree))
            {
                foreach (Base component in rootComponent.ChildObjects)
                    EnumComponents(component, node.Nodes);
            }
        }

        private bool CheckChanges(Base rootComponent, TreeNode rootNode)
        {
            if (rootNode.Tag != rootComponent)
                return false;
            if (!(rootComponent is Report))
            {
                if (rootNode.Text != rootComponent.Name)
                    return false;
            }
            if (!rootComponent.HasFlag(Flags.CanShowChildrenInReportTree))
                return true;

            ObjectCollection childObjects = rootComponent.ChildObjects;
            if (childObjects.Count != rootNode.Nodes.Count)
                return false;
            for (int i = 0; i < childObjects.Count; i++)
            {
                if (!CheckChanges(childObjects[i], rootNode.Nodes[i]))
                    return false;
            }
            return true;
        }

        private void tree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (passSelection)
                return;
            if (updating)
                return;
            if (Designer.SelectedObjects != null)
            {
                if (Control.ModifierKeys == Keys.Control && !mouseButtonRight)
                {
                    if(Designer.SelectedObjects[0] is BandBase || Designer.SelectedObjects[0] is ReportPage)
                    {
                        Designer.SelectedObjects.Clear();
                    }
                    if (Designer.SelectedObjects.Contains(e.Node.Tag as Base))
                    {
                        Designer.SelectedObjects.Remove(e.Node.Tag as Base);
                        LastData = false;
                        Designer.SelectionChanged(this);
                        UpdateSelection();
                        return;
                    }
                    else
                    {
                        if (e.Node.Tag is BandBase)
                        {
                            return;
                        }
                        Designer.SelectedObjects.Add(e.Node.Tag as Base);
                        nodeTree = e.Node;
                        LastData = false;
                        Designer.SelectionChanged(this);
                        UpdateSelection();
                        return;

                    }
                }

                Base c = e.Node.Tag as Base;

                if (Control.ModifierKeys == Keys.Shift)
                {
                    if (nodeTree != null && isSameLevel(nodeTree, e.Node))
                    {
                        List<TreeNode> nodeBas = nodes;
                        int nodeTreeIndex = getNodeIndexInNodes(nodeTree);
                        int eNodeTreeIndex = getNodeIndexInNodes(e.Node);
                        int from = Math.Min(nodeTreeIndex, eNodeTreeIndex);
                        int to = Math.Max(nodeTreeIndex, eNodeTreeIndex);
                        bool needReverse = nodeTreeIndex > eNodeTreeIndex;

                        if (!(c is Report))
                            Designer.ActiveReportTab.ActivePage = c.Page;
                        if (RewriteSelect)
                        {
                            Designer.SelectedObjects.Clear();
                            RewriteSelect = false;
                        }
                        if (from == lastFromitem && LastData)
                        {
                            if (to < lastToitem)
                            {
                                Designer.SelectedObjects.Clear();
                                for (int i = from; i <= to; i++)
                                {
                                    if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                    {
                                        continue;
                                    }

                                    Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                }
                            }
                            else
                            {
                                for (int i = lastToitem + 1; i <= to; i++)
                                {
                                    if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                    {
                                        continue;
                                    }
                                    Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                }
                            }
                        }
                        if (to == lastToitem && LastData)
                        {
                            if (needReverse)
                            {
                                if (from > lastFromitem)
                                {
                                    Designer.SelectedObjects.Clear();
                                    for (int i = to; i >= from; i--)
                                    {
                                        if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                        {
                                            continue;
                                        }
                                        Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                    }
                                }
                                else
                                {

                                    for (int i = lastFromitem - 1; i >= from; i--)
                                    {
                                        if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                            continue;

                                        Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                    }
                                }
                            }
                        }

                        if (LastData == false || lastNeedReverse != needReverse)
                        {
                            Designer.SelectedObjects.Clear();
                            if (needReverse)
                            {
                                for (int i = to; i >=from ; i--)
                                {
                                    if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                        continue;

                                    Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                }
                                
                            }
                            else
                            {
                                for (int i = from; i <= to; i++)
                                {
                                    if (nodeBas[i].Tag is BandBase || nodeBas[i].Tag is ReportPage || nodeBas[i].Tag is Report)
                                        continue;

                                    Designer.SelectedObjects.Add(nodeBas[i].Tag as Base);
                                }
                            }
                        }

                        if (Designer.SelectedObjects.Count == 0)
                        {
                            Designer.SelectedObjects.Add(nodeTree.Tag as Base);
                            tree.SelectedNode = nodeTree;
                            RewriteSelect = true;
                        }
                        

                        LastData = true;
                        lastFromitem = from;
                        lastToitem = to;
                        lastNeedReverse = needReverse;
                        tree.SelectedNode = nodes[(components.IndexOf(Designer.SelectedObjects[Designer.SelectedObjects.Count - 1]))];
                        Designer.SelectionChanged(this);
                        UpdateSelection();

                    }
                }
                else
                {
                    LastData = false;
                    updating = true;
                    if (!(c is Report))
                        Designer.ActiveReportTab.ActivePage = c.Page;
                    updating = false;

                    nodeTree = e.Node;
                    Designer.SelectedObjects.Clear();
                    Designer.SelectedObjects.Add(c);
                    Designer.SelectionChanged(null);
                }
            }
        }

        private int getNodeIndexInNodes(TreeNode node1)
        {
            int i = 0;
            foreach (TreeNode itemNode in nodes)
            {
                if (itemNode == node1)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        private bool isSameLevel(TreeNode node1, TreeNode node2)
        {
            foreach (TreeNode element in nodes)
            {
                if (element == node2)
                {
                    return true;
                }

            }
            return false;
        }



        private void tree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            SelectedObjectCollection draggedComponent = Designer.SelectedObjects;
            foreach (Base itemDragComp in draggedComponent)
            {
                if (itemDragComp is ComponentBase &&
                  (itemDragComp.IsAncestor || itemDragComp.HasFlag(Flags.CanChangeParent)))
                {


                    tree.DoDragDrop(draggedComponent, DragDropEffects.Move);
                }
                else
                {
                    tree.DoDragDrop(draggedComponent, DragDropEffects.None);

                }

            }

        }

        private void tree_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode targetNode = tree.GetNodeAt(tree.PointToClient(new Point(e.X, e.Y)));
            Base targetComponent = targetNode.Tag as Base;

            // cases: 
            // - target can contain dragged. Just change parent.
            // - target cannot contain dragged. Change creation order (Z-order).

            foreach (Base item in Designer.SelectedObjects)
            {
                if (targetComponent is IParent && (targetComponent as IParent).CanContain(item))
                {
                    item.Parent = targetComponent;
                }
                else
                {
                    Base parent = targetComponent.Parent;
                    item.Parent = parent;
                    item.ZOrder = targetComponent.ZOrder;
                }
            }

            // update all designer plugins (this one too)

            Designer.SetModified(null, "ChangeParent");
            Base b = Designer.SelectedObjects[Designer.SelectedObjects.Count - 1];
            int i = components.IndexOf(b);
            nodeTree = nodes[i];
            UpdateSelection();
        }

        private void tree_DragOver(object sender, DragEventArgs e)
        {
            List<TreeNode> draggedNode = (List<TreeNode>)e.Data.GetData(typeof(List<TreeNode>));
            TreeNode targetNode = (tree.GetNodeAt(tree.PointToClient(new Point(e.X, e.Y))));
            if (targetNode == null)
            {
                return;
            }
            SelectedObjectCollection draggedComponent = Designer.SelectedObjects;
            Base targetComponent = targetNode.Tag as Base;

            // allowed moves are:
            // - target is not dragged
            // - target is not child of dragged
            // - target can contain dragged, or 
            // parent of target can contain dragged

            foreach (Base itemDragComp in draggedComponent)
            {
                if (itemDragComp != targetComponent &&
                  !targetComponent.HasParent(itemDragComp) &&
                  (((targetComponent is IParent) && (targetComponent as IParent).CanContain(itemDragComp) ||
                  (targetComponent.Parent != null && (targetComponent.Parent as IParent).CanContain(itemDragComp)))))
                {

                    e.Effect = e.AllowedEffect;
                }
                else
                {
                    e.Effect = DragDropEffects.None;
                }
            }

            // disable the Designer.OnSelectionChanged

            updating = true;
            tree.SelectedNode = targetNode;
            updating = false;

        }

        private void tree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
                Designer.cmdDelete.Invoke();
        }



        private void tree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (Designer.SelectedObjects.Count == 1 && Control.ModifierKeys == Keys.None)
            {
                Base c = e.Node.Tag as Base;
                if (c is Report)
                    e.CancelEdit = false;
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        private void tree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (Designer.SelectedObjects.Count == 1 && Control.ModifierKeys == Keys.None)
            {
                if (e.Label != null)
                {
                    Base c = e.Node.Tag as Base;
                    string saveName = c.Name;
                    try
                    {
                        c.Name = e.Label;
                        Designer.SetModified(this, "Change");
                    }
                    catch (Exception ex)
                    {
                        FRMessageBox.Error(ex.Message);
                        e.CancelEdit = true;
                    }
                }
            }
            else
            {
                e.CancelEdit = true;
            }
        }

        private void FTree_MouseUp(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                mouseButtonRight = true;
            }
            else
            {
                mouseButtonRight = false;
            }

            if (Control.ModifierKeys == Keys.Control && e.Button != MouseButtons.Right)
            {
                if (e.Node.Tag is BandBase || e.Node.Tag is ReportPage || e.Node.Tag is Report)
                {
                    ShiftNonSelect = true;
                    Designer.SelectionChanged(this);
                    UpdateSelection();
                    cancelNextSelection = true;
                    return;
                }

                if (tree.SelectedNode == e.Node && Designer.SelectedObjects.Count > 1 && Designer.SelectedObjects.Contains(e.Node.Tag as Base))
                {
                passSelection = true;
                tree.SelectedNode = nodes[(components.IndexOf(Designer.SelectedObjects[Designer.SelectedObjects.Count - 2]))];
                nodeTree = nodes[(components.IndexOf(Designer.SelectedObjects[Designer.SelectedObjects.Count - 2]))];
                LastData = false;
                passSelection = false;

                Designer.SelectedObjects.Remove(e.Node.Tag as Base);
                Designer.SelectionChanged(this);
                UpdateSelection();
                cancelNextSelection = true;
                }
                return;
            }
            else
            {
                if (Designer.SelectedObjects.Count > 1 && e.Node == tree.SelectedNode && e.Button != MouseButtons.Right)
                {
                    Designer.SelectedObjects.Clear();
                    Designer.SelectedObjects.Add(e.Node.Tag as Base);
                    nodeTree = e.Node;
                    LastData = false;
                    Designer.SelectionChanged(null);
                    UpdateSelection();
                    
                }
                
            }
            if (e.Button == MouseButtons.Right)
            {
                tree.SelectedNode = tree.GetNodeAt(e.Location);
                if (tree.SelectedNode != null)
                {
                    ContextMenuBase menu = (tree.SelectedNode.Tag as Base).GetContextMenu();
                    if (menu != null)
                    {
                        menu.Show(tree, e.Location);
                    }
                }
            }
        }
        private void tree_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (updating || passSelection)
                return;
            if (cancelNextSelection)
            {
                e.Cancel = true;
                cancelNextSelection = false;
                return;
            }
            if (Control.ModifierKeys == Keys.Control && Designer.SelectedObjects.Contains(e.Node.Tag as Base)&&!ShiftNonSelect)
            {
                e.Cancel = true;
                Designer.SelectedObjects.Remove(e.Node.Tag as Base);
                Designer.SelectionChanged(null);
                UpdateSelection();
                return;
            }
        }
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            UpdateSelection();
        }

        /// <inheritdoc/>
        public override void UpdateContent()
        {
            UpdateTree();
            UpdateSelection();
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            Text = Res.Get("Designer,ToolWindow,ReportTree");
        }
#if !MONO
        /// <inheritdoc/>
        public override void ReinitDpiSize()
        {
            if (!Bar.Docked)
                return;
            Reinit();
        }

        ///<inheritdoc/>
        public override void CallReinit(float ratio)
        {
            Reinit(ratio);
        }
#endif
#endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportTreeWindow"/> class with default settings.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public ReportTreeWindow(Designer designer) : base(designer)
        {
            Name = "ReportTreeWindow";
#if !MONO
            Image = Res.GetImage(189);
#endif
            components = new List<Base>();
            nodes = new List<TreeNode>();
            nodesSelected = new List<TreeNode>();

            tree = new TreeView();
            tree.Dock = DockStyle.Fill;
            tree.BorderStyle = BorderStyle.None;
            tree.ShowRootLines = false;
            tree.HideSelection = true;
            tree.LabelEdit = true;
            tree.Margin = DpiHelper.ConvertUnits(new System.Windows.Forms.Padding(3));
            tree.ImageList = Res.GetImages();
            tree.Indent = 3 + tree.ImageList.ImageSize.Width;
            tree.ItemHeight = tree.ImageList.ImageSize.Width;
            tree.AllowDrop = true;
            tree.AfterSelect += tree_AfterSelect;
            tree.BeforeSelect += tree_BeforeSelect;
            tree.ItemDrag += tree_ItemDrag;
            tree.DragOver += tree_DragOver;
            tree.DragDrop += tree_DragDrop;
            tree.KeyDown += tree_KeyDown;
            tree.BeforeLabelEdit += tree_BeforeLabelEdit;
            tree.AfterLabelEdit += tree_AfterLabelEdit;
            tree.NodeMouseClick += FTree_MouseUp;
#if !MONO
            ParentControl.Controls.Add(tree);
#else
			Controls.Add(tree);
#endif
            Localize();
        }
    }
}
