using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using FastReport.Design;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport.Barcode
{
  internal class BarcodeObjectMenu : ReportComponentBaseMenu
  {
    private Report report;
    private SelectedObjectCollection selection;
    public ContextMenuItem miBarcodeType;
    public ContextMenuItem miAutoSize;
    public ContextMenuItem miShowText;

    private List<BarcodeObject> ModifyList
    {
      get
      {
        List<BarcodeObject> list = new List<BarcodeObject>();
        foreach (Base c in selection)
        {
          if (c is BarcodeObject && !c.HasRestriction(Restrictions.DontModify))
            list.Add(c as BarcodeObject);
        }
        return list;
      }
    }

    #region Private Methods
    private void CreateBarcodeTypes(BarcodeBase b)
    {
      string[] symbologyNames = Barcodes.GetSymbologyNames();
      for (int i = 0; i < symbologyNames.Length; i++)
      {
        ContextMenuItem mi = CreateMenuItem(symbologyNames[i], new EventHandler(miBarcodeType_Click));
        mi.Tag = symbologyNames[i];
        mi.Checked = symbologyNames[i] == b.Name;
        miBarcodeType.DropDownItems.Add(mi);
      }
    }

    private void miBarcodeType_Click(object sender, EventArgs e)
    {
      foreach (BarcodeObject c in ModifyList)
      {
        c.SymbologyName = (string)((sender as ContextMenuItem).Tag);
      }
      Change();
    }
    
    private void miAutoSize_Click(object sender, EventArgs e)
    {
      foreach (BarcodeObject c in ModifyList)
      {
        c.AutoSize = miAutoSize.Checked;
      }
      Change();
    }

    private void miShowText_Click(object sender, EventArgs e)
    {
      foreach (BarcodeObject c in ModifyList)
      {
        c.ShowText = miShowText.Checked;
      }
      Change();
    }
    #endregion

    public BarcodeObjectMenu(Designer designer) : base(designer)
    {
      report = designer.Report;
      selection = Designer.SelectedObjects;

      MyRes res = new MyRes("ComponentMenu,Barcode");
      miBarcodeType = CreateMenuItem(res.Get("BarcodeType"));
      miBarcodeType.BeginGroup = true;
      miAutoSize = CreateMenuItem(res.Get("AutoSize"), new EventHandler(miAutoSize_Click));
      miAutoSize.BeginGroup = true;
      miAutoSize.CheckOnClick = true;
      miShowText = CreateMenuItem(res.Get("ShowText"), new EventHandler(miShowText_Click));
      miShowText.CheckOnClick = true;

      int insertPos = Items.IndexOf(miCut);
      Items.Insert(insertPos, miBarcodeType);
      Items.Insert(insertPos + 1, miAutoSize);
      Items.Insert(insertPos + 2, miShowText);

      BarcodeObject barcode = selection[0] as BarcodeObject;
      CreateBarcodeTypes(barcode.Barcode);
      
      bool enabled = !barcode.HasRestriction(Restrictions.DontModify);
      miBarcodeType.Enabled = enabled;
      miAutoSize.Enabled = enabled;
      miShowText.Enabled = enabled;
      if (enabled)
      {
        miAutoSize.Checked = barcode.AutoSize;
        miShowText.Checked = barcode.ShowText;
      }
    }
  }
}
