using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Data
{
  partial class BusinessObjectConverter
  {
    private bool IsLoop(TreeNode node, Type type)
    {
      while (node != null)
      {
        Column column = node.Tag as Column;
        if (column.DataType == type)
          return true;
        node = node.Parent;
      }
      return false;
    }

    private void AddNode(TreeNodeCollection nodes, Column column)
    {
      TreeNode node = new TreeNode();
      node.Text = column.Alias;
      node.Checked = column.Enabled;
      node.Tag = column;
      node.ImageIndex = column.GetImageIndex();
      node.SelectedImageIndex = node.ImageIndex;
      nodes.Add(node);

      // handle nested nodes
      if (!IsSimpleType(column.Name, column.DataType))
      {
        if (node.Checked)
        {
          // node is enabled? Discover its subnodes
          DiscoverNode(node);
        }
        else
        {
          // add empty node to enable expansion
          AddEmptyNode(node);
        }
      }
    }

    // adds an empty child node to allow node expansion
    private void AddEmptyNode(TreeNode node)
    {
      TreeNode emptyNode = new TreeNode();
      node.Nodes.Add(emptyNode);
    }

    // discovers the tree node structure (create subnodes)
    private void DiscoverNode(TreeNode node)
    {
      Column column = node.Tag as Column;

      // create tree nodes based on info from business object. Use existing Column only to 
      // correct the checked & text properties. This will guarantee that we have tree 
      // that match actual schema
      PropertyDescriptorCollection properties = GetProperties(column);
      if (properties.Count > 0)
      {
        foreach (PropertyDescriptor prop in properties)
        {
          Type type = prop.PropertyType;
          bool isSimpleProperty = IsSimpleType(prop.Name, type);
          bool isEnumerable = IsEnumerable(prop.Name, type);

          // find existing column
          Column childColumn = column.FindByPropName(prop.Name);

          // column not found, create new one with default settings
          if (childColumn == null)
          {
            if (isEnumerable)
              childColumn = new BusinessObjectDataSource();
            else
              childColumn = new Column();

            childColumn.Name = prop.Name;
            childColumn.Alias = prop.DisplayName;
            childColumn.SetBindableControlType(type);

            // enable column if it is simple property (such as int), or it is class-type
            // property that will not lead to loop. The latter is needed to enable all nested 
            // properties automatically
            childColumn.Enabled = isSimpleProperty || (!isEnumerable && !IsLoop(node, type));
          }

          // update column's DataType - the schema may be changed 
          childColumn.DataType = type;
          childColumn.PropName = prop.Name;
          childColumn.PropDescriptor = prop;
          if (!isSimpleProperty)
            GetReference(column, childColumn);

          AddNode(node.Nodes, childColumn);
        }
      }
      else if (IsEnumerable(column.Name, column.DataType))
      {
        Column childColumn = CreateListValueColumn(column);
        AddNode(node.Nodes, childColumn);
      }
    }

    // creates the tree based on the datasource structure
    public void CreateTree(TreeNodeCollection nodes, Column dataSource)
    {
      AddNode(nodes, dataSource);
    }

    public void CheckNode(TreeNode node)
    {
      Column column = node.Tag as Column;
      if (column == null)
        return;
      column.Enabled = node.Checked;
      node.Nodes.Clear();

      if (!IsSimpleType(column.Name, column.DataType))
      {
        if (node.Checked)
        {
          DiscoverNode(node);
          node.Expand();
        }
        else
        {
          AddEmptyNode(node);
          node.Collapse();
        }
      }
    }

    // creates the datasource structure based on the tree
    public void CreateDataSource(TreeNode node)
    {
      Column column = node.Tag as Column;
      // clear the Columns collection. Do not use Clear method because it will
      // destroy the objects.
      while (column.Columns.Count > 0)
      {
        column.Columns.RemoveAt(0);
      }

      foreach (TreeNode childNode in node.Nodes)
      {
        Column childColumn = childNode.Tag as Column;
        bool isDataSource = childColumn is BusinessObjectDataSource;

        if (childNode.Checked)
        {
          // fix datasource name
          if (isDataSource)
          {
            string saveAlias = childColumn.Alias;
            childColumn.Name = dictionary.CreateUniqueName(childColumn.Name);
            childColumn.Alias = saveAlias;
          }
          
          column.Columns.Add(childColumn);
          CreateDataSource(childNode);
        }
        else if (childColumn != null && !isDataSource)
        {
          // column is not enabled, clear its subcolumns and add to the collection as disabled
          // (case: we have a very deep business object with many nested class-type properties)
          childColumn.Columns.Clear();
          column.Columns.Add(childColumn);
        }
      }
    }
  }
}
