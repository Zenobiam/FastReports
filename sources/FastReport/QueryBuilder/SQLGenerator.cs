using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.FastQueryBuilder
{
    class SQLGenerator
    {
        private Query query;
        private List<Table> tableHasAlias = new List<Table>();

        public string qch;

        public SQLGenerator(Query qr)
        {
            query = qr;
        }

        public string getSql()
        {
            tableHasAlias.Clear();
            string result = "";
            result += getSelect();
            result += getFrom();
            result += getWhere();
            result += getGroup();
            result += getOrder();
            return result;
        }
      


        private string getGroup()
        {
            string result = string.Empty;

            foreach (Field fld in query.groupedFields)
                result += fld.getFullName(qch) + ", ";

            if (result.Length > 0)
                return "GROUP BY " + result.Substring(0, result.Length - 2) + "\r\n";
            else
                return string.Empty;
        }

        private string getOrder()
        {
            string result = string.Empty;

            foreach (Field fld in query.selectedFields)
                if (fld.Order != null)
                    result += fld.getFullName(qch) + " " + fld.Order + ", ";

            if (result.Length > 0)
                return "ORDER BY " + result.Substring(0, result.Length - 2) + "\r\n";
            else
                return string.Empty;
        }
        
        private string getSelect()
        {
            if (query.selectedFields.Count == 0)
                return string.Empty;

            string result = "";
            foreach (Field fld in query.selectedFields)
            {
                string tmp;
                if (fld.Func != null)
                    tmp = fld.Func + '(' + fld.getFullName(qch) + ')';
                else
                    tmp = fld.getFullName(qch);
                result += tmp;
                if (fld.Alias.Length != 0)
                    result += " AS " + fld.Alias;
                result += ", ";
            }            
            return "SELECT " + result.Remove(result.Length - 2) + "\r\n";
        }

        private bool ThereIsTableInJoin(Table table, List<Link> linkList)
        {
            foreach (Link lnk in linkList)
                if ((lnk.from.table == table) || (lnk.to.table == table))
                    return true;
            return false;
        }

        private string getJoin(Link lnk)
        {
            string result = QueryEnums.JoinTypesToStr[(int)lnk.join] + " " + 
              lnk.to.table.getFromName(qch, !tableHasAlias.Contains(lnk.to.table)) + " ON " + 
              lnk.from.getFullName(qch, !tableHasAlias.Contains(lnk.from.table)) + 
              " " + QueryEnums.WhereTypesToStr[1][(int)lnk.where] + " " +
              lnk.to.getFullName(qch, !tableHasAlias.Contains(lnk.to.table)) + " \r\n";
            tableHasAlias.Add(lnk.to.table);
            return result;
        }

        private string getFrom()
        {
            if (query.tableList.Count == 0)
                return string.Empty;

            string result = "";
           
            List<Table> tableList = new List<Table>();            
            foreach (Table tbl in query.tableList)
                tableList.Add(tbl);

            foreach (Link lnk in query.linkList)
            {
                if (lnk.join != QueryEnums.JoinTypes.Where)
                {
                    if (result == string.Empty)
                    {
                        result += lnk.from.table.getFromName(qch) + " ";
                        tableList.Remove(lnk.from.table);
                        result += getJoin(lnk);
                    }
                    else
                    {
                        result = "(" + result + ") " + getJoin(lnk);
                    }
                    tableList.Remove(lnk.to.table);
                }
                else
                {

                    if (!tableList.Contains(lnk.to.table))
                    {
                        if (result != string.Empty)
                            result += ", ";
                        result += lnk.to.table.getFromName(qch);
                        tableList.Remove(lnk.to.table);
                    }
                }
            }
            foreach (Table tbl in tableList)
            {
                if (result != string.Empty)
                    result += ", ";
                result += tbl.getFromName(qch);
            }

            return "FROM " + result + "\r\n";
        }

        private string getWhere()
        {
            string result = string.Empty;
            foreach (Field fld in query.selectedFields)
                if (fld.Filter != string.Empty)
                    result += fld.getFullName(qch) + fld.Filter + " AND ";

            if (query.linkList.Count != 0)
            {
                foreach (Link lnk in query.linkList)
                {
                    string wop = string.Empty;
                    if (lnk.join == QueryEnums.JoinTypes.Where)
                    {
                        wop = QueryEnums.WhereTypesToStr[1][(int)lnk.where];
                        result += lnk.from.getFullName(qch) + " " + wop + " " + lnk.to.getFullName(qch) + " AND ";
                    }
                }
            }

            if (result != string.Empty)
            {
                result = result.Substring(0, result.Length - 5);
                return "WHERE " + result + "\r\n";
            }
            else
                return string.Empty;
        }

    }
}
