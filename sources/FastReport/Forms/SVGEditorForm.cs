using FastReport.SVG;
using FastReport.Utils;
using Svg;
using System;
using System.Windows.Forms;

namespace FastReport.Forms
{
    internal partial class SVGEditorForm : BaseDialogForm
    {
        private SVGObject svgObject;
        private readonly SVGObject originalSvgObject;
        private readonly string originalSvgString;

        public SVGObject SvgObject
        {
            get { return svgObject; }
            set { svgObject = value; }
        }

        private void ResetSvgObjectToOriginal()
        {
            svgObject = (SVGObject)originalSvgObject.Clone();
            if (!string.IsNullOrEmpty(originalSvgString))
                svgObject.SVGString = originalSvgString;
        }

        private void SetImage(SVGObject svgObject)
        {
            if ((svgObject != null) && (svgObject.SvgDocument != null))
            {
                if ((Is100Percent() && svgObject.Width < pbPicture.Width &&
                    svgObject.Height < pbPicture.Height) ||
                    (!Is100Percent() && svgObject.SvgDocument.Width < pbPicture.Width &&
                    svgObject.SvgDocument.Width < pbPicture.Height))
                {
                    pbPicture.SizeMode = PictureBoxSizeMode.CenterImage;
                }
                else
                {
                    pbPicture.SizeMode = PictureBoxSizeMode.Zoom;
                }
                if (svgObject.Grayscale)
                {
                    this.svgObject.SVGGrayscale = this.svgObject.GetSVGGrayscale();
                    pbPicture.Image = this.svgObject.SVGGrayscale.Draw();
                }
                else
                    pbPicture.Image = svgObject.SvgDocument.Draw();
                string sizeTxt = "";
                if (Is100Percent())
                    sizeTxt = svgObject.Width.ToString() + " × " + svgObject.Height.ToString();
                else
                    sizeTxt = svgObject.SvgDocument.Width.ToString() + " × " + svgObject.SvgDocument.Height.ToString();
                lblSize.Text = sizeTxt;
            }
            else
            {
                lblSize.Text = "";
                pbPicture.Image = null;
            }

        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,SvgFile");
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    svgObject.SetSVGByPath(dialog.FileName);
                    SetImage(svgObject);
                }
            }
        }

        private void btnPaste_Click(object sender, EventArgs e)
        {
            /*  if (Clipboard.ContainsImage())
                  SetImage(Clipboard.GetImage());*/
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            SetImage(null);
            svgObject.SVGString = null;

        }

        private void tbFileName_ButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = Res.Get("FileFilters,SvgFile");
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
                using (SVGEditorAdvancedForm f = new SVGEditorAdvancedForm(svgObject))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        svgObject = f.SvgObject;
                        svgObject.SVGString = f.SvgObject.SVGString;
                        SetImage(f.SvgObject);
                    }
                    else
                        ResetSvgObjectToOriginal();
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
            if (svgObject.IsDataColumn)
                pcPages.ActivePageIndex = 1;
            else if (svgObject.IsFileLocation)
            {
                pcPages.ActivePageIndex = 2;
                tbFileName.Text = svgObject.ImageLocation;
            }
            else if (svgObject.IsWebLocation)
            {
                pcPages.ActivePageIndex = 3;
                tbUrl.Text = svgObject.ImageLocation;
            }
            else
            {
                pcPages.ActivePageIndex = 0;
                SetImage(svgObject);
            }
            tvData.CreateNodes(svgObject.Report.Dictionary);
            tvData.SelectedItem = svgObject.DataColumn;
        }

        private void Done()
        {
            if (DialogResult == DialogResult.OK)
            {
                //svgObject.Image = null;
                svgObject.DataColumn = "";
                svgObject.ImageLocation = "";

                switch (pcPages.ActivePageIndex)
                {
                    case 0:
                        //svgObject.Image = pbPicture.Image;
                        break;

                    case 1:
                        svgObject.DataColumn = tvData.SelectedItem;
                        break;

                    case 2:
                        svgObject.ImageLocation = tbFileName.Text;
                        break;

                    case 3:
                        svgObject.ImageLocation = tbUrl.Text;
                        break;
                }
            }
            else
                ResetSvgObjectToOriginal();
        }

        private bool Is100Percent()
        {
            return svgObject.SvgDocument.Width.Type == Svg.SvgUnitType.Percentage &&
                    svgObject.SvgDocument.Width.Value == 100;
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

        public SVGEditorForm(SVGObject svgPicture)
        {
            svgObject = svgPicture;
            originalSvgObject = (SVGObject) svgPicture.Clone();
            if (svgPicture.SVGString != null)
                originalSvgString = svgPicture.SVGString;
            InitializeComponent();
            Localize();
            Init();
            Scale();
        }
    }
}

