using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace FastReport.Export
{
    class ExportIEMPage
    {
        #region Private fields
        private float value;
        private bool landscape;
        private float width;
        private float height;
        private int rawPaperSize;
        private float leftMargin;
        private float topMargin;
        private float bottomMargin;
        private float rightMargin;
        private MemoryStream watermarkPictureStream;
        #endregion

        #region Public properties
        public MemoryStream WatermarkPictureStream
        {
            get { return watermarkPictureStream; }
            set { watermarkPictureStream = value; }
        }
        public float Value
        {
            get { return value; }
            set { this.value = value; }
        }
        public bool Landscape
        {
            get { return landscape; }
            set { landscape = value; }
        }
        public float Width
        {
            get { return width; }
            set { width = value; }
        }
        public float Height
        {
            get { return height; }
            set { height = value; }
        }
        public int RawPaperSize
        {
          get { return rawPaperSize; }
          set 
          {
            if (value == 0)
              rawPaperSize = 9;
            else
              rawPaperSize = value; 
          }
        }
        public float LeftMargin
        {
            get { return leftMargin; }
            set { leftMargin = value; }
        }
        public float RightMargin
        {
            get { return rightMargin; }
            set { rightMargin = value; }
        }
        public float TopMargin
        {
            get { return topMargin; }
            set { topMargin = value; }
        }
        public float BottomMargin
        {
            get { return bottomMargin; }
            set { bottomMargin = value; }
        }
        #endregion
    }
}
