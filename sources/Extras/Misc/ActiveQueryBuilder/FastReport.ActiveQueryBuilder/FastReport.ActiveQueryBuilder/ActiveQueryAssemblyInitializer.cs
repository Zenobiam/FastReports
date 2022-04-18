using FastReport.Data;
using FastReport.Design;
using FastReport.Utils;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace FastReport.Forms
{
    public class ActiveQueryAssemblyInitializer : AssemblyInitializerBase
    {
        public ActiveQueryAssemblyInitializer()
        {
            Config.DesignerSettings.CustomQueryBuilder += new CustomQueryBuilderEventHandler(DesignerSettings_CustomQueryBuilder);
        }

        private void DesignerSettings_CustomQueryBuilder(object sender, CustomQueryBuilderEventArgs e)
        {
            using (ActiveQBForm form = new ActiveQBForm())
            {
                form.Connection = e.Connection;
                form.SQL = e.SQL;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    e.SQL = form.SQL;
                    foreach (var qbPar in form.QueryBuilder.Parameters)
                    {
                        bool isUniq = true;
                        if (e.Parameters.Count > 0)
                            foreach (CommandParameter frPar in e.Parameters)
                                if (qbPar.Name == frPar.Name)
                                    isUniq = false;
                        if (isUniq)
                            e.Parameters.Add(QBToFRParam(qbPar, form.Connection));
                    }
                }
            }
        }

        private CommandParameter QBToFRParam(ActiveQueryBuilder.Core.Parameter par, DataConnectionBase con)
        {
            CommandParameter frPar = new CommandParameter();
            frPar.Name = par.Name;
            frPar.DataType = con.GetType().Name == "MsSqlDataConnection" ? (int)ToSqlDbType(par.DataType) : (int)par.DataType;
            return frPar;
        }

        SqlDbType ToSqlDbType(DbType type)
        {
            SqlParameter parm = new SqlParameter();
            try
            {
                parm.DbType = type;
            }
            catch (Exception ex)
            {
                // can't map
                parm.DbType = DbType.String;
            }
            return parm.SqlDbType;
        }
    }
}
