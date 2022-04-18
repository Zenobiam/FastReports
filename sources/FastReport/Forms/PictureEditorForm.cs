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
  internal partial class PictureEditorForm : BaseDialogForm
  {
    private PictureObject picture;

    private void SetImage(Image image)
    {
      pbPicture.Image = image;
      if (image != null)
      {
        if (image.Width < pbPicture.Width && image.Height < pbPicture.Height)
          pbPicture.SizeMode = PictureBoxSizeMode.CenterImage;
        else
          pbPicture.SizeMode = PictureBoxSizeMode.Zoom;
        lblSize.Text = image.Width.ToString() + " × " + image.Height.ToString();
      }  
      else
        lblSize.Text = "";
    }
    
    private void btnLoad_Click(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,Images");
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          SetImage(Image.FromFile(dialog.FileName));
        }
      }
    }

    private void btnPaste_Click(object sender, EventArgs e)
    {
      if (Clipboard.ContainsImage())
        SetImage(Clipboard.GetImage());
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
      SetImage(null);
    }

    private void tbFileName_ButtonClick(object sender, EventArgs e)
    {
      using (OpenFileDialog dialog = new OpenFileDialog())
      {
        dialog.Filter = Res.Get("FileFilters,Images");
        if (dialog.ShowDialog() == DialogResult.OK)
        {
          tbFileName.Text = dialog.FileName;
        }
      }
    }

    private void btnEdit_Click(object sender, EventArgs e)
    {
        if (pbPicture.Image != null)
        {
            using (PictureEditorAdvancedForm f = new PictureEditorAdvancedForm(pbPicture.Image))
            {
                if (f.ShowDialog() == DialogResult.OK)
                {
                    SetImage(f.Image);
                }
            }
        }
    }

    private void PictureEditorForm_Shown(object sender, EventArgs e)
    {
      // needed for 120dpi mode
      lblNote.Width = tbFileName.Width;
    }

    private void PictureEditorForm_FormClosed(object sender, FormClosedEventArgs e)
    {
      Done();
    }

    private void Init()
    {
      ProfessionalColorTable vs2005ColorTable = new ProfessionalColorTable();
      vs2005ColorTable.UseSystemColors = true;
      ts1.Renderer = new ToolStripProfessionalRenderer(vs2005ColorTable);
      
      tbFileName.ImageIndex = 1;
      btnLoad.Image = Res.GetImage(1);
      btnPaste.Image = Res.GetImage(7);
      btnClear.Image = Res.GetImage(82);
      btnEdit.Image = Res.GetImage(198);
      
      SetImage(null);
      if (picture.IsDataColumn)
        pcPages.ActivePageIndex = 1;
      else if (picture.IsFileLocation)
      {
        pcPages.ActivePageIndex = 2;
        tbFileName.Text = picture.ImageLocation;
      }  
      else if (picture.IsWebLocation)
      {
        pcPages.ActivePageIndex = 3;
        tbUrl.Text = picture.ImageLocation;
      }  
      else
      {
        pcPages.ActivePageIndex = 0;
        SetImage(picture.Image);
      }
      tvData.CreateNodes(picture.Report.Dictionary);
      tvData.SelectedItem = picture.DataColumn;
    }
    
    private void Done()
    {
      if (DialogResult == DialogResult.OK)
      {
        picture.Image = null;
        picture.DataColumn = "";
        picture.ImageLocation = "";
        
        switch (pcPages.ActivePageIndex)
        {
          case 0:
            picture.Image = pbPicture.Image;
            break;
          
          case 1:
            picture.DataColumn = tvData.SelectedItem;
            break;

          case 2:
            picture.ImageLocation = tbFileName.Text;
            break;
            
          case 3:
            picture.ImageLocation = tbUrl.Text;
            break;
        }
      }
    }

    public override void Localize()
    {
      base.Localize();
      MyRes res = new MyRes("Forms,PictureEditor");
      Text = res.Get("");
      pnPicture.Text = res.Get("Picture");
      btnLoad.Text = res.Get("Load");
      btnPaste.Text = res.Get("Paste");
      btnClear.Text = res.Get("Clear");
      btnEdit.Text = res.Get("Edit");
      pnDataColumn.Text = res.Get("DataColumn");
      pnFileName.Text = res.Get("FileName");
      pnUrl.Text = res.Get("Url");
      lblFile.Text = res.Get("FileHint1");
      lblNote.Text = res.Get("FileHint2");
      lblUrl.Text = res.Get("UrlHint");
    }
    
    public PictureEditorForm(PictureObject picture)
    {
            this.picture = picture;
      InitializeComponent();
      Localize();
      Init();
            Scale();
    }

#if !MONO

        protected override void Scale()
        {
            base.Scale();
            //btnCancel.Location = new Point(pcPages.Right - btnCancel.Width, ClientSize.Height - btnCancel.Height - DpiHelper.ConvertUnits(8));
            //btnOk.Location = new Point(btnCancel.Left - DpiHelper.ConvertUnits(17) - btnOk.Width, btnCancel.Location.Y);
        }
#endif
    }
}

