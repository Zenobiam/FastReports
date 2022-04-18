using System.Text;

namespace FastReport.Export.Dxf.Sections
{
    public class SectionBlocks : Section
    {
        #region Public Constructors

        public SectionBlocks() : base("BLOCKS")
        {
        }

        #endregion Public Constructors

        #region Public Methods

        public override void AppendTo(StringBuilder s)
        {
            StartSectionAppendTo(s);
            s.Append("\n");
            EndSectionAppendTo(s);
        }

        #endregion Public Methods
    }
}