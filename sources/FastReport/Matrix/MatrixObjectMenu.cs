using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Table;
using FastReport.Design;
using FastReport.Utils;

namespace FastReport.Matrix
{
  internal class MatrixObjectMenu : TableObjectMenu
  {
    private MatrixObject matrix;
    private ContextMenuItem miAutoSize;
    private ContextMenuItem miShowTitle;
    private ContextMenuItem miCellsSideBySide;
    private ContextMenuItem miStyle;

    private new void Change()
    {
      matrix.BuildTemplate();
      base.Change();
    }

    private void BuildStyleMenu()
    {
      ContextMenuItem styleItem = CreateMenuItem(Res.GetImage(76), Res.Get("Designer,Toolbar,Style,NoStyle"), new EventHandler(styleItem_Click));
      styleItem.Tag = "";
      styleItem.Checked = matrix.Style == "";
      styleItem.CheckOnClick = true;
      miStyle.DropDownItems.Add(styleItem);

      for (int i = 0; i < matrix.StyleSheet.Count; i++)
      {
        string text = matrix.StyleSheet[i].Name;
        styleItem = CreateMenuItem(matrix.StyleSheet.GetStyleBitmap(i),
          Res.Get("ComponentsMisc,Matrix," + text), new EventHandler(styleItem_Click));
        styleItem.Tag = text;
        styleItem.Checked = matrix.Style == text;
        styleItem.CheckOnClick = true;
        miStyle.DropDownItems.Add(styleItem);
      }
    }

    private void styleItem_Click(object sender, EventArgs e)
    {
      matrix.Style = (string)(sender as ContextMenuItem).Tag;
      Change();
    }
    
    private void miAutoSize_Click(object sender, EventArgs e)
    {
      matrix.AutoSize = miAutoSize.Checked;
      Change();
    }

    private void miShowTitle_Click(object sender, EventArgs e)
    {
      matrix.ShowTitle = miShowTitle.Checked;
      Change();
    }

    private void miCellsSideBySide_Click(object sender, EventArgs e)
    {
      matrix.CellsSideBySide = miCellsSideBySide.Checked;
      Change();
    }
    
    public MatrixObjectMenu(Designer designer) : base(designer)
    {
      matrix = Designer.SelectedObjects[0] as MatrixObject;
      MyRes res = new MyRes("ComponentMenu,MatrixObject");

      miAutoSize = CreateMenuItem(Res.Get("ComponentMenu,TableRow,AutoSize"), new EventHandler(miAutoSize_Click));
      miAutoSize.BeginGroup = true;
      miAutoSize.CheckOnClick = true;
      miShowTitle = CreateMenuItem(res.Get("ShowTitle"), new EventHandler(miShowTitle_Click));
      miShowTitle.CheckOnClick = true;
      miCellsSideBySide = CreateMenuItem(res.Get("CellsSideBySide"), new EventHandler(miCellsSideBySide_Click));
      miCellsSideBySide.CheckOnClick = true;
      miStyle = CreateMenuItem(Res.GetImage(87), res.Get("Style"), null);
      miStyle.BeginGroup = true;

      int insertIndex = Items.IndexOf(miRepeatHeaders);
      Items.Insert(insertIndex, miAutoSize);
      Items.Insert(insertIndex + 1, miShowTitle);
      Items.Insert(insertIndex + 3, miCellsSideBySide);
      Items.Insert(insertIndex + 4, miStyle);
      
      miRepeatHeaders.BeginGroup = false;
      miCanBreak.Visible = false;
      miGrowToBottom.Visible = false;
      
      miAutoSize.Checked = matrix.AutoSize;
      miShowTitle.Checked = matrix.ShowTitle;
      miCellsSideBySide.Checked = matrix.CellsSideBySide;
      BuildStyleMenu();
    }
  }
}
