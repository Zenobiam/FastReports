using FastReport.Export.Dxf.Groups;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Export.Dxf.Sections.Entities
{
    public class EntityBase : GroupsStore
    {
        #region Private Fields

        //List<GroupBase> groups;
        private List<LineStyle> styles;

        #endregion Private Fields

        #region Public Properties

        public List<LineStyle> Styles
        {
            get { return styles; }
        }

        #endregion Public Properties

        //public List<GroupBase> Groups
        //{
        //    get { return groups; }
        //    set { groups = value; }
        //}

        #region Public Constructors

        public EntityBase()
        {
            styles = new List<LineStyle>();
            //Groups = new List<GroupBase>();
        }

        #endregion Public Constructors

        #region Public Methods

        public void AddLineStyle(LineStyle lineStyle)
        {
            // Linetype name
            Groups.Add(GroupUtils.CreateGroup(6, lineStyle.ToString()));
            // Add line type to table LTYPE
            bool shouldAdd = true;
            foreach (LineStyle s in styles)
            {
                if (s == lineStyle)
                    shouldAdd = false;
            }
            if (shouldAdd)
                styles.Add(lineStyle);
        }

        public void AppendTo(StringBuilder s)
        {
            int i = 0;
            foreach (GroupBase g in Groups)
            {
                i++;
                g.AppendTo(s);
                if (i < Groups.Count)
                    s.Append("\n");
            }
        }

        #endregion Public Methods
    }
}