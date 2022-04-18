using FastReport.Export.Dxf.Groups;
using FastReport.Export.Dxf.Sections.Tables;

namespace FastReport.Export.Dxf.Sections
{
    public class TableLtype : TableBase
    {
        //public static readonly string DASH_NAME = LineStyle.Dash.ToString(); // "ISO dash";
        //public static readonly string DOT_NAME = LineStyle.Dot.ToString(); //= "ISO dot";

        #region Public Constructors

        public TableLtype() : base("LTYPE")
        {
            StartTable.Add(GroupUtils.CreateName("LTYPE"));
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddDashDotDotType()
        {
            // 0
            // LTYPE
            // 2
            // ISO dash double-dot
            // 70
            // 0
            // 3
            // _. .
            // 72
            // 65
            // 73
            // 6
            // 40
            // 2.1
            // 49
            // 1.2
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0
            // 49
            // 0
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0
            // 49
            // 0
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0

            Groups.Add(GroupUtils.CreateTypeName("LTYPE"));
            // Linetype name
            Groups.Add(GroupUtils.CreateName(LineStyle.DashDotDot.ToString()));
            // Standard flag values
            Groups.Add(GroupUtils.CreateGroup(70, 0));
            // Descriptive text for linetype
            Groups.Add(GroupUtils.CreateGroup(3, "_. ."));
            // Alignment code; value is always 65, the ASCII code for A
            Groups.Add(GroupUtils.CreateGroup(72, 65));
            // The number of linetype elements
            Groups.Add(GroupUtils.CreateGroup(73, 6));
            // Total pattern length
            Groups.Add(GroupUtils.CreateGroup(40, 2.1));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 1.2));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));

            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 0));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));

            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 0));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));
        }

        public void AddDashDotType()
        {
            // 0
            // LTYPE
            // 2
            // ISO dash dot
            // 70
            // 0
            // 3
            // _.
            // 72
            // 65
            // 73
            // 4
            // 40
            // 1.8
            // 49
            // 1.2
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0
            // 49
            // 0
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0

            Groups.Add(GroupUtils.CreateTypeName("LTYPE"));
            // Linetype name
            Groups.Add(GroupUtils.CreateName(LineStyle.DashDot.ToString()));
            // Standard flag values
            Groups.Add(GroupUtils.CreateGroup(70, 0));
            // Descriptive text for linetype
            Groups.Add(GroupUtils.CreateGroup(3, "_."));
            // Alignment code; value is always 65, the ASCII code for A
            Groups.Add(GroupUtils.CreateGroup(72, 65));
            // The number of linetype elements
            Groups.Add(GroupUtils.CreateGroup(73, 4));
            // Total pattern length
            Groups.Add(GroupUtils.CreateGroup(40, 1.8));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 1.2));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));

            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 0));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));
        }

        public void AddDashType()
        {
            //  0
            //LTYPE
            //  2
            //ISO dash
            // 70
            //0
            //  3
            //_
            // 72
            //65
            // 73
            //2
            // 40
            //1.5
            // 49
            //1.2
            // 74
            //0
            // 49
            //- 0.3
            // 74
            //0
            Groups.Add(GroupUtils.CreateTypeName("LTYPE"));
            // Linetype name
            Groups.Add(GroupUtils.CreateName(LineStyle.Dash.ToString()));
            // Standard flag values
            AddGroup(70, 0);
            // Descriptive text for linetype
            AddGroup(3, "_");
            // Alignment code; value is always 65, the ASCII code for A
            AddGroup(72, 65);
            // The number of linetype elements
            AddGroup(73, 2);
            // Total pattern length
            AddGroup(40, 1.5);
            // Dash, dot or space length (one entry per element)
            AddGroup(49, 1.2);
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            // The following codes are bit values:
            // 1 = If set, code 50 specifies an absolute rotation; if not set, code 50 specifies a relative rotation.
            // 2 = Embedded element is a text string.
            // 4 = Embedded element is a shape.
            AddGroup(74, 0);
            // Dash, dot or space length (one entry per element)
            AddGroup(49, -0.3);
            // Complex linetype element type
            AddGroup(74, 0);
        }

        public void AddDotType()
        {
            // 0
            // LTYPE
            //  2
            // ISO dot
            // 70
            // 0
            // 3
            // .
            // 72
            // 65
            // 73
            // 2
            // 40
            // 0.3
            // 49
            // 0
            // 74
            // 0
            // 49
            // - 0.3
            // 74
            // 0

            Groups.Add(GroupUtils.CreateTypeName("LTYPE"));
            // Linetype name
            Groups.Add(GroupUtils.CreateName(LineStyle.Dot.ToString()));
            // Standard flag values
            Groups.Add(GroupUtils.CreateGroup(70, 0));
            // Descriptive text for linetype
            Groups.Add(GroupUtils.CreateGroup(3, "."));
            // Alignment code; value is always 65, the ASCII code for A
            Groups.Add(GroupUtils.CreateGroup(72, 65));
            // The number of linetype elements
            Groups.Add(GroupUtils.CreateGroup(73, 2));
            // Total pattern length
            Groups.Add(GroupUtils.CreateGroup(40, 0.3));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, 0));
            // Complex linetype element type(one per element). Default is 0(no embedded shape / text).
            // The following codes are bit values:
            // 1 = If set, code 50 specifies an absolute rotation; if not set, code 50 specifies a relative rotation.
            // 2 = Embedded element is a text string.
            // 4 = Embedded element is a shape.
            Groups.Add(GroupUtils.CreateGroup(74, 0));
            // Dash, dot or space length (one entry per element)
            Groups.Add(GroupUtils.CreateGroup(49, -0.3));
            // Complex linetype element type
            Groups.Add(GroupUtils.CreateGroup(74, 0));
        }

        #endregion Public Methods
    }
}