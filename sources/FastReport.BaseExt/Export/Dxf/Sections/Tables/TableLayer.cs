using FastReport.Export.Dxf.Groups;

namespace FastReport.Export.Dxf.Sections.Tables
{
    public class TableLayer : TableBase
    {
        #region Public Constructors

        // 0
        // TABLE
        // 2
        // LAYER
        // 70
        // 2
        // 6
        // CONTINUOUS
        // 2
        // Layer_1
        // 0
        // ENDTAB
        public TableLayer() : base("LAYER")
        {
            StartTable.Add(GroupUtils.CreateName("LAYER"));
            AddGroup(70, 2);
            AddGroup(6, "CONTINUOUS");
            AddGroup(2, "Layer_1");
        }

        #endregion Public Constructors
    }
}