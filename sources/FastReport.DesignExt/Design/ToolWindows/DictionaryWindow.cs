using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using FastReport.Utils;
using FastReport.Data;
using FastReport.Forms;
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
    /// Represents the "Data Dictionary" window.
    /// </summary>
    public class DictionaryWindow : ToolWindowBase
    {
        #region Fields
#if !MONO
        private Bar toolbar;
        private ButtonItem btnActions;
        private ButtonItem btnEdit;
        private ButtonItem btnDelete;
        private ButtonItem btnView;
        private ButtonItem miNew;
        private ButtonItem miOpen;
        private ButtonItem miMerge;
        private ButtonItem miSave;
        private ButtonItem miChooseData;
        private ButtonItem miNewDataSource;
        private ButtonItem miSortDataSources;
        private ButtonItem miNewRelation;
        private ButtonItem miNewParameter;
        private ButtonItem miNewTotal;
        private ButtonItem miNewCalculatedColumn;

        private ContextMenuBar mnuContext;
        private ButtonItem mnuContextRoot;
        private ButtonItem miRename;
        private ButtonItem miEdit;
        private ButtonItem miDelete;
        private ButtonItem miDeleteAlias;
        private ButtonItem miView;
        private ButtonItem miViewJson;
        private ButtonItem miSortDataFields;
        private ButtonItem miCopyDataSource;
#else
        private ToolStrip toolbar;
        private ToolStripDropDownButton btnActions;
        private ToolStripButton btnEdit;
        private ToolStripButton btnDelete;
        private ToolStripButton btnView;
        private ToolStripMenuItem miNew;
        private ToolStripMenuItem miOpen;
        private ToolStripMenuItem miMerge;
        private ToolStripMenuItem miSave;
        private ToolStripMenuItem miChooseData;
        private ToolStripMenuItem miNewDataSource;
        private ToolStripMenuItem miSortDataSources;
        private ToolStripMenuItem miNewRelation;
        private ToolStripMenuItem miNewParameter;
        private ToolStripMenuItem miNewTotal;
        private ToolStripMenuItem miNewCalculatedColumn;
        private ToolStripMenuItem miCopyDataSource;

        private ContextMenuStrip mnuContext;
        private ToolStripMenuItem miNewDataSource1;
        private ToolStripMenuItem miNewParameter1;
        private ToolStripMenuItem miNewTotal1;
        private ToolStripMenuItem miNewCalculatedColumn1;
        private ToolStripMenuItem miRename;
        private ToolStripMenuItem miEdit;
        private ToolStripMenuItem miDelete;
        private ToolStripMenuItem miDeleteAlias;
        private ToolStripMenuItem miView;
        private ToolStripMenuItem miViewJson;
        private ToolStripMenuItem miSortDataFields;
#endif
        private TreeViewMultiSelect tree;
        private Splitter splitter;
        private DescriptionControl lblDescription;
        private Report report;
        private List<string> expandedNodes;
        private static DraggedItemCollection draggedItems = new DraggedItemCollection();
        #endregion

        #region Properties
        private bool IsDataComponent
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is DataComponentBase; }
        }

        private bool IsVariable
        {
            get
            {
                return tree.SelectedNode != null && tree.SelectedNode.Tag is Parameter &&
                  !(tree.SelectedNode.Parent.Tag is SystemVariables);
            }
        }

        private bool IsTotal
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is Total; }
        }

        private bool IsConnection
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is DataConnectionBase; }
        }

        private bool IsTable
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is DataSourceBase; }
        }

        private bool IsJsonTable
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is FastReport.Data.JsonConnection.JsonTableDataSource; }
        }

        private bool IsRelation
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is Relation; }
        }

        private bool IsEditableColumn
        {
            get
            {
                TreeNode node = tree.SelectedNode;
                bool result = node != null && node.Tag is Column;
                if (result)
                {
                    // check if column belongs to the datasource, not relation.
                    while (node != null)
                    {
                        if (node.Tag is Relation)
                        {
                            result = false;
                            break;
                        }
                        else if (node.Tag is DataSourceBase)
                            break;

                        node = node.Parent;
                    }
                }

                return result;
            }
        }

        private bool IsCube
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag is CubeSourceBase; }
        }

        private bool IsDataSources
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag == report.Dictionary.DataSources; }
        }

        private bool IsVariables
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag == report.Dictionary.Parameters; }
        }

        private bool IsSystemVariables
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag == report.Dictionary.SystemVariables; }
        }

        private bool IsTotals
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag == report.Dictionary.Totals; }
        }

        private bool IsCubeSources
        {
            get { return tree.SelectedNode != null && tree.SelectedNode.Tag == report.Dictionary.CubeSources; }
        }

        private bool CanEdit
        {
            get
            {
                return (IsDataComponent || IsVariable || IsTotal) &&
                  !Designer.Restrictions.DontEditData &&
                  (tree.SelectedNode.Tag as Base).HasFlag(Flags.CanEdit) &&
                  !(tree.SelectedNode.Tag as Base).HasRestriction(Restrictions.DontEdit);
            }
        }

        private bool CanDelete
        {
            get
            {
                return (IsDataComponent || IsVariable || IsTotal) &&
                  !Designer.Restrictions.DontEditData &&
                  (tree.SelectedNode.Tag as Base).HasFlag(Flags.CanDelete) &&
                  !(tree.SelectedNode.Tag as Base).HasRestriction(Restrictions.DontDelete);
            }
        }

        private bool CanCreateCalculatedColumn
        {
            get
            {
                return tree.SelectedNode != null && tree.SelectedNode.Tag is DataSourceBase &&
                  !(tree.SelectedNode.Tag is BusinessObjectDataSource);
            }
        }

        private bool IsAliased
        {
            get
            {
                return tree.SelectedNode != null && tree.SelectedNode.Tag is DataComponentBase &&
                  (tree.SelectedNode.Tag as DataComponentBase).IsAliased;
            }
        }
        #endregion

        #region Private Methods
        private TreeNode FindNode(TreeNodeCollection parent, string text)
        {
            foreach (TreeNode node in parent)
            {
                if (node.Text == text)
                    return node;
            }
            return null;
        }

        private void NavigateTo(string path)
        {
            string[] parts = path.Split(new char[] { '.' });
            TreeNodeCollection parent = tree.Nodes;
            TreeNode node = null;
            foreach (string part in parts)
            {
                node = FindNode(parent, part);
                parent = node.Nodes;
            }
            tree.SelectedNode = node;
        }

        private void GetExpandedNodes(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsExpanded)
                    expandedNodes.Add(node.FullPath);
                GetExpandedNodes(node.Nodes);
            }
        }

        private bool CompareNodes(TreeNodeCollection fromNodes, TreeNodeCollection toNodes)
        {
            if (fromNodes.Count != toNodes.Count)
                return false;
            for (int i = 0; i < fromNodes.Count; i++)
            {
                if (fromNodes[i].Text != toNodes[i].Text || fromNodes[i].ImageIndex != toNodes[i].ImageIndex)
                    return false;
                toNodes[i].Tag = fromNodes[i].Tag;
                if (!CompareNodes(fromNodes[i].Nodes, toNodes[i].Nodes))
                    return false;
            }
            return true;
        }

        private void CopyNodes(TreeNodeCollection fromNodes, TreeNodeCollection toNodes)
        {
            foreach (TreeNode fromNode in fromNodes)
            {
                TreeNode toNode = toNodes.Add(fromNode.Text);
                toNode.Tag = fromNode.Tag;
                toNode.ImageIndex = fromNode.ImageIndex;
                toNode.SelectedImageIndex = fromNode.SelectedImageIndex;
                CopyNodes(fromNode.Nodes, toNode.Nodes);
                if (expandedNodes.Contains(fromNode.FullPath))
                    toNode.Expand();
            }
        }

        private void UpdateTree()
        {
            expandedNodes.Clear();
            GetExpandedNodes(tree.Nodes);

            TreeView buffer = new TreeView();
            if (report != null)
            {
                bool canShowData = report.Dictionary.Connections.Count > 0;
                foreach (DataSourceBase data in report.Dictionary.DataSources)
                {
                    if (data.Enabled)
                    {
                        canShowData = true;
                        break;
                    }
                }

                bool canShowCube = report.Dictionary.CubeSources.Count > 0;

                TreeNode rootNode = null;
                if (canShowData)
                {
                    rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,DataSources"));
                    rootNode.Tag = report.Dictionary.DataSources;
                    rootNode.ImageIndex = 53;
                    rootNode.SelectedImageIndex = rootNode.ImageIndex;
                    DataTreeHelper.CreateDataTree(report.Dictionary, rootNode.Nodes, true, true, true, true);
                }

                // system variables
                rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,SystemVariables"));
                rootNode.Tag = report.Dictionary.SystemVariables;
                rootNode.ImageIndex = 60;
                rootNode.SelectedImageIndex = rootNode.ImageIndex;
                DataTreeHelper.CreateVariablesTree(report.Dictionary.SystemVariables, rootNode.Nodes);

                // totals
                rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,Totals"));
                rootNode.Tag = report.Dictionary.Totals;
                rootNode.ImageIndex = 132;
                rootNode.SelectedImageIndex = rootNode.ImageIndex;
                DataTreeHelper.CreateTotalsTree(report.Dictionary.Totals, rootNode.Nodes);

                // parameters
                rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,Parameters"));
                rootNode.Tag = report.Dictionary.Parameters;
                rootNode.ImageIndex = 234;
                rootNode.SelectedImageIndex = rootNode.ImageIndex;
                DataTreeHelper.CreateParametersTree(report.Dictionary.Parameters, rootNode.Nodes);

                // functions
                rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,Functions"));
                rootNode.ImageIndex = 52;
                rootNode.SelectedImageIndex = rootNode.ImageIndex;
                DataTreeHelper.CreateFunctionsTree(report, rootNode.Nodes);

                if (canShowCube)
                {
                    rootNode = buffer.Nodes.Add(Res.Get("Designer,ToolWindow,Dictionary,CubeSources"));
                    rootNode.Tag = report.Dictionary.CubeSources;
                    rootNode.ImageIndex = 248;
                    rootNode.SelectedImageIndex = rootNode.ImageIndex;
                    DataTreeHelper.CreateCubeTree(report.Dictionary, rootNode.Nodes, false);
                }
            }

            if (!CompareNodes(buffer.Nodes, tree.Nodes))
            {
                tree.BeginUpdate();
                tree.Nodes.Clear();
                CopyNodes(buffer.Nodes, tree.Nodes);
                tree.EndUpdate();
            }

            buffer.Dispose();
            UpdateControls();
        }

        private void Change()
        {
            Designer.SetModified(this, "EditData");
        }

        private void UpdateControls()
        {
            btnEdit.Enabled = CanEdit;
            btnDelete.Enabled = CanDelete;
            btnView.Enabled = IsTable && !IsJsonTable;
        }

        private void miNew_Click(object sender, EventArgs e)
        {
            report.Dictionary.Clear();
            report.Dictionary.ReRegisterData();
            UpdateTree();
            Change();
        }

        private void miOpen_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,Dictionary");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    report.Dictionary.Load(dialog.FileName);
                    UpdateTree();
                    Change();
                }
            }
        }

        private void miSave_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,Dictionary");
                dialog.DefaultExt = "frd";
                dialog.FileName = "Dictionary.frd";
                if (dialog.ShowDialog() == DialogResult.OK)
                    report.Dictionary.Save(dialog.FileName);
            }
        }

        private void miMerge_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,Dictionary");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Dictionary dict = new Dictionary();
                    dict.Load(dialog.FileName);
                    report.Dictionary.Merge(dict);
                    UpdateTree();
                    Change();
                }
            }
        }

#if !MONO
        private void btnActions_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            while (btnActions.SubItems[btnActions.SubItems.Count - 1] != miChooseData)
            {
                btnActions.SubItems.RemoveAt(btnActions.SubItems.Count - 1);
            }
            btnActions.SubItems.AddRange(new ButtonItem[] { 
#else
        private void btnActions_DropDownOpening(object sender, EventArgs e)
        {
            while (!(btnActions.DropDownItems[btnActions.DropDownItems.Count - 1] is ToolStripSeparator))
            {
                btnActions.DropDownItems.RemoveAt(btnActions.DropDownItems.Count - 1);
            }
            btnActions.DropDownItems.AddRange(new ToolStripItem[] { 
#endif			
        miNewDataSource, miNewRelation, miNewCalculatedColumn, miNewParameter, miNewTotal });

            miNew.Enabled = Designer.cmdChooseData.Enabled;
            miOpen.Enabled = Designer.cmdChooseData.Enabled;
            miMerge.Enabled = Designer.cmdChooseData.Enabled;
            miSave.Enabled = Designer.cmdChooseData.Enabled;
            miChooseData.Enabled = Designer.cmdChooseData.Enabled;
            miNewDataSource.Enabled = Designer.cmdAddData.Enabled;
            miSortDataSources.Enabled = Designer.cmdSortDataSources.Enabled;
            miNewRelation.Enabled = Designer.cmdChooseData.Enabled;
            miNewParameter.Enabled = Designer.cmdChooseData.Enabled;
            miNewTotal.Enabled = Designer.cmdChooseData.Enabled;
            miNewCalculatedColumn.Enabled = Designer.cmdChooseData.Enabled && CanCreateCalculatedColumn;
        }

#if !MONO
        private void mnuContextRoot_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            mnuContextRoot.SubItems.Clear();
            if (!Designer.cmdChooseData.Enabled)
            {
                e.Cancel = true;
                return;
            }
            if ((IsDataSources || IsConnection) && Designer.cmdAddData.Enabled)
            {
                mnuContextRoot.SubItems.Add(miNewDataSource);

                // If data sources more than 1 and enabled at least 2 data sources, then it's possible to sort them.
                if (report.Dictionary.DataSources.Count > 1)
                {
                    int enabledDataSources = 0;
                    for (int i = 0; enabledDataSources < 2 && i < report.Dictionary.DataSources.Count; i++)
                    {
                        if (report.Dictionary.DataSources[i].Enabled)
                        {
                            enabledDataSources++;
                        }
                    }
                    if (enabledDataSources > 1)
                    {
                        mnuContextRoot.SubItems.Add(miSortDataSources);
                    }
                }
            }
            else if (IsVariables || IsVariable)
                mnuContextRoot.SubItems.Add(miNewParameter);
            else if (IsTotals || IsTotal)
                mnuContextRoot.SubItems.Add(miNewTotal);
            else if (CanCreateCalculatedColumn)
            {
                mnuContextRoot.SubItems.Add(miNewCalculatedColumn);
                miNewCalculatedColumn.Enabled = true;
            }

            if (CanEdit)
                mnuContextRoot.SubItems.Add(miEdit);
            if (IsConnection)
                mnuContextRoot.SubItems.Add(miCopyDataSource);
            if (IsTable || IsEditableColumn || IsVariable || IsTotal)
                mnuContextRoot.SubItems.Add(miRename);
            if (CanDelete)
                mnuContextRoot.SubItems.Add(miDelete);
            if (IsAliased)
                mnuContextRoot.SubItems.Add(miDeleteAlias);
            if (IsJsonTable)
            {
                mnuContextRoot.SubItems.Add(miViewJson);

                DataSourceBase data = tree.SelectedNode.Tag as DataSourceBase;
                if (data != null && data.Columns.Count > 1)
                {
                    mnuContextRoot.SubItems.Add(miSortDataFields);
                }
            }
            else if (IsTable)
            {
                mnuContextRoot.SubItems.Add(miView);

                DataSourceBase data = tree.SelectedNode.Tag as DataSourceBase;
                if (data != null && data.Columns.Count > 1)
                {
                    mnuContextRoot.SubItems.Add(miSortDataFields);
                }
            }

            if (mnuContextRoot.SubItems.Count == 0)
                e.Cancel = true;
        }
#else
        private void mnuContext_Opening(object sender, CancelEventArgs e)
        {
            mnuContext.Items.Clear();
            if (!Designer.cmdChooseData.Enabled)
            {
                e.Cancel = true;
                return;
            }

            if ((IsDataSources || IsConnection) && Designer.cmdAddData.Enabled)
                mnuContext.Items.Add(miNewDataSource1);
            else if (IsVariables || IsVariable)
                mnuContext.Items.Add(miNewParameter1);
            else if (IsTotals || IsTotal)
                mnuContext.Items.Add(miNewTotal1);
            else if (CanCreateCalculatedColumn)
                mnuContext.Items.Add(miNewCalculatedColumn1);

            if (CanEdit)
                mnuContext.Items.Add(miEdit);
            if (IsConnection)
                mnuContext.Items.Add(miCopyDataSource);
            if (IsTable || IsEditableColumn || IsVariable || IsTotal)
                mnuContext.Items.Add(miRename);
            if (CanDelete)
                mnuContext.Items.Add(miDelete);
            if (IsAliased)
                mnuContext.Items.Add(miDeleteAlias);
            if (IsTable)
                mnuContext.Items.Add(miView);

            if (mnuContext.Items.Count == 0)
                e.Cancel = true;
        }
#endif

        private void miNewRelation_Click(object sender, EventArgs e)
        {
            Relation relation = new Relation();
            report.Dictionary.Relations.Add(relation);
            using (RelationEditorForm form = new RelationEditorForm(relation))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    relation.Name = report.Dictionary.CreateUniqueName(relation.ParentDataSource.Name + "_" +
                      relation.ChildDataSource.Name);
                    UpdateTree();
                    Change();
                }
                else
                    relation.Dispose();
            }
        }

        private void miNewCalculatedColumn_Click(object sender, EventArgs e)
        {
            DataSourceBase data = tree.SelectedNode.Tag as DataSourceBase;
            Column c = new Column();
            c.Name = data.Columns.CreateUniqueName("Column");
            c.Alias = data.Columns.CreateUniqueAlias(c.Alias);
            c.Calculated = true;
            data.Columns.Add(c);

            UpdateTree();
            string navigatePath = Res.Get("Designer,ToolWindow,Dictionary,DataSources");
            if (data.Parent is DataConnectionBase)
                navigatePath += "." + data.Parent.Name;
            navigatePath += "." + data.Alias + "." + c.Alias;
            NavigateTo(navigatePath);
            Change();
        }

        private void miNewParameter_Click(object sender, EventArgs e)
        {
            Parameter p = new Parameter();
            ParameterCollection parent = null;
            if (IsVariable)
                parent = (tree.SelectedNode.Tag as Parameter).Parameters;
            else
                parent = report.Dictionary.Parameters;

            p.Name = parent.CreateUniqueName("Parameter");
            parent.Add(p);
            UpdateTree();
            NavigateTo(Res.Get("Designer,ToolWindow,Dictionary,Parameters") + "." + p.FullName);
            Change();
        }

        private void miNewTotal_Click(object sender, EventArgs e)
        {
            using (TotalEditorForm form = new TotalEditorForm(Designer))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    report.Dictionary.Totals.Add(form.Total);
                    UpdateTree();
                    NavigateTo(Res.Get("Designer,ToolWindow,Dictionary,Totals") + "." + form.Total.Name);
                    Change();
                }
            }
        }

        private void miRename_Click(object sender, EventArgs e)
        {
            if (tree.SelectedNode == null)
                return;
            tree.SelectedNode.BeginEdit();
        }

        private void miEdit_Click(object sender, EventArgs e)
        {
            if (!CanEdit)
                return;

            IHasEditor c = tree.SelectedNode.Tag as IHasEditor;
            if (c != null && c.InvokeEditor())
            {
                UpdateTree();
                Change();
            }
        }

        private void miDelete_Click(object sender, EventArgs e)
        {
            if (!CanDelete)
                return;

            (tree.SelectedNode.Tag as Base).Delete();
            TreeNode parentNode = tree.SelectedNode.Parent;
            int index = parentNode.Nodes.IndexOf(tree.SelectedNode);
            tree.SelectedNode.Remove();
            if (index < parentNode.Nodes.Count)
                tree.SelectedNode = parentNode.Nodes[index];
            else if (index > 0)
                tree.SelectedNode = parentNode.Nodes[index - 1];
            Change();
        }

        private void miDeleteAlias_Click(object sender, EventArgs e)
        {
            if (!IsAliased)
                return;
            DataComponentBase c = tree.SelectedNode.Tag as DataComponentBase;
            c.Alias = c.Name;
            tree.SelectedNode.Text = c.Name;
            Change();
        }

        private void miView_Click(object sender, EventArgs e)
        {
            if (!IsTable)
                return;

            DataSourceBase data = tree.SelectedNode.Tag as DataSourceBase;
            if (data == null)
                return;

            try
            {
                data.Init();
            }
            catch (Exception ex)
            {
                FRMessageBox.Error(ex.Message);
                return;
            }

            object dataSource = null;
            if (data is TableDataSource)
            {
                dataSource = (data as TableDataSource).Table;
            }
            else
                dataSource = data.Rows;
            if (dataSource == null)
                return;

            using (DataViewForm form = new DataViewForm(data))
            {
                form.ShowDialog();
            }
        }

        private void miViewJson_Click(object sender, EventArgs e)
        {
            if (!IsJsonTable)
                return;

            Data.JsonConnection.JsonTableDataSource data = tree.SelectedNode.Tag as Data.JsonConnection.JsonTableDataSource;
            if (data == null)
                return;

            try
            {
                data.Init();
            }
            catch (Exception ex)
            {
                FRMessageBox.Error(ex.Message);
                return;
            }

            using (JsonEditorForm jsonEditorForm = new JsonEditorForm())
            {
                StringBuilder sb = new StringBuilder();
                data.Json.WriteTo(sb, 2);
                jsonEditorForm.JsonText = sb.ToString();
                jsonEditorForm.SetToReadOnly();
                jsonEditorForm.ShowDialog();
            }
        }

        private void miCopyDataSource_Click(object sender, EventArgs e)
        {
            if(tree.SelectedNode.Tag is DataConnectionBase)
            {
                DataConnectionBase data = tree.SelectedNode.Tag as DataConnectionBase;
                data.Clone();
            }
            Change();
            UpdateTree();
            Refresh();
            //Designer.SetModified();
        }

        private void miSortDataFields_Click(object sender, EventArgs e)
        {
            if (!IsTable)
                return;

            TableDataSource data = tree.SelectedNode.Tag as TableDataSource;
            if (data == null)
                return;

            if (data.Columns.Count > 1)
            {
                data.Columns.Sort();
                UpdateTree();
                Change();
            }
        }

        private void FTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2)
                miRename_Click(this, null);
            else if (e.KeyCode == Keys.Delete)
                miDelete_Click(this, null);
            else if (e.Control)
            {
                TreeNode node = tree.SelectedNode;
                if (node != null && (node.Tag is Parameter) && !(node.Tag is SystemVariable))
                {
                    Parameter par = node.Tag as Parameter;
                    TreeNode parentNode = node.Parent;
                    ParameterCollection parCollection = null;
                    if (parentNode.Tag is ParameterCollection)
                        parCollection = parentNode.Tag as ParameterCollection;
                    else
                        parCollection = (parentNode.Tag as Parameter).Parameters;

                    if (e.KeyCode == Keys.Up)
                    {
                        parCollection.MoveUp(par);
                        e.Handled = true;
                    }
                    else if (e.KeyCode == Keys.Down)
                    {
                        parCollection.MoveDown(par);
                        e.Handled = true;
                    }

                    if (e.Handled)
                    {
                        // update all designer plugins (this one too)
                        Designer.SetModified(null, "EditData");
                        NavigateTo(Res.Get("Designer,ToolWindow,Dictionary,Parameters") + "." + par.FullName);
                    }
                }
            }
        }

        private void FTree_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                TreeNode node = tree.GetNodeAt(e.Location);
                if (tree.SelectedNode != node)
                    tree.SelectedNode = node;
            }
        }

        private void FTree_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            bool canEdit = (IsTable || IsEditableColumn || IsVariable || IsTotal) &&
              !Designer.Restrictions.DontEditData &&
              !(tree.SelectedNode.Tag as Base).HasRestriction(Restrictions.DontModify);

            if (!canEdit)
                e.CancelEdit = true;
        }

        private void FTree_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            string newLabel = e.Label == null ? tree.SelectedNode.Text : e.Label;
            if (newLabel == tree.SelectedNode.Text)
                return;

            Base obj = tree.SelectedNode.Tag as Base;
            bool duplicateName = false;

            if (obj is DataSourceBase)
            {
                if (report.Dictionary.FindByAlias(newLabel) != null)
                    duplicateName = true;
                else
                {
                    string oldAlias = (obj as DataSourceBase).Alias;

                    (obj as DataSourceBase).Alias = newLabel;

                    // Update expressions in components.
                    foreach (Base component in report.AllObjects)
                    {
                        // Update Text in TextObject instances.
                        if (component is TextObject)
                        {
                            TextObject text = component as TextObject;
                            string bracket = text.Brackets.Split(new char[] { ',' })[0];
                            if (String.IsNullOrEmpty(bracket))
                            {
                                bracket = "[";
                            }
                            text.Text = text.Text.Replace(bracket + oldAlias + ".", bracket + newLabel + ".");
                        }
                        // Update DataColumn in PictureObject instances.
                        else if (component is PictureObject)
                        {
                            PictureObject picture = component as PictureObject;
                            picture.DataColumn = picture.DataColumn.Replace(oldAlias + ".", newLabel + ".");
                        }
                        // Update Filter and Sort in DataBand instances.
                        else if (component is DataBand)
                        {
                            DataBand data = component as DataBand;
                            data.Filter = data.Filter.Replace("[" + oldAlias + ".", "[" + newLabel + ".");
                            foreach (Sort sort in data.Sort)
                            {
                                sort.Expression = sort.Expression.Replace("[" + oldAlias + ".", "[" + newLabel + ".");
                            }
                        }
                    }
                }
            }
            else if (obj is Column)
            {
                // get column name, take parent columns into account
                string columnName = newLabel;
                TreeNode node = tree.SelectedNode;
                while (true)
                {
                    node = node.Parent;
                    if (node.Tag is DataSourceBase)
                        break;
                    columnName = node.Text + "." + columnName;
                }

                DataSourceBase data = obj.Parent as DataSourceBase;
                if (data.Columns.FindByAlias(columnName) != null)
                    duplicateName = true;
                else
                    (obj as Column).Alias = columnName;
            }
            else if (obj is Parameter)
            {
                TreeNode parentNode = tree.SelectedNode.Parent;
                ParameterCollection parent = null;
                if (parentNode.Tag is Parameter)
                    parent = (parentNode.Tag as Parameter).Parameters;
                else
                    parent = report.Dictionary.Parameters;

                if (parent.FindByName(newLabel) != null)
                    duplicateName = true;
                else
                    obj.Name = newLabel;
            }
            else if (obj is Total)
            {
                if (report.Dictionary.FindByName(newLabel) != null)
                    duplicateName = true;
                else
                    obj.Name = newLabel;
            }

            if (duplicateName)
            {
                e.CancelEdit = true;
                FRMessageBox.Error(Res.Get("Designer,ToolWindow,Dictionary,DuplicateName"));
            }
            else
                Change();
        }

        private void FTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (tree.SelectedNode == null)
                return;
            object selected = tree.SelectedNode.Tag;

            Designer.SelectedObjects.Clear();
            if (selected is Base)
                Designer.SelectedObjects.Add(selected as Base);
            Designer.SelectionChanged(this);
            UpdateControls();

            bool descrVisible = selected is MethodInfo || selected is SystemVariable;
            splitter.Visible = descrVisible;
            lblDescription.Visible = descrVisible;

            if (descrVisible)
                lblDescription.ShowDescription(report, selected);
        }

        private void FTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            List<TreeNode> selectedNodes;
            draggedItems.Clear();

            if (tree.SelectedNodes.Contains(e.Item as TreeNode))
            {
                selectedNodes = tree.SelectedNodes;
            }
            else
            {
                tree.SelectedNode = e.Item as TreeNode;
                //selectedNodes = new List<TreeNode>() { FTree.SelectedNode }; //.net 2 compatibility code
                selectedNodes = new List<TreeNode>();
                selectedNodes.Add(tree.SelectedNode);
            }

            foreach (TreeNode n in selectedNodes)
            {
                string selectedItem = "";
                TreeNode node = n;

                if (node == null)
                    continue;

                if (node.Tag is Column && !(node.Tag is DataSourceBase))
                {
                    while (true)
                    {
                        if (node.Tag is DataSourceBase)
                        {
                            selectedItem = (node.Tag as DataSourceBase).FullName + "." + selectedItem;
                            break;
                        }
                        selectedItem = node.Text + (selectedItem == "" ? "" : ".") + selectedItem;
                        node = node.Parent;
                    }
                }
                else if (node.Tag is Parameter || node.Tag is Total)
                {
                    while (node != null && node.Tag != null)
                    {
                        if (node.Tag is Parameter || node.Tag is Total)
                            selectedItem = node.Text + (selectedItem == "" ? "" : ".") + selectedItem;
                        node = node.Parent;
                    }
                }
                else if (node.Tag is MethodInfo)
                {
                    MethodInfo info = node.Tag as MethodInfo;
                    ParameterInfo[] pars = info.GetParameters();
                    int parsLength = pars.Length;
                    if (parsLength > 0 && pars[0].Name == "thisReport")
                        parsLength--;

                    selectedItem = info.Name + "(" + (parsLength > 1 ? "".PadRight(parsLength - 1, ',') : "") + ")";
                }

                if (selectedItem != "")
                    draggedItems.Add(new DraggedItem(n.Tag, selectedItem));
            }

            if (draggedItems.Count > 0)
                tree.DoDragDrop(draggedItems, DragDropEffects.Move);
            else
                tree.DoDragDrop(e.Item, DragDropEffects.None);
        }

        private void FTree_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.None;
            TreeNode targetNode = tree.GetNodeAt(tree.PointToClient(new Point(e.X, e.Y)));

            if (draggedItems.Count == 0 ||
                targetNode == null ||
                targetNode.Tag is SystemVariable ||
                targetNode.Tag is SystemVariables)
                return;

            int allow = 0;

            foreach (DraggedItem draggedItem in draggedItems)
            {
                if (draggedItem.obj is Parameter &&
                    !(draggedItem.obj is SystemVariable) &&
                    !(draggedItem.obj is SystemVariables))
                {
                    if (targetNode.Tag is ParameterCollection ||
                        (targetNode.Tag is Parameter &&
                        targetNode.Tag != draggedItem.obj &&
                        !(targetNode.Tag as Parameter).HasParent(draggedItem.obj as Parameter)))
                    {
                        allow++;
                    }
                }
            }

            if (allow != 0 && allow == draggedItems.Count)
                e.Effect = e.AllowedEffect;
        }

#if !MONO
        private void Reinit(float ratio = 0)
        {
            if (ratio == 0)
                ratio = DpiHelper.Multiplier;
            base.ReinitDpiSize();
            lblDescription.Height = DpiHelper.ConvertUnits(70);
            Image = Res.GetImage(72, ratio);
            tree.ImageList = GetCloneImageList();
            toolbar.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            mnuContext.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);

            btnEdit.Image = Res.GetImage(68, ratio);
            btnEdit.toolTipFont = DpiHelper.ParseFontSize(new Font("Microsoft Sans Serif", 12), 12, ratio);
            btnDelete.Image = Res.GetImage(51, ratio);
            btnDelete.toolTipFont = DpiHelper.ParseFontSize(new Font("Microsoft Sans Serif", 12), 12, ratio);
            btnView.Image = Res.GetImage(54, ratio);
            btnView.toolTipFont = DpiHelper.ParseFontSize(new Font("Microsoft Sans Serif", 12), 12, ratio);

            miNew.Image = Res.GetImage(0, ratio);
            miOpen.Image = Res.GetImage(1, ratio);
            miSave.Image = Res.GetImage(2, ratio);

            miNewDataSource.Image = Res.GetImage(137, ratio);
            miNewRelation.Image = Res.GetImage(139, ratio);
            miNewCalculatedColumn.Image = Res.GetImage(55, ratio);
            miNewParameter.Image = Res.GetImage(56, ratio);
            miNewTotal.Image = Res.GetImage(65, ratio);

            miEdit.Image = Res.GetImage(68, ratio);
            miDelete.Image = Res.GetImage(51, ratio);
            miView.Image = Res.GetImage(54, ratio);
            miViewJson.Image = Res.GetImage(54, ratio);
            toolbar.UpdateDpiDependencies();
        }

#endif
        private void FTree_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode targetNode = tree.GetNodeAt(tree.PointToClient(new Point(e.X, e.Y)));
            if (targetNode == null)
                return;

            Object targetComponent = targetNode.Tag;
            if ((targetComponent is SystemVariable) || (targetComponent is SystemVariables))
                return;

            if (draggedItems.Count == 0)
                return;

            string draggedName = "";

            foreach (DraggedItem draggedItem in draggedItems)
            {
                if ((draggedItem.obj is SystemVariable) || (draggedItem.obj is SystemVariables))
                    continue;

                Parameter draggedComponent = draggedItem.obj as Parameter;

                if (targetComponent is ParameterCollection)
                {
                    ParameterCollection collection = targetComponent as ParameterCollection;
                    if (collection.IndexOf(draggedComponent) != -1)
                    {
                        collection.Remove(draggedComponent);
                        collection.Insert(0, draggedComponent);
                    }
                    else
                    {
                        collection.Add(draggedComponent);
                    }
                }
                else if (targetComponent is Parameter)
                {
                    if ((targetComponent as Parameter).Parameters.IndexOf(draggedComponent) != -1)
                    {
                        draggedComponent.ZOrder = 0;
                    }
                    else
                    {
                        draggedComponent.Parent = targetComponent as Parameter;
                    }
                }

                draggedName = draggedComponent.FullName;
            }

            tree.SelectedNode = targetNode;
            // update all designer plugins (this one too)
            Designer.SetModified(null, "EditData");
            NavigateTo(Res.Get("Designer,ToolWindow,Dictionary,Parameters") + "." + draggedName);
        }

#if !MONO
        private ButtonItem AddButton(Image image, EventHandler click)
        {
            ButtonItem button = new ButtonItem();
            button.Image = image;
            if (click != null)
                button.Click += click;
            return button;
        }
#else
        private ToolStripButton AddButton(Image image, EventHandler click)
        {
            return new ToolStripButton("", image, click);
        }

        private ToolStripMenuItem AddMenuItem(Image image, EventHandler click)
        {
            return new ToolStripMenuItem("", image, click);
        }
#endif
#endregion

#region Public Methods
        /// <inheritdoc/>
        public override void SelectionChanged()
        {
            base.SelectionChanged();
            if (Designer.SelectedObjects.Count == 0 || Designer.SelectedObjects[0] is ComponentBase)
            {
                tree.SelectedNode = null;
                UpdateControls();
            }
        }

        /// <inheritdoc/>
        public override void UpdateContent()
        {
            report = Designer.ActiveReport;
            UpdateTree();
        }

        /// <inheritdoc/>
        public override void Localize()
        {
            MyRes res = new MyRes("Designer,ToolWindow,Dictionary");

            Text = res.Get("");
            btnActions.Text = Res.Get("Buttons,Actions");
#if !MONO
            btnEdit.Tooltip = res.Get("Edit");
            btnDelete.Tooltip = res.Get("Delete");
            btnView.Tooltip = res.Get("View");
#else
            btnEdit.ToolTipText = res.Get("Edit");
            btnDelete.ToolTipText = res.Get("Delete");
            btnView.ToolTipText = res.Get("View");
#endif
            miNew.Text = res.Get("New");
            miOpen.Text = res.Get("Open");
            miMerge.Text = res.Get("Merge");
            miSave.Text = res.Get("Save");
            miChooseData.Text = Res.Get("Designer,Menu,Data,Choose");
            miNewDataSource.Text = res.Get("NewDataSource");
            miSortDataSources.Text = res.Get("SortDataSources");
            miNewRelation.Text = res.Get("NewRelation");
            miNewParameter.Text = res.Get("NewParameter");
            miNewTotal.Text = res.Get("NewTotal");
            miNewCalculatedColumn.Text = res.Get("NewCalculatedColumn");
            miRename.Text = res.Get("Rename");
            miEdit.Text = res.Get("Edit");
            miDelete.Text = res.Get("Delete");
            miDeleteAlias.Text = res.Get("DeleteAlias");
            miView.Text = res.Get("View");
            miViewJson.Text = res.Get("ViewJson");
            miSortDataFields.Text = res.Get("SortDataFields");
            miCopyDataSource.Text = res.Get("CopyDataSource");
#if MONO
            miNewDataSource1.Text = miNewDataSource.Text;
            miNewParameter1.Text = miNewParameter.Text;
            miNewTotal1.Text = miNewTotal.Text;
            miNewCalculatedColumn1.Text = miNewCalculatedColumn.Text;
#endif
            UpdateTree();
        }

        /// <inheritdoc/>
        public override void UpdateUIStyle()
        {
            base.UpdateUIStyle();
#if !MONO
            toolbar.Style = UIStyleUtils.GetDotNetBarStyle(Designer.UIStyle);
            mnuContext.Style = toolbar.Style;
            splitter.BackColor = UIStyleUtils.GetControlColor(Designer.UIStyle);
#else
            toolbar.Renderer = UIStyleUtils.GetToolStripRenderer(Designer.UIStyle);
            mnuContext.Renderer = toolbar.Renderer;
            splitter.BackColor = UIStyleUtils.GetColorTable(Designer.UIStyle).ControlBackColor;
#endif
        }

        /// <inheritdoc/>
        public override void SaveState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            xi.SetProp("DescriptionHeight", lblDescription.Height.ToString());
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            XmlItem xi = Config.Root.FindItem("Designer").FindItem(Name);
            string s = xi.GetProp("DescriptionHeight");
            if (s != "")
                lblDescription.Height = int.Parse(s);
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
        /// Initializes a new instance of the <see cref="DictionaryWindow"/> class with default settings.
        /// </summary>
        /// <param name="designer">The report designer.</param>
        public DictionaryWindow(Designer designer)
            : base(designer)
        {
            Name = "DictionaryWindow";

#if !MONO
            Image = Res.GetImage(72);
            toolbar = new Bar();
            toolbar.Dock = DockStyle.Top;
            toolbar.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            toolbar.RoundCorners = false;

            btnActions = new ButtonItem();
            btnActions.AutoExpandOnClick = true;
            btnActions.Name = "Actions";
            btnEdit = AddButton(Res.GetImage(68), miEdit_Click);
            btnDelete = AddButton(Res.GetImage(51), miDelete_Click);
            btnView = AddButton(Res.GetImage(54), miView_Click);
            toolbar.Items.AddRange(new ButtonItem[] { btnActions, btnEdit, btnDelete, btnView });

            miNew = AddButton(Res.GetImage(0), miNew_Click);
            miOpen = AddButton(Res.GetImage(1), miOpen_Click);
            miMerge = AddButton(null, miMerge_Click);
            miSave = AddButton(Res.GetImage(2), miSave_Click);
            miChooseData = AddButton(null, Designer.cmdChooseData.Invoke);
            miChooseData.BeginGroup = true;

            miNewDataSource = AddButton(Res.GetImage(137), Designer.cmdAddData.Invoke);
            miNewDataSource.BeginGroup = true;
            miCopyDataSource = AddButton(Res.GetImage(6), miCopyDataSource_Click);
            miSortDataSources = AddButton(null, Designer.cmdSortDataSources.Invoke);
            miNewRelation = AddButton(Res.GetImage(139), miNewRelation_Click);
            miNewCalculatedColumn = AddButton(Res.GetImage(55), miNewCalculatedColumn_Click);
            miNewParameter = AddButton(Res.GetImage(56), miNewParameter_Click);
            miNewTotal = AddButton(Res.GetImage(65), miNewTotal_Click);
            btnActions.SubItems.AddRange(new ButtonItem[] { miNew, miOpen, miMerge, miSave, miChooseData, miNewDataSource,
          miSortDataSources, miNewRelation, miNewCalculatedColumn, miNewParameter, miNewTotal });
            btnActions.PopupOpen += btnActions_PopupOpen;

            mnuContext = new ContextMenuBar();
            mnuContext.Font = DpiHelper.ConvertUnits(DrawUtils.DefaultFont, true);
            mnuContextRoot = new ButtonItem();
            mnuContext.Items.Add(mnuContextRoot);
            miRename = AddButton(null, miRename_Click);
            miRename.Shortcuts.Add(eShortcut.F2);
            miEdit = AddButton(Res.GetImage(68), miEdit_Click);
            miDelete = AddButton(Res.GetImage(51), miDelete_Click);
            miDeleteAlias = AddButton(null, miDeleteAlias_Click);
            miView = AddButton(Res.GetImage(54), miView_Click);
            miViewJson = AddButton(Res.GetImage(54), miViewJson_Click);
            miSortDataFields = AddButton(null, miSortDataFields_Click);
            mnuContextRoot.SubItems.AddRange(new ButtonItem[] {
        miRename, miEdit, miCopyDataSource, miDelete, miDeleteAlias, miView, miViewJson, miSortDataFields });
            mnuContextRoot.PopupOpen += mnuContextRoot_PopupOpen;

            tree = new TreeViewMultiSelect();
            tree.Dock = DockStyle.Fill;
            tree.BorderStyle = BorderStyle.None;
            tree.ImageList = Res.GetImages();
            tree.LabelEdit = true;
            tree.HideSelection = false;
            tree.AllowDrop = true;
            tree.MouseDown += FTree_MouseDown;
            tree.BeforeLabelEdit += FTree_BeforeLabelEdit;
            tree.AfterLabelEdit += FTree_AfterLabelEdit;
            tree.KeyDown += FTree_KeyDown;
            tree.AfterSelect += FTree_AfterSelect;
            tree.DoubleClick += miEdit_Click;
            tree.ItemDrag += FTree_ItemDrag;
            tree.DragOver += FTree_DragOver;
            tree.DragDrop += FTree_DragDrop;
            mnuContext.SetContextMenuEx(tree, mnuContextRoot);

            splitter = new Splitter();
            splitter.Dock = DockStyle.Bottom;
            splitter.Visible = false;

            lblDescription = new DescriptionControl();
            lblDescription.Dock = DockStyle.Bottom;
            lblDescription.Height = DpiHelper.ConvertUnits(70);
            lblDescription.Visible = false;

            ParentControl.Controls.AddRange(new Control[] { tree, splitter, lblDescription, toolbar });
#else
            toolbar = new ToolStrip();
            toolbar.Dock = DockStyle.Top;
            toolbar.Font = DrawUtils.DefaultFont;
            toolbar.GripStyle = ToolStripGripStyle.Hidden;
            toolbar.Padding = new Padding(2, 0, 0, 0);

            btnActions = new ToolStripDropDownButton();
            btnEdit = AddButton(Res.GetImage(68), miEdit_Click);
            btnDelete = AddButton(Res.GetImage(51), miDelete_Click);
            btnView = AddButton(Res.GetImage(54), miView_Click);
            // mono fix
            btnView.AutoSize = false;
            btnView.Size = new Size(23, 22);
            toolbar.Items.AddRange(new ToolStripItem[] { btnActions, btnEdit, btnDelete, btnView });

            miNew = AddMenuItem(Res.GetImage(0), miNew_Click);
            miOpen = AddMenuItem(Res.GetImage(1), miOpen_Click);
            miMerge = AddMenuItem(null, miMerge_Click);
            miSave = AddMenuItem(Res.GetImage(2), miSave_Click);
            miChooseData = AddMenuItem(null, Designer.cmdChooseData.Invoke);

            miSortDataSources = AddMenuItem(null, Designer.cmdSortDataSources.Invoke);
            miNewDataSource = AddMenuItem(Res.GetImage(137), Designer.cmdAddData.Invoke);
            miNewRelation = AddMenuItem(Res.GetImage(139), miNewRelation_Click);
            miNewCalculatedColumn = AddMenuItem(Res.GetImage(55), miNewCalculatedColumn_Click);
            miNewParameter = AddMenuItem(Res.GetImage(56), miNewParameter_Click);
            miCopyDataSource = AddMenuItem(Res.GetImage(6), miCopyDataSource_Click);
            miNewTotal = AddMenuItem(Res.GetImage(65), miNewTotal_Click);
            btnActions.DropDownItems.AddRange(new ToolStripItem[] { 
        miNew, miOpen, miMerge, miSave, miChooseData, new ToolStripSeparator(),
        miNewDataSource, miSortDataSources, miNewRelation, miNewCalculatedColumn, miNewParameter, miNewTotal });
            btnActions.DropDownOpening += btnActions_DropDownOpening;

      miNewDataSource.Visible = false;

            mnuContext = new ContextMenuStrip();
            mnuContext.Font = DrawUtils.DefaultFont;
            miNewDataSource1 = AddMenuItem(Res.GetImage(137), Designer.cmdAddData.Invoke);
            miNewCalculatedColumn1 = AddMenuItem(Res.GetImage(55), miNewCalculatedColumn_Click);
            miNewParameter1 = AddMenuItem(Res.GetImage(56), miNewParameter_Click);
            miNewTotal1 = AddMenuItem(Res.GetImage(65), miNewTotal_Click);
            miRename = AddMenuItem(null, miRename_Click);
            miRename.ShortcutKeys = Keys.F2;
            miEdit = AddMenuItem(Res.GetImage(68), miEdit_Click);
            miDelete = AddMenuItem(Res.GetImage(51), miDelete_Click);
            miDeleteAlias = AddMenuItem(null, miDeleteAlias_Click);
            miView = AddMenuItem(Res.GetImage(54), miView_Click);
            miViewJson = AddMenuItem(Res.GetImage(54), miViewJson_Click);
            miSortDataFields = AddMenuItem(null, miSortDataFields_Click);
            mnuContext.Items.AddRange(new ToolStripItem[] {
        miRename, miEdit, miDelete, miDeleteAlias, miView, miCopyDataSource, miViewJson, miSortDataFields });
            mnuContext.Opening += mnuContext_Opening;

            tree = new TreeViewMultiSelect();
            tree.Dock = DockStyle.Fill;
            tree.BorderStyle = BorderStyle.None;
            tree.ImageList = Res.GetImages();
            tree.HideSelection = false;
            tree.AllowDrop = true;
            tree.ContextMenuStrip = mnuContext;
            tree.MouseDown += FTree_MouseDown;
            tree.BeforeLabelEdit += FTree_BeforeLabelEdit;
            tree.AfterLabelEdit += FTree_AfterLabelEdit;
            tree.KeyDown += FTree_KeyDown;
            tree.AfterSelect += FTree_AfterSelect;
            tree.DoubleClick += miEdit_Click;
            tree.ItemDrag += FTree_ItemDrag;
            tree.DragOver += FTree_DragOver;
            tree.DragDrop += FTree_DragDrop;

            splitter = new Splitter();
            splitter.Dock = DockStyle.Bottom;
            splitter.Visible = false;

            lblDescription = new DescriptionControl();
            lblDescription.Dock = DockStyle.Bottom;
            lblDescription.Height = 70;
            lblDescription.Visible = false;

            Controls.AddRange(new Control[] { tree, splitter, lblDescription, toolbar });
#endif
            expandedNodes = new List<string>();
            Localize();
        }

        /// <summary>
        /// Describes an item dragged from the "Data Dictionary" window.
        /// </summary>
        public class DraggedItem
        {
            /// <summary>
            /// The dragged object.
            /// </summary>
            public Object obj;

            /// <summary>
            /// The text of dragged object.
            /// </summary>
            public string text;

            internal DraggedItem(Object obj, string text)
            {
                this.obj = obj;
                this.text = text;
            }
        }

        /// <summary>
        /// Collection of dragged items.
        /// </summary>
        public class DraggedItemCollection : List<DraggedItem>
        {
            internal DraggedItemCollection() : base() { }
        }

        internal static class DragUtils
        {
            public static DraggedItemCollection GetAll(DragEventArgs e)
            {
                // holding dragged objects data in DragEventArgs does not work in Mono. Use simpler way
                //DraggedItemCollection items = (DraggedItemCollection)e.Data.GetData(typeof(DraggedItemCollection));
                DraggedItemCollection items = DictionaryWindow.draggedItems;

                if (items == null || items.Count == 0)
                    return null;

                return items;
            }

            public static DraggedItem GetOne(DragEventArgs e)
            {
                DraggedItemCollection items = DictionaryWindow.draggedItems;

                if (items == null || items.Count == 0)
                    return null;

                return items[items.Count - 1];
            }
        }
    }
}
