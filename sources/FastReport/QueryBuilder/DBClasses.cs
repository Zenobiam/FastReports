using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Drawing;
using FastReport.Data;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Collections;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.FastQueryBuilder
{

    internal static class QueryEnums
    {
        public enum JoinTypes { Where, InnerJoin, LeftOuterJoin, RightOuterJoin, FullOuterJoin };
        public static string[] JoinTypesToStr = {"WHERE", "INNER JOIN",
                "LEFT OUTER JOIN", "RIGHT OUTER JOIN", "FULL OUTER JOIN"};

        public enum WhereTypes { Equal, NotEqual, GreaterOrEqual, Greater, LessOrEqual, Less, Like, NotLike }
        public static string[][] WhereTypesToStr =  {
           new string[] {"=", "<>", ">=", ">", "<=", "<", "LIKE", "NOT LIKE"},
           new string[] {"=",       "<>",          ">=",                ">",         "<=",             "<",      "LIKE", "NOT LIKE"}};
    }
    
    enum SqlFunc { Avg, Count, Min, Max, Sum };

    enum SortTypes { Asc, Desc };

    /// <summary>
    /// For internal use only.
    /// </summary>
    public class Field
    {
        internal Field()
        {
        }

        private string _name;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _alias = string.Empty;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Alias
        {
            get { return _alias; }
            set { _alias = value.Trim(); 
            }
        }
     
        private string _filter = string.Empty;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set 
            {
                _filter = value.Trim();
                if (_filter == string.Empty)
                    return;

                Regex reg = new Regex("^(=|<|>).*");
                if (reg.IsMatch(_filter))
                    return;
                else
                {
                    if (this.fieldType == _filter.GetType()) // if string
                        _filter = "'" + _filter + "'";
                    _filter = '=' + _filter;
                }
            }
        }

        private bool _group = false;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public bool Group
        {
            get { return _group; }
            set { _group = value;}
        }

        private string _order;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Order
        {
            get { return _order; }
            set { _order = value; }
        }

        private string _func;
        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Func
        {
            get { return _func; }
            set { _func = value;}
        }

        internal Type fieldType;
        internal Table table;

        internal bool IsNumeric
        {
           get 
           {
             return (fieldType == typeof(byte) ||
               fieldType == typeof(sbyte) ||
               fieldType == typeof(short) ||
               fieldType == typeof(int) ||
               fieldType == typeof(long) ||
               fieldType == typeof(ushort) ||
               fieldType == typeof(uint) ||
               fieldType == typeof(ulong) ||
               fieldType == typeof(float) ||
               fieldType == typeof(double) ||
               fieldType == typeof(decimal));
           }
        }

        internal bool TypesCompatible(Field other)
        {
           return (fieldType == other.fieldType) || (IsNumeric && other.IsNumeric);
        }

        internal bool CanLink(Field other)
        {
           return (table != other.table) && TypesCompatible(other);
        }

        internal string FullName
        {
            get { return table.Name + "." + Name; }
        }

        internal string getFullName(string quote)
        {
            if (Name.IndexOfAny(" ~`!@#$%^&*()-+=[]{};:'\",.<>/?\\|".ToCharArray()) != -1)
                return table.Alias + "." + quote[0] + Name + quote[1];
            else
                return table.Alias + "." + Name;
        }

        internal string getFullName(string quote, bool printName)
        {
            if (printName)
                return getFullName(quote);
            else
                return table.Alias + "." + Name;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Name + "(" + fieldType.Name + ")";
        }
    }

    internal class Table : ICloneable
    {
        private string name;
        private string alias;
        private TableDataSource originalTable;
        private List<Field> fieldList;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                if (Alias == "" || Alias == null)
                    Alias = name;
            }
        }

        public string Alias
        {
            get
            {
                return alias;
            }
            set
            {
                alias = value;
            }
        }

        public ITableView tableView;
        public List<Field> FieldList
        {
            get
            {
                if (fieldList.Count == 0)
                {
                    if (originalTable.Columns.Count == 0)
                        originalTable.InitSchema();
                  
                    foreach (Column clm in originalTable.Columns)
                    {
                        Field fld = new Field();
                        fld.Name = clm.Name;
                        fld.fieldType = clm.DataType;
                        fld.table = this;
                        fieldList.Add(fld);
                    }
                }
                return fieldList;
            }
        }

        public Table(TableDataSource tbl)
        {
            originalTable = tbl;
            Name = tbl.TableName;
            fieldList = new List<Field>();
        }
        public override string ToString()
        {
            return Name;
        }

        public string getFullName(string quote)
        {
            if (name.Contains(" "))
              return quote[0] + Name + quote[1];
            else
              return Name;
        }

        #region ICloneable Members

        public object Clone()
        {
            return new Table(originalTable);
        }

        #endregion

        internal string getNameAndAlias()
        {
            return Name + ' ' + Alias;
        }

        internal string getFromName(string quote)
        {
            if ((quote != "") && !Name.Contains(quote[0].ToString()))
                return quote[0] + Name + quote[1] + " " + Alias;
            return Name + " " + Alias;
        }

        internal string getFromName(string quote, bool printName)
        {
            if (printName)
                return getFromName(quote);
            else
                return Alias;
        }
    }

    /// <summary>
    /// For internal use only.
    /// </summary>
    public class Link
    {              
        internal Link(Field from, Field to)
        {
            this.from = from;
            this.to = to;
            
        }
        internal Field from;
        internal Field to;

        internal QueryEnums.JoinTypes join;
        internal QueryEnums.WhereTypes where;

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Editor
        {
            get { return Res.Get("Forms,QueryBuilder,Change"); }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Delete
        {
          get { return Res.Get("Forms,QueryBuilder,Delete"); }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string Name
        {
            get { return from.FullName + " JOIN " + to.FullName; }
        }

        internal void Paint(Control cntr)
        {
            LinkPosition lp1, lp2;

            int cp1 = from.table.tableView.GetLeft() + from.table.tableView.GetWidth() / 2;
            int cp2 = to.table.tableView.GetLeft() + to.table.tableView.GetWidth() / 2;

            if (cp1 > cp2)
            {
                if ((from.table.tableView.GetLeft()) < to.table.tableView.GetLeft() + to.table.tableView.GetWidth())
                {
                    lp1 = LinkPosition.Right;                    
                    lp2 = LinkPosition.Right;
                }
                else
                {
                    lp1 = LinkPosition.Left;
                    lp2 = LinkPosition.Right;
                }
            }
            else
            {
                if ((from.table.tableView.GetLeft() + from.table.tableView.GetWidth()) > to.table.tableView.GetLeft())
                {
                    lp2 = LinkPosition.Left;
                    lp1 = LinkPosition.Left;
                }
                else
                {
                    lp2 = LinkPosition.Left;
                    lp1 = LinkPosition.Right;
                }
            }

            Point pnt1 = cntr.PointToClient(from.table.tableView.GetPosition(this.from, lp1));
            Point pnt2 = cntr.PointToClient(to.table.tableView.GetPosition(this.to, lp2));
            Graphics g2 = cntr.CreateGraphics();
            g2.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            
            using (Pen pen = new Pen(Color.Black))
            {
                g2.DrawLine(pen, pnt1, pnt2);
                
                Point pnt3 = (lp1 == LinkPosition.Left) ? 
                    new Point(pnt1.X + 8, pnt1.Y) : new Point(pnt1.X - 8, pnt1.Y);
                g2.DrawLine(pen, pnt1, pnt3);

                pnt3 = (lp2 == LinkPosition.Left) ?
                    new Point(pnt2.X + 8, pnt2.Y) : new Point(pnt2.X - 8, pnt2.Y);
                g2.DrawLine(pen, pnt2, pnt3);
            }    
        }
    }

    internal class Query
    {
        public List<Table> tableList = new List<Table>();
        public List<Link>  linkList  = new List<Link>();
        public List<Field> selectedFields = new List<Field>();
        public List<Field> groupedFields = new List<Field>();

        public void deleteTable(Table table)
        {
            for (int i = linkList.Count - 1; i >= 0; i--)
            {
                if ((linkList[i].from.table == table) || (linkList[i].to.table == table))
                    linkList.RemoveAt(i);
            }
            tableList.Remove(table);
        }
    }
}
