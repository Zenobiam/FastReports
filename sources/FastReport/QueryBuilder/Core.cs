using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Windows.Forms;
using FastReport.Data;
using System.Collections;
using FastReport.Forms;

namespace FastReport.FastQueryBuilder
{
    internal class Core
    {
        private IQueryDesigner queryDesigner;
        private DataBase dataBase;
        private List<Table> tableList = new List<Table>();
        private DialogResult dialogResult;        
        public Query query = new Query();
        private SqlParser parser;
        private bool useJoin = true;
        private SqlParser.LinkStruct link;
        
        // unstable features
        private bool unstableFeatures;
        private bool unstableFeaturesForce;
        private string unstableFeaturesSQL;
        private List<TableView> unstableFeraturesTableViews;

        public Core(IQueryDesigner qd, DataBase db)
        {
            unstableFeatures = true;
            dataBase = db;
            queryDesigner = qd;
            queryDesigner.OnOk += OnOk;
            queryDesigner.OnCancel += OnCancel;
            queryDesigner.OnGetTableList += OnGetTableList;
            queryDesigner.OnAddTable += OnAddTable;
            queryDesigner.OnGenerateSQL += OnGenerateSQL;
            queryDesigner.OnRunSQL += OnRunSQL;
            queryDesigner.Fields = query.selectedFields;
            queryDesigner.Groups = query.groupedFields;
            queryDesigner.Links = query.linkList;
        }

        public bool UseJoin
        {
            get { return useJoin; }
            set { useJoin = value; }
        }

        public DialogResult DesignQuery()
        {
            queryDesigner.DesignQuery();
            return dialogResult;
        }

        private void OnOk(object sender, EventArgs e)
        {
            dialogResult = DialogResult.OK;
            queryDesigner.Close();
        }

        private void OnCancel(object sender, EventArgs e)
        {
            dialogResult = DialogResult.Cancel;
            queryDesigner.Close();
        }

        private void OnGetTableList(object sender, EventArgs e)
        {
            List<Table> str = dataBase.GetTableList();
            queryDesigner.DoFillTableList(str);
            int number = 0;
            if (unstableFeatures && parser != null)
            {
                //add tables
                foreach (SqlParser.TableStruct t in parser.Tables)
                {
                    Table table = FindTable(str, t.Name);
                    if (table != null)
                    {
                        number++;
                        OnAddTable(queryDesigner, new AddTableEventArgs(table, new System.Drawing.Point(20 * number, 10 * number)));

                    }
                    else
                    {
                        ShowError(new FormatException("Table " + t.Name + " is not found!"));
                    }
                    
                    
                }


                if (unstableFeatures && parser != null)
                {
                    //select fields
                    foreach (SqlParser.FieldStruct fs in parser.Fields)
                    {
                        if (!unstableFeatures)
                            break;
                        string tableName = fs.Table;
                        if (unstableFeraturesTableViews != null)
                            foreach (TableView tv in unstableFeraturesTableViews)
                            {
                                if (!unstableFeatures)
                                    break;
                                Table tbl = tv.Table;
                                if (tbl.Name == tableName || tbl.Alias == tableName)
                                {

                                    if (!tv.SelectCheckBox(fs.Name, fs.Func, fs.Alias))
                                        ShowError(new FormatException("Field " + fs.Name + " is not found!"));
                                    break;
                                }
                            }
                    }

                    foreach (SqlParser.FieldStruct fs in parser.Where)
                    {
                        if (!unstableFeatures)
                            break;

                        Field f = FindSelectedField(fs);
                        if (f == null)
                        {
                            ShowError(new FormatException("Field " + fs.Name + " is not found!"));
                        }
                        else
                        {
                            f.Filter = fs.Filter;
                        }

                    }

                    foreach (SqlParser.FieldStruct fs in parser.Groups)
                    {
                        if (!unstableFeatures)
                            break;


                        Field f = FindSelectedField(fs);
                        if (f == null)
                        {
                            ShowError(new FormatException("Field " + fs.Name + " is not found!"));
                        }
                        else
                        {
                            f.Group = true;
                            query.groupedFields.Add(f);
                        }

                    }

                    foreach (SqlParser.FieldStruct fs in parser.Orders)
                    {
                        if (!unstableFeatures)
                            break;

                        Field f = FindSelectedField(fs);
                        if (f == null)
                        {
                            ShowError(new FormatException("Field " + fs.Name + " is not found!"));
                        }
                        else
                        {
                            switch (fs.SortType)
                            {
                                case SortTypes.Asc:
                                    f.Order = "Asc";
                                    break;
                                case SortTypes.Desc:
                                    f.Order = "Desc";
                                    break;
                            }
                        }

                    }


                }


                if (unstableFeatures)
                {


                    foreach (SqlParser.LinkStruct ls in parser.Links)
                    {
                        link = ls;
                        Field oneField = FindField(ls.One);
                        Field secondField = FindField(ls.Two);
                        if (oneField == null)
                        {
                            ShowError(new FormatException("Field " + ls.One + " is not found!"));
                        }

                        if (secondField == null)
                        {
                            ShowError(new FormatException("Field " + ls.Two + " is not found!"));
                        }

                        OnAddLink(queryDesigner, new AddLinkEventArgs(oneField, secondField));
                        link = null;
                    }
                }

                if (unstableFeatures)
                {
                    SQLGenerator sql = new SQLGenerator(query);
                    sql.qch = dataBase.GetQuotationChars();

                    string newSql = sql.getSql();

                    SqlLexer lexerOld = new SqlLexer(unstableFeaturesSQL, sql.qch);
                    SqlLexer lexerNew = new SqlLexer(newSql, sql.qch);

                    List<SqlToken> tokensOld = lexerOld.Parse();
                    List<SqlToken> tokensNew = lexerNew.Parse();

                    if (tokensOld.Count != tokensNew.Count)
                    {
                        ShowError(new Exception("Sql is not equals"));
                    }
                    if (unstableFeatures)
                    {
                        for (int i = 0; i < tokensOld.Count; i++)
                        {
                            if (!unstableFeatures)
                                break;
                            if (!tokensOld[i].Equals(tokensNew[i]))
                                ShowError(new Exception("Sql is not equals"));
                        }
                    }
                }



                
                if (!unstableFeatures)
                {
                    query.linkList.Clear();
                    query.selectedFields.Clear();
                    query.groupedFields.Clear();
                    query.tableList.Clear();
                    queryDesigner.Clear();
                    queryDesigner.Fields = query.selectedFields;
                    queryDesigner.DoRefreshLinks();
                    
                }
            }
            parser = null;
            unstableFeraturesTableViews = null;
        }

        private Table FindTable(List<Table> list, string tableName)
        {
            foreach (Table table in list)
            {
                if (table.Name == tableName)
                    return table;
            }
            return null;
        }

        private void OnAddTable(object sender, AddTableEventArgs e)
        {
            Table tbl = (Table)e.table.Clone();          
            tbl.Alias = GetUniqueAlias(tbl);
            TableView tv = queryDesigner.DoAddTable(tbl, e.position);

            tv.OnAddLink += OnAddLink;
            tv.OnSelectField += OnSelectField;
            tv.OnDeleteTable += OnDeleteTable;
            query.tableList.Add(tbl);
            if(unstableFeatures && unstableFeraturesTableViews != null)
            {
                unstableFeraturesTableViews.Add(tv);
            }


        }

        private Field FindSelectedField(SqlParser.FieldStruct fs)
        {
            foreach (Field f in query.selectedFields)
            {
                if ((f.table.Alias == fs.Table || f.table.Name == fs.Table) && FieldEqFieldStruct(f, fs))
                {
                    return f;
                }
            }
            return null;
        }

        private Field FindField(SqlParser.FieldStruct fs)
        {
            foreach(Table t  in query.tableList)
            {
                if(t.Name == fs.Table || t.Alias == fs.Table)
                {
                    foreach(Field f in t.FieldList)
                    {
                        if(FieldEqFieldStruct(f, fs))
                        {
                            return f;
                        }
                    }
                }
            }
            return null;
        }

        private bool FieldEqFieldStruct(Field f, SqlParser.FieldStruct fs)
        {
            return f.Name == fs.Name ||
                            !String.IsNullOrEmpty(f.Alias) && f.Alias == fs.Name ||
                            !String.IsNullOrEmpty(fs.Alias) && f.Name == fs.Alias ||
                            !String.IsNullOrEmpty(f.Alias) && !String.IsNullOrEmpty(fs.Alias) && f.Alias == fs.Alias;
        }



        private bool hasDublicate(string alias)
        {
            foreach (Table t in query.tableList)
                if (t.Alias == alias)
                    return true;
            return false;
        }

        private string GetUniqueAlias(Table tbl)
        {
            string al = tbl.Name[0].ToString().ToUpper();
            if (al[0] < 'A' || al[0] > 'Z')
              al = "A";
            int n = 1;

            if (unstableFeatures && parser != null)
            {
                foreach(SqlParser.TableStruct tableStruct in parser.Tables)
                {
                    if(tableStruct.Name == tbl.Name)
                    {
                        al = tableStruct.Alias;
                        break;
                    }
                }
            }

            while (hasDublicate(al))
            {
                al = tbl.Name[0] + n.ToString();
                n++;
            }
            return al;
        }

        private void OnDeleteTable(object sender, AddTableEventArgs e)
        {
            for (int i = query.selectedFields.Count - 1; i >= 0; i--)
            {
                if (query.selectedFields[i].table == e.table)
                    query.selectedFields.RemoveAt(i);
            }
            query.deleteTable(e.table);
            queryDesigner.Fields = query.selectedFields;
            queryDesigner.DoRefreshLinks();
        }

        private void OnAddLink(object sender, AddLinkEventArgs e)
        {
            Link lnk;
            if (LinkHasFrom(query.linkList, e.fieldTo.table))
                lnk = new Link(e.fieldTo, e.fieldFrom);
            else
                lnk = new Link(e.fieldFrom, e.fieldTo);


            if (LinkHas(query.linkList, lnk))
                return;

            if (unstableFeatures && link != null)
            {
                lnk.join = link.JoinType;
                lnk.where = link.WhereType;
                link = null;
            }
            else
            {

                if (UseJoin)
                    lnk.join = QueryEnums.JoinTypes.InnerJoin;
                else
                    lnk.join = QueryEnums.JoinTypes.Where;

                lnk.where = QueryEnums.WhereTypes.Equal;
            }
            
            query.linkList.Add(lnk);
            queryDesigner.DoRefreshLinks();
        }

        private bool LinkHas(List<Link> list, Link lnk)
        {
            foreach (Link link in list)
            {
                if ((link.from == lnk.from) && (link.to == lnk.to))
                    return true;
            }
            return false;
        }

        private bool LinkHasFrom(List<Link> list, Table from)
        {
            foreach (Link link in list)
            {
                if ((link.from.table == from))
                    return true;
            }
            return false;
        }

        private void OnSelectField(object sender, CheckFieldEventArgs e)
        {
            if (e.value)
            {
                query.selectedFields.Add(e.field);
                queryDesigner.Fields = query.selectedFields;
            }
            else
            {
                query.selectedFields.Remove(e.field);
                queryDesigner.Fields = query.selectedFields;
            }
        }

        private void OnGenerateSQL(object sender, EventArgs e)
        {
            SQLGenerator sql = new SQLGenerator(query);
            sql.qch = dataBase.GetQuotationChars();

            queryDesigner.SQLText = sql.getSql();

            #region Debug
            //queryDesigner.SQLText += "\n\n\n\n/*Tables:\n";
            //foreach (Table tbl in query.TableList)
            //{
            //    queryDesigner.SQLText += tbl.Name + "\n";  
            //}
            //queryDesigner.SQLText += "\nLinks:\n";
            //foreach (Link lnk in query.LinkList)
            //{
            //    queryDesigner.SQLText += lnk.From.Table + ":" + lnk.From.Name + " => " + lnk.To.Table + ":" + lnk.To.Name + "\n";
            //}
            //queryDesigner.SQLText += "\nFields:\n";
            //foreach (Field fld in query.SelectedFields)
            //{
            //    queryDesigner.SQLText += fld.Table.ToString() + '.' + fld.Name + "\n";
            //}
            //queryDesigner.SQLText += "*/";
            #endregion
        }
        
        private void OnRunSQL(object sender, EventArgs e)
        {
            SQLGenerator sql = new SQLGenerator(query);                        
            sql.qch = dataBase.GetQuotationChars();
            string SQL;

            if (queryDesigner.SQLText == string.Empty)
                SQL = sql.getSql();
            else
                SQL = queryDesigner.SQLText;

            DataTable table = new DataTable();
            DataConnectionBase dataConnection = dataBase.dataBase;
            DbConnection conn = dataConnection.GetConnection();
            try
            {
              dataConnection.OpenConnection(conn);
              using (DbDataAdapter adapter = dataConnection.GetAdapter(SQL, conn, new CommandParameterCollection(null)))
              {
                table.Clear();
                adapter.Fill(table);
              }
            }
            finally
            {
              dataConnection.DisposeConnection(conn);
            }
            queryDesigner.DataSource = table;
        }

        public string GetSql()
        {
            OnGenerateSQL(null, null);
            return queryDesigner.SQLText;
        }

        private void ShowError(Exception e)
        {
#if DEBUG
            ExceptionForm form = new ExceptionForm(e);
            form.ShowDialog();
#endif
            if (unstableFeatures && !unstableFeaturesForce)
            {
                switch (MessageBox.Show(FastReport.Utils.Res.Get("Forms,QueryBuilder,QueryIsCorrupted"),
                    FastReport.Utils.Res.Get("Forms,QueryBuilder,QueryIsCorruptedTitle"),
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Yes:
                        unstableFeaturesForce = true;
                        break;
                    case DialogResult.No:
                        unstableFeatures = false;
                        
                        break;
                }
            }
        }

        public void SetSql(string sql)
        {
            
            if (unstableFeatures && !String.IsNullOrEmpty(sql))
            {
                unstableFeaturesSQL = sql;
                parser = new SqlParser(sql);
                parser.qch = dataBase.GetQuotationChars();
                unstableFeraturesTableViews = new List<TableView>();
                //#if !DEBUG
                try
                {
                    //#endif
                    Query query = parser.Parse();
                    //#if !DEBUG
                }
                catch (Exception e)
                {
                    ShowError(e);
                    parser = null;
                    unstableFeraturesTableViews = null;
                }
                //#endif
            }
        }
    }
}
