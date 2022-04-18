using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;
#if !MONO
using FastReport.DevComponents;
#else
using FastReport.MonoCap;
#endif

namespace FastReport.Forms
{
#if !COMMUNITY
    public partial class ExportsOptionsEditorForm : BaseDialogForm
    {
        #region Helper functions

        private TreeNode createTreeNode(ExportsOptions.ExportsTreeNode node)
        {
            TreeNode newNode = new TreeNode(node.ToString());
            // Using out of range index because there is no other way to draw node without image
            newNode.ImageIndex = node.ImageIndex == -1 ? tvExportsMenu.ImageList.Images.Count : node.ImageIndex;
            newNode.SelectedImageIndex = newNode.ImageIndex;
            newNode.Tag = node;
            setChecked(newNode, node.Enabled);
            return newNode;
        }

        private void fillExportsMenu(List<ExportsOptions.ExportsTreeNode> nodes, TreeNodeCollection currentTreeLayer)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                currentTreeLayer.Add(createTreeNode(nodes[i]));
                if (nodes[i].Nodes.Count > 0)
                {
                    fillExportsMenu(nodes[i].Nodes, currentTreeLayer[i].Nodes);
                }
            }
        }

        private bool isCategory(TreeNode node)
        {
            return (node.Tag as ExportsOptions.ExportsTreeNode).ExportType == null;
        }

        private bool isDraggable(TreeNode node)
        {
            return (node.Tag as ExportsOptions.ExportsTreeNode).IsExport;
        }

        private bool containsNode(TreeNode targetNode, TreeNode draggedNode)
        {
            while (targetNode != null)
            {
                if (targetNode.Equals(draggedNode))
                {
                    return true;
                }
                targetNode = targetNode.Parent;
            }
            return false;
        }

        private bool arePathsEqual(List<int> lhs, List<int> rhs)
        {
            if (lhs == null || rhs == null || lhs.Count != rhs.Count)
            {
                return false;
            }

            for (int i = 0; i < lhs.Count; ++i)
            {
                if (lhs[i] != rhs[i])
                {
                    return false;
                }
            }

            return true;
        }

        private List<int> getNodePath(TreeNode node)
        {
            List<int> res = new List<int>();

            while (node != null)
            {
                res.Insert(0, node.Index);
                node = node.Parent;
            }

            return res;
        }

        private InsertionPosition getInsertionPosition(TreeNode targetNode, Point cursorPoint)
        {
            int offsetY = cursorPoint.Y - targetNode.Bounds.Y;
            if (isCategory(targetNode))
            {
                if (offsetY < targetNode.Bounds.Height / 3)
                {
                    return InsertionPosition.Top;
                }
                else if (offsetY < (2 * targetNode.Bounds.Height) / 3)
                {
                    return InsertionPosition.Inside;
                }
                else
                {
                    return InsertionPosition.Bottom;
                }
            }
            else
            {
                if (offsetY < targetNode.Bounds.Height / 2)
                {
                    return InsertionPosition.Top;
                }
                else
                {
                    return InsertionPosition.Bottom;
                }
            }
        }

        private void drawInsertionLine(InsertionPosition insertionPosition, List<int> treeNodePath)
        {
            tvExportsMenu.Refresh();
            using (Graphics g = tvExportsMenu.CreateGraphics())
            {

                if (insertionPosition == InsertionPosition.Inside)
                {
                    return;
                }

                TreeNode treeNode = null;
                TreeNodeCollection nodes = tvExportsMenu.Nodes;
                foreach (int index in treeNodePath)
                {
                    treeNode = nodes[index];
                    nodes = treeNode.Nodes;
                }

                if (treeNode != null)
                {

                    int lineStartX = treeNode.Bounds.X - 50;
                    int lineEndX = tvExportsMenu.Bounds.Width;
                    int lineY = treeNode.Bounds.Y;
                    if (insertionPosition == InsertionPosition.Bottom)
                    {
                        lineY += treeNode.Bounds.Height;
                    }
                    using (Pen pen = new Pen(Brushes.Black, 2))
                    {
                        g.DrawLine(pen, new Point(lineStartX, lineY), new Point(lineEndX, lineY));
                    }
                }
            }
        }

        private void passChanges(TreeNodeCollection tvNodes, List<ExportsOptions.ExportsTreeNode> menuNodes)
        {
            foreach (TreeNode node in tvNodes)
            {
                ExportsOptions.ExportsTreeNode menuNode = node.Tag as ExportsOptions.ExportsTreeNode;
                menuNode.Enabled = node.Checked;
                menuNode.Nodes.Clear();
                menuNodes.Add(menuNode);
                if (node.Nodes.Count > 0)
                {
                    passChanges(node.Nodes, menuNode.Nodes);
                }
            }
        }

        private void passChanges(TreeNodeCollection tvNodes)
        {
            foreach (TreeNode node in tvNodes)
            {
                ExportsOptions.ExportsTreeNode menuNode = node.Tag as ExportsOptions.ExportsTreeNode;
                if (menuNode.Name == "Cloud")
                {
                    options.CloudMenu.Enabled = node.Checked;
                    options.CloudMenu.Nodes.Clear();
                    passChanges(node.Nodes, menuNode.Nodes);
                }
                else if (menuNode.Name == "Messengers")
                {
                    options.MessengerMenu.Enabled = node.Checked;
                    options.MessengerMenu.Nodes.Clear();
                    passChanges(node.Nodes, menuNode.Nodes);
                }
                else
                {
                    menuNode.Enabled = node.Checked;
                    menuNode.Nodes.Clear();
                    options.ExportsMenu.Add(menuNode);
                    if (node.Nodes.Count > 0)
                    {
                        passChanges(node.Nodes, menuNode.Nodes);
                    }
                }
            }
        }

        private bool isSingleChecked(TreeNode testedNode)
        {
            if (testedNode.Parent != null)
            {
                foreach (TreeNode node in testedNode.Parent.Nodes)
                {
                    if (node != testedNode && node.Checked)
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        // Because seting Checked property raises AfterCheck event
        private void setChecked(TreeNode node, bool isChecked)
        {
            tvExportsMenu.AfterCheck -= tvExportsMenu_AfterCheck;
            node.Checked = isChecked;
            tvExportsMenu.AfterCheck += tvExportsMenu_AfterCheck;
        }

        private void checkAllChilds(TreeNode node, bool isChecked)
        {
            Queue<TreeNode> queue = new Queue<TreeNode>();
            queue.Enqueue(node);
            while (queue.Count > 0)
            {
                TreeNode currentNode = queue.Dequeue();
                setChecked(currentNode, isChecked);
                foreach (TreeNode item in currentNode.Nodes)
                {
                    queue.Enqueue(item);
                }
            }
        }

        #endregion

        private enum InsertionPosition
        {
            Top,
            Inside,
            Bottom
        }

        private ExportsOptions options = ExportsOptions.GetInstance();

        private InsertionPosition currentInsertionPosition;
        private List<int> prevNodePath;

        /// <summary>
        /// Editor for rearrangement Exports Menu elements
        /// </summary>
        public ExportsOptionsEditorForm()
        {
            InitializeComponent();
            Localize();

            // Because there is no other way to make spacing between CheckBox and Icon in TreeNode
            Point destPt = new Point(6, 0);
            Size size = DpiHelper.ConvertUnits(new Size(22, 16));
            tvExportsMenu.ImageList = new ImageList();
            tvExportsMenu.ImageList.ImageSize = size;
            foreach (Image image in Res.GetImages().Images)
            {
                Bitmap bmp = new Bitmap(size.Width, size.Height);
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(image, destPt);
                }
                tvExportsMenu.ImageList.Images.Add((Image)bmp);
            }

            tvExportsMenu.BeginUpdate();
            List<ExportsOptions.ExportsTreeNode> menu = options.ExportsMenu;
            menu.Add(options.CloudMenu);
            menu.Add(options.MessengerMenu);
            fillExportsMenu(menu, tvExportsMenu.Nodes);
            tvExportsMenu.EndUpdate();
            tvExportsMenu.ExpandAll();

            Scale();
            tvExportsMenu.ItemHeight = DpiHelper.ConvertUnits(tvExportsMenu.ItemHeight);

            // not working
            //using (Bitmap bmp = new Bitmap(DpiHelper.ConvertUnits(16), DpiHelper.ConvertUnits(16)))
            //using (Graphics g = Graphics.FromImage(bmp)) 
            //{
            //    tvExportsMenu.StateImageList = new ImageList();
            //    ControlPaint.DrawCheckBox(g, new Rectangle(0, 0, bmp.Width, bmp.Height), ButtonState.Normal);
            //    tvExportsMenu.StateImageList.Images.Add(new Bitmap((Bitmap)bmp.Clone()));
            //    g.Clear(Color.White);
            //    ControlPaint.DrawCheckBox(g, new Rectangle(0, 0, bmp.Width, bmp.Height), ButtonState.Checked);
            //    tvExportsMenu.StateImageList.Images.Add(new Bitmap((Bitmap)bmp.Clone()));
            //}
            //tvExportsMenu.StateImageList.ImageSize = DpiHelper.ConvertUnits(tvExportsMenu.StateImageList.ImageSize);
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            this.Text = Res.Get("Export,Editor");
            gbExportsMenu.Text = Res.Get("Export,Editor,ExportsMenu");
            btnDefaultSettings.Text = Res.Get("Export,Editor,DefaultMenu");
        }

        private void tvExportsMenu_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(e.Item, DragDropEffects.Move);
        }

        private void tvExportsMenu_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)) && isDraggable(e.Data.GetData(typeof(TreeNode)) as TreeNode))
            {
                e.Effect = e.AllowedEffect;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void tvExportsMenu_DragOver(object sender, DragEventArgs e)
        {
            Point point = tvExportsMenu.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = tvExportsMenu.GetNodeAt(point) as TreeNode;
            TreeNode draggedNode = e.Data.GetData(typeof(TreeNode)) as TreeNode;

            if (targetNode != null && !containsNode(targetNode, draggedNode) && isDraggable(draggedNode) && isDraggable(targetNode))
            {
                e.Effect = DragDropEffects.Move;
                List<int> nodePath = getNodePath(targetNode);
                InsertionPosition insertionPosition = getInsertionPosition(targetNode, point);
                if (arePathsEqual(nodePath, prevNodePath) && insertionPosition == currentInsertionPosition)
                {
                    return;
                }
                currentInsertionPosition = insertionPosition;
                prevNodePath = nodePath;
                drawInsertionLine(insertionPosition, nodePath);
            }
            else
            {
                e.Effect = DragDropEffects.None;
                tvExportsMenu.Refresh();
            }

        }

        private void tvExportsMenu_DragDrop(object sender, DragEventArgs e)
        {
            Point targetPoint = tvExportsMenu.PointToClient(new Point(e.X, e.Y));
            TreeNode targetNode = tvExportsMenu.GetNodeAt(targetPoint);
            TreeNode draggedNode = (TreeNode)e.Data.GetData(typeof(TreeNode));

            if (targetNode == null)
            {
                draggedNode.Remove();
                tvExportsMenu.Nodes.Add(draggedNode);
            }
            else if (!containsNode(targetNode, draggedNode))
            {
                if (e.Effect == DragDropEffects.Move)
                {
                    if (draggedNode.Checked == true && isSingleChecked(draggedNode))
                    {
                        setChecked(draggedNode.Parent, false);
                    }
                    draggedNode.Remove();
                    TreeNodeCollection nodes = targetNode.Parent != null ? targetNode.Parent.Nodes : tvExportsMenu.Nodes;
                    switch (currentInsertionPosition)
                    {
                        case InsertionPosition.Top:
                            nodes.Insert(targetNode.Index, draggedNode);
                            break;
                        case InsertionPosition.Inside:
                            targetNode.Nodes.Add(draggedNode);
                            break;
                        case InsertionPosition.Bottom:
                            nodes.Insert(targetNode.Index + 1, draggedNode);
                            break;
                    }
                    if (draggedNode.Checked == true && isSingleChecked(draggedNode))
                    {
                        setChecked(draggedNode.Parent, true);
                    }
                }
                targetNode.Expand();
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            options.ExportsMenu.Clear();

            passChanges(tvExportsMenu.Nodes);
        }

        private void btnDefaultSettings_Click(object sender, EventArgs e)
        {
            tvExportsMenu.BeginUpdate();

            tvExportsMenu.Nodes.Clear();
            List<ExportsOptions.ExportsTreeNode> defaultMenu = options.DefaultExports();
            defaultMenu.Add(options.DefaultCloud());
            defaultMenu.Add(options.DefaultMessengers());
            fillExportsMenu(defaultMenu, tvExportsMenu.Nodes);
            tvExportsMenu.ExpandAll();

            tvExportsMenu.EndUpdate();
        }

        private void tvExportsMenu_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Checked)
            {
                TreeNode node = e.Node;
                while (node.Parent != null)
                {
                    node = node.Parent;
                    setChecked(node, true);
                }
            }
            else
            {
                if (isSingleChecked(e.Node))
                {
                    setChecked(e.Node.Parent, false);
                }
            }
            checkAllChilds(e.Node, e.Node.Checked);
        }
    }
#endif
}
