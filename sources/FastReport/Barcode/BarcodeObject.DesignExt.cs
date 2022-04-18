using System;
using System.Drawing;
using System.Windows.Forms;
using FastReport.Utils;
using FastReport.Forms;

namespace FastReport.Barcode
{
    partial class BarcodeObject : IHasEditor
    {
        #region Private Methods
        private bool ShouldSerializePadding()
        {
            return Padding != new System.Windows.Forms.Padding();
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override SmartTagBase GetSmartTag()
        {
            return new BarcodeSmartTag(this);
        }

        /// <inheritdoc/>
        public override ContextMenuBase GetContextMenu()
        {
            return new BarcodeObjectMenu(Report.Designer);
        }

        /// <inheritdoc/>
        public override SizeF GetPreferredSize()
        {
            if ((Page as ReportPage).IsImperialUnitsUsed)
                return new SizeF(Units.Inches * 1, Units.Inches * 1);
            return new SizeF(Units.Centimeters * 2.5f, Units.Centimeters * 2.5f);
        }

        /// <inheritdoc/>
        public override void OnBeforeInsert(int flags)
        {
            Barcode = (BarcodeBase)Activator.CreateInstance(Barcodes.Items[flags].objType);
            if (Barcode is BarcodeIntelligentMail)
                Text = "12345678901234567890";
            else if (Barcode is BarcodeDeutscheIdentcode)
                Text = "12345123456";
            else if (Barcode is BarcodeDeutscheLeitcode)
                Text = "12345123123125";
        }

        /// <inheritdoc/>
        public bool InvokeEditor()
        {
            bool res = false;

            bool isRichBarcode = false;
            if (Barcode is BarcodeQR || Barcode is BarcodeAztec)
                isRichBarcode = true;

            using (BarcodeEditorForm form = new BarcodeEditorForm(Text, Report, Brackets, isRichBarcode))
            {
                if (!form.IsDisposed)
                {
                    DialogResult result = form.ShowDialog();

                    if (result == DialogResult.OK)
                    {
                        AllowExpressions = true;
                        Text = form.result;
                        res = true;
                    }
                }
            }

            return res;
        }
        #endregion

    }
}